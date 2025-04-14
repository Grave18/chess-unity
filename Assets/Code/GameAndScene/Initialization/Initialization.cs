using System.Threading.Tasks;
using Ai;
using AssetsAndResources;
using ChessBoard;
using Highlighting;
using Logic;
using Logic.MovesBuffer;
using Logic.Players;
using Notation;
using Ui.MainMenu;
using Ui.Promotion;
using UnityEngine;

#if UNITY_EDITOR
using EditorCools;
#endif

namespace GameAndScene.Initialization
{
    [DefaultExecutionOrder(-1)]
    public sealed class Initialization : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Assets assets;

        [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private Clock clock;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private UciBuffer uciBuffer;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

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

            InitPlayers();
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
            Player playerWhite = GetPlayer(_gameSettings.Player1Settings);
            Player playerBlack = GetPlayer(_gameSettings.Player2Settings);

            competitors.Init(game, playerWhite, playerBlack);
        }

        private Player GetPlayer(PlayerSettings playerSettings)
        {
            return playerSettings.PlayerType switch
            {
                PlayerType.Computer => new Computer(game, playerSettings, _stockfish),
                PlayerType.Offline => new PlayerOffline(game, mainCamera, highlighter, layerMask,
                    _gameSettings.IsAutoPromoteToQueen, promotionPanel),
                _ => null,
            };
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

            Player playerWhite = GetPlayer(_gameSettings.Player1Settings);
            Player playerBlack = GetPlayer(_gameSettings.Player2Settings);

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