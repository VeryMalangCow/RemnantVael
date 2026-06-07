
public class StrikeTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_Type()
    {
        typeIcon.sprite = ResourceManager.instance.Get_STPrisonIcon(false);

        allySprites = ResourceManager.instance.strikeTeamAllySprites;
        for (int i = 0; i < prisonAllySrList.Count; i++)
        {
            prisonAllySrList[i].sprite = allySprites.bind;
        }
    }

    protected override void Offset()
    {
        base.Offset();

        Offset_Type();
    }

    #endregion

    #region Unlock

    public override void Set_Unlock()
    {
        base.Set_Unlock();

        PlayerManager.instance.playerController.GainStrikePresence(allyAmount);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        typeTxt.text = $"<size=150%>)</size> {ResourceManager.instance.strikeTeamString}";
    }

    #endregion
}
