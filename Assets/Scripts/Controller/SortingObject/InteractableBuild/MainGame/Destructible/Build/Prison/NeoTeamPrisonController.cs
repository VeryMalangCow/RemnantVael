
public class NeoTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        TypeIcon.sprite = ResourceManager.instance.Get_NTPrisonIcon(false);

        AllySprites = ResourceManager.instance.neoTeamAllySprites;
        for (int i = 0; i < PrisonAllySRList.Count; i++)
        {
            PrisonAllySRList[i].sprite = AllySprites.bind;
        }
    }

    protected override void Offset()
    {
        base.Offset();

        Offset_TypeIconTxt();
    }

    #endregion

    #region Unlock

    public override void Set_Unlock()
    {
        base.Set_Unlock();

        PlayerManager.instance.playerController.neoTeamPresence.Value += AllyAmount;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        TypeTxt.text = $"<size=150%>)</size> {ResourceManager.instance.neoTeamString}";
    }

    #endregion
}
