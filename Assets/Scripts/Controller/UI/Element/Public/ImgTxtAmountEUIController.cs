using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImgTxtAmountEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> ImgTxt Amount")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Sprite sprite;
    [SerializeField] public Image innerImg;

    [HideInInspector] private Transform tf;
    [HideInInspector] public List<Image> amountImgs;
    [HideInInspector] public TMP_Text amountTxt;

    [HideInInspector] private Sequence dotweenSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        tf = this.transform;

        amountImgs = DevTool.Get_ChildList<Image>(tf);
        List<TMP_Text> tmpList = DevTool.Get_ChildList<TMP_Text>(tf);
        amountTxt = tmpList.Count > 0 ? tmpList[0] : null;

        SetOff_All(amountImgs);
        if (amountTxt != null) amountTxt.gameObject.SetActive(false);
        
    }

    #endregion

    #region Set

    private void SetOff_All<T>(List<T> targetList) where T : Component
    {
        DevTool.Set_Active(targetList , false);
        
    }

    public void Set_Amount(int value)
    {
        for (int i = 0; i < amountImgs.Count; i++)
            amountImgs[i].gameObject.SetActive(value > i ? true : false);

        Set_AmountTxt(value - amountImgs.Count);
    }

    public void Set_Amount(int value, float durTime)
    {
        for (int i = 0; i < amountImgs.Count; i++)
        {
            if (value > i) Play_OnOffImg(
                amountImgs[i], 
                onOff: true, 
                doScale: 1.35f, 
                durTime);

            else Play_OnOffImg(
                amountImgs[i], 
                onOff: false,
                doScale: 1.35f, 
                durTime);
        }

        Set_AmountTxt(value - amountImgs.Count);

        Play_Inner();
    }

    private void Set_AmountTxt(int value)
    {
        if (amountTxt == null) return;

        amountTxt.gameObject.SetActive(value > 0 ? true : false);
        amountTxt.text = value > 0 ? value.ToString() : null;
    }

    #endregion

    #region Get

    public int Get_GOEnableAmount()
    {
        int result = 0;
        for (int i = 0; i < amountImgs.Count; i++)
        {
            if (amountImgs[i].gameObject.activeSelf)
                result++;
            else
                break;
        }
        return result;
    }

    #endregion

    #region Tween

    private void Play_OnOffImg(Image img, bool onOff, float doScale, float durTime)
    {
        DevTool.Set_CompleteTween(dotweenSeq);
        dotweenSeq = DOTween.Sequence();

        Sequence ElementSeq = DOTween.Sequence();

        Color clr = img.color;
        if (img.gameObject.activeSelf && !onOff) // Off
        {
            ElementSeq.Append(img.transform.DOScale(doScale, durTime / 2));
            ElementSeq.Join(img.DOFade(0, durTime / 2));
            ElementSeq.Append(img.transform.DOScale(1f, durTime / 2));
            ElementSeq
                .OnStart(() =>
                {
                    clr.a = 1f; 
                    img.color = clr;
                })
                .OnComplete(() => { img.gameObject.SetActive(false); });
        }
        else if (!img.gameObject.activeSelf && onOff) // On
        {
            ElementSeq.Append(img.transform.DOScale(doScale, durTime / 2));
            ElementSeq.Join(img.DOFade(1, durTime / 2));
            ElementSeq.Append(img.transform.DOScale(1f, durTime / 2));
            ElementSeq
                .OnStart(() => 
                {
                    clr.a = 0f; 
                    img.color = clr;

                    img.gameObject.SetActive(true); 
                });
        }

        dotweenSeq.Join(ElementSeq);
    }

    private void Play_Inner()
    {
        if (innerImg == null) return;

        DevTool.Set_KillTween(innerImg);

        innerImg.DOFade(1f, 0.2f)
            .OnComplete(() => { innerImg.DOFade(0.25f, 0.2f); });
    }


    #endregion
}