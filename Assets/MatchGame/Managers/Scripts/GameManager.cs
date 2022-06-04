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
        [Inject] private GUIController gUIController;

        public event Action pointsChangedEvent;

        public event Action gameplayStartEvent;
        public event Action gameplayEndEvent;
        public event Action refreshEvent;

        [SerializeField] private int correctAnswerPointsAdd;
        [SerializeField] private int wrongAnswerPointsRemove;
        [SerializeField] private int maxCombo = 10;

        public int CurrentPoints { get; private set; }
        public int MaxAchievedPoints { get; private set; }
        public int CorrectAnswersInRow { get; private set; }
        public PlayerPrefsData PlayerPrefsData { get; private set; }

        private TrackController trackController;
        private PauseManager pauseManager;
        private CameraController cameraController;
        private PlayerVisualChanger playerVisualChanger;

        private void Awake()
        {
            Application.targetFrameRate = 240;
            pauseManager = new PauseManager();
            PlayerPrefsData = new PlayerPrefsData();
            PlayerPrefsData.Refresh();
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
                    if (gUIController.TutorialWasShown) StartGameplay();                  
                    gUIController.SetPreparations();
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
            CurrentPoints = 0;
            CorrectAnswersInRow = 0;
        }

        private void SetPoints(bool isCorrectAnswer)
        {
            CorrectAnswersInRow = isCorrectAnswer ? Mathf.Clamp(CorrectAnswersInRow + 1, 0, maxCombo) : 0;
            CurrentPoints += (isCorrectAnswer ? correctAnswerPointsAdd * CorrectAnswersInRow : -wrongAnswerPointsRemove);
            MaxAchievedPoints = CurrentPoints > MaxAchievedPoints ? CurrentPoints : MaxAchievedPoints;
            if (CurrentPoints < 0)
            {
                if (MaxAchievedPoints > PlayerPrefsData.RecordScore) 
                { 
                    PlayerPrefsData.SetRecordScore(MaxAchievedPoints); 
                }
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
                foreach (var obj in FindObjectsOfType<MonoBehaviour>().OfType<IPausable>())
                {
                    pausables.Add(obj);
                }
            }
        }
    }   
}


