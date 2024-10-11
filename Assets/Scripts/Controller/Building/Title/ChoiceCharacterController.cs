
public class ChoiceCharacterController : BuildingController, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.OpenThisPanel();
    }

    #endregion
}
