using MatchGame.Managers;
using UnityEngine;
using Zenject;

namespace MatchGame.Audio
{
    public class AudioManager : MonoBehaviour
    {
        enum MusicTheme
        {
            Main
        }

        [Inject] private GameManager gameManager;

        [SerializeField] private AudioClip mainTheme;

        private AudioSource audioSource;
        private MusicTheme currentTheme;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            gameManager.launchGameEvent += () => SetTheme(MusicTheme.Main);
        }

        private void SetTheme(MusicTheme theme)
        {
            currentTheme = theme;
            switch (theme)
            {
                case MusicTheme.Main:
                    audioSource.loop = true;
                    audioSource.clip = mainTheme;
                    audioSource.Play();
                    break;
                default:
                    break;
            }
        }
    }

}
