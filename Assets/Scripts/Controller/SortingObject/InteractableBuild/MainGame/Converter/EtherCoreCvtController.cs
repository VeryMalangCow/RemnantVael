
public class EtherCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return $"{ResourceManager.instance.Get_StaticWord(119)}\n{ResourceManager.instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.instance.etherCoreCvt_UIController.SetOn_ThisPanel();
    }

    #endregion
}
