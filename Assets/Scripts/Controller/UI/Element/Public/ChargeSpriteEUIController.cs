using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeSpriteEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Charge Img")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Image Img;
    [SerializeField] public Image CompleteImg;
    [SerializeField] public List<Sprite> LevelSpr;

    [Space(10)]
    [Header("=== Extra")]
    [SerializeField] public Image LightInner;

    [HideInInspector] private RectTransform CompleteRT;

    [HideInInspector] private Sequence DotweenSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        CompleteRT = DevTool.Get_ComponentTType(CompleteImg.gameObject, out RectTransform rt) ? rt : null;

        Change_Sprite(0);
        CompleteImg.gameObject.SetActive(false);
    }

    #endregion

    #region Unique

    public void Change_Sprite(int _Level)
    {
        Img.sprite = LevelSpr[_Level];

        DevTool.Set_KillTween(LightInner);

        LightInner.DOFade(1f, 0.2f)
            .OnComplete(() => { LightInner.DOFade(0.25f, 0.2f); });
    }

    public void Set_Complete(float _FadeInTime, float _StayTime, float _FadeOutTime)
    {
        DevTool.Set_KillTween(DotweenSeq);
        DotweenSeq = DOTween.Sequence();

        // Scale
        DotweenSeq.Append(CompleteImg.DOFade(1, _FadeInTime));
        DotweenSeq.Join(CompleteImg.transform.DOScale(1.3f, _FadeInTime));

        DotweenSeq.AppendInterval(_StayTime);

        DotweenSeq.Append(CompleteImg.transform.DOScale(1f, _FadeOutTime));
        DotweenSeq.Join(CompleteImg.DOFade(0, _FadeOutTime));
        DotweenSeq.Join(CompleteRT.DOAnchorPos((Vector2.down * CompleteRT.rect.height), _FadeOutTime));

        //End
        DotweenSeq
            .OnStart(() => 
            { 
                CompleteImg.gameObject.SetActive(true);

                CompleteImg.color = new Color(1, 1, 1, 0);
                CompleteRT.anchoredPosition = Vector2.zero;
                CompleteImg.transform.localScale = Vector3.one;

            })
            .OnComplete(() => { CompleteImg.gameObject.SetActive(false); });
    }

    #endregion
}