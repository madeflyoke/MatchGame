using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MatchGame.Managers
{
    public class ScoreController : MonoBehaviour
    {
        [Inject] private GameManager gameManager;

        public event Action pointsChangedEvent;
        public event Action endGameScoreEvent;

        [SerializeField] private int startGameAllowedWrongAnswers;
        [SerializeField] private int defaultCorrectAnswerPointsAdd;      //estimate max points can be achieved 4775 by 100 steps
        [SerializeField] private int defaultWrongAnswerPointsRemove;
        [SerializeField] private int bonusPointsBySecond = 10;
        [SerializeField] private int maxCombo = 10;

        [SerializeField] private int pointsModifier = 1;
        [SerializeField] private int correctAnswersToModify;

        public int CorrectAnswersInRow { get; private set; }
        public int BonusPointsBySecond { get => bonusPointsBySecond; }
        private int currentCorrectAnswers;
        private int currentPointsModifier = 1;
        private int currentAllowedWrongAnswers=3;

        private void Awake()
        {
            currentAllowedWrongAnswers = startGameAllowedWrongAnswers;
        }

        private void OnEnable()
        {
            gameManager.TrackController.isCorrectAnswerEvent += SetPoints;
            gameManager.refreshEvent += RefreshPoints;
        }
        private void OnDisable()
        {
            gameManager.TrackController.isCorrectAnswerEvent -= SetPoints;
            gameManager.refreshEvent -= RefreshPoints;
        }

        private void SetPoints(bool isCorrectAnswer)
        {
            currentCorrectAnswers += isCorrectAnswer ? 1 : 0;
            if (currentCorrectAnswers == correctAnswersToModify)
            {
                currentCorrectAnswers = 0;
                currentPointsModifier += pointsModifier;
            }
            CorrectAnswersInRow = isCorrectAnswer ? Mathf.Clamp(CorrectAnswersInRow + 1, 0, maxCombo) : 0;
            int points = isCorrectAnswer ? defaultCorrectAnswerPointsAdd * CorrectAnswersInRow * currentPointsModifier
                : -defaultWrongAnswerPointsRemove * (currentPointsModifier+1); //punish with wrong answers
            if (isCorrectAnswer == false && currentAllowedWrongAnswers > 0 && currentPointsModifier == 1)
            {
                points = 0;        //allowed wrong answers (mostly for start game to looks more nicely)
                currentAllowedWrongAnswers--;
            }
            gameManager.PlayerPrefsData.ChangePoints(points);
            if (gameManager.PlayerPrefsData.CurrentPoints < 0)
            {
                gameManager.PlayerPrefsData.SetRecordScore();
                endGameScoreEvent?.Invoke();
                return;
            }
            pointsChangedEvent?.Invoke();
        }

        private void RefreshPoints()
        {
            gameManager.PlayerPrefsData.RefreshPoints();
            currentAllowedWrongAnswers = startGameAllowedWrongAnswers;
            CorrectAnswersInRow = 0;
            currentCorrectAnswers = 0;
            currentPointsModifier = 1;
        }
    }
}
