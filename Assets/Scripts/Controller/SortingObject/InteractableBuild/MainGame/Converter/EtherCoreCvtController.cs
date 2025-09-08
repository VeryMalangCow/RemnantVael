
public class EtherCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;
        return $"{ResourceManager.Instance.Get_StaticWord(119)}\n{ResourceManager.Instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.Instance.EtherCoreCvt_UIController.SetOn_ThisPanel();
    }

    #endregion
}
