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
        [SerializeField] private Clock clock;

        private async void Start()
        {
            await SetupAsync();
        }

        private async Task SetupAsync()
        {
            GameObject[] prefabs = await assets.InitAsync();
            board.Init(game, assets.Preset, prefabs);
            clock.Init(game, 1);
            game.Init(board, assets.Preset.TurnColor);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _ = SetupAsync();
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