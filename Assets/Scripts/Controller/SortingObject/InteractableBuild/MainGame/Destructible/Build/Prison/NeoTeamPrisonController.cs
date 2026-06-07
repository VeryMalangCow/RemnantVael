
public class NeoTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        typeIcon.sprite = ResourceManager.instance.Get_NTPrisonIcon(false);

        allySprites = ResourceManager.instance.neoTeamAllySprites;
        for (int i = 0; i < prisonAllySrList.Count; i++)
        {
            prisonAllySrList[i].sprite = allySprites.bind;
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

        PlayerManager.instance.playerController.GainNeoPresence(allyAmount);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        typeTxt.text = $"<size=150%>)</size> {ResourceManager.instance.neoTeamString}";
    }

    #endregion
}
