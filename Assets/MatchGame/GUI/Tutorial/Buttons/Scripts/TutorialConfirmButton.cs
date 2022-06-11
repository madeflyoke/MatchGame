namespace MatchGame.GUI.Tutorial.Buttons
{
    public class TutorialConfirmButton : BaseButton
    {
        protected override void OnEnable()
        {
            Button.enabled = true;
            base.OnEnable();
        }

        public override void Listeners()
        {
            gameManager.ButtonCall(this);
            Button.enabled = false;
        }
    }

}
