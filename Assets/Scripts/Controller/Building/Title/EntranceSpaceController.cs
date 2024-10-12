
public class EntranceSpaceController : BuildingController_OnlyPlayerLayer, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.EntranceSpace_UIController.OpenThisPanel();
    }

    #endregion
}
