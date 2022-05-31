using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Zenject;
using MatchGame.Managers;

namespace MatchGame.GamePlay.Player
{
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour, IPausable
    {
        [Inject] private GameManager gameManager; 

        public event Action<bool> isCorrectAnswerEvent;
        public event Action<CategoryType> playerCategoryChangedEvent;

        [SerializeField] private float sideSpeed;
        [SerializeField] private Transform leftLine;
        [SerializeField] private Transform rightLine;

        public CategoryType CurrentType { get => currentType; }
        public bool IsPaused { get; set; }

        private CancellationTokenSource cancellationTokenSource;
        private float leftLineX;
        private float rightLineX;
        private PlayerState currentState;
        private CategoryType currentType;
        private BoxCollider boxCollider;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            leftLineX = leftLine.position.x;
            rightLineX = rightLine.position.x;
            cancellationTokenSource = new CancellationTokenSource();
            transform.position = new Vector3(UnityEngine.Random.Range(0, 2) == 0 ? leftLineX : rightLineX,
                transform.position.y, transform.position.z);
            SetState(PlayerState.None);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void OnEnable()
        {
            gameManager.gameStartEvent += ChangeCategory;
        }
        private void OnDisable()
        {
            gameManager.gameStartEvent -= ChangeCategory;
        }

        void Update()
        {
            if (IsPaused==true)
            {
                return;
            }
            if (currentState != PlayerState.None)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetState(PlayerState.MovingRight);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetState(PlayerState.MovingLeft);
            }
        }

        public void Pause(bool isPaused)
        {
            IsPaused = isPaused;
            boxCollider.isTrigger = !IsPaused;
        }

        private void EndGameLogic()
        {
            enabled = false;
            boxCollider.isTrigger = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == (int)Layer.CardCorrectAnswer)
            {
                isCorrectAnswerEvent?.Invoke(true);
            }
            else if (other.gameObject.layer == (int)Layer.CardWrongAnswer)
            {
                isCorrectAnswerEvent?.Invoke(false);
            }
            ChangeCategory();
        }

        private void ChangeCategory()
        {
            int rndNumber = UnityEngine.Random.Range(0, Enum.GetValues(typeof(CategoryType)).Length);
            CategoryType nextType = (CategoryType)rndNumber;
            currentType = nextType;
            playerCategoryChangedEvent?.Invoke(currentType);
        }

        private void SetState(PlayerState state)
        {
            currentState = state;
            switch (state)
            {
                case PlayerState.None:
                    break;
                case PlayerState.MovingLeft:
                    SideMove(leftLineX);
                    break;
                case PlayerState.MovingRight:
                    SideMove(rightLineX);
                    break;
                default:
                    break;
            }
        }

        private async void SideMove(float xPos)
        {
            while (Mathf.Approximately(transform.position.x, xPos) == false)
            {
                if (IsPaused == true)
                {
                    await UniTask.Yield(cancellationToken: cancellationTokenSource.Token);
                    continue;
                }
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(xPos, transform.position.y, transform.position.z), Time.deltaTime * sideSpeed);
                await UniTask.Yield(cancellationToken: cancellationTokenSource.Token);
            }
            SetState(PlayerState.None);
        }
    }
}
