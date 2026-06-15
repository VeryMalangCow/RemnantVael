using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TxtAmountForBuyEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(10)]
    [Header("=== Open")]

    [Space(5)]
    [Header("-- Controller")]
    [SerializeField] public ImgTxtAmountEUIController imgTxtAmountEui;
    [SerializeField] public OwnBtnEUIController buyBtn;

    [Space(5)]
    [Header("-- Comp")]
    [SerializeField] public TMP_Text descTxt;
    [SerializeField] public Image costImg;


    [Space(10)]
    [Header("=== Always")]

    [Space(5)]
    [Header("-- Comp")]
    [SerializeField] private Button alwaysPanelBtn;
    [SerializeField] private Image skillIconImg;
    [SerializeField] public TMP_Text skillNameTxt;
    [SerializeField] public TMP_Text skillLvTxt;


    [Space(10)]
    [Header("=== Visual")]

    [Space(5)]
    [Header("-- Inner")]
    [SerializeField] public List<Image> innerImgList;

    [Space(5)]
    [Header("-- Size")]
    [SerializeField] private float maximumSize = 1350;
    [SerializeField] private float minimumSize = 300;
    [SerializeField] private float sizeDeltaTime = 0.2f;

    // Comp
    [HideInInspector] public RectTransform rt;

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
        rt.sizeDelta = new Vector2(minimumSize, rt.sizeDelta.y);
        buyBtn.Offset();
    }

    public void Offset(BaseUpgradeUIController owner, Sprite icon)
    {
        this.Offset();
        imgTxtAmountEui.Offset();
        skillIconImg.sprite = icon;
        skillIconImg.SetNativeSize();
        buyBtn.ownerUIController = owner;
    }

    #endregion

    #region Set

    public void Set(int lv, int costValue)
    {
        imgTxtAmountEui.Set_Amount(lv, 0.2f);
        costImg.sprite = BaseUpgradeManager.instance.costSpriteList[costValue];

        skillLvTxt.text = "[ LV : <b><#FFFFFF>" + lv + "</color></b> ]";
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
        DevTool.SetKillTween(rt);

        rt.DOSizeDelta(new Vector2(maximumSize, rt.sizeDelta.y), sizeDeltaTime);

        MainGameUIManager.instance.buUi.SetOn_Desc(this, skillNameTxt.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DevTool.SetKillTween(rt);

        rt.DOSizeDelta(new Vector2(minimumSize, rt.sizeDelta.y), sizeDeltaTime);

        MainGameUIManager.instance.buUi.SetOff_Desc();
    }

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt(string name, string desc)
    {
        skillNameTxt.text = name;
        descTxt.text = desc;
        DevTool.Get_ComponentTType<TMP_Text>(costImg.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(costImg, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(46);
        DevTool.Get_ComponentTType<TMP_Text>(buyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(buyBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(47);
    }

    #endregion
}