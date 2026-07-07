
public class PreminumCreditCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true; 
        var langs = StaticResourceManager.instance.staticWords;
        return $"{langs.GetLanguage(124)}\n{langs.GetLanguage(125)}";
    }

    public void PlayInteract()
    {
        MainGameUIManager.instance.premiumCreditCvtUi.SetOnThisPanel();
    }

    #endregion
}
