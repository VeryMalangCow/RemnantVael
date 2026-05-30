
public class ProtoCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return $"{ResourceManager.instance.Get_StaticWord(118)}\n{ResourceManager.instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.instance.protoCoreCvtUi.SetOn_ThisPanel();
    }

    #endregion
}
