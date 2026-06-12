
public class EtherCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return $"{ResourceManager.instance.Get_StaticWord(119)}\n{ResourceManager.instance.Get_StaticWord(125)}";
    }

    public void PlayInteract()
    {
        MainGameUIManager.instance.etherCoreCvtUi.SetOnThisPanel();
    }

    #endregion
}
