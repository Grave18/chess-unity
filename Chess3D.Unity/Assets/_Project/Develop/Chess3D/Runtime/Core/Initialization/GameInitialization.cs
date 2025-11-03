using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Ai;
using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.Highlighting;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.MenuStates;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Core.Ui.BoardUi;
using Chess3D.Runtime.Core.Ui.ClockUi;
using Chess3D.Runtime.Core.Ui.Promotion;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities.Common.Singleton;
using Cysharp.Threading.Tasks;
using EditorCools;
using UnityEngine;

namespace Chess3D.Runtime.Core.Initialization
{
    [DefaultExecutionOrder(-1)]
    public sealed class GameInitialization : SingletonBehaviour<GameInitialization>
    {
        [Header("Settings")]
        [SerializeField] private Assets assets;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private LayerMask layerMask;

        [Header("Game")]
        [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private NameCanvas nameCanvas;
        [SerializeField] private ClockOffline clockOffline;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private UciBuffer uciBuffer;

        [Header("State machines")]
        [SerializeField] private GameStateMachineOnline gameStateMachineOnline;
        [SerializeField] private GameStateMachineOffline gameStateMachineOffline;
        [SerializeField] private MenuStateMachine menuStateMachine;

        [Header("Ui")]
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private ClockPanelCanvas whiteClockPrefab;
        [SerializeField] private ClockPanelCanvas blackClockPrefab;

        [Header("Camera")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera uiCamera;

        private Stockfish _stockfish;
        private GameSettings _gameSettings;
        private PieceColor _turnColor;
        private FenFromString _fenFromString;
        private IClock _clock;
        private IGameStateMachine _gameStateMachine;

        private bool IsVsPlayer => _gameSettings.Player1Settings.PlayerType == PlayerType.Human
                                   && _gameSettings.Player2Settings.PlayerType == PlayerType.Human;

        public async UniTask Init()
        {
            InitSettingsContainer();
            InitUciBuffer();
            InitGameStateMachine();
            InitGame();
            InitClock();
            InitCamera();
            InitHighlighter();
            await InitBoard();
            InitNameCanvas();
            InitFenGenerator();
            InitAi();
            InitPlayers();
        }

        public async UniTask StartGame()
        {
            await game.StartGame();
        }

        private void InitSettingsContainer()
        {
            // For whatever reason, the initialization can't be done in the Awake
            gameSettingsContainer.Init();

            _gameSettings = gameSettingsContainer.GameSettings;
            _fenFromString = new FenFromString(_gameSettings.CurrentFen);
            _turnColor = _fenFromString.TurnColor;
        }

        private void InitUciBuffer()
        {
            uciBuffer.Init(fenFromBoard, _fenFromString);
        }

        private void InitGameStateMachine()
        {
            if (OnlineInstanceHandler.IsOnline)
            {
                _gameStateMachine = gameStateMachineOnline;
            }
            else
            {
                gameStateMachineOffline.Init();
                _gameStateMachine = gameStateMachineOffline;
            }
        }

        private void InitGame()
        {
            game.Init(board, competitors, cameraController, uciBuffer,
                _turnColor, gameSettingsContainer, _gameStateMachine, menuStateMachine);
        }

        private void InitHighlighter()
        {
            highlighter.Init(game);
        }

        private void InitClock()
        {
            _clock = OnlineInstanceHandler.IsOffline ? clockOffline : OnlineInstanceHandler.Clock;
            _clock.Init(game, gameSettingsContainer.GetTime());

            InstantiateClockPanel();
        }

        private void InstantiateClockPanel()
        {
            ClockPanelCanvas clockPanelCanvas = _gameSettings.PlayerColor == PieceColor.White ? whiteClockPrefab : blackClockPrefab;
            ClockPanelCanvas instance = Instantiate(clockPanelCanvas, transform);
            instance.Init(game, _clock, uiCamera);
        }

        private void InitCamera()
        {
            // Rotate camera to side in fen string only if 2 players
            PieceColor rotateCameraToColor = IsVsPlayer ? _turnColor : _gameSettings.PlayerColor;
            bool isAutoRotationOn = IsVsPlayer && OnlineInstanceHandler.IsOffline && gameSettingsContainer.IsAutoRotateCamera;
            cameraController.Init(game, rotateCameraToColor, isAutoRotationOn);
        }

        private async UniTask InitBoard()
        {
            LoadedPrefabs loadedPrefabs = await assets.LoadPrefabs();
            board.Init(game, uciBuffer, _fenFromString, loadedPrefabs.BoardPrefab, loadedPrefabs.PiecesPrefabs, _turnColor);

            board.Build();
            RotateBoard();
        }

        private void RotateBoard()
        {
            var boardPositioner = board.GetComponent<BoardPositioner>();

            if (_gameSettings.PlayerColor == PieceColor.White)
            {
                boardPositioner.PositionForWhitePieces();
            }
            else if (_gameSettings.PlayerColor == PieceColor.Black)
            {
                boardPositioner.PositionForBlackPieces();
            }
        }

        private void InitNameCanvas()
        {
            string whitePlayerName = _gameSettings.Player1Settings.Name;
            string blackPlayerName = _gameSettings.Player2Settings.Name;

            nameCanvas.Init(whitePlayerName, blackPlayerName);
        }

        private void InitFenGenerator()
        {
            fenFromBoard.Init(game, board, uciBuffer, _gameSettings.CurrentFen);
        }

        private void InitPlayers()
        {
            IPlayer playerWhite = ConstructPlayer(_gameSettings.Player1Settings, PieceColor.White);
            IPlayer playerBlack = ConstructPlayer(_gameSettings.Player2Settings, PieceColor.Black);

            competitors.Init(game, playerWhite, playerBlack, _turnColor);
        }

        private IPlayer ConstructPlayer(PlayerSettings playerSettings, PieceColor color)
        {
            IInputHandler inputHandler = GetInput(playerSettings);
            IPlayer player = GetPlayer(color, inputHandler);

            return player;
        }

        private IInputHandler GetInput(PlayerSettings playerSettings)
        {
            if (playerSettings.PlayerType == PlayerType.Computer)
            {
                return new InputComputer(game, playerSettings, _stockfish);
            }

            if (playerSettings.PlayerType == PlayerType.Human)
            {
                var inputHuman = new InputHuman(game, mainCamera, highlighter, layerMask, _gameSettings.IsAutoPromoteToQueen, promotionPanel);
                return inputHuman;
            }

            return null;
        }

        private IPlayer GetPlayer(PieceColor color, IInputHandler inputHandler)
        {
            IPlayer player = null;

            if (OnlineInstanceHandler.IsOnline)
            {
                if (color == PieceColor.White)
                {
                    player = OnlineInstanceHandler.PlayerWhite;
                    OnlineInstanceHandler.PlayerWhite.Init(inputHandler);
                }
                else if (color == PieceColor.Black)
                {
                    player = OnlineInstanceHandler.PlayerBlack;
                    OnlineInstanceHandler.PlayerBlack.Init(inputHandler);
                }
            }
            else
            {
                player = new PlayerOffline(inputHandler);
            }

            return player;
        }

        private void InitAi()
        {
            if (IsNeedToConfigureAi())
            {
                _stockfish = new Stockfish(uciBuffer, _gameSettings.CurrentFen);
                _ = _stockfish.Start();
            }
        }

        private bool IsNeedToConfigureAi()
        {
            return _stockfish == null
                   && (_gameSettings.Player1Settings.PlayerType == PlayerType.Computer
                       || _gameSettings.Player2Settings.PlayerType == PlayerType.Computer);
        }

        private void OnDestroy()
        {
            _stockfish?.Dispose();
        }

#if UNITY_EDITOR

        [Button(space: 10)]
        private void ShowStockfishState()
        {
            _ = _stockfish?.ShowInternalBoardState();
        }

        [Button(space: 10)]
        private void ShowStockfishOutput()
        {
            _stockfish?.ShowProcessOutput();
        }

#endif
    }
}