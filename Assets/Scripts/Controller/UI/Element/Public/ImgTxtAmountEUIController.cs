using DG.Tweening;
using System;
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
    [SerializeField] public Sprite ThisSprite;
    [SerializeField] public Image InnerImg;

    [HideInInspector] private Transform ThisTF;
    [HideInInspector] public List<Image> AmountImgs;
    [HideInInspector] public TMP_Text AmountTxt;

    [HideInInspector] private Sequence DotweenSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisTF = this.transform;

        AmountImgs = DevTool.Get_ChildList<Image>(ThisTF);
        List<TMP_Text> tmpList = DevTool.Get_ChildList<TMP_Text>(ThisTF);
        AmountTxt = tmpList.Count > 0 ? tmpList[0] : null;

        SetOff_All(AmountImgs);
        if (AmountTxt != null) AmountTxt.gameObject.SetActive(false);
        
    }

    #endregion

    #region Set

    private void SetOff_All<T>(List<T> _TargetList) where T : Component
    {
        DevTool.Set_Active(_TargetList , false);
        
    }

    public void Set_Amount(int _Value)
    {
        for (int i = 0; i < AmountImgs.Count; i++)
            AmountImgs[i].gameObject.SetActive(_Value > i ? true : false);

        Set_AmountTxt(_Value - AmountImgs.Count);
    }

    public void Set_Amount(int _Value, float _DurTime)
    {
        for (int i = 0; i < AmountImgs.Count; i++)
        {
            if (_Value > i) Play_OnOffImg(
                AmountImgs[i], 
                _OnOff: true, 
                _DoScale: 1.35f, 
                _DurTime);

            else Play_OnOffImg(
                AmountImgs[i], 
                _OnOff: false,
                _DoScale: 1.35f, 
                _DurTime);
        }

        Set_AmountTxt(_Value - AmountImgs.Count);

        Play_Inner();
    }

    private void Set_AmountTxt(int _Value)
    {
        if (AmountTxt == null) return;

        AmountTxt.gameObject.SetActive(_Value > 0 ? true : false);
        AmountTxt.text = _Value > 0 ? _Value.ToString() : null;
    }

    #endregion

    #region Get

    public int Get_GOEnableAmount()
    {
        int result = 0;
        for (int i = 0; i < AmountImgs.Count; i++)
        {
            if (AmountImgs[i].gameObject.activeSelf)
                result++;
            else
                break;
        }
        return result;
    }

    #endregion

    #region Tween

    private void Play_OnOffImg(Image _Img, bool _OnOff, float _DoScale, float _DurTime)
    {
        DevTool.Set_CompleteTween(DotweenSeq);
        DotweenSeq = DOTween.Sequence();

        Sequence ElementSeq = DOTween.Sequence();

        Color clr = _Img.color;
        if (_Img.gameObject.activeSelf && !_OnOff) // Off
        {
            ElementSeq.Append(_Img.transform.DOScale(_DoScale, _DurTime / 2));
            ElementSeq.Join(_Img.DOFade(0, _DurTime / 2));
            ElementSeq.Append(_Img.transform.DOScale(1f, _DurTime / 2));
            ElementSeq
                .OnStart(() =>
                {
                    clr.a = 1f; 
                    _Img.color = clr;
                })
                .OnComplete(() => { _Img.gameObject.SetActive(false); });
        }
        else if (!_Img.gameObject.activeSelf && _OnOff) // On
        {
            ElementSeq.Append(_Img.transform.DOScale(_DoScale, _DurTime / 2));
            ElementSeq.Join(_Img.DOFade(1, _DurTime / 2));
            ElementSeq.Append(_Img.transform.DOScale(1f, _DurTime / 2));
            ElementSeq
                .OnStart(() => 
                {
                    clr.a = 0f; 
                    _Img.color = clr;

                    _Img.gameObject.SetActive(true); 
                });
        }

        DotweenSeq.Join(ElementSeq);
    }

    private void Play_Inner()
    {
        if (InnerImg == null) return;

        DevTool.Set_KillTween(InnerImg);

        InnerImg.DOFade(1f, 0.2f)
            .OnComplete(() => { InnerImg.DOFade(0.25f, 0.2f); });
    }


    #endregion
}