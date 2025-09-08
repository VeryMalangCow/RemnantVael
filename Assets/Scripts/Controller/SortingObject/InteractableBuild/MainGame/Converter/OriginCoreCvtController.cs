
public class OriginCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;
        return $"{ResourceManager.Instance.Get_StaticWord(120)}\n{ResourceManager.Instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.Instance.OriginCoreCvt_UIController.SetOn_ThisPanel();
    }

    #endregion
}
