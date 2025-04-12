using System.Collections.Generic;
using UnityEngine;

public class StrikeTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_Type()
    {
        AllySprites = UnitManager.Instance.StrikeTeamAllySprites;
        TypeIcon.sprite = UnitManager.Instance.StrikeTeamIcon.TypeSpecial;
        TypeTxt.text = $"<size=150%>)</size> {UnitManager.Instance.StrikeTeamString}";

        for (int i = 0; i < PrisonAllySRList.Count; i++)
        {
            PrisonAllySRList[i].sprite = AllySprites.Bind;
        }
    }

    protected override void Offset()
    {
        base.Offset();

        Offset_Type();
    }

    #endregion

}
