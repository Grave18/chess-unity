using System.Threading.Tasks;
using AssetsAndResources;
using ChessBoard;
using Highlighting;
using Logic;
using Logic.Players;
using UnityEngine;

namespace GameAndScene.Initialization
{
    public class Initialization : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Assets assets;
        [SerializeField] private Board board;
        [SerializeField] private Game game;
        [SerializeField] private CommandInvoker commandInvoker;
        [SerializeField] private Clock clock;
        [SerializeField] private UciString uciString;
        [SerializeField] private Competitors competitors;
        [SerializeField] private Highlighter highlighter;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask layerMask;

        private async void Start()
        {
            await Setup();
        }

        private async Task Setup()
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

            game.StartGame();
        }

        private void InitPlayers()
        {
            GameSettings gameSettings = LoadGameSettings();

            if (gameSettings != null)
            {
                ConfigurePlayers(gameSettings);
            }
            else
            {
                Debug.LogError("GameSettings is null");
            }
        }

        private static GameSettings LoadGameSettings()
        {
            string json = PlayerPrefs.GetString(GameSettings.Key, string.Empty);
            var gameSettings = JsonUtility.FromJson<GameSettings>(json);
            return gameSettings;
        }

        private void ConfigurePlayers(GameSettings gameSettings)
        {
            Player playerWhite = GetPlayer(gameSettings.Player1Settings);
            Player playerBlack = GetPlayer(gameSettings.Player2Settings);

            competitors.Init(game, playerWhite, playerBlack);

            playerWhite.Start();
            playerBlack.Start();
        }

        private Player GetPlayer(PlayerSettings playerSettings)
        {
            return playerSettings.PlayerType switch
            {
                PlayerType.Computer => new Computer(game, commandInvoker, board, playerSettings.ComputerSkillLevel, playerSettings.ComputerThinkTimeMs),
                PlayerType.Offline  => new PlayerOffline(game, commandInvoker, mainCamera, highlighter, layerMask),
                _ => null,
            };
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _ = Setup();
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