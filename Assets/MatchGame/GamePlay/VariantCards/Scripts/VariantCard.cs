using UnityEngine;


namespace MatchGame.GamePlay.VariantCards
{
    public class VariantCard : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public bool IsCorrect { get; set; } = false;

        public void SetSprite(Sprite sprite)
        {
            this.spriteRenderer.sprite = sprite;
        }       
        

    }
}

