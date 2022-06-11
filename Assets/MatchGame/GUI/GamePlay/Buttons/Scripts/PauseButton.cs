using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MatchGame.GUI.GamePlay.Buttons
{
    public class PauseButton : BaseButton
    {
        [SerializeField] private Sprite resumeSprite;
        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private GameObject pausedLabel;

        private Image currentImage;

        protected override void Awake()
        {
            base.Awake();
            currentImage = GetComponent<Image>();
            currentImage.sprite = pauseSprite;
            pausedLabel.SetActive(false);
        }

        private void Pause()
        {
            if (currentImage.sprite==null)
            {
                Debug.LogWarning("Pause button sprite is null");
                return;
            }
            Button.enabled = false;
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f);
            if (currentImage.sprite==pauseSprite)
            {
                pausedLabel.SetActive(true);
                currentImage.sprite = resumeSprite;
                gameManager.ButtonCall(this);
            }
            else
            {
                pausedLabel.SetActive(false);
                currentImage.sprite = pauseSprite;
                gameManager.ButtonCall(this);
            }
            Button.enabled = true;
        }

        public override void Listeners()
        {
            Pause();
        }
    }
}
