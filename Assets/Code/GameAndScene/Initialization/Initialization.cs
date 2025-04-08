using System.Threading.Tasks;
using Ai;
using AssetsAndResources;
using ChessBoard;
using EditorCools;
using Highlighting;
using Logic;
using Logic.MovesBuffer;
using Logic.Players;
using Notation;
using Ui.Promotion;
using UnityEngine;

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
        [SerializeField] private FenString fenString;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private UciBuffer uciBuffer;

        [Header("Settings")]
        [SerializeField] private GameSettings gameSettings;

        private Stockfish _stockfish;

        private async void Awake()
        {
            await Initialize();
            game.StartGame();
        }

        private async Task Initialize()
        {
            ParsedPreset parsedPreset = assets.GetParsedPreset();
            PieceColor turnColor = Assets.GetTurnColorFromPreset(parsedPreset);
            game.Init(board, uciBuffer, turnColor);
            clock.Init(game);
            highlighter.Init(game);

            GameObject[] prefabs = await assets.LoadPrefabs();

            board.Init(game, uciBuffer, parsedPreset, prefabs, turnColor);
            fenString.Init(game, board, assets);

            InitPlayers();
        }

        private void InitPlayers()
        {
            bool isGameSettingsLoaded = LoadGameSettings();

            if (isGameSettingsLoaded)
            {
                ConfigureAiIfNeeded();
                ConfigurePlayers();
            }
            else
            {
                Debug.LogError("GameSettings is null");
            }
        }

        private bool LoadGameSettings()
        {
            string json = PlayerPrefs.GetString(GameSettings.Key, string.Empty);
            gameSettings = JsonUtility.FromJson<GameSettings>(json);

            return gameSettings != null;
        }

        private void ConfigureAiIfNeeded()
        {
            if (IsNeedToConfigureAi())
            {
                _stockfish = new Stockfish(uciBuffer, assets.BoardPreset.Fen);
                _ = _stockfish.Start();
            }
        }

        private bool IsNeedToConfigureAi()
        {
            return _stockfish == null
                   && (gameSettings.Player1Settings.PlayerType == PlayerType.Computer
                       || gameSettings.Player2Settings.PlayerType == PlayerType.Computer);
        }

        private void ConfigurePlayers()
        {
            Player playerWhite = GetPlayer(gameSettings.Player1Settings);
            Player playerBlack = GetPlayer(gameSettings.Player2Settings);

            competitors.Init(game, playerWhite, playerBlack);
        }

        private Player GetPlayer(PlayerSettings playerSettings)
        {
            return playerSettings.PlayerType switch
            {
                PlayerType.Computer => new Computer(game, playerSettings, _stockfish),
                PlayerType.Offline  => new PlayerOffline(game, mainCamera, highlighter, layerMask, gameSettings.IsAutoPromoteToQueen, promotionPanel),
                _ => null,
            };
        }

        private void OnDestroy()
        {
            _stockfish?.Dispose();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        [Button(space: 10)]
        private void UpdatePlayers()
        {
            ConfigureAiIfNeeded();

            Player playerWhite = GetPlayer(gameSettings.Player1Settings);
            Player playerBlack = GetPlayer(gameSettings.Player2Settings);

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _ = Initialize();
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