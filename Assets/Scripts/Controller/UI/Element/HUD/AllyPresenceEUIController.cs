using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyPresenceEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Main")]
    public Image InnerImg;
    public TMP_Text PresenceValueTxt;
    public TMP_Text PresenceLangTxt;

    [Space(10)]
    [Header("=== Cap")]
    public RectTransform CapRT;

    [HideInInspector] public Transform CapMiddleRT;
    [HideInInspector] public static readonly float CapCloseY = 24f;
    [HideInInspector] public static readonly float CapOpenY_InputGuide = 96f;

    #endregion

    #region Offset

    public override void Offset()
    {
        CapMiddleRT = DevTool.Get_ComponentTType(CapRT.transform.GetChild(0).gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Play

    public void Play_Amount(int _Amount, int _NeedLvUp)
    {
        PresenceValueTxt.text = $"<b>{_Amount}</b><size=60%>/{_NeedLvUp}</size>";

        bool canLvUp = (_Amount >= _NeedLvUp);
        float a = canLvUp ? 1f : 0.5f;
        DevTool.Set_AlphaColor(PresenceValueTxt, a);
        DevTool.Set_AlphaColor(PresenceLangTxt, a);

        DevTool.Set_KillTween(InnerImg);
        DevTool.Play_FadePulse(InnerImg, 1f, 0.25f);

        Play_CapOpen_CanLvUp(canLvUp);

    }

    private void Play_CapOpen_CanLvUp(bool _Can)
    {
        DevTool.Set_KillTween(CapRT);

        CapRT.DOSizeDelta(new Vector2(CapRT.rect.width, _Can ? CapOpenY_InputGuide : CapCloseY), 0.5f);
    }

    #endregion
}
