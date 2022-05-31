using MatchGame.GamePlay.VariantCards;
using MatchGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using MatchGame.GamePlay.Player;

namespace MatchGame.GamePlay.Track
{
    public class TrackController : MonoBehaviour, IPausable
    {
        [Inject] private Pooler pooler;
        [Inject] private PlayerController playerController;
        [Inject] private GameManager gameManager;

        [SerializeField] private float cardsStep;
        [SerializeField] private float roadSpeed;
        [SerializeField] private Poolable variantCard;
        [SerializeField] private Material mat;

        public bool IsPaused { get; set; }
        private CancellationTokenSource movingCancellationTokenSource;
        private CancellationTokenSource stuffCancellationTokenSource;
        private Queue<GameObject> cardsQue;
        private Dictionary<GameObject, VariantCardsController> cardTypes;
        private float CardSpeed { get => roadSpeed * 20f; }

        private void Awake()
        {
            cardsQue = new Queue<GameObject>();
            cardTypes = new Dictionary<GameObject, VariantCardsController>();
            movingCancellationTokenSource = new CancellationTokenSource();
            stuffCancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            gameManager.pointsChangedEvent += SetCards;
            gameManager.gameStartEvent += Move;
            gameManager.gameStartEvent += InitCards;
            //gameManager.gameEndEvent += Stop;
            pooler.capacityChangedEvent += SyncCardTypes;
        }
        private void OnDisable()
        {
            gameManager.pointsChangedEvent -= SetCards;
            gameManager.gameStartEvent -= Move;
            gameManager.gameStartEvent -= InitCards;
            //gameManager.gameEndEvent -= Stop;
            pooler.capacityChangedEvent -= SyncCardTypes;
            Refresh();
        }

        private void InitCards()
        {
            SyncCardTypes(variantCard.gameObject);
            Vector3 step = new Vector3(0f, 0f, cardsStep);
            if (cardsQue.Count == 0) //first initialization cards setting
            {
                Vector3 startPos = new Vector3(transform.position.x, transform.position.y,
                    playerController.transform.position.z + cardsStep);
                int startCount = 3;
                for (int i = 0; i < startCount; i++)
                {
                    var obj = pooler.GetObjectFromPool(variantCard.gameObject, startPos + (step * i));
                    cardsQue.Enqueue(obj);
                }
                cardsQue.Peek().gameObject.SetActive(true);
            }
        }

        private async void SetCards()
        {
            Vector3 step = new Vector3(0f, 0f, cardsStep);
            var obj = pooler.GetObjectFromPool(variantCard.gameObject, cardsQue.Peek().transform.position + (step * 3));
            cardsQue.Enqueue(obj);
            cardTypes[cardsQue.Peek()].AnswerGotLogic();
            await UniTask.Delay(1000, cancellationToken: stuffCancellationTokenSource.Token);
            cardsQue.Dequeue().SetActive(false);
            cardsQue.Peek().SetActive(true);
        }

        private async void Move()
        {
            while (true)
            {
                if (IsPaused == true)
                {
                    await UniTask.Yield(cancellationToken: movingCancellationTokenSource.Token);
                    continue;
                }
                Debug.Log("next");
                mat.mainTextureOffset -= new Vector2(0f, Time.deltaTime * roadSpeed);
                if (cardsQue != null && cardsQue.Count != 0)
                {
                    foreach (var item in cardsQue)
                    {
                        if (item != null)
                            item.transform.position += new Vector3(0f, 0f, -(Time.deltaTime * CardSpeed));
                    }
                }
                await UniTask.Yield(cancellationToken: movingCancellationTokenSource.Token);
            }
        }

        public void Pause(bool isPaused)
        {
            IsPaused = isPaused;
            
        }

        private void Stop()
        {
            IsPaused = true;
        }

        private void Refresh()
        {
            mat.mainTextureOffset = Vector2.zero;
        }

        private void SyncCardTypes(GameObject prefab)
        {
            foreach (var item in pooler.PoolDict[prefab])
            {
                cardTypes.Add(item, item.GetComponent<VariantCardsController>());
            }
        }

        
    }
}

