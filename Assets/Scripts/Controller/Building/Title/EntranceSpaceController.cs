
public class EntranceSpaceController : BuildingController, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.EntranceSpace_UIController.OpenThisPanel();
    }

    #endregion
}
