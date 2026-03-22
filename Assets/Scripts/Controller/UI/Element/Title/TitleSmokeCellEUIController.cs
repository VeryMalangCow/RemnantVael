using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleSmokeCellEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Title Smoke")]

    [Space(10)]
    [Header("=== Comp")]

    #endregion

    #region - Hide

    [HideInInspector] public TitleSmokeEUIController ownerEuiController;

    [HideInInspector] private RectTransform rt;
    [HideInInspector] private Image img;

    [HideInInspector] private bool isOffsetted = false;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        if (isOffsetted) return;
        isOffsetted = true;

        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
        img = DevTool.Get_ComponentTType(gameObject, out Image _img) ? _img : null;
    }

    #endregion

    #region Play

    public void Play_Smoke(Sprite sprite, 
        CoupleData<Color> clr, float startSize,
        float startPosX, float movingDis, float durTime)
    {
        img.sprite = sprite;
        img.SetNativeSize();
        img.color = clr.typeBase;
        this.gameObject.transform.localScale = Vector2.one * startSize;
        rt.anchoredPosition = new Vector3(startPosX, 0, 0);
        rt.rotation = Quaternion.identity;

        this.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Join(rt.DOAnchorPosY(movingDis, durTime));
        seq.Join(img.DOColor(clr.typeSpecial, durTime));
        seq.Join(rt.DOScale(0, durTime));
        seq.Join(rt.DORotate(new Vector3(0, 0, Random.Range(-360, 360)), durTime, RotateMode.FastBeyond360));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            ownerEuiController.Set_OP_Enqueue(this);
        });
    }

    #endregion

}
