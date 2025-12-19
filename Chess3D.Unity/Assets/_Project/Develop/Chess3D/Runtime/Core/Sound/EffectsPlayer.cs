using UnityEngine;

namespace Chess3D.Runtime.Core.Sound
{
    public sealed class EffectsPlayer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource audioSource;

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

        public void PlayGameStartSound()
        {
            audioSource.PlayOneShot(gameStart);
        }

        public void PlayGameEndSound()
        {
            audioSource.PlayOneShot(gameEnd);
        }

        public void PlayTenSecondsSound()
        {
            audioSource.PlayOneShot(tenSeconds);
        }

        public void PlayMoveSound()
        {
            audioSource.PlayOneShot(move);
        }

        public void PlayMoveOpponentSound()
        {
            audioSource.PlayOneShot(moveOpponent);
        }

        public void PlayCaptureSound()
        {
            audioSource.PlayOneShot(capture);
        }

        public void PlayCastleSound()
        {
            audioSource.PlayOneShot(castle);
        }

        public void PlayCheckSound()
        {
            audioSource.PlayOneShot(check);
        }

        public void PlayPromoteSound()
        {
            audioSource.PlayOneShot(promote);
        }

        public void PlayNotifySound()
        {
            audioSource.PlayOneShot(notify);
        }
    }
}
