
public class UplinkTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        TypeIcon.sprite = UnitManager.Instance.UplinkTeamIcon.TypeSpecial;
        TypeTxt.text = $"<size=150%>)</size> {UnitManager.Instance.UplinkTeamString}";

        AllySprites = UnitManager.Instance.UplinkTeamAllySprites;
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
}
