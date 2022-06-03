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
                    case CategoryType.Human:
                        sprites = Resources.LoadAll<Sprite>("Sprites/Human").ToList();
                        break;
                    case CategoryType.Toys:
                        sprites = Resources.LoadAll<Sprite>("Sprites/Toys").ToList();
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
                    case CategoryType.Human:
                        visuals = Resources.LoadAll<GameObject>("PlayerVisuals/Human").ToList();
                        break;
                    case CategoryType.Toys:
                        visuals = Resources.LoadAll<GameObject>("PlayerVisuals/Toys").ToList();
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
        public CardCategory CardHuman { get => cardHuman; }
        public CardCategory CardToys { get => cardToys; }
        private CardCategory cardFood;
        private CardCategory cardVehicle;
        private CardCategory cardHuman;
        private CardCategory cardToys;

        public List<PlayerCategory> PlayerCategories { get; private set; }
        public PlayerCategory PlayerFood { get => playerFood; }
        public PlayerCategory PlayerVehicle { get => playerVehicle; }
        public PlayerCategory PlayerHuman { get => playerHuman; }
        public PlayerCategory PlayerToys { get => playerToys; }
        private PlayerCategory playerFood;
        private PlayerCategory playerVehicle;
        private PlayerCategory playerHuman;
        private PlayerCategory playerToys;

        public CategoryData()
        {
            cardFood = new CardCategory(CategoryType.Food);
            cardVehicle = new CardCategory(CategoryType.Vehicle);
            cardHuman = new CardCategory(CategoryType.Human);
            cardToys = new CardCategory(CategoryType.Toys);
            CardCategories = new List<CardCategory>();
            CardCategories.Add(cardFood);
            CardCategories.Add(cardVehicle);
            CardCategories.Add(cardHuman);
            CardCategories.Add(cardToys);

            playerFood = new PlayerCategory(CategoryType.Food);
            playerVehicle = new PlayerCategory(CategoryType.Vehicle);
            playerHuman = new PlayerCategory(CategoryType.Human);
            playerToys = new PlayerCategory(CategoryType.Toys);
            PlayerCategories = new List<PlayerCategory>();
            PlayerCategories.Add(playerFood);
            PlayerCategories.Add(playerVehicle);
            PlayerCategories.Add(playerHuman);
            PlayerCategories.Add(playerToys);
        }

        public Sprite GetRandomWrongSprite(CategoryType type)
        {
            var wrongCategories = CardCategories.Where((x) => x.type != type);
            CardCategory concreteCategory = wrongCategories.ElementAt(Random.Range(0, wrongCategories.Count()));
            return concreteCategory.sprites[Random.Range(0, concreteCategory.sprites.Count)];
        }

        public Sprite GetRandomCorrectSprite(CategoryType type)
        {
            return CardCategories.Where((x) => x.type == type)
                 .Select((x) => x.sprites[Random.Range(0, x.sprites.Count)]).FirstOrDefault();
        }
    }
}
