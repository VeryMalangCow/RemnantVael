using System.Collections.Generic;
using UnityEngine;

public class StrikeTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_Type()
    {
        TypeIcon.sprite = ResourceManager.instance.Get_STPrisonIcon(false);

        AllySprites = ResourceManager.instance.strikeTeamAllySprites;
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

        PlayerManager.instance.playerController.StrikeTeamPresence.Value += AllyAmount;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        TypeTxt.text = $"<size=150%>)</size> {ResourceManager.instance.strikeTeamString}";
    }

    #endregion
}
