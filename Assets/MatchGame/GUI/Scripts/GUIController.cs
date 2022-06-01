using UnityEngine;
using TMPro;
using Zenject;
using MatchGame.Managers;

namespace MatchGame.GUI
{
    public class GUIController : MonoBehaviour
    {
        [Inject] private GameManager gameManager;

        [SerializeField] private TMP_Text pointsField;
        [SerializeField] private TMP_Text comboField;

        private void OnEnable()
        {
            gameManager.pointsChangedEvent += SetPoints;
        }
        private void OnDisable()
        {
            gameManager.pointsChangedEvent -= SetPoints;
        }

        public void SetPoints()
        {
            pointsField.text = gameManager.CurrentPoints.ToString();
            comboField.text = gameManager.CorrectAnswersInRow>0? 
                "x "+gameManager.CorrectAnswersInRow.ToString():string.Empty;
        }
    }
}

