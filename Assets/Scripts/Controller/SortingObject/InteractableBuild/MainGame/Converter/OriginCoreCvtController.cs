
public class OriginCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return $"{ResourceManager.instance.Get_StaticWord(120)}\n{ResourceManager.instance.Get_StaticWord(125)}";
    }

    public void PlayInteract()
    {
        MainGameUIManager.instance.originCoreCvtUi.SetOn_ThisPanel();
    }

    #endregion
}
