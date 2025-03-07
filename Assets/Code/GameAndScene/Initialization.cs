using System.Threading.Tasks;
using AssetsAndResources;
using ChessBoard;
using Logic;
using UnityEngine;

namespace GameAndScene
{
    public class Initialization : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Assets assets;
        [SerializeField] private Board board;
        [SerializeField] private Game game;
        [SerializeField] private CommandInvoker commandInvoker;
        [SerializeField] private Clock clock;

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

            game.StartGame();
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