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
                        Debug.Log("Card Category instance is failed due to unknown CategoryType");
                        sprites = null;
                        return;
                }
            }
        }

        public struct PlayerCategory
        {
            public CategoryType type;
            public List<GameObject> visuals;

            public PlayerCategory(CategoryType type)
            {
                this.type = type;
                switch (type)
                {
                    case CategoryType.Food:
                        visuals = Resources.LoadAll<GameObject>("PlayerVisuals/Food").ToList();
                        break;
                    case CategoryType.Vehicle:
                        visuals = Resources.LoadAll<GameObject>("PlayerVisuals/Vehicle").ToList();
                        break;
                    default:
                        Debug.Log("Player Category instance is failed due to unknown CategoryType");
                        visuals = null;
                        return;
                }
            }
        }

        public List<CardCategory> CardCategories { get; private set; }
        public CardCategory CardFood { get => cardFood; }
        public CardCategory CardVehicle { get => cardVehicle; }
        private CardCategory cardFood;
        private CardCategory cardVehicle;

        public List<PlayerCategory> PlayerCategories { get; private set; }
        public PlayerCategory PlayerFood { get => playerFood; }
        public PlayerCategory PlayerVehicle { get => playerVehicle;}
        private PlayerCategory playerFood;
        private PlayerCategory playerVehicle;

        public CategoryData()
        {
            cardFood = new CardCategory(CategoryType.Food);
            cardVehicle = new CardCategory(CategoryType.Vehicle);
            CardCategories = new List<CardCategory>();
            CardCategories.Add(cardFood);
            CardCategories.Add(cardVehicle);

            playerFood = new PlayerCategory(CategoryType.Food);
            playerVehicle = new PlayerCategory(CategoryType.Vehicle);
            PlayerCategories = new List<PlayerCategory>();
            PlayerCategories.Add(playerFood);
            PlayerCategories.Add(playerVehicle);
        }

    }

}
