using MatchGame.GUI.GamePlay.Buttons;
using MatchGame.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace MatchGame.GUI.GamePlay
{
    public class GamePlayScreen : BaseScreen
    {
        [Inject] private GameManager gameManager;

        [SerializeField] private TMP_Text pointsField;
        [SerializeField] private TMP_Text comboField;
        [SerializeField] private PauseButton pauseButton;

        public PauseButton PauseButton { get => pauseButton; }

        private void Awake()
        {
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
            pointsField.text = gameManager.CurrentPoints.ToString();
            comboField.text = gameManager.CorrectAnswersInRow > 0 ?
                "x" + gameManager.CorrectAnswersInRow.ToString() : string.Empty;
        }

        public void Refresh()
        {
            pointsField.text = "0";
            comboField.text = string.Empty;
            pauseButton.gameObject.SetActive(true);
        }
    }
}

