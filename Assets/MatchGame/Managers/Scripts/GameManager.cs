using MatchGame.GamePlay.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;

namespace MatchGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private PlayerController playerController;

        public event Action pointsChangedEvent;
        public event Action gameStartEvent;
        public event Action gameEndEvent;

        [SerializeField] private int correctAnswerPointsAdd;
        [SerializeField] private int wrongAnswerPointsRemove;
        //SET MULTIPLIER BY COMBO!!!!!!!!!!!
        public int CurrentPoints { get; private set; }
        public int CorrectAnswers { get; private set; }
        private List<IPausable> pausables;

        private void Start()
        {
            pausables = new List<IPausable>();
            foreach (var obj in FindObjectsOfType<MonoBehaviour>().OfType<IPausable>())
            {
                pausables.Add(obj);
            }
            gameStartEvent?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause(true);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                Pause(false);
            }

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    gameEndEvent?.Invoke();
            //}
        }

        private void OnEnable()
        {
            playerController.isCorrectAnswerEvent += SetPoints;
        }
        private void OnDisable()
        {
            playerController.isCorrectAnswerEvent -= SetPoints;
        }

        private void Pause(bool isTrue)
        {
            foreach (var item in pausables)
            {
                item.Pause(isTrue);
            }
        }

        private void RefreshPoints()
        {
            CurrentPoints = 0;
            pointsChangedEvent?.Invoke();
        }

        private void SetPoints(bool isCorrectAnswer)
        {
            CorrectAnswers += isCorrectAnswer ? 1 : 0;
            CurrentPoints += (isCorrectAnswer ? correctAnswerPointsAdd : -wrongAnswerPointsRemove);
            //if (CurrentPoints<0)
            //{
            //    gameEndEvent?.Invoke();
            //    return;
            //}
            pointsChangedEvent?.Invoke();
        }
    }
}

