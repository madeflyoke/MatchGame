using UnityEngine;
using Zenject;
using MatchGame.Managers;
using MatchGame.GUI.EndGame;
using MatchGame.GUI.GamePlay;
using MatchGame.GUI.Tutorial;

namespace MatchGame.GUI
{
    public class GUIController : MonoBehaviour, IPausable
    {
        [Inject] private GameManager gameManager;

        [SerializeField] private EndGameScreen endGameScreen;
        [SerializeField] private GamePlayScreen gamePlayScreen;
        [SerializeField] private TutorialController tutorialController;
        [SerializeField] private GameObject titleObj;

        public bool TutorialWasShown { get; private set; }
        public bool IsPaused { get; set; }
        public GamePlayScreen GamePlayScreen { get => gamePlayScreen; }

        private void Awake()
        {
            gamePlayScreen.Hide(true);
            endGameScreen.Hide(true);
            tutorialController.Hide(true);
        }

        private void OnEnable()
        {
            gameManager.launchGameEvent += LaunchGameLogic;
            gameManager.gameplayStartEvent += StartGameLogic;
            gameManager.gameplayEndEvent += EndGameLogic;
            gameManager.refreshEvent += Refresh;
        }
        private void OnDisable()
        {
            gameManager.launchGameEvent -= LaunchGameLogic;
            gameManager.gameplayStartEvent -= StartGameLogic;
            gameManager.gameplayEndEvent -= EndGameLogic;
            gameManager.refreshEvent -= Refresh;
        }

        public void LaunchGameLogic()
        {
            titleObj.SetActive(false);
        }

        public void SetPreparations()
        {
            gamePlayScreen.Hide(false);
            if (TutorialWasShown == false)
            {
                tutorialController.Hide(false);
                TutorialWasShown = true;
                gamePlayScreen.BlockHUD(true);
            }
            gamePlayScreen.Timer();
        }

        private void StartGameLogic()
        {
            tutorialController.Hide(true);
            gamePlayScreen.BlockHUD(false);
        }

        private void EndGameLogic()
        {
            gamePlayScreen.Hide(true);
            gamePlayScreen.BlockHUD(true);
            endGameScreen.Hide(false);
            endGameScreen.SetValues(gamePlayScreen.Stopwatch.Elapsed.ToString("mm\\:ss"),
                gameManager.PlayerPrefsData.EndGameFinalTimeBonus,
                gameManager.PlayerPrefsData.AttemptMaxAchievedPoints,
                gameManager.PlayerPrefsData.RecordScore);
        }

        private void Refresh()
        {
            endGameScreen.Hide(true);
            gamePlayScreen.Refresh();
        }

        public void Pause(bool isPaused)
        {
            IsPaused = isPaused;
            if (isPaused)
            {
                gamePlayScreen.Stopwatch.Stop();
            }
            else
            {
                gamePlayScreen.Stopwatch.Start();
            }
        }
    }
}

