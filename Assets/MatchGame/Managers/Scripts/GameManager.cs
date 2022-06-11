using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using MatchGame.GamePlay.Track;
using MatchGame.GUI.GamePlay.Buttons;
using MatchGame.GUI;
using MatchGame.Utils;
using MatchGame.GamePlay.Player;
using Cysharp.Threading.Tasks;
using MatchGame.Repository;
using MatchGame.GUI.Tutorial.Buttons;
using MatchGame.GUI.EndGame.Buttons;

namespace MatchGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] public GUIController GUIController { get;private set; }
        [Inject] private CameraController cameraController;

        public event Action gameplayStartEvent;
        public event Action gameplayEndEvent;
        public event Action refreshEvent;
        public event Action launchGameEvent;

        [SerializeField] private int fps;
        [SerializeField] private ScoreController scoreController;
        [SerializeField] private TrackController trackController;
        public PlayerPrefsData PlayerPrefsData { get; private set; }
        public TrackController TrackController { get => trackController; }
        public ScoreController ScoreController { get => scoreController; }
        private PauseManager pauseManager;
        private PlayerVisualChanger playerVisualChanger;

        private void Awake()
        {          
            Application.targetFrameRate = fps;
            pauseManager = new PauseManager();
            PlayerPrefsData = new PlayerPrefsData(this);
            //PlayerPrefsData.ResetPlayerPrefs(); //debug score record
            playerVisualChanger = FindObjectOfType<PlayerVisualChanger>();
            pauseManager.Pause(true);
        }

        private void OnEnable()
        {
            ScoreController.endGameScoreEvent += EndGamePlay;
        }
        private void OnDisable()
        {
            ScoreController.endGameScoreEvent -= EndGamePlay;
        }

        private void StartPreparations() //try callback??
        {
            cameraController.SetPreparation().GetAwaiter()
                .OnCompleted(() => UniTask.Delay(250).GetAwaiter()
                .OnCompleted(() => playerVisualChanger.SetPreparation().GetAwaiter()
                .OnCompleted(() => UniTask.Delay(1000).GetAwaiter()
                .OnCompleted(() =>
                {
                    TrackController.SetPreparation();
                    if (GUIController.TutorialWasShown) StartGameplay();
                    GUIController.SetPreparations();
                }
                ))));
        }

        public void ButtonCall(BaseButton button)
        {
            var type = button.GetType();
            if (type == typeof(LaunchGameButton))
            {
                launchGameEvent?.Invoke();
                StartPreparations();
            }
            else if (type == typeof(TutorialConfirmButton))
            {
                StartGameplay();
            }
            else if (type == typeof(PauseButton))
            {
                pauseManager.Pause();
            }
            else if (type == typeof(RetryButton))
            {
                RetryGame();
            }
        }

        private void StartGameplay()
        {
            pauseManager.Pause(false);
            gameplayStartEvent?.Invoke();
        }

        private void EndGamePlay()
        {
            pauseManager.Pause(true);
            gameplayEndEvent?.Invoke();
        }

        private async void RetryGame()
        {
            refreshEvent?.Invoke();
            await UniTask.Delay(1000); // wait for everyone??
            StartPreparations();
        }

        private class PauseManager
        {
            private List<IPausable> pausables;
            private bool isPaused;

            public void Pause()
            {
                foreach (var item in pausables)
                {
                    item.Pause(!isPaused);
                }
                isPaused = !isPaused;
            }

            public void Pause(bool isPaused)
            {
                foreach (var item in pausables)
                {
                    item.Pause(isPaused);
                }
                this.isPaused = isPaused;
            }

            public PauseManager()
            {
                isPaused = false;
                pausables = new List<IPausable>();
                foreach (var obj in FindObjectsOfType<MonoBehaviour>(true).OfType<IPausable>())
                {

                    pausables.Add(obj);
                }
            }
        }
    }
}


