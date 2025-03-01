
public class ChoiceCharacterController : InteractableBuildController, IInteract
{
    #region Interact

    public void Play_Interact()
    {
        TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.SetOn_ThisPanel();
    }

    #endregion
}
