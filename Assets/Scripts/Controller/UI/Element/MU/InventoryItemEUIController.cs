using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemEUIController : OwnBtnEUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Partner")]
    [SerializeField] public InventorySlotEUIController ThisSlot = null;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(90, 90);

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Image ThisImg;
    [SerializeField] public Image RankImg;

    // Seq
    [HideInInspector] private static readonly float SelectSize = 1.1f;
    [HideInInspector] private static readonly float SelectDurTime = 0.1f;
    [HideInInspector] private Sequence SignSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisRT.sizeDelta = ThisSizeDelta;
    }

    #endregion

    #region Set

    public void Set_Data(InventoryItemEUIController _ItemEUI)
    {
        ThisImg.sprite = _ItemEUI.ThisImg.sprite;
        RankImg.sprite = _ItemEUI.RankImg.sprite;
        RankImg.SetNativeSize();
    }

    public void Set_Data(ItemData_UIVisual _State)
    {
        ThisImg.sprite = _State.Icon;
        RankImg.sprite = _State.RankIcon;
        RankImg.SetNativeSize();
    }

    public void Set_EquipedImg(bool _IsOn)
    {
        DevTool.Set_AlphaColor(ThisImg, _IsOn ? 0.35f : 1f);
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect)
        { return; }

        Play_Selected(SelectSize, SelectDurTime);

        OwnerUIController.CurrentItemBtn = this;

        if (DevTool.Can_CastingTType(OwnerUIController, out ModuleUpgradeUIController muui))
            muui.SetOn_Desc(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect)
        { return; }

        Play_Selected(1f, SelectDurTime);

        OwnerUIController.CurrentItemBtn = null;

        if (DevTool.Can_CastingTType(OwnerUIController, out ModuleUpgradeUIController muui))
            muui.SetOff_Desc();
    }

    #endregion

    #region Selected

    private void Play_Selected(float _TargetScale, float _DurTime)
    {
        DevTool.Set_KillTween(SignSeq);
        SignSeq = DOTween.Sequence();

        SignSeq.Append(ThisRT.DOScale(_TargetScale, _DurTime).SetEase(Ease.Linear));
    }

    #endregion
}
