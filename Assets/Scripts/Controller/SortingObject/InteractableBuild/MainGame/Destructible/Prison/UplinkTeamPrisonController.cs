
public class UplinkTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        TypeIcon.sprite = UnitManager.Instance.UplinkTeamIcon.TypeSpecial;
        TypeTxt.text = $"<size=150%>)</size> {UnitManager.Instance.UplinkTeamString}";
    }

    protected override void Offset()
    {
        base.Offset();

        Offset_TypeIconTxt();
    }

    #endregion
}
