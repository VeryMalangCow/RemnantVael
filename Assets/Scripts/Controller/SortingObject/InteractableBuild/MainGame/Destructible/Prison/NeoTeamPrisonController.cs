
public class NeoTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        TypeIcon.sprite = UnitManager.Instance.NeoTeamIcon.TypeSpecial;
        TypeTxt.text = $"<size=150%>)</size> {UnitManager.Instance.NeoTeamString}"; 
        
        AllySprites = UnitManager.Instance.NeoTeamAllySprites;
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

        PlayerManager.Instance.PlayerController.NeoTeamPresence.Value += AllyAmount;
    }

    #endregion
}
