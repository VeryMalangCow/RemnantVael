using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyPresenceEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Main")]
    [SerializeField] public Image innerImg;
    [SerializeField] public TMP_Text presenceValueTxt;
    [SerializeField] public TMP_Text presenceLangTxt;

    [Space(10)]
    [Header("=== Cap")]
    [SerializeField] public RectTransform capRt;


    [HideInInspector] public Transform capMiddleRt;
    [HideInInspector] public static readonly float capCloseY = 24f;
    [HideInInspector] public static readonly float capOpenY_InputGuide = 96f;

    #endregion

    #region Offset

    public override void Offset()
    {
        capMiddleRt = DevTool.Get_ComponentTType(capRt.transform.GetChild(0).gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Play

    public void Play_Amount(int amount, int needLvUp)
    {
        presenceValueTxt.text = $"<b>{amount}</b><size=60%>/{needLvUp}</size>";

        bool canLvUp = (amount >= needLvUp);
        float a = canLvUp ? 1f : 0.5f;
        DevTool.Set_AlphaColor(presenceValueTxt, a);
        DevTool.Set_AlphaColor(presenceLangTxt, a);

        DevTool.SetKillTween(innerImg);
        DevTool.Play_FadePulse(innerImg, 1f, 0.25f);

        Play_CapOpen_CanLvUp(canLvUp);

    }

    private void Play_CapOpen_CanLvUp(bool can)
    {
        DevTool.SetKillTween(capRt);

        capRt.DOSizeDelta(new Vector2(capRt.rect.width, can ? capOpenY_InputGuide : capCloseY), 0.5f);
    }

    #endregion
}
