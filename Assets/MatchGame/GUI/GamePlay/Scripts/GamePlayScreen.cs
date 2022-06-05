using MatchGame.GUI.GamePlay.Buttons;
using MatchGame.Managers;
using TMPro;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Diagnostics;
using System;
using System.Threading;
using DG.Tweening;

namespace MatchGame.GUI.GamePlay
{
    public class GamePlayScreen : BaseScreen
    {
        [Inject] private GameManager gameManager;
        [Inject] private GUIController gUIController;

        [SerializeField] private TMP_Text pointsField;
        [SerializeField] private TMP_Text comboField;
        [SerializeField] private PauseButton pauseButton;
        [SerializeField] private TMP_Text timeField;
        [SerializeField] private TMP_Text wakeUp;

        public PauseButton PauseButton { get => pauseButton; }

        public Stopwatch Stopwatch { get; private set; }

        private CancellationTokenSource cancellationTokenSource;
        private int prevPoints=0;
        private Color defaultPointsColor;

        private void Awake()
        {
            defaultPointsColor = pointsField.color;
            Refresh();  
        }     

        public void BlockHUD(bool isBlocked)
        {
            pauseButton.Button.enabled = !isBlocked;
        }

        private void OnEnable()
        {
            gameManager.pointsChangedEvent += SetPoints;
        }
        private void OnDisable()
        {
            gameManager.pointsChangedEvent -= SetPoints;
        }

        private void SetPoints()
        {
            int points = gameManager.PlayerPrefsData.CurrentPoints;
            pointsField.DOKill();
            if (points > prevPoints)
            {
                pointsField.DOColor(Color.green, 0.5f)
                    .OnComplete(() => pointsField.DOColor(defaultPointsColor, 0.2f));
            }
            else
            {             
                pointsField.DOColor(Color.red, 0.5f)
                    .OnComplete(() => pointsField.DOColor(defaultPointsColor, 0.2f));
            }
            prevPoints = points;
            pointsField.text = points.ToString();           
            comboField.text = gameManager.CorrectAnswersInRow > 0 ?
                "x" + gameManager.CorrectAnswersInRow.ToString() : string.Empty;
        }

        public void Refresh()
        {
            pointsField.text = "0";
            comboField.text = string.Empty;
            timeField.text = string.Empty;
            prevPoints = 0;
            pauseButton.gameObject.SetActive(true);
            if (cancellationTokenSource != null) { cancellationTokenSource.Cancel(); }
            cancellationTokenSource = new CancellationTokenSource();
            if (Stopwatch != null) { Stopwatch.Reset(); }
            Stopwatch = new Stopwatch();
        }

        public async void Timer()
        {
            while (true)
            {
                if (gUIController.IsPaused)
                {
                    await UniTask.Yield(cancellationTokenSource.Token);
                    continue;
                }
                timeField.text = Stopwatch.Elapsed.ToString("mm\\:ss\\.ff");
                await UniTask.Yield(cancellationTokenSource.Token);
            }
        }
    }
}

