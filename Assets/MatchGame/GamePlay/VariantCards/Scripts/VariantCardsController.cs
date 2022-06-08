using MatchGame.GamePlay.Category;
using UnityEngine;
using Zenject;
using MatchGame.GamePlay.Player;

namespace MatchGame.GamePlay.VariantCards
{
    public class VariantCardsController : MonoBehaviour
    {
        private static int s_PrevRandomSide=-1;
        private static int s_CurrentCorrectCardsInRow = 0;

        [Inject] private CategoryData categoryData;
        [Inject] private PlayerController playerController;

        [SerializeField] private VariantCard leftCard;
        [SerializeField] private VariantCard rightCard;

        public VariantCard LeftCard { get => leftCard; }
        public VariantCard RightCard { get => rightCard; }

        private int maxCorrectCardsInRow = 5;

        private void OnDisable()
        {
            Refresh();
        }

        public void SetVariants()
        {          
            int rnd = Random.Range(0, 2);
            var correctCard = rnd == 0 ? leftCard : rightCard;

            if (s_PrevRandomSide == rnd)
            {
                s_CurrentCorrectCardsInRow++;
                if (s_CurrentCorrectCardsInRow == maxCorrectCardsInRow - 1)
                {
                    correctCard = rnd == 0 ? rightCard : leftCard;
                    s_PrevRandomSide = correctCard == rightCard ? 0 : 1;
                    s_CurrentCorrectCardsInRow = 0;
                }
            }
            else { s_PrevRandomSide = rnd; }
            correctCard.IsCorrect = true;
            var wrongCard = correctCard == leftCard ? rightCard : leftCard;
            wrongCard.IsCorrect = false;
            correctCard.SetSprite(categoryData.GetRandomCorrectSprite(playerController.CurrentType));
            wrongCard.SetSprite(categoryData.GetRandomWrongSprite(playerController.CurrentType));
            gameObject.SetActive(true);
        }

        private void Refresh()
        {
            leftCard.SetSprite(null);
            rightCard.SetSprite(null);
            leftCard.IsCorrect = false;
            rightCard.IsCorrect = false;
        }
    }
}

