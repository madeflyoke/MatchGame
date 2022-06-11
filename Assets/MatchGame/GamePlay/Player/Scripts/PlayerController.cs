using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Zenject;
using MatchGame.Managers;
using UnityEngine.EventSystems;

namespace MatchGame.GamePlay.Player
{
    public class PlayerController : MonoBehaviour, IPausable
    {
        [Inject] private GameManager gameManager;

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
        private PlayerVisualChanger visualChanger;
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
            visualChanger = GetComponentInChildren<PlayerVisualChanger>();
            Initialize();
        }

        private void Start()
        {
            ChangeCategory();
        }

        private void Initialize()
        {
            leftLineX = leftLine.position.x;
            rightLineX = rightLine.position.x;
            cancellationTokenSource = new CancellationTokenSource();
            transform.position = new Vector3(UnityEngine.Random.Range(0, 2) == 0 ? leftLineX : rightLineX,
                transform.position.y, transform.position.z);
            SetState(PlayerState.None);
        }

        private void OnEnable()
        {
            gameManager.ScoreController.pointsChangedEvent += ChangeCategory;
            gameManager.gameplayEndEvent += EndGameLogic;
            gameManager.refreshEvent += Refresh;
        }
        private void OnDisable()
        {
            gameManager.ScoreController.pointsChangedEvent -= ChangeCategory;
            gameManager.gameplayEndEvent -= EndGameLogic;
            gameManager.refreshEvent -= Refresh;
        }

        void Update()
        {
            if (IsPaused == true)
            {
                return;
            }
            if (currentState != PlayerState.None)
            {
                return;
            }
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase!=TouchPhase.Began||EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }
                float xPos = cam.ScreenToViewportPoint(touch.rawPosition).x;
                if (xPos > 0.55f)
                {
                    SetState(PlayerState.MovingRight);
                }
                else if (xPos < 0.45)
                {
                    SetState(PlayerState.MovingLeft);
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                float xPos = cam.ScreenToViewportPoint(Input.mousePosition).x;
                if (xPos > 0.55f)
                {
                    SetState(PlayerState.MovingRight);
                }
                else if (xPos < 0.45)
                {
                    SetState(PlayerState.MovingLeft);
                }
            }
#endif
        }

        public void Pause(bool isPaused)
        {
            IsPaused = isPaused;
        }

        private void Refresh()
        {
            cancellationTokenSource = new CancellationTokenSource();
            ChangeCategory();
            visualChanger.Refresh();
        }

        private void EndGameLogic()
        {
            cancellationTokenSource.Cancel();
            SetState(PlayerState.None);
        }

        private void ChangeCategory()
        {
            int rndNumber = UnityEngine.Random.Range(0, Enum.GetValues(typeof(CategoryType)).Length);
            CategoryType nextType = (CategoryType)rndNumber;
            currentType = nextType;
            visualChanger.ChangeVisual(currentType);
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
