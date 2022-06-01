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
        [SerializeField] private float speedUpAddByCorrectAnswer;
        [SerializeField] private Poolable variantCard;
        [SerializeField] private Material mat;

        public bool IsPaused { get; set; }
        private CancellationTokenSource movingCancellationTokenSource;
        private CancellationTokenSource stuffCancellationTokenSource;
        private Queue<GameObject> cardsQue;
        private Dictionary<GameObject, VariantCardsController> cardTypes;
        private float CardSpeed { get => currentSpeed * 20f; }
        private float currentSpeed;
        private Vector3 playerCenterPosition
        {
              get => new Vector3(transform.position.x,
              transform.position.y,
              playerController.transform.position.z);
        }

        private bool wait;

        private void Awake()
        {
            currentSpeed = roadSpeed;
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

        private void Update()
        {
            if (wait==false&&cardsQue != null && Vector3.Distance(playerController.transform.position, cardsQue.Peek().transform.position) <5f)
            {
                float d1 = cardTypes[cardsQue.Peek()].LeftCard.transform.position.x
                    - playerController.transform.position.x;
                float d2 = cardTypes[cardsQue.Peek()].RightCard.transform.position.x 
                    - playerController.transform.position.x;
                Debug.Log("LEFT: " + Mathf.Abs(d1) + " RIGHT: " + Mathf.Abs(d2));
                wait = true; 
            }
        }

        private void InitCards()
        {
            SyncCardTypes(variantCard.gameObject);
            Vector3 step = new Vector3(0f, 0f, cardsStep);
            if (cardsQue.Count == 0) //first initialization cards setting
            {
                Vector3 startPos = playerCenterPosition + step;
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
            currentSpeed = roadSpeed + (gameManager.CorrectAnswers * speedUpAddByCorrectAnswer);
            Vector3 step = new Vector3(0f, 0f, cardsStep);
            cardTypes[cardsQue.Peek()].AnswerGotLogic();
            //await UniTask.Delay(1000, cancellationToken: stuffCancellationTokenSource.Token);
            var obj = pooler.GetObjectFromPool(variantCard.gameObject, playerCenterPosition + (step * 3));
            cardsQue.Dequeue().SetActive(false); //remove current(prev) object from head
            cardsQue.Enqueue(obj); //set new object in tail
            cardsQue.Peek().SetActive(true); //activate next obj in head
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
                mat.mainTextureOffset -= new Vector2(0f, Time.deltaTime * currentSpeed);
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

