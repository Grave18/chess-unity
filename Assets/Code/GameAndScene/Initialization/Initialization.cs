using System.Threading.Tasks;
using Ai;
using AssetsAndResources;
using ChessBoard;
using Highlighting;
using Logic;
using Logic.Players;
using UnityEngine;

namespace GameAndScene.Initialization
{
    public sealed class Initialization : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Assets assets;
        [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private CommandInvoker commandInvoker;
        [SerializeField] private Clock clock;
        [SerializeField] private UciString uciString;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask layerMask;

        private Stockfish _stockfish;
        private GameSettings _gameSettings;

        private async void Start()
        {
            await Initialize();
            game.StartGame();
        }

        private async Task Initialize()
        {
            GameObject[] prefabs = await assets.LoadPrefabs();
            ParsedPreset parsedPreset = assets.GetParsedPreset();
            PieceColor turnColor = Assets.GetTurnColorFromPreset(parsedPreset);

            game.Init(board, turnColor);
            board.Init(game, commandInvoker, parsedPreset, prefabs, turnColor);
            clock.Init(game);
            uciString.Init(game, board, commandInvoker, assets);
            highlighter.Init(game);

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
            _gameSettings = JsonUtility.FromJson<GameSettings>(json);

            return _gameSettings != null;
        }

        private void ConfigureAiIfNeeded()
        {
            if (_gameSettings.Player1Settings.PlayerType == PlayerType.Computer
                || _gameSettings.Player2Settings.PlayerType == PlayerType.Computer)
            {
                _stockfish = new Stockfish(board, game, commandInvoker);
                _stockfish?.Start();
            }
        }

        private void ConfigurePlayers()
        {
            Player playerWhite = GetPlayer(_gameSettings.Player1Settings);
            Player playerBlack = GetPlayer(_gameSettings.Player2Settings);

            competitors.Init(game, playerWhite, playerBlack);

            playerWhite.Start();
            playerBlack.Start();
        }

        private Player GetPlayer(PlayerSettings playerSettings)
        {
            return playerSettings.PlayerType switch
            {
                PlayerType.Computer => new Computer(game, commandInvoker, playerSettings, _stockfish),
                PlayerType.Offline  => new PlayerOffline(game, commandInvoker, mainCamera, highlighter, layerMask, playerSettings),
                _ => null,
            };
        }

        private void OnDestroy()
        {
            _stockfish.Dispose();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

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