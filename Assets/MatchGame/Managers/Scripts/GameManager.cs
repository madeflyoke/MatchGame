using MatchGame.GamePlay.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MatchGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private PlayerController playerController;

        public event Action pointsChangedEvent;

        [SerializeField] private int correctAnswerPointsAdd;
        [SerializeField] private int wrongAnswerPointsRemove;

        public int CurrentPoints { get; private set; }

        private void Start()
        {
            RefreshPoints();
        }

        private void OnEnable()
        {
            playerController.isCorrectAnswerEvent += SetPoints;
        }
        private void OnDisable()
        {
            playerController.isCorrectAnswerEvent -= SetPoints;
        }

        public void RefreshPoints()
        {
            CurrentPoints = 0;
            pointsChangedEvent?.Invoke();
        }

        public void SetPoints(bool isCorrectAnswer)
        {
            CurrentPoints += (isCorrectAnswer ? correctAnswerPointsAdd : -wrongAnswerPointsRemove);
            pointsChangedEvent?.Invoke();
        }
    }
}

