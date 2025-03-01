
public class EntranceSpaceController : InteractableBuildController, IInteract
{
    #region Interact

    public void Play_Interact()
    {
        TitleLobbyUIManager.Instance.EntranceSpace_UIController.SetOn_ThisPanel();
    }

    #endregion
}
