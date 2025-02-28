
public class ChoiceCharacterController : InteractableBuildController, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.OpenThisPanel();
    }

    #endregion
}
