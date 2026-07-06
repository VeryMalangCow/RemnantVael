using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AllyCardRerollEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text needAmountTxt;
    [SerializeField] private RectTransform btnRt;

    // Value
    private int needAmount = 1;
    private static readonly int maxNeedAmount = 5;

    // Owner
    [HideInInspector] public AllyCardEUIController targetCardEuiController;

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        nameTxt.text = ResourceManager.instance.Get_StaticWord(57);
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        Reset_Amount();
    }

    #endregion

    #region Reset

    private void Reset_Amount()
    {
        needAmount = 1;
        SetAmount();
    }

    #endregion
    
    #region Set

    private void SetAmount()
    {
        string needAmountText = $"x <size=1{needAmount}0%><b>{needAmount}</b></size>";
        if (needAmount == maxNeedAmount)
            needAmountText += $"<size=75%>({ResourceManager.instance.Get_StaticWord(84)})</size>";

        needAmountTxt.SetText(needAmountText);
    }

    #endregion

    #region Interact

    public bool TryInteract()
    {
        if (needAmount > PlayerManager.instance.playerController.overrider ||
            targetCardEuiController == null)
            return false;

        PlayerManager.instance.playerController.UseOverrider(needAmount);
        Play_Click();

        if (maxNeedAmount > needAmount)
        {
            needAmount++;
            SetAmount();
        }
        return true;
    }

    #endregion

    #region Play

    private void Play_Click()
    {
        DevTool.SetKillTween(rt);

        Sequence seq = DOTween.Sequence();
        seq.Append(btnRt.DOScale(1.2f, 0.1f));
        seq.Append(btnRt.DOScale(1f, 0.1f));
    }

    #endregion


    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        DevTool.SetKillTween(rt);
        rt.DOScale(1.1f, 0.1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        DevTool.SetKillTween(rt);
        rt.DOScale(1.0f, 0.1f);
    }


    #endregion
}
