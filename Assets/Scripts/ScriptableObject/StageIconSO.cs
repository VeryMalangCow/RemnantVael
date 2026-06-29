using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageIconSO", menuName = "ScriptableObject/StageIconSO")]
public class StageIconSO : ScriptableObject
{
    public CoupleData<Sprite> vault_Icon;
    public CoupleData<Sprite> elevator_Icon;
    public CoupleData<Sprite> shop_Icon;
    public CoupleData<Sprite> allyShop_Icon;
    public CoupleData<Sprite> st_Prison_Icon;
    public CoupleData<Sprite> ut_Prison_Icon;
    public CoupleData<Sprite> nt_Prison_Icon;

    public Dictionary<Type, CoupleData<Sprite>> iconDict = null;

    private void TryInit()
    {
        if (iconDict != null)
            return;

        iconDict = new Dictionary<Type, CoupleData<Sprite>>
        {
            { typeof(VaultRuleController), vault_Icon },
            { typeof(EntranceRuleController), elevator_Icon },
            { typeof(ShopRuleController), shop_Icon },
            { typeof(AllyShopRuleController), allyShop_Icon },

            { typeof(StrikeTeamPrisonController), st_Prison_Icon },
            { typeof(UplinkTeamPrisonController), ut_Prison_Icon },
            { typeof(NeoTeamPrisonController), nt_Prison_Icon },
        };
    }

    public CoupleData<Sprite> GetMinimapIcon(RoomRuleController roomRule)
    {
        TryInit();

        if (roomRule == null)
            return null;

        Type type = roomRule.GetType();
        if (iconDict.TryGetValue(type, out CoupleData<Sprite> data))
        {
            return data;
        }
        else if (roomRule is PrisonRuleController prisonRule)
        {
            Type prisonType = prisonRule.prison.GetType();
            if (iconDict.TryGetValue(prisonType, out CoupleData<Sprite> prisonData))
            {
                return prisonData;
            }
        }

        return null;
    }


}
