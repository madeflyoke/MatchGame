using UnityEngine;


namespace MatchGame.GamePlay.VariantCards
{
    [RequireComponent(typeof(Collider))]
    public class VariantCard : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void Initialize()
        {
            gameObject.layer = (int)Layer.CardWrongAnswer;
        }

        public void SetSprite(Sprite sprite)
        {
            this.spriteRenderer.sprite = sprite;
        }       
        

    }
}

