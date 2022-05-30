using MatchGame.GamePlay.VariantCards;
using MatchGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using MatchGame.GamePlay.Player;

namespace MatchGame.GamePlay.Road
{
    public class RoadController : MonoBehaviour
    {
        [Inject] private Pooler pooler;
        [Inject] private PlayerController playerController;

        [SerializeField] private float cardsStep;
        [SerializeField] private Poolable variantCard;
        [SerializeField] private float speed;
        [SerializeField] private Transform movablePart;
        private Material mat;
        private List<VariantCardsController> activeCards;
        private CancellationTokenSource cancellationTokenSource;

        private void Awake()
        {
            mat = GetComponent<Material>();
            activeCards = new List<VariantCardsController>();
        }

        private void Initialize()
        {
            Vector3 step = new Vector3(0f, 0f, cardsStep);
            Vector3 startPos = playerController.transform.position + step;
            int startCount = 2;
            for (int i = 0; i < startCount; i++)
            {
                activeCards.Add(pooler.GetObjectFromPool(variantCard.gameObject))
            }
        }

        private void SetCards(int count)
        {
            
        }

        private async void Move()
        {
            while (true)
            {
                mat.mainTextureOffset += new Vector2(0f, Time.deltaTime * speed);
                
                await UniTask.Yield(cancellationToken: cancellationTokenSource.Token);
            }
        }
    }
}

