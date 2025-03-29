using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindowEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Message")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private TMP_Text ThisTxt;
    [SerializeField] private Transform InnerParentTF;

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] private Color InnerColor;
    [SerializeField] private Color TxtColor;

    // Comp
    [HideInInspector] private RectTransform ThisRT;
    [HideInInspector] private CanvasGroup ThisCG;

    // Value
    [HideInInspector] private float ActivingHeight = 445f;
    [HideInInspector] public bool CanPass = false;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;

        DevTool.Set_Color(InnerColor, DevTool.Get_ChildList<Image>(InnerParentTF));
        DevTool.Set_Color(TxtColor, ThisTxt);

        ActivingHeight = ThisRT.rect.height;

        ThisRT.sizeDelta = new Vector2(ThisRT.rect.width, 0f);
        ThisCG.alpha = 0f;
        ThisTxt.text = "";

        this.transform.SetAsLastSibling();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Reset

    public void Reset_Data()
    {
        DevTool.Set_KillTween(ThisRT);
        DevTool.Set_KillTween(ThisCG);
        DevTool.Set_KillTween(ThisTxt);

        ThisRT.sizeDelta = new Vector2(ThisRT.rect.width, 0f);
        ThisCG.alpha = 0f;
        ThisTxt.text = "";

        this.gameObject.SetActive(false);
        CanPass = false;
    }

    #endregion

    #region Tween

    public Sequence Play_On(string _Txt, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(ThisRT.DOSizeDelta(new Vector2(ThisRT.rect.width, ActivingHeight), _DurTime));
        seq.Join(ThisCG.DOFade(1f, _DurTime));
        seq.Join(ThisTxt.DOText(_Txt, _DurTime));

        DevTool.Play_Tween(seq,
            new Dele(() =>
            {
                this.gameObject.SetActive(true);
            }),
            _Update: null,
            new Dele(() =>
            { 
                CanPass = true;
            }));

        return seq;
    }

    public Sequence Play_Off(float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(ThisRT.DOSizeDelta(new Vector2(ThisRT.rect.width, 0), _DurTime));
        seq.Join(ThisCG.DOFade(0f, _DurTime));
        seq.Join(ThisTxt.DOText("", _DurTime));

        DevTool.Play_Tween(seq,
            new Dele(() =>
            {
                CanPass = false;
            }),
            _Update: null,
            new Dele(() =>
            {
                this.gameObject.SetActive(false);
            }));

        return seq;
    }

    #endregion
}
