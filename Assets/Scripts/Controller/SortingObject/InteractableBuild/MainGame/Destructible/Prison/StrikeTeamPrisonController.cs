using System.Collections.Generic;
using UnityEngine;

public class StrikeTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_Type()
    {
        TypeIcon.sprite = UnitManager.Instance.StrikeTeamIcon.TypeSpecial;
        TypeTxt.text = $"<size=150%>)</size> {UnitManager.Instance.StrikeTeamString}";

        AllySprites = UnitManager.Instance.StrikeTeamAllySprites;
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

    #region Unlock

    public override void Set_Unlock()
    {
        base.Set_Unlock();

        PlayerManager.Instance.PlayerController.StrikeTeamPresence.Value += AllyAmount;
    }

    #endregion
}
