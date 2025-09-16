using Ai;
using AssetManagement;
using ChessBoard;
using Cysharp.Threading.Tasks;
using Highlighting;
using Logic;
using Logic.MenuStates;
using Logic.MovesBuffer;
using Logic.Players;
using MainCamera;
using Network;
using Notation;
using Settings;
using UnityEngine;
using UnityEngine.Assertions;
using UnityUi.InGame.BoardUi;
using UnityUi.InGame.ClockUi;
using UnityUi.InGame.Promotion;
using UtilsCommon.Singleton;

#if UNITY_EDITOR
using EditorCools;
#endif

namespace Initialization
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
        [SerializeField] private GameStateMachine gameStateMachine;
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

        private bool IsOffline => _gameSettings.Player1Settings.PlayerType != PlayerType.Online
                                  && _gameSettings.Player2Settings.PlayerType != PlayerType.Online;

        private bool IsVsPlayer => _gameSettings.Player1Settings.PlayerType == PlayerType.Human
                                   && _gameSettings.Player2Settings.PlayerType == PlayerType.Human;

        public async UniTask Init()
        {
            InitSettingsContainer();
            InitUciBuffer();
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

        public void StartGame()
        {
            game.StartGame();
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
            int halfMoveClock = _fenFromString.HalfMoveClock;
            int fullMoveCounter = _fenFromString.FullMoveCounter;
            uciBuffer.Init(fenFromBoard, halfMoveClock, fullMoveCounter);
        }

        private void InitGame()
        {
            game.Init(board, competitors, cameraController, uciBuffer,
                _turnColor, gameSettingsContainer, gameStateMachine, menuStateMachine);
        }

        private void InitHighlighter()
        {
            highlighter.Init(game);
        }

        private void InitClock()
        {
            _clock = IsOffline ? clockOffline : OnlineInstanceHandler.Clock;
            Assert.IsNotNull(_clock, $"{nameof(GameInitialization)}: Clock is null");

            _clock.Init(game, gameSettingsContainer.GetTime());

            InstantiateClockPanel();
        }

        private void InstantiateClockPanel()
        {
            ClockPanelCanvas clockPanelCanvas = _gameSettings.PlayerColor == PieceColor.White ? whiteClockPrefab : blackClockPrefab;
            ClockPanelCanvas instance = Instantiate(clockPanelCanvas);
            instance.Init(game, _clock, uiCamera);
        }

        private void InitCamera()
        {
            // Rotate camera to side in fen string only if 2 players
            PieceColor rotateCameraToColor = IsVsPlayer ? _turnColor : _gameSettings.PlayerColor;
            bool isAutoRotationOn = IsVsPlayer;
            cameraController.Init(game, rotateCameraToColor, isAutoRotationOn);
        }

        private async UniTask InitBoard()
        {
            RotateBoard();

            LoadedPrefabs loadedPrefabs = await assets.LoadPrefabs();
            board.Init(game, uciBuffer, _fenFromString, loadedPrefabs.BoardPrefab, loadedPrefabs.PiecesPrefabs, _turnColor);

            board.Build();
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
            IPlayer playerWhite = GetPlayer(_gameSettings.Player1Settings, PieceColor.White);
            IPlayer playerBlack = GetPlayer(_gameSettings.Player2Settings, PieceColor.Black);

            competitors.Init(game, playerWhite, playerBlack, _turnColor);
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

        private IPlayer GetPlayer(PlayerSettings playerSettings, PieceColor color)
        {
            if (playerSettings.PlayerType == PlayerType.Computer)
            {
                return new PlayerComputer(game, playerSettings, _stockfish);
            }

            if (playerSettings.PlayerType == PlayerType.Human)
            {
                return new PlayerOffline(game, mainCamera, highlighter, layerMask,
                    _gameSettings.IsAutoPromoteToQueen, promotionPanel);
            }

            if (playerSettings.PlayerType == PlayerType.Online)
            {
                if (color == PieceColor.White)
                {
                    PlayerOnline playerOnlineWhite = OnlineInstanceHandler.PlayerWhite;
                    Assert.IsNotNull(playerOnlineWhite, $"{nameof(GameInitialization)}: PlayerOnline White is null");

                    playerOnlineWhite.Init(game, mainCamera, highlighter, layerMask,
                        _gameSettings.IsAutoPromoteToQueen, promotionPanel);

                    return playerOnlineWhite;
                }

                if (color == PieceColor.Black)
                {
                    PlayerOnline playerOnlineBlack = OnlineInstanceHandler.PlayerBlack;
                    Assert.IsNotNull(playerOnlineBlack, $"{nameof(GameInitialization)}: PlayerOnline Black is null");

                    playerOnlineBlack.Init(game, mainCamera, highlighter, layerMask,
                        _gameSettings.IsAutoPromoteToQueen, promotionPanel);

                    return playerOnlineBlack;
                }
            }

            return null;
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