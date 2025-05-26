using DG.Tweening;
using LeTai.TrueShadow;
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

    [HideInInspector] public TitleSmokeEUIController OwnerEUIController;

    [HideInInspector] private RectTransform ThisRT;
    [HideInInspector] private Image ThisImg;
    [HideInInspector] private TrueShadow ThisTS;

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
        ThisTS = DevTool.Get_ComponentTType(gameObject, out TrueShadow ts) ? ts : null;
    }

    #endregion

    #region Play

    public void Play_Smoke(Sprite _Sprite, 
        CoupleData<Color> _Color, float _StartSize,
        float _StartPosX, float _MovingDis, float _DurTime)
    {
        ThisImg.sprite = _Sprite;
        ThisImg.color = _Color.TypeBase;
        this.gameObject.transform.localScale = Vector2.one * _StartSize;
        ThisRT.anchoredPosition = new Vector3(_StartPosX, 0, 0);
        ThisRT.rotation = Quaternion.identity;

        this.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Join(ThisRT.DOAnchorPosY(_MovingDis, _DurTime));
        seq.Join(ThisImg.DOColor(_Color.TypeSpecial, _DurTime));
        seq.Join(ThisRT.DOScale(0, _DurTime));
        seq.Join(ThisRT.DORotate(new Vector3(0, 0, Random.Range(-360, 360)), _DurTime, RotateMode.FastBeyond360));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            OwnerEUIController.Set_OP_Enqueue(this);
        });
    }

    #endregion

}
