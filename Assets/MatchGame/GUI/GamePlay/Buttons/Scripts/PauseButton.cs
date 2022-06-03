using MatchGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MatchGame.GUI.GamePlay.Buttons
{
    public class PauseButton : MonoBehaviour
    {
        public event Action <bool> pauseButtonEvent;

        [SerializeField] private Sprite resumeSprite;
        [SerializeField] private Sprite pauseSprite;

        private Button button;
        private Image currentImage;

        private void Awake()
        {
            button = GetComponent<Button>();
            currentImage = GetComponent<Image>();
            currentImage.sprite = pauseSprite;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Pause);
        }
        private void OnDisable()
        {           
            button.onClick.RemoveAllListeners();
        }

        private void Pause()
        {
            if (currentImage.sprite==null)
            {
                Debug.LogWarning("Pause button sprite is null");
                return;
            }
            button.enabled = false;
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f).OnComplete(()=>button.enabled=true);
            if (currentImage.sprite==pauseSprite)
            {
                currentImage.sprite = resumeSprite;
                pauseButtonEvent?.Invoke(true);
            }
            else
            {
                currentImage.sprite = pauseSprite;
                pauseButtonEvent?.Invoke(false);
            }
        }
    }
}
