
public class ProtoCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;
        return $"{ResourceManager.Instance.Get_StaticWord(118)}\n{ResourceManager.Instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.Instance.protoCoreCvt_UIController.SetOn_ThisPanel();
    }

    #endregion
}
