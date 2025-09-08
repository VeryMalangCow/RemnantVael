
public class PreminumCreditCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;
        return $"{ResourceManager.Instance.Get_StaticWord(124)}\n{ ResourceManager.Instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.Instance.PremiumCreditCvt_UIController.SetOn_ThisPanel();
    }

    #endregion
}
