using UnityEngine;
using TMPro;
using Zenject;
using MatchGame.Managers;
using MatchGame.GUI.GamePlay.Buttons;

namespace MatchGame.GUI
{
    public class GUIController : MonoBehaviour
    {
        [Inject] private GameManager gameManager;

        [SerializeField] private TMP_Text pointsField;
        [SerializeField] private TMP_Text comboField;
        [SerializeField] private PauseButton pauseButton;
        [SerializeField] private StartGameButton startGameButton;

        public PauseButton PauseButton { get => pauseButton; }
        public StartGameButton StartGameButton { get => startGameButton; }

        private void Awake()
        {
            HideHUD(true);
        }

        private void OnEnable()
        {
            gameManager.gameStartEvent += StartGameLogic;
            gameManager.pointsChangedEvent += SetPoints;
        }
        private void OnDisable()
        {
            gameManager.pointsChangedEvent -= SetPoints;
            gameManager.gameStartEvent -= StartGameLogic;
        }

        private void HideHUD(bool isHidden)
        {
            pointsField.gameObject.SetActive(!isHidden);
            comboField.gameObject.SetActive(!isHidden);
            pauseButton.gameObject.SetActive(!isHidden);
        }

        private void StartGameLogic()
        {
            startGameButton.gameObject.SetActive(false);
            SetPoints();
            HideHUD(false);
        }

        private void SetPoints()
        {
            pointsField.text = gameManager.CurrentPoints.ToString();
            comboField.text = gameManager.CorrectAnswersInRow>0? 
                "x"+gameManager.CorrectAnswersInRow.ToString():string.Empty;
        }
    }
}

