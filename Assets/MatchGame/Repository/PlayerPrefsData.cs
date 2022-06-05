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
        public int MaxAchievedPoints { get; private set; } = 0;
        public int FinalTimeBonus { get; private set; } = 0;
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
            MaxAchievedPoints = CurrentPoints > MaxAchievedPoints ? CurrentPoints : MaxAchievedPoints;
        }

        public void SetRecordScore()
        {
            FinalTimeBonus = ((int)gameManager.GUIController.GamePlayScreen.Stopwatch.ElapsedMilliseconds / 1000)
                * gameManager.BonusPointsBySecond;
            if (MaxAchievedPoints+FinalTimeBonus>RecordScore)
            {
                PlayerPrefs.SetInt(RecordScoreKey, MaxAchievedPoints+FinalTimeBonus);
            }
        }

        public void Refresh()
        {
            CurrentPoints = 0;
            MaxAchievedPoints = 0;
            FinalTimeBonus = 0;
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefs.SetInt(RecordScoreKey, 0);
            RecordScore = 0;
        }
    }
}
