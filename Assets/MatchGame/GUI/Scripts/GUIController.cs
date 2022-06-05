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
            gameManager.gameplayStartEvent += StartGameLogic;
            gameManager.gameplayEndEvent += EndGameLogic;
            gameManager.refreshEvent += Refresh;
        }
        private void OnDisable()
        {
            gameManager.gameplayStartEvent -= StartGameLogic;
            gameManager.gameplayEndEvent -= EndGameLogic;
            gameManager.refreshEvent -= Refresh;
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
                gameManager.PlayerPrefsData.FinalTimeBonus,
                gameManager.PlayerPrefsData.MaxAchievedPoints,
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

