using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace MatchGame.GUI.EndGame
{
    public class EndGameScreen : BaseScreen
    {
        private const string scoreText = "MAX SCORE: ";
        private const string timeBonusText = "TIME BONUS";
        private const string finalScoreText = "FINAL: ";
        private const string recordText = "RECORD: ";

        [SerializeField] private GameObject newRecordLabel;
        [SerializeField] private TMP_Text finalScoreField;
        [SerializeField] private TMP_Text recordField;
        [SerializeField] private TMP_Text timeBonus;
        [SerializeField] private TMP_Text scoreField;

        private void Awake()
        {
            newRecordLabel.SetActive(false);
        }

        private void OnDisable()
        {
            newRecordLabel.SetActive(false);
        }

        public async void SetValues(string time, int timeBonus, int score, int recordScore)
        {
            int finalScore = score + timeBonus;
            this.timeBonus.text = timeBonusText + $"({time}): +" + timeBonus;
            finalScoreField.text = finalScoreText + finalScore.ToString();
            scoreField.text = scoreText + score.ToString();
            if (finalScore > recordScore)
            {
                recordField.text = recordText + finalScore.ToString();
                newRecordLabel.SetActive(true);
                Tween tween = newRecordLabel.transform.DOPunchScale(Vector3.one * 0.05f, 1.5f, vibrato: 1).SetEase(Ease.Linear).SetLoops(-1);
                while (gameObject.activeInHierarchy == true)
                {
                    await UniTask.Yield();
                }
                tween.Kill();
            }
            else
            {
                recordField.text = recordText + recordScore.ToString();
            }
        }
    }
}
