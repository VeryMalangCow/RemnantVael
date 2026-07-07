
public class EtherCoreCvtController : ConverterController, IInteract
{
    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        var langs = StaticResourceManager.instance.staticWords;
        return $"{langs.GetLanguage(119)}\n{langs.GetLanguage(125)}";
    }

    public void PlayInteract()
    {
        MainGameUIManager.instance.etherCoreCvtUi.SetOnThisPanel();
    }

    #endregion
}
