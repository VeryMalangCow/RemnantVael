using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifyDescPanel_ForModuleUpgrade : UIModule
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Desc _ For ModuleUpgrade")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] private Image ItemIconImg;
    [SerializeField] private TMP_Text ItemNameTxt;

    [Space(10)]
    [Header("=== Rank")]
    [SerializeField] private Image CurrentRankImg;
    [SerializeField] private TMP_Text CurrentRankTxt;

    [Space(10)]
    [Header("=== Boost Lv")]
    [SerializeField] private ModifyImgAmountAndTxt CurrentBoostLvMIAT;
    [SerializeField] private TMP_Text CurrentBoostLvTxt;

    // Other
    [HideInInspector] private RectTransform ThisRT;
    [HideInInspector] private float OriginalHeight;
    [HideInInspector] private CanvasGroup ThisCG;

    #endregion

    #region Offset

    public override void Offset()
    {
        if (ThisRT == null && this.TryGetComponent(out RectTransform rt))
        {
            ThisRT = rt;
            OriginalHeight = ThisRT.rect.height;

            ThisRT.sizeDelta = new Vector2(ThisRT.sizeDelta.x, 0f);
        }
        if (ThisCG == null && this.TryGetComponent(out CanvasGroup cg))
        {
            ThisCG = cg;

            ThisCG.alpha = 0f;
        }

        CurrentBoostLvMIAT.Offset();
    }

    #endregion

    #region Open / Close

    public void OpenThisPanel(float _DurTime)
    {
        ThisRT.DOSizeDelta(new Vector2(ThisRT.sizeDelta.x, OriginalHeight), _DurTime);
        ThisCG.DOFade(1f, _DurTime);
    }

    public void CloseThisPanel(float _DurTime)
    {
        ThisRT.DOSizeDelta(new Vector2(ThisRT.sizeDelta.x, 0f), _DurTime);
        ThisCG.DOFade(0f, _DurTime);
    }

    #endregion

    #region Desc

    public void SetDesc(PassiveSkill _PS)
    {
        foreach (Transform child in this.transform)
        { child.gameObject.SetActive(true); }

        // Item
        ItemIconImg.sprite = _PS.ThisItemData.Sprite;
        ItemNameTxt.text = _PS.ThisItemData.Name;

        // Rank
        CurrentRankImg.sprite = _PS.ThisMEII[0].RankImg.sprite;
        CurrentRankTxt.text = _PS.ThisItemData.Rank.ToString();

        // Boost Lv
        CurrentBoostLvMIAT.SetAmount(_PS.ThisItemData.BoostLv, 0.1f);
        CurrentBoostLvTxt.text = _PS.ThisItemData.BoostLv.ToString();
    }

    public void SetOffDesc()
    {
        foreach(Transform child in this.transform)
        { child.gameObject.SetActive(false); }
    }

    #endregion
}
