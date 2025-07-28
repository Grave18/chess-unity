using UnityEngine;
using UnityEngine.Assertions;
using UtilsCommon.Singleton;

[RequireComponent(typeof(AudioSource))]
public class EffectsPlayer : SingletonBehaviour<EffectsPlayer>
{
    [Header("Clips")]
    [SerializeField] private AudioClip gameStart;
    [SerializeField] private AudioClip gameEnd;
    [SerializeField] private AudioClip move;
    [SerializeField] private AudioClip moveOpponent;
    [SerializeField] private AudioClip capture;
    [SerializeField] private AudioClip castle;
    [SerializeField] private AudioClip check;
    [SerializeField] private AudioClip promote;
    [SerializeField] private AudioClip notify;
    [SerializeField] private AudioClip tenSeconds;

    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(_audioSource);
    }

    public void PlayGameStartSound()
    {
        _audioSource.PlayOneShot(gameStart);
    }

    public void PlayGameEndSound()
    {
        _audioSource.PlayOneShot(gameEnd);
    }

    public void PlayTenSecondsSound()
    {
        _audioSource.PlayOneShot(tenSeconds);
    }

    public void PlayMoveSound()
    {
        _audioSource.PlayOneShot(move);
    }

    public void PlayMoveOpponentSound()
    {
        _audioSource.PlayOneShot(moveOpponent);
    }

    public void PlayCaptureSound()
    {
        _audioSource.PlayOneShot(capture);
    }

    public void PlayCastleSound()
    {
        _audioSource.PlayOneShot(castle);
    }

    public void PlayCheckSound()
    {
        _audioSource.PlayOneShot(check);
    }

    public void PlayPromoteSound()
    {
        _audioSource.PlayOneShot(promote);
    }

    public void PlayNotifySound()
    {
        _audioSource.PlayOneShot(notify);
    }
}
