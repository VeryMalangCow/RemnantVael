using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class UIController : MonoBehaviour
{
    #region Module

    #region Image

    protected void SetFillImgSmooth(Image _Img, float _CurrentValue, float _MaxValue)
    {
        float fillValue = _CurrentValue / _MaxValue;

        DOTween.Kill(_Img.fillAmount);
        _Img.DOFillAmount(fillValue, 0.1f);
    }

    #endregion

    #endregion
}

[System.Serializable]
public class Modify_Sprite
{
    [SerializeField] public Image Img;
    [SerializeField] public Image CompleteImg;
    [SerializeField] public List<Sprite> LevelSpr;
    Sequence DotweenSeq;

    public void Offset()
    {
        ModifySprite(0);
        CompleteImg.gameObject.SetActive(false);
    }

    public void ModifySprite(int _Level)
    {
        Img.sprite = LevelSpr[_Level];
    }

    public void Complete(float _DurTime)
    {
        if (DotweenSeq != null && DOTween.IsTweening(DotweenSeq))
        { DOTween.Kill(DotweenSeq); }

        CompleteImg.gameObject.SetActive(true);
        DotweenSeq = DOTween.Sequence();

        // Fade
        CompleteImg.color = Color.white;
        DotweenSeq.Append(CompleteImg.DOFade(0, _DurTime));

        // Move
        if(CompleteImg.TryGetComponent(out RectTransform RT))
        {
            DOTween.Kill(RT);
            RT.anchoredPosition = Vector2.zero;
            DotweenSeq.Join(RT.DOAnchorPos((Vector2.down * RT.rect.height), _DurTime));
        }

        //End
        DotweenSeq
            .OnComplete(() =>
            {
                CompleteImg.gameObject.SetActive(false);
            });
    }
}

[System.Serializable]
public class Modify_ImgAmountAndTxt
{
    [SerializeField] public Transform Img_ParentTF;
    [SerializeField] public List<Image> Img_List;
    [SerializeField] public TMP_Text Txt_ExtraAmount;

    Sequence DotweenSeq;

    public void Offset()
    {
        Img_List = GameManager.SetList<Image>(Img_ParentTF);

        foreach (Image img in Img_List)
        {
            TurnOff<Image>(img);
        }
        TurnOff<TMP_Text>(Txt_ExtraAmount);
    }

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

    public void SetAmount(int _Value, float _DurTime)
    {
        int currentChargedValue = _Value - 1;
        for(int i = 0; i < Img_List.Count; i++)
        {
            if(currentChargedValue >= i)
            {
                ChangeOnOffImg(Img_List[i], true, _DurTime);
            }
            else
            {
                ChangeOnOffImg(Img_List[i], false, _DurTime);
            }
        }
    }

    public void ChangeOnOffImg(Image _Img, bool _OnOff, float _DurTime)
    {
        if (DotweenSeq != null && DOTween.IsTweening(DotweenSeq))
        { DOTween.Complete(DotweenSeq); }

        DotweenSeq = DOTween.Sequence();

        // Off
        if (_Img.gameObject.activeSelf && !_OnOff)
        {
            _Img.DOFade(1, 0);

            DotweenSeq.Join(_Img.DOFade(0, _DurTime));
            DotweenSeq.OnComplete(() => { _Img.gameObject.SetActive(false); });
        }
        // On
        else if (!_Img.gameObject.activeSelf && _OnOff)
        {
            _Img.DOFade(0, 0);

            DotweenSeq.Join(_Img.DOFade(1, _DurTime));
            DotweenSeq.OnStart(() => { _Img.gameObject.SetActive(true); });
        }
    }
}