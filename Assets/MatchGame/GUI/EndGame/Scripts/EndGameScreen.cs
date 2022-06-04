using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace MatchGame.GUI.EndGame
{
    public class EndGameScreen : BaseScreen
    {
        private const string scoreText = "MAX SCORE: ";
        private const string recordText = "RECORD: ";

        [SerializeField] private GameObject newRecordLabel;
        [SerializeField] private TMP_Text scoreField;
        [SerializeField] private TMP_Text recordField;

        private void Awake()
        {
            newRecordLabel.SetActive(false);
        }

        private void OnDisable()
        {
            newRecordLabel.SetActive(false);
        }

        public async void SetValues(int score, int recordScore)
        {
            scoreField.text = scoreText + score.ToString();
            recordField.text = recordText + recordScore.ToString();
            if (score > recordScore)
            {
                newRecordLabel.SetActive(true);
                Tween tween = newRecordLabel.transform.DOPunchScale(Vector3.one * 0.05f, 1.5f, vibrato: 1).SetEase(Ease.Linear).SetLoops(-1);
                while (gameObject.activeInHierarchy == true)
                {
                    await UniTask.Yield();
                }
                tween.Kill();
            }
        }
    }
}
