using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MatchGame.GUI.GamePlay.Buttons
{
    public class StartGameButton : MonoBehaviour
    {
        public event Action startGameButtonEvent; 

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(()=> 
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f)
            .OnComplete(()=>startGameButtonEvent?.Invoke()));
        }
        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

    }
}

