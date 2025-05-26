using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleCloudCellEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Title Smoke")]

    [Space(10)]
    [Header("=== Comp")]

    #endregion

    #region - Hide

    [HideInInspector] public TitleCloudEUIController OwnerEUIController;

    [HideInInspector] private RectTransform ThisRT;
    [HideInInspector] private Image ThisImg;

    [HideInInspector] private bool IsOffsetted = false;

    #endregion

    #endregion


    #region Offset

    public override void Offset()
    {
        if (IsOffsetted) return;
        IsOffsetted = true;

        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
        ThisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;
    }

    #endregion

    #region Play

    public void Play_Cloud(Sprite _Sprite, float _StartSize,
        float _StartPosY, float _EndPosX, float _DurTime)
    {
        ThisImg.sprite = _Sprite;
        ThisImg.SetNativeSize();

        this.gameObject.transform.localScale = Vector2.one * _StartSize;
        ThisRT.anchoredPosition = new Vector3(0, _StartPosY, 0);

        this.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Join(ThisRT.DOAnchorPosX(_EndPosX, _DurTime).SetEase(Ease.Linear));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            OwnerEUIController.Set_OP_Enqueue(this);
        });
    }

    #endregion
}
