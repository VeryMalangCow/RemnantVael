using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifyTextAmountForBuy : UIModule, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform ThisRT;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 MaximumSize = new Vector2(1350, 225);
    [SerializeField] private Vector2 MinimumSize = new Vector2(300, 225);
    [SerializeField] private float SizeDeltaTime = 0.2f;

    [Space(10)]
    [Header("=== Always Component")]
    [SerializeField] private Button AlwaysPanelBtn;
    [SerializeField] public TMP_Text SkillNameTxt;
    [SerializeField] private Image SkillIconImg;

    [Space(10)]
    [Header("=== Open Component")]
    [SerializeField] public ModifyImgAmountAndTxt ThisMIAAT;
    [SerializeField] public TMP_Text SimpleDescTxt;
    [SerializeField] public Image CostImg;
    [SerializeField] private List<Sprite> CostSpriteList;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT.sizeDelta = MinimumSize;
    }
    #endregion

    #region Unique

    public void Set(int _Level, int _CostValue)
    {
        ThisMIAAT.SetAmount(_Level, 0.1f);
        SetCostImg(_CostValue);
    }

    public void SetCostImg(int _CostValue)
    {
        CostImg.sprite = CostSpriteList[_CostValue];
    }

    #endregion

    #region Pointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DOTween.IsTweening(ThisRT)) 
        { DOTween.Kill(ThisRT); }

        ThisRT.DOSizeDelta(MaximumSize, SizeDeltaTime);

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetDesc(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DOTween.IsTweening(ThisRT)) 
        { DOTween.Kill(ThisRT); }

        ThisRT.DOSizeDelta(MinimumSize, SizeDeltaTime);
    }

    #endregion
}
