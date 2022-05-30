using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace MatchGame.GamePlay.Player
{
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        public event Action<bool> isCorrectAnswerEvent;
        public event Action<CategoryType> playerCategoryChangedEvent;

        [SerializeField] private float forwardSpeed;
        [SerializeField] private float sideSpeed;
        [SerializeField] private Transform leftLine;
        [SerializeField] private Transform rightLine;
        private CancellationTokenSource cancellationTokenSource;
        private float leftLineX;
        private float rightLineX;
        private PlayerState currentState;
        private CategoryType currentType;
        private BoxCollider boxCollider;

        private void Awake()
        {
            leftLineX = leftLine.position.x;
            rightLineX = rightLine.position.x;
            cancellationTokenSource = new CancellationTokenSource();
            transform.position = new Vector3(UnityEngine.Random.Range(0, 2) == 0 ? leftLineX : rightLineX,
                transform.position.y, transform.position.z);
            currentState = PlayerState.None;
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void Start()
        {
            currentType = CategoryType.Food;
            playerCategoryChangedEvent?.Invoke(currentType);
        }

        void Update()
        {
            //transform.Translate(transform.forward * (Time.deltaTime * forwardSpeed));
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer==(int)Layer.CardCorrectAnswer)
            {
                isCorrectAnswerEvent?.Invoke(true);
            }
            else if(other.gameObject.layer == (int)Layer.CardWrongAnswer)
            {
                isCorrectAnswerEvent?.Invoke(false);
            }
            boxCollider.isTrigger = false; //prevent multiple collisions
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
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(xPos, transform.position.y, transform.position.z), Time.deltaTime * sideSpeed);
                await UniTask.Yield(cancellationToken: cancellationTokenSource.Token);
            }
            SetState(PlayerState.None);
        }
    }
}
