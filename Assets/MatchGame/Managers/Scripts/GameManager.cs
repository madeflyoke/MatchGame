using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using MatchGame.GamePlay.Track;
using MatchGame.GUI.GamePlay.Buttons;
using MatchGame.GUI;

namespace MatchGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private GUIController gUIController;

        public event Action pointsChangedEvent;
        public event Action gameStartEvent;
        public event Action gameEndEvent;

        [SerializeField] private int correctAnswerPointsAdd;
        [SerializeField] private int wrongAnswerPointsRemove;
        [SerializeField] private int maxCombo = 10;

        public int CurrentPoints { get; private set; }
        public int CorrectAnswersInRow { get; private set; }

        private TrackController trackController;
        private PauseManager pauseManager;

        private void Awake()
        {
            pauseManager = new PauseManager(gUIController.PauseButton);
            trackController = FindObjectOfType<TrackController>();
            pauseManager.Pause(true);
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    gameEndEvent?.Invoke();
            //}
        }

        private void OnEnable()
        {
            gUIController.StartGameButton.startGameButtonEvent += StartGame;
            trackController.isCorrectAnswerEvent += SetPoints;
            pauseManager.OnEnable();
        }
        private void OnDisable()
        {
            gUIController.StartGameButton.startGameButtonEvent -= StartGame;
            trackController.isCorrectAnswerEvent -= SetPoints;
            pauseManager.OnDisable();
        }

        public void StartGame()
        {
            pauseManager.Pause(false);
            gameStartEvent?.Invoke();
        }

        private void RefreshPoints()
        {
            CurrentPoints = 0;
            CorrectAnswersInRow = 0;
            pointsChangedEvent?.Invoke();
        }

        private void SetPoints(bool isCorrectAnswer)
        {
            CorrectAnswersInRow = isCorrectAnswer ? Mathf.Clamp(CorrectAnswersInRow + 1, 0, maxCombo) : 0;
            CurrentPoints += (isCorrectAnswer ? correctAnswerPointsAdd * CorrectAnswersInRow : -wrongAnswerPointsRemove);
            //if (CurrentPoints<0)
            //{
            //    gameEndEvent?.Invoke();
            //    return;
            //}
            pointsChangedEvent?.Invoke();
        }

        private class PauseManager
        {
            private PauseButton pauseButton;
            private List<IPausable> pausables;

            public void Pause(bool isTrue)
            {
                foreach (var item in pausables)
                {
                    item.Pause(isTrue);
                }
            }

            public void OnEnable()
            {
                if (pauseButton != null) this.pauseButton.pauseButtonEvent += Pause;
            }
            public void OnDisable()
            {
                if (pauseButton != null) this.pauseButton.pauseButtonEvent -= Pause;
            }

            public PauseManager(PauseButton pauseButton)
            {
                this.pauseButton = pauseButton;
                pausables = new List<IPausable>();
                foreach (var obj in FindObjectsOfType<MonoBehaviour>().OfType<IPausable>())
                {
                    pausables.Add(obj);
                }
            }
        }
    }
}

