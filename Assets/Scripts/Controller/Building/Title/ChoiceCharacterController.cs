
public class ChoiceCharacterController : BuildingController_OnlyPlayerLayer, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.OpenThisPanel();
    }

    #endregion
}
