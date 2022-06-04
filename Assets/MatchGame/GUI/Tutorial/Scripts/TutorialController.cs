using MatchGame.GUI.Tutorial.Buttons;
using UnityEngine;

namespace MatchGame.GUI.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private TutorialConfirmButton tutorialConfirmButton;

        public void Hide(bool isHidden)
        {
            gameObject.SetActive(!isHidden);
        }
    }
}

