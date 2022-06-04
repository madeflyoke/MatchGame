using UnityEngine;

namespace MatchGame.Repository
{
    public class PlayerPrefsData
    {
        private const string RecordScoreKey = "RecordScoreKey";
        public int RecordScore { get;private set; }

        public PlayerPrefsData()
        {
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

        public void SetRecordScore(int score)
        {
            PlayerPrefs.SetInt(RecordScoreKey, score);
        }

        public void Refresh()
        {
            PlayerPrefs.SetInt(RecordScoreKey, 0);
            RecordScore = 0;
        }
    }
}
