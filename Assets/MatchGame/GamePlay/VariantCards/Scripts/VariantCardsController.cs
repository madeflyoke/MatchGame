using MatchGame.GamePlay.Category;
using UnityEngine;
using Zenject;
using System.Linq;
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

        //private void Awake()
        //{
        //    leftCard.Initialize();
        //    rightCard.Initialize();
        //}

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
            //correctCard.gameObject.layer = (int)Layer.CardCorrectAnswer;
            //wrongCard.gameObject.layer = (int)Layer.CardWrongAnswer;
            correctCard.SetSprite(categoryData.CardCategories.Where((x) => x.type == playerType)
                .Select((x) => x.sprites[Random.Range(0, x.sprites.Count)]).FirstOrDefault());
            wrongCard.SetSprite(categoryData.CardCategories.Where((x) => x.type != playerType)
                .Select((x) => x.sprites[Random.Range(0, x.sprites.Count)]).FirstOrDefault());
        }

        //public void AnswerGotLogic()
        //{
        //    leftCard.gameObject.layer = (int)Layer.Default;
        //    rightCard.gameObject.layer = (int)Layer.Default;
        //}

        private void Refresh()
        {
            leftCard.SetSprite(null);
            rightCard.SetSprite(null);
            leftCard.IsCorrect = false;
            rightCard.IsCorrect = false;
            //leftCard.gameObject.layer = (int)Layer.Default;
            //rightCard.gameObject.layer = (int)Layer.Default;
        }
    }
}

