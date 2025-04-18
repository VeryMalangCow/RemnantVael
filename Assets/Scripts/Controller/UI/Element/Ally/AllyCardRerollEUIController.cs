using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AllyCardRerollEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text NeedAmountTxt;
    [SerializeField] private RectTransform ThisBtnRT;

    // Value
    [HideInInspector] private int NeedAmount = 1;
    [HideInInspector] private static readonly int MaxNeedAmount = 5;

    // Owner
    [HideInInspector] public AllyCardEUIController TargetCardEUIController;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        NameTxt.text = CSVManager.Instance.Get_StaticWord(57);
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
        NeedAmount = 1;
        Set_Amount();
    }

    #endregion
    
    #region Set

    private void Set_Amount()
    {
        string needAmountText = $"x <size=1{NeedAmount}0%><b>{NeedAmount}</b></size>";
        if (NeedAmount == MaxNeedAmount)
            needAmountText += $"<size=75%>({CSVManager.Instance.Get_StaticWord(84)})</size>";

        NeedAmountTxt.text = needAmountText;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (NeedAmount > PlayerManager.Instance.PlayerController.CurrentOverrider.Value ||
            TargetCardEUIController == null)
            return;

        PlayerManager.Instance.PlayerController.Add_CurrentOverrider(-NeedAmount);
        Play_Click();

        if (MaxNeedAmount > NeedAmount)
        {
            NeedAmount++;
            Set_Amount();
        }
    }

    #endregion

    #region Play

    private void Play_Click()
    {
        DevTool.Set_KillTween(ThisRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(ThisBtnRT.DOScale(1.2f, 0.1f));
        seq.Append(ThisBtnRT.DOScale(1f, 0.1f));
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
