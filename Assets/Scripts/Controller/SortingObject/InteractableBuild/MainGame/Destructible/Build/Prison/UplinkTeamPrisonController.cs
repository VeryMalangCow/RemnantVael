
public class UplinkTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        TypeIcon.sprite = ResourceManager.Instance.Get_UTPrisonIcon(false);

        AllySprites = ResourceManager.Instance.UplinkTeamAllySprites;
        for (int i = 0; i < PrisonAllySRList.Count; i++)
        {
            PrisonAllySRList[i].sprite = AllySprites.Bind;
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

        PlayerManager.Instance.PlayerController.UplinkTeamPresence.Value += AllyAmount;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        TypeTxt.text = $"<size=150%>)</size> {ResourceManager.Instance.UplinkTeamString}";
    }

    #endregion
}
