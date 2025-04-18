using System.Threading.Tasks;
using Ai;
using AssetsAndResources;
using ChessBoard;
using Highlighting;
using Logic;
using Logic.MovesBuffer;
using Logic.Players;
using MainCamera;
using Notation;
using ParrelSync;
using Ui.Game.Promotion;
using Ui.MainMenu;
using UnityEngine;

#if UNITY_EDITOR
using EditorCools;
#endif

namespace GameAndScene.Initialization
{
    [DefaultExecutionOrder(-1)]
    public sealed class Initialization : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private Assets assets;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private LayerMask layerMask;

        [Header("Game")] [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private Clock clock;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private UciBuffer uciBuffer;

        [Header("Ui")] [SerializeField] private PromotionPanel promotionPanel;

        [Header("Camera")] [SerializeField] private Camera mainCamera;
        [SerializeField] private CameraController cameraController;

        [Header("Players")] [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;

        private Stockfish _stockfish;
        private GameSettings _gameSettings;

        private async void Awake()
        {
            await Init();
            game.StartGame();
        }

        private async Task Init()
        {
            gameSettingsContainer.Init();
            _gameSettings = gameSettingsContainer.GetGameSettings();
            FenSplit fenSplit = FenUtility.GetFenSplit(_gameSettings.CurrentFen);

            PieceColor turnColor = Assets.GetTurnColorFromPreset(fenSplit);
            game.Init(board, uciBuffer, turnColor);
            clock.Init(game);
            highlighter.Init(game);

            GameObject[] prefabs = await assets.LoadPrefabs();

            board.Init(game, uciBuffer, fenSplit, prefabs, turnColor);
            fenFromBoard.Init(game, board, _gameSettings.CurrentFen);

            InitCamera();
            InitPlayers();
        }

        private void InitCamera()
        {
            if (ClonesManager.IsClone())
            {
                cameraController.Init(PieceColor.Black);
            }
            else
            {
                cameraController.Init(PieceColor.White);
            }
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
                    playerOnlineWhite.Init(game, mainCamera, highlighter, layerMask,
                        _gameSettings.IsAutoPromoteToQueen, promotionPanel, color);

                    return playerOnlineWhite;
                }

                if (color == PieceColor.Black)
                {
                    playerOnlineBlack.Init(game, mainCamera, highlighter, layerMask,
                        _gameSettings.IsAutoPromoteToQueen, promotionPanel, color);

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