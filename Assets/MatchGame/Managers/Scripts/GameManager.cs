using MatchGame.GamePlay.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using MatchGame.GamePlay.Track;

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
        [SerializeField] private int maxCombo = 10;
        //SET MULTIPLIER BY COMBO!!!!!!!!!!! maybe
        public int CurrentPoints { get; private set; }
        public int CorrectAnswersInRow { get; private set; }
        private List<IPausable> pausables;
        private TrackController trackController;

        private void Awake()
        {
            trackController = FindObjectOfType<TrackController>();
        }

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
            trackController.isCorrectAnswerEvent += SetPoints;
        }
        private void OnDisable()
        {
            trackController.isCorrectAnswerEvent -= SetPoints;
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
            CorrectAnswersInRow = 0;
            pointsChangedEvent?.Invoke();
        }

        private void SetPoints(bool isCorrectAnswer)
        {
            CorrectAnswersInRow = isCorrectAnswer ? Mathf.Clamp(CorrectAnswersInRow+1,0,maxCombo) : 0;
            CurrentPoints += (isCorrectAnswer ? correctAnswerPointsAdd*CorrectAnswersInRow : -wrongAnswerPointsRemove);
            //if (CurrentPoints<0)
            //{
            //    gameEndEvent?.Invoke();
            //    return;
            //}
            pointsChangedEvent?.Invoke();
        }
    }
}

