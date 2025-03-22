using System.Threading.Tasks;
using Ai;
using AssetsAndResources;
using ChessBoard;
using EditorCools;
using Highlighting;
using Logic;
using Logic.MovesBuffer;
using Logic.Players;
using Ui.Promotion;
using UnityEngine;
using Utils;

namespace GameAndScene.Initialization
{
    public sealed class Initialization : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Assets assets;
        [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private Clock clock;
        [SerializeField] private UciString uciString;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PromotionPanel promotionPanel;

        [Header("Settings")]
        [SerializeField] private GameSettings gameSettings;

        private Stockfish _stockfish;
        private readonly Buffer _commandBuffer = new();

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

            game.Init(board, _commandBuffer, turnColor);
            board.Init(game, _commandBuffer, parsedPreset, prefabs, turnColor);
            clock.Init(game);
            uciString.Init(game, board, assets);
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
            gameSettings = JsonUtility.FromJson<GameSettings>(json);

            return gameSettings != null;
        }

        private void ConfigureAiIfNeeded()
        {
            if (IsNeedToConfigureAi())
            {
                _stockfish = new Stockfish(board, game, _commandBuffer, assets.BoardPreset.Fen);
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
                PlayerType.Computer => new Computer(game, _commandBuffer, playerSettings, _stockfish),
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

            competitors.ChangePlayers(playerWhite, playerBlack);
        }

        [Button(space: 10)]
        private void ShowStockfishState()
        {
            _ = _stockfish?.ShowState();
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