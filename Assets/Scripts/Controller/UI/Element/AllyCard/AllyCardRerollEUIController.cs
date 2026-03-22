using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class AllyCardRerollEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;
    [FormerlySerializedAs("NeedAmountTxt")][SerializeField] private TMP_Text needAmountTxt;
    [FormerlySerializedAs("ThisBtnRT")][SerializeField] private RectTransform btnRt;

    // Value
    [HideInInspector] private int needAmount = 1;
    [HideInInspector] private static readonly int maxNeedAmount = 5;

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
        Set_Amount();
    }

    #endregion
    
    #region Set

    private void Set_Amount()
    {
        string needAmountText = $"x <size=1{needAmount}0%><b>{needAmount}</b></size>";
        if (needAmount == maxNeedAmount)
            needAmountText += $"<size=75%>({ResourceManager.instance.Get_StaticWord(84)})</size>";

        needAmountTxt.text = needAmountText;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (needAmount > PlayerManager.instance.playerController.currentOverrider.Value ||
            targetCardEuiController == null)
            return;

        PlayerManager.instance.playerController.Add_CurrentOverrider(-needAmount);
        Play_Click();

        if (maxNeedAmount > needAmount)
        {
            needAmount++;
            Set_Amount();
        }
    }

    #endregion

    #region Play

    private void Play_Click()
    {
        DevTool.Set_KillTween(ThisRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(btnRt.DOScale(1.2f, 0.1f));
        seq.Append(btnRt.DOScale(1f, 0.1f));
    }

    #endregion


    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        DevTool.Set_KillTween(ThisRT);
        ThisRT.DOScale(1.1f, 0.1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        DevTool.Set_KillTween(ThisRT);
        ThisRT.DOScale(1.0f, 0.1f);
    }


    #endregion
}
