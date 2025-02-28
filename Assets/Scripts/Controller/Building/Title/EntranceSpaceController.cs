
public class EntranceSpaceController : InteractableBuildController, IInteract
{
    #region Interact

    public void Interact()
    {
        TitleLobbyUIManager.Instance.EntranceSpace_UIController.OpenThisPanel();
    }

    #endregion
}
