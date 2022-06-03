using MatchGame.GamePlay.Category;
using UnityEngine;
using Zenject;
using MatchGame.GamePlay.Player;

namespace MatchGame.GamePlay.VariantCards
{
    public class VariantCardsController : MonoBehaviour
    {
        [Inject] private CategoryData categoryData;
        [Inject] private PlayerController playerController;

        [SerializeField] private VariantCard leftCard;
        [SerializeField] private VariantCard rightCard;

        public VariantCard LeftCard { get => leftCard; }
        public VariantCard RightCard { get => rightCard; }


        private void OnEnable()
        {
            SetVariants(playerController.CurrentType);
        }
        private void OnDisable()
        {
            Refresh();
        }

        private void SetVariants(CategoryType playerType)
        {
            var correctCard = Random.Range(0, 2) == 0 ? leftCard : rightCard;
            correctCard.IsCorrect = true;
            var wrongCard = correctCard == leftCard ? rightCard : leftCard;
            wrongCard.IsCorrect = false;
            correctCard.SetSprite(categoryData.GetRandomCorrectSprite(playerType));
            wrongCard.SetSprite(categoryData.GetRandomWrongSprite(playerType));
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

