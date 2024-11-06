
public class ChoiceCharacterController : InteractableBuildingController, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.OpenThisPanel();
    }

    #endregion
}
