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
    [SerializeField] public Image img;
    [SerializeField] public Image completeImg;
    [SerializeField] public List<Sprite> lvSpr;

    [Space(10)]
    [Header("=== Extra")]
    [SerializeField] public Image lightInner;

    [HideInInspector] private RectTransform completeRt;

    [HideInInspector] private Sequence dotweenSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        completeRt = DevTool.Get_ComponentTType(completeImg.gameObject, out RectTransform rt) ? rt : null;

        ChangeSprite(0);
        completeImg.gameObject.SetActive(false);
    }

    #endregion

    #region Unique

    public void ChangeSprite(int lv)
    {
        img.sprite = lvSpr[lv];

        DevTool.SetKillTween(lightInner);

        lightInner.DOFade(1f, 0.2f)
            .OnComplete(() => { lightInner.DOFade(0.25f, 0.2f); });
    }

    public void Set_Complete(float fadeInTime, float stayTime, float fadeOutTime)
    {
        DevTool.SetKillTween(dotweenSeq);
        dotweenSeq = DOTween.Sequence();

        // Scale
        dotweenSeq.Append(completeImg.DOFade(1, fadeInTime));
        dotweenSeq.Join(completeImg.transform.DOScale(1.3f, fadeInTime));

        dotweenSeq.AppendInterval(stayTime);

        dotweenSeq.Append(completeImg.transform.DOScale(1f, fadeOutTime));
        dotweenSeq.Join(completeImg.DOFade(0, fadeOutTime));
        dotweenSeq.Join(completeRt.DOAnchorPos((Vector2.down * completeRt.rect.height), fadeOutTime));

        //End
        dotweenSeq
            .OnStart(() => 
            { 
                completeImg.gameObject.SetActive(true);

                completeImg.color = new Color(1, 1, 1, 0);
                completeRt.anchoredPosition = Vector2.zero;
                completeImg.transform.localScale = Vector3.one;

            })
            .OnComplete(() => { completeImg.gameObject.SetActive(false); });
    }

    #endregion
}