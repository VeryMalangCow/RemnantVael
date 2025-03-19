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
    [Header("=== Component")]
    [SerializeField] private RectTransform ThisRT;
    [SerializeField] public List<Image> InnerImgList;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 MaximumSize = new Vector2(1350, 225);
    [SerializeField] private Vector2 MinimumSize = new Vector2(300, 225);
    [SerializeField] private float SizeDeltaTime = 0.2f;

    [Space(10)]
    [Header("=== Always Component")]
    [SerializeField] private Button AlwaysPanelBtn;
    [SerializeField] public TMP_Text SkillNameTxt;
    [SerializeField] public TMP_Text SkillLvTxt;
    [SerializeField] public TMP_Text SkillOpenSimpleTxt;
    [SerializeField] private Image SkillIconImg;

    [Space(10)]
    [Header("=== Open Component")]
    [SerializeField] public ImgTxtAmountEUIController ThisMIAAT;
    [SerializeField] public TMP_Text SimpleDescTxt;
    [SerializeField] public Image CostImg;
    [SerializeField] private List<Sprite> CostSpriteList;
    [SerializeField] public OwnBtnEUIController BuyBtn;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT.sizeDelta = MinimumSize;
        BuyBtn.Offset();
    }

    #endregion

    #region Set

    public void Set_StateInfo(string _Name, string _Desc, BaseUpgradeUIController _Owner)
    {
        SkillNameTxt.text = _Name;
        SkillOpenSimpleTxt.text = _Desc;
        BuyBtn.OwnerUIController = _Owner;
    }

    public void Set(int _Level, int _CostValue)
    {
        ThisMIAAT.Set_Amount(_Level, 0.1f);
        Set_CostImg(_CostValue);
        SkillLvTxt.text = "[ LV : <b><#FFFFFF>" + _Level + "</color></b> ]";
    }

    public void Set_CostImg(int _CostValue)
    {
        CostImg.sprite = CostSpriteList[_CostValue];
    }

    public void Set_InnerAlpha(float _A)
    {
        for (int i = 0; i < InnerImgList.Count; i++) 
        {
            if (DOTween.IsTweening(InnerImgList[i]))
            { DOTween.Complete(InnerImgList[i]); }

            Sequence seq = DOTween.Sequence();
            seq.Append(InnerImgList[i].DOFade(1, 0.2f));
            seq.Append(InnerImgList[i].DOFade(_A, 0.2f));
        }
    }

    #endregion

    #region Pointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DOTween.IsTweening(ThisRT)) 
        { DOTween.Kill(ThisRT); }

        ThisRT.DOSizeDelta(MaximumSize, SizeDeltaTime);

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetOn_Desc(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DOTween.IsTweening(ThisRT)) 
        { DOTween.Kill(ThisRT); }

        ThisRT.DOSizeDelta(MinimumSize, SizeDeltaTime);

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_Desc();
    }

    #endregion
}
