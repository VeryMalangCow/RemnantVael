using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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
    [FormerlySerializedAs("InnerImgList")][SerializeField] public List<Image> innerImgList;

    [Space(5)]
    [Header("-- Size")]
    [FormerlySerializedAs("MaximumSize")][SerializeField] private float maximumSize = 1350;
    [FormerlySerializedAs("MinimumSize")][SerializeField] private float minimumSize = 300;
    [FormerlySerializedAs("SizeDeltaTime")][SerializeField] private float sizeDeltaTime = 0.2f;

    #endregion

    #region - Hide

    // Comp
    [HideInInspector] private RectTransform rt;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
        rt.sizeDelta = new Vector2(minimumSize, rt.sizeDelta.y);
        BuyBtn.Offset();
    }

    public void Offset(BaseUpgradeUIController owner)
    {
        this.Offset();
        ThisImgTxtAmountEUI.Offset();

        BuyBtn.ownerUIController = owner;
    }

    #endregion

    #region Set

    public void Set(int lv, int costValue)
    {
        ThisImgTxtAmountEUI.Set_Amount(lv, 0.2f);
        CostImg.sprite = BaseUpgradeManager.instance.costSpriteList[costValue];

        SkillLvTxt.text = "[ LV : <b><#FFFFFF>" + lv + "</color></b> ]";
    }


    public void Set_InnerAlpha(float a)
    {
        for (int i = 0; i < innerImgList.Count; i++) 
        {
            DevTool.Set_CompleteTween(innerImgList[i]);
            Sequence seq = DOTween.Sequence();

            seq.Append(innerImgList[i].DOFade(1, 0.2f));
            seq.Append(innerImgList[i].DOFade(a, 0.2f));
        }
    }

    #endregion

    #region Pointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        DevTool.Set_KillTween(rt);

        rt.DOSizeDelta(new Vector2(maximumSize, rt.sizeDelta.y), sizeDeltaTime);

        MainGameUIManager.instance.baseUpgrade_UIController.SetOn_Desc(this, SkillNameTxt.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DevTool.Set_KillTween(rt);

        rt.DOSizeDelta(new Vector2(minimumSize, rt.sizeDelta.y), sizeDeltaTime);

        MainGameUIManager.instance.baseUpgrade_UIController.SetOff_Desc();
    }

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt(string name, string desc)
    {
        SkillNameTxt.text = name;
        DescTxt.text = desc;
        DevTool.Get_ComponentTType<TMP_Text>(CostImg.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(CostImg, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(46);
        DevTool.Get_ComponentTType<TMP_Text>(BuyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(BuyBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(47);
    }

    #endregion
}