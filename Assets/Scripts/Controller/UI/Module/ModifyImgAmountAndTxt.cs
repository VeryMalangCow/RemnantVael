using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifyImgAmountAndTxt : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Transform Img_ParentTF;
    [SerializeField] public List<Image> Img_List;
    [SerializeField] public Sprite ThisSprite;
    [SerializeField] public TMP_Text Txt_ExtraAmount;

    Sequence DotweenSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        Img_List = GameManager.SetList<Image>(Img_ParentTF);

        foreach (Image img in Img_List)
        {
            img.sprite = ThisSprite;
            TurnOff<Image>(img);
        }
        TurnOff<TMP_Text>(Txt_ExtraAmount);
    }

    #endregion

    #region Unique

    private void TurnOff<T>(T _Type)
    {
        // Image
        if (_Type is Image typeImg)
        {
            typeImg.gameObject.SetActive(false);
        }

        // TMP Text
        else if (_Type is TMP_Text typeTxt)
        {
            typeTxt.gameObject.SetActive(false);
            typeTxt.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void TurnOn<T>(T _Type)
    {
        // Image
        if (_Type is Image typeImg)
        {
            typeImg.gameObject.SetActive(true);
        }

        // TMP Text
        else if (_Type is TMP_Text typeTxt)
        {
            typeTxt.gameObject.SetActive(true);
            typeTxt.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void SetAmount(int _Value, float _DurTime)
    {
        for (int i = 0; i < Img_List.Count; i++)
        {
            if (_Value > i)
            {
                ChangeOnOffImg(Img_List[i], true, 1.35f, _DurTime);
            }
            else
            {
                ChangeOnOffImg(Img_List[i], false, 1.35f, _DurTime);
            }
        }

        int overAmount = _Value - Img_List.Count;
        if (overAmount > 0)
        {
            Txt_ExtraAmount.text = overAmount.ToString();
            TurnOn<TMP_Text>(Txt_ExtraAmount);
        }
        else
        {
            TurnOff<TMP_Text>(Txt_ExtraAmount);
        }

    }

    public void ChangeOnOffImg(Image _Img, bool _OnOff, float _DoScale, float _DurTime)
    {
        if (DotweenSeq != null && DOTween.IsTweening(DotweenSeq))
        { DOTween.Complete(DotweenSeq); }

        DotweenSeq = DOTween.Sequence();
        Sequence DotweenSeq2 = DOTween.Sequence();

        // Off
        if (_Img.gameObject.activeSelf && !_OnOff)
        {
            Color clr = _Img.color;
            clr.a = 1f;
            _Img.color = clr;

            DotweenSeq2.Append(_Img.transform.DOScale(_DoScale, _DurTime / 2));
            DotweenSeq2.Append(_Img.transform.DOScale(1f, _DurTime / 2));

            DotweenSeq.Join(_Img.DOFade(0, _DurTime));


            DotweenSeq.Join(DotweenSeq2);
            DotweenSeq.OnComplete(() => { _Img.gameObject.SetActive(false); });
        }
        // On
        else if (!_Img.gameObject.activeSelf && _OnOff)
        {
            Color clr = _Img.color;
            clr.a = 0f;
            _Img.color = clr;

            DotweenSeq2.Append(_Img.transform.DOScale(_DoScale, _DurTime / 2));
            DotweenSeq2.Append(_Img.transform.DOScale(1f, _DurTime / 2));

            DotweenSeq.Join(_Img.DOFade(1, _DurTime));


            DotweenSeq.Join(DotweenSeq2);
            DotweenSeq.OnStart(() => { _Img.gameObject.SetActive(true); });
        }
    }

    #endregion
}