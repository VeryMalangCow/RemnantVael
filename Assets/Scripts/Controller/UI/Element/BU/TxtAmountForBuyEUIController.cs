using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TxtAmountForBuyEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Open")]

    [Space(5)]
    [Header("-- Controller")]
    [SerializeField] public ImgTxtAmountEUIController ThisImgTxtAmountEUI;
    [SerializeField] public OwnBtnEUIController BuyBtn;

    [Space(5)]
    [Header("-- Comp")]
    [SerializeField] public TMP_Text DescTxt;
    [SerializeField] public Image CostImg;


    [Space(10)]
    [Header("=== Always")]

    [Space(5)]
    [Header("-- Comp")]
    [SerializeField] private Button AlwaysPanelBtn;
    [SerializeField] private Image SkillIconImg;
    [SerializeField] public TMP_Text SkillNameTxt;
    [SerializeField] public TMP_Text SkillLvTxt;


    [Space(10)]
    [Header("=== Visual")]

    [Space(5)]
    [Header("-- Inner")]
    [SerializeField] public List<Image> InnerImgList;

    [Space(5)]
    [Header("-- Size")]
    [SerializeField] private float MaximumSize = 1350;
    [SerializeField] private float MinimumSize = 300;
    [SerializeField] private float SizeDeltaTime = 0.2f;

    #endregion

    #region - Hide

    // Comp
    [HideInInspector] private RectTransform ThisRT;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
        ThisRT.sizeDelta = new Vector2(MinimumSize, ThisRT.sizeDelta.y);
        BuyBtn.Offset();
    }

    public void Offset(string _Name, string _Desc, BaseUpgradeUIController _Owner)
    {
        this.Offset();
        ThisImgTxtAmountEUI.Offset();

        SkillNameTxt.text = _Name;
        DescTxt.text = _Desc;
        BuyBtn.OwnerUIController = _Owner;
    }

    #endregion

    #region Set

    public void Set(int _Level, int _CostValue)
    {
        ThisImgTxtAmountEUI.Set_Amount(_Level, 0.2f);
        CostImg.sprite = BaseUpgradeManager.Instance.CostSpriteList[_CostValue];

        SkillLvTxt.text = "[ LV : <b><#FFFFFF>" + _Level + "</color></b> ]";
    }


    public void Set_InnerAlpha(float _A)
    {
        for (int i = 0; i < InnerImgList.Count; i++) 
        {
            DevTool.Set_CompleteTween(InnerImgList[i]);
            Sequence seq = DOTween.Sequence();

            seq.Append(InnerImgList[i].DOFade(1, 0.2f));
            seq.Append(InnerImgList[i].DOFade(_A, 0.2f));
        }
    }

    #endregion

    #region Pointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        DevTool.Set_KillTween(ThisRT);

        ThisRT.DOSizeDelta(new Vector2(MaximumSize, ThisRT.sizeDelta.y), SizeDeltaTime);

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetOn_Desc(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DevTool.Set_KillTween(ThisRT);

        ThisRT.DOSizeDelta(new Vector2(MinimumSize, ThisRT.sizeDelta.y), SizeDeltaTime);

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_Desc();
    }

    #endregion
}