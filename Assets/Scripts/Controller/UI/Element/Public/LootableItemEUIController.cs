using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LootableItemEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("<><><><><> Lootable Item")]

    [Space(5)]
    [Header("=== Comp")]
    [FormerlySerializedAs("RT")][SerializeField] private RectTransform rt;
    [FormerlySerializedAs("InnerImg")][SerializeField] private Image innerImg;
    [FormerlySerializedAs("AmountTxt")][SerializeField] private TMP_Text amountTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        DevTool.Set_Color(PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false), amountTxt);
        DevTool.Set_Color(PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true), innerImg);
    }

    #endregion

    #region Tween

    public void Play_Amount(int amount)
    {
        amountTxt.text = amount.ToString();

        DevTool.Set_KillTween(rt);
        DevTool.Set_KillTween(innerImg);

        DevTool.Play_ScalePulse(rt, 1.4f);
        DevTool.Play_FadePulse(innerImg, 1f, 0.25f);
    }


    #endregion
}
