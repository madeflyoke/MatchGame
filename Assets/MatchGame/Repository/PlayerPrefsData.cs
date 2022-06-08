using MatchGame.Managers;
using UnityEngine;
using Zenject;

namespace MatchGame.Repository
{
    public class PlayerPrefsData
    {
        private const string RecordScoreKey = "RecordScoreKey";
        public int RecordScore { get; private set; }
        public int CurrentPoints { get; private set; } = 0;
        public int AttemptMaxAchievedPoints { get; private set; } = 0;
        public int EndGameFinalTimeBonus { get; private set; } = 0;

        private GameManager gameManager;

        public PlayerPrefsData(GameManager gameManager)
        {
            this.gameManager = gameManager;
            if (PlayerPrefs.HasKey(RecordScoreKey) == true)
            {
                RecordScore = PlayerPrefs.GetInt(RecordScoreKey);
            }
            else
            {
                PlayerPrefs.SetInt(RecordScoreKey, 0); 
                RecordScore = 0; 
            }
        }

        public void ChangePoints(int additionalPoints)
        {
            CurrentPoints += additionalPoints;
            AttemptMaxAchievedPoints = CurrentPoints > AttemptMaxAchievedPoints ? CurrentPoints : AttemptMaxAchievedPoints;
        }

        public void SetRecordScore()
        {
            EndGameFinalTimeBonus = ((int)gameManager.GUIController.GamePlayScreen.Stopwatch.ElapsedMilliseconds / 1000)
                * gameManager.ScoreController.BonusPointsBySecond;
            var finalScore = AttemptMaxAchievedPoints + EndGameFinalTimeBonus;
            if (finalScore>RecordScore)
            {
                PlayerPrefs.SetInt(RecordScoreKey, AttemptMaxAchievedPoints+EndGameFinalTimeBonus);
                RecordScore = finalScore;
            }
        }

        public void RefreshPoints()
        {
            CurrentPoints = 0;
            AttemptMaxAchievedPoints = 0;
            EndGameFinalTimeBonus = 0;
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefs.SetInt(RecordScoreKey, 0);
            RecordScore = 0;
        }
    }
}
