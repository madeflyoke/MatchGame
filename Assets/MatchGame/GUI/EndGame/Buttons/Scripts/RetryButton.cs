using DG.Tweening;
using UnityEngine;

namespace MatchGame.GUI.EndGame.Buttons
{
    public class RetryButton : BaseButton
    {
        public override void Listeners()
        {
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f)
                .OnComplete(() => gameManager.ButtonCall(this));
        }
    }
}

