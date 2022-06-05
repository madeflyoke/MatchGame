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

        public event Action pointsChangedEvent;

        public event Action gameplayStartEvent;
        public event Action gameplayEndEvent;
        public event Action refreshEvent;

        [SerializeField] private int correctAnswerPointsAdd;      //estimate max points can be achieved 4775 by 100 steps
        [SerializeField] private int wrongAnswerPointsRemove;
        [SerializeField] private int bonusPointsBySecond = 10;
        [SerializeField] private int maxCombo = 10;

        public int CorrectAnswersInRow { get; private set; }

        public PlayerPrefsData PlayerPrefsData { get; private set; }
        public int BonusPointsBySecond { get => bonusPointsBySecond; }

        private TrackController trackController;
        private PauseManager pauseManager;
        private CameraController cameraController;
        private PlayerVisualChanger playerVisualChanger;

        private void Awake()
        {          
            Application.targetFrameRate = 240;
            pauseManager = new PauseManager();
            PlayerPrefsData = new PlayerPrefsData(this);
            //PlayerPrefsData.ResetPlayerPrefs();
            trackController = FindObjectOfType<TrackController>();
            cameraController = FindObjectOfType<CameraController>();
            playerVisualChanger = FindObjectOfType<PlayerVisualChanger>();
            pauseManager.Pause(true);
        }

        private void OnEnable()
        {
            trackController.isCorrectAnswerEvent += SetPoints;
        }
        private void OnDisable()
        {
            trackController.isCorrectAnswerEvent -= SetPoints;
        }

        private void StartPreparations() //try callback!!!!!!!!!
        {
            cameraController.SetPreparation().GetAwaiter()
                .OnCompleted(() => UniTask.Delay(250).GetAwaiter()
                .OnCompleted(() => playerVisualChanger.SetPreparation().GetAwaiter()
                .OnCompleted(() => UniTask.Delay(1000).GetAwaiter()
                .OnCompleted(() =>
                {
                    trackController.SetPreparation();
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
            RefreshPoints();
            refreshEvent?.Invoke();
            await UniTask.Delay(1000); // wait for everyone??
            StartPreparations();
        }

        private void RefreshPoints()
        {
            PlayerPrefsData.Refresh();
            CorrectAnswersInRow = 0;
        }

        private void SetPoints(bool isCorrectAnswer)
        {
            CorrectAnswersInRow = isCorrectAnswer ? Mathf.Clamp(CorrectAnswersInRow + 1, 0, maxCombo) : 0;
            int points = isCorrectAnswer ? correctAnswerPointsAdd * CorrectAnswersInRow : -wrongAnswerPointsRemove;
            PlayerPrefsData.ChangePoints(points);
            if (PlayerPrefsData.CurrentPoints < 0)
            {
                PlayerPrefsData.SetRecordScore();
                EndGamePlay();
                return;
            }
            pointsChangedEvent?.Invoke();
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


