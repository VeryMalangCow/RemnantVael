using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifySprite : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Image Img;
    [SerializeField] public Image CompleteImg;
    [SerializeField] public List<Sprite> LevelSpr;

    [Space(10)]
    [Header("=== Extra")]
    [SerializeField] private Image LightInner;

    Sequence DotweenSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        Modify_Sprite(0);
        CompleteImg.gameObject.SetActive(false);
    }

    #endregion

    #region Unique

    public void Modify_Sprite(int _Level)
    {
        Img.sprite = LevelSpr[_Level];

        if (DOTween.IsTweening(LightInner))
        { DOTween.Kill(LightInner); }

        LightInner.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                LightInner.DOFade(0.25f, 0.2f);
            });
    }

    public void Complete(float _StayTime, float _FadeDurTime)
    {
        if (DotweenSeq != null && DOTween.IsTweening(DotweenSeq))
        { DOTween.Kill(DotweenSeq); }

        CompleteImg.gameObject.SetActive(true);
        DotweenSeq = DOTween.Sequence();

        CompleteImg.color = Color.white;
        CompleteImg.transform.localScale = Vector3.one;

        // Stay
        DotweenSeq.AppendInterval(_StayTime);
        
        // Scale
        Sequence DotweenSeq2 = DOTween.Sequence();
        DotweenSeq2.Append(CompleteImg.transform.DOScale(1.3f, _StayTime / 2));
        DotweenSeq2.Append(CompleteImg.transform.DOScale(1f, _StayTime / 2));
        DotweenSeq.Join(DotweenSeq2);

        // Fade
        DotweenSeq.Append(CompleteImg.DOFade(0, _FadeDurTime));

        // Move
        if (CompleteImg.TryGetComponent(out RectTransform RT))
        {
            DOTween.Kill(RT);
            RT.anchoredPosition = Vector2.zero;
            DotweenSeq.Join(RT.DOAnchorPos((Vector2.down * RT.rect.height), _FadeDurTime));
        }

        //End
        DotweenSeq
            .OnComplete(() =>
            {
                CompleteImg.gameObject.SetActive(false);
            });
    }

    #endregion
}