using System.Collections;
using System.Threading.Tasks;
using Ai;
using AssetsAndResources;
using ChessGame.ChessBoard;
using ChessGame.Logic;
using ChessGame.Logic.MovesBuffer;
using ChessGame.Logic.Players;
using Highlighting;
using MainCamera;
using Network;
using Notation;
using Ui.Game.Promotion;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using EditorCools;
#endif

namespace Initialization
{
    [DefaultExecutionOrder(-1)]
    public sealed class Initialization : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Assets assets;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private LayerMask layerMask;

        [Header("Game")]
        [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private ClockOffline clockOffline;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private UciBuffer uciBuffer;

        [Header("Ui")]
        [SerializeField] private PromotionPanel promotionPanel;

        [Header("Camera")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Camera mainCamera;

        private Stockfish _stockfish;
        private GameSettings _gameSettings;

        public IClock Clock { get; private set; }

        public bool IsOffline => _gameSettings.Player1Settings.PlayerType != PlayerType.Online
                                && _gameSettings.Player2Settings.PlayerType != PlayerType.Online;
        public bool IsOnline => !IsOffline;

        private async void Awake()
        {
            await Init();

            board.Build();

            WarmUpGame();
        }

        private async Task Init()
        {
            gameSettingsContainer.Init();
            _gameSettings = gameSettingsContainer.GetGameSettings();
            FenSplit fenSplit = FenUtility.GetFenSplit(_gameSettings.CurrentFen);

            PieceColor turnColor = Assets.GetTurnColorFromPreset(fenSplit);
            game.Init(board, uciBuffer, turnColor);

            InitClock();
            InitCamera();
            highlighter.Init(game);

            GameObject[] prefabs = await assets.LoadPrefabs();
            board.Init(game, uciBuffer, fenSplit, prefabs, turnColor);

            InitPlayers();
            fenFromBoard.Init(game, board, _gameSettings.CurrentFen);
        }

        private void InitClock()
        {
            Clock = IsOffline ? clockOffline : OnlineInstanceHandler.Clock;
            Assert.IsNotNull(Clock, $"{nameof(Initialization)}: Clock is null");

            Clock.Init(game, _gameSettings);
        }

        private void InitCamera()
        {
            cameraController.Init(_gameSettings.PlayerColor, game, IsOffline);
        }

        private void InitPlayers()
        {
            ConfigureAiIfNeeded();
            ConfigurePlayers();
        }

        private void ConfigureAiIfNeeded()
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

        private void ConfigurePlayers()
        {
            IPlayer playerWhite = GetPlayer(_gameSettings.Player1Settings, PieceColor.White);
            IPlayer playerBlack = GetPlayer(_gameSettings.Player2Settings, PieceColor.Black);

            competitors.Init(game, playerWhite, playerBlack);
        }

        private IPlayer GetPlayer(PlayerSettings playerSettings, PieceColor color)
        {
            if (playerSettings.PlayerType == PlayerType.Computer)
            {
                return new PlayerComputer(game, playerSettings, _stockfish);
            }

            if (playerSettings.PlayerType == PlayerType.Offline)
            {
                return new PlayerOffline(game, mainCamera, highlighter, layerMask,
                    _gameSettings.IsAutoPromoteToQueen, promotionPanel);
            }

            if (playerSettings.PlayerType == PlayerType.Online)
            {
                if (color == PieceColor.White)
                {
                    PlayerOnline playerOnlineWhite = OnlineInstanceHandler.PlayerWhite;
                    Assert.IsNotNull(playerOnlineWhite, $"{nameof(Initialization)}: PlayerOnline is null");

                    playerOnlineWhite.Init(game, mainCamera, highlighter, layerMask,
                        _gameSettings.IsAutoPromoteToQueen, promotionPanel, color);

                    return playerOnlineWhite;
                }

                if (color == PieceColor.Black)
                {
                    PlayerOnline playerOnlineBlack = OnlineInstanceHandler.PlayerBlack;
                    Assert.IsNotNull(playerOnlineBlack, $"{nameof(Initialization)}: PlayerOnline is null");

                    playerOnlineBlack.Init(game, mainCamera, highlighter, layerMask,
                        _gameSettings.IsAutoPromoteToQueen, promotionPanel, color);

                    return playerOnlineBlack;
                }
            }

            return null;
        }

        private void WarmUpGame()
        {
            bool isCameraSet = false;
            cameraController.RotateToStartPosition(_gameSettings.PlayerColor, () => isCameraSet = true);

            StartCoroutine(StartGameRoutine());

            return;

            IEnumerator StartGameRoutine()
            {
                yield return new WaitUntil(() => isCameraSet);

                game.StartGame();
            }
        }

        private void OnDestroy()
        {
            _stockfish?.Dispose();
        }

#if UNITY_EDITOR

        [Button(space: 10)]
        private void UpdatePlayers()
        {
            ConfigureAiIfNeeded();

            IPlayer playerWhite = GetPlayer(_gameSettings.Player1Settings, PieceColor.White);
            IPlayer playerBlack = GetPlayer(_gameSettings.Player2Settings, PieceColor.Black);

            competitors.SubstitutePlayers(playerWhite, playerBlack);
        }

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

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _ = Init();
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                board.DestroyBoardAndPieces();
                assets.ReleaseAssets();
            }
        }

#endif
    }
}