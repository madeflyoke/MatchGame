using MatchGame.GamePlay.Category;
using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
            leftCard.Initialize();
            rightCard.Initialize();
        }

        private void OnEnable()
        {
            playerController.playerCategoryChangedEvent += SetVariants;
        }
        private void OnDisable()
        {
            playerController.playerCategoryChangedEvent -= SetVariants;
        }

        private void SetVariants(CategoryType playerType)
        {
            var correctCard = Random.Range(0, 2) == 0 ? leftCard : rightCard;
            var wrongCard = correctCard == leftCard ? rightCard : leftCard;
            correctCard.gameObject.layer = (int)Layer.CardCorrectAnswer;
            wrongCard.gameObject.layer = (int)Layer.CardWrongAnswer;
            correctCard.SetSprite(categoryData.Categories.Where((x) => x.type == playerType)
                .Select((x) => x.sprites[Random.Range(0, x.sprites.Count)]).FirstOrDefault());
            wrongCard.SetSprite(categoryData.Categories.Where((x) => x.type != playerType)
                .Select((x) => x.sprites[Random.Range(0, x.sprites.Count)]).FirstOrDefault());
        }
    }
}

