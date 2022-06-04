using UnityEngine;
using DG.Tweening;

namespace MatchGame.GUI.GamePlay.Buttons
{
    public class LaunchGameButton : BaseButton
    {
        protected override void OnEnable()
        {
            Button.enabled = true;
            base.OnEnable();
        }

        public override void Listeners()
        {
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f)
                .OnComplete(() => { gameManager.ButtonCall(this); gameObject.SetActive(false); });        
            Button.enabled = false;
        }
    }
}

