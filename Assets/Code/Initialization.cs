using System.Threading.Tasks;
using ChessBoard.Builder;
using Logic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Assets assets;
    [SerializeField] private Board board;
    [SerializeField] private Game game;

    private async void Start()
    {
        await SetupAsync();
    }

    private async Task SetupAsync()
    {
        GameObject[] prefabs = await assets.InitAsync();
        board.Init(game, assets.Preset, prefabs);
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