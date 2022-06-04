using MatchGame.GamePlay.VariantCards;
using MatchGame.Managers;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using MatchGame.GamePlay.Player;
using System;

namespace MatchGame.GamePlay.Track
{
    public class TrackController : MonoBehaviour, IPausable
    {
        [Inject] private Pooler pooler;
        [Inject] private PlayerController playerController;
        [Inject] private GameManager gameManager;

        public event Action<bool> isCorrectAnswerEvent;

        [SerializeField] private float cardsStep;
        [SerializeField] private float roadSpeed;
        [SerializeField] private float maxRoadSpeed;
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
        private VariantCardsController currentCardsController;
        private Vector3 playerCenterPosition
        {
            get => new Vector3(transform.position.x,
            transform.position.y,
            playerController.transform.position.z);
        }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            SyncCardTypes(variantCard.gameObject);
            SetupStartCards();
        }

        private void Initialize()
        {
            currentSpeed = roadSpeed;
            cardsQue = new Queue<GameObject>();
            movingCancellationTokenSource = new CancellationTokenSource();
            stuffCancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            gameManager.gameplayStartEvent += StartGameLogic;
            gameManager.gameplayEndEvent += Stop;
            gameManager.refreshEvent += Refresh;
            pooler.capacityChangedEvent += SyncCardTypes;
        }
        private void OnDisable()
        {
            gameManager.gameplayStartEvent -= StartGameLogic;
            gameManager.gameplayEndEvent -= Stop;
            gameManager.refreshEvent -= Refresh;
            pooler.capacityChangedEvent -= SyncCardTypes;
        }
        public void SetPreparation()
        {
            currentCardsController.gameObject.SetActive(true);
        }

        private void StartGameLogic()
        {
            Move();
            CheckPlayerPosition();
        }

        public void Pause(bool isPaused)
        {
            IsPaused = isPaused;
        }

        private void Refresh()
        {
            currentCardsController.gameObject.SetActive(false);
            currentSpeed = roadSpeed;
            mat.mainTextureOffset = Vector2.zero;
            cardsQue = new Queue<GameObject>();
            movingCancellationTokenSource = new CancellationTokenSource();
            stuffCancellationTokenSource = new CancellationTokenSource();
            SetupStartCards();
        }

        private void Stop()
        {
            movingCancellationTokenSource.Cancel();
            stuffCancellationTokenSource.Cancel();
        }

        private async void CheckPlayerPosition()
        {
            if (cardsQue == null || currentCardsController == null) return;
            while (Vector3.Distance(playerCenterPosition,
                currentCardsController.transform.position) > 2f)
            {
                if (IsPaused == true)
                {
                    await UniTask.Yield(cancellationToken: stuffCancellationTokenSource.Token);
                    continue;
                }
                await UniTask.Yield(cancellationToken: stuffCancellationTokenSource.Token);
            }
            CheckPlayerAnswer();
        }

        private void CheckPlayerAnswer()
        {
            float leftCardDistance = currentCardsController.LeftCard.transform.position.x
                - playerController.transform.position.x;
            float rightCardDistance = currentCardsController.RightCard.transform.position.x
                - playerController.transform.position.x;
            var closestCard = Mathf.Abs(leftCardDistance) < Mathf.Abs(rightCardDistance) ?
                currentCardsController.LeftCard : currentCardsController.RightCard;
            bool isCorrectAnswer = closestCard.IsCorrect ? true : false;
            isCorrectAnswerEvent?.Invoke(isCorrectAnswer);
            if (isCorrectAnswer == true && currentSpeed <= maxRoadSpeed)
            {
                currentSpeed = Mathf.Clamp(currentSpeed + speedUpAddByCorrectAnswer, 0f, maxRoadSpeed);
            }
            SetCards();
        }

        private void SetupStartCards()
        {
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
                currentCardsController = cardTypes[cardsQue.Peek()];
            }
        }

        private async void SetCards()
        {
            Vector3 step = new Vector3(0f, 0f, cardsStep);
            await UniTask.Delay(500, cancellationToken: stuffCancellationTokenSource.Token);
            var obj = pooler.GetObjectFromPool(variantCard.gameObject, playerCenterPosition + (step * 3));
            cardsQue.Dequeue().SetActive(false); //remove current(prev) object from head
            cardsQue.Enqueue(obj); //set new object in tail
            currentCardsController = cardTypes[cardsQue.Peek()];
            currentCardsController.gameObject.SetActive(true); //activate next obj in head
            CheckPlayerPosition();
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

        private void SyncCardTypes(GameObject prefab) //set dictionary with pooler to get type
        {
            cardTypes = new Dictionary<GameObject, VariantCardsController>();
            foreach (var item in pooler.PoolDict[prefab])
            {
                cardTypes.Add(item, item.GetComponent<VariantCardsController>());
            }
        }
    }
}

