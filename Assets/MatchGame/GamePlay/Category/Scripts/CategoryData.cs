using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MatchGame.GamePlay.Category
{
    public class CategoryData
    {   
        public struct CardCategory
        {
            public CategoryType type;
            public List<Sprite> sprites;

            public CardCategory(CategoryType type)
            {
                this.type = type;
                switch (type)
                {
                    case CategoryType.Food:
                        sprites = Resources.LoadAll<Sprite>("Sprites/Food").ToList();
                        break;
                    case CategoryType.Vehicle:
                        sprites = Resources.LoadAll<Sprite>("Sprites/Vehicle").ToList();
                        break;
                    default:
                        Debug.Log("Category instance is failed due to unknown CategoryType");
                        sprites = null;
                        return;
                }
            }
        }

        public CardCategory Food { get => food; }
        public CardCategory Vehicle { get => vehicle; }
        public List<CardCategory> Categories { get; private set; }

        private CardCategory food;
        private CardCategory vehicle;

        public CategoryData()
        {
            food = new CardCategory(CategoryType.Food);
            vehicle = new CardCategory(CategoryType.Vehicle);
            Categories = new List<CardCategory>();
            Categories.Add(food);
            Categories.Add(vehicle);
        }

    }

}
