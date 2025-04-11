
public class StrikeTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        TypeIcon.sprite = UnitManager.Instance.StrikeTeamIcon.TypeSpecial;
        TypeTxt.text = $"<size=150%>)</size> {UnitManager.Instance.StrikeTeamString}";
    }

    protected override void Offset()
    {
        base.Offset();

        Offset_TypeIconTxt();
    }

    #endregion
}
