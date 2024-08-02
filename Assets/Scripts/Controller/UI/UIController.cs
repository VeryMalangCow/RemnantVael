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
        if (DOTween.IsTweening(DotweenSeq))
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
            Color clr = typeImg.color;
            clr.a = 0;
            typeImg.color = clr;
        }

        // TMP Text
        else if (_Type is TMP_Text typeTxt)
        {
            Color clr = typeTxt.color;
            clr.a = 0;
            typeTxt.color = clr;
            typeTxt.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void SetAmount(int _Value)
    {

    }
}