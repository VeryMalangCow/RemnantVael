using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootableItemEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("<><><><><> Lootable Item")]

    [Space(5)]
    [Header("=== Comp")]
    [SerializeField] private RectTransform RT;
    [SerializeField] private Image InnerImg;
    [SerializeField] private TMP_Text AmountTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        DevTool.Set_Color(PlayerManager.Instance.playerController.Get_CorrectColor(eDamageType.Energy, false), AmountTxt);
        DevTool.Set_Color(PlayerManager.Instance.playerController.Get_CorrectColor(eDamageType.Energy, true), InnerImg);
    }

    #endregion

    #region Tween

    public void Play_Amount(int _Amount)
    {
        AmountTxt.text = _Amount.ToString();

        DevTool.Set_KillTween(RT);
        DevTool.Set_KillTween(InnerImg);

        DevTool.Play_ScalePulse(RT, 1.4f);
        DevTool.Play_FadePulse(InnerImg, 1f, 0.25f);
    }


    #endregion
}
