using ChessGame.Logic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Game game;
    [SerializeField] private EffectsPlayer effectsPlayer;

    private void OnEnable()
    {
        game.OnStart += effectsPlayer.PlayGameStartSound;
        game.OnEnd += effectsPlayer.PlayGameEndSound;
    }

    private void OnDisable()
    {
        game.OnStart -= effectsPlayer.PlayGameStartSound;
        game.OnEnd -= effectsPlayer.PlayGameEndSound;
    }
}
