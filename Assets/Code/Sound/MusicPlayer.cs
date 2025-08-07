using UnityEngine;

namespace Sound
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip mainTheme;

        private void Start()
        {
            audioSource.clip = mainTheme;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
