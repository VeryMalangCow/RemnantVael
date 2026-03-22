using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MessageWindowEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Message")]

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("ThisTxt")][SerializeField] private TMP_Text txt;
    [FormerlySerializedAs("InnerParentTF")][SerializeField] private Transform innerParentTf;

    [Space(10)]
    [Header("=== Color")]
    [FormerlySerializedAs("InnerColor")][SerializeField] private Color innerClr;
    [FormerlySerializedAs("TxtColor")][SerializeField] private Color txtClr;

    // Comp
    [HideInInspector] private RectTransform rt;
    [HideInInspector] private CanvasGroup cg;

    // Value
    [HideInInspector] private float activingHeight = 445f;
    [HideInInspector] public bool canPass = false;

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
        cg = DevTool.Get_ComponentTType(gameObject, out CanvasGroup _cg) ? _cg : null;

        DevTool.Set_Color(innerClr, DevTool.Get_ChildList<Image>(innerParentTf));
        DevTool.Set_Color(txtClr, txt);

        activingHeight = rt.rect.height;

        rt.sizeDelta = new Vector2(rt.rect.width, 0f);
        cg.alpha = 0f;
        txt.text = "";

        this.transform.SetAsLastSibling();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Reset

    public void Reset_Data()
    {
        if (rt == null) return;

        DevTool.Set_KillTween(rt);
        DevTool.Set_KillTween(cg);
        DevTool.Set_KillTween(txt);

        rt.sizeDelta = new Vector2(rt.rect.width, 0f);
        cg.alpha = 0f;
        txt.text = "";

        this.gameObject.SetActive(false);
        canPass = false;
    }

    #endregion

    #region Tween

    public Sequence Play_On(string txt, float durTime)
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_Reject");

        Sequence seq = DOTween.Sequence();

        seq.Join(rt.DOSizeDelta(new Vector2(rt.rect.width, activingHeight), durTime));
        seq.Join(cg.DOFade(1f, durTime));
        seq.Join(this.txt.DOText(txt, durTime));

        DevTool.Play_Tween(seq,
            new Dele(() =>
            {
                this.gameObject.SetActive(true);
            }),
            update: null,
            new Dele(() =>
            { 
                canPass = true;
            }));

        return seq;
    }

    public Sequence Play_Off(float durTime)
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

        Sequence seq = DOTween.Sequence();

        seq.Join(rt.DOSizeDelta(new Vector2(rt.rect.width, 0), durTime));
        seq.Join(cg.DOFade(0f, durTime));
        seq.Join(txt.DOText("", durTime));

        DevTool.Play_Tween(seq,
            new Dele(() =>
            {
                canPass = false;
            }),
            update: null,
            new Dele(() =>
            {
                this.gameObject.SetActive(false);
            }));

        return seq;
    }

    #endregion
}
