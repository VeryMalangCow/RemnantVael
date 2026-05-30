
public class PreminumCreditCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return $"{ResourceManager.instance.Get_StaticWord(124)}\n{ ResourceManager.instance.Get_StaticWord(125)}";
    }

    public void Play_Interact()
    {
        MainGameUIManager.instance.premiumCreditCvtUi.SetOn_ThisPanel();
    }

    #endregion
}
