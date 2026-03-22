using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryItemEUIController : OwnBtnEUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Partner")]
    [FormerlySerializedAs("ThisSlot")][SerializeField] public InventorySlotEUIController slot = null;

    [Space(10)]
    [Header("=== Value")]
    [FormerlySerializedAs("ThisSizeDelta")][SerializeField] private Vector2 sizeDelta = new Vector2(90, 90);

    [Space(10)]
    [Header("=== Component")]
    [FormerlySerializedAs("ThisImg")][SerializeField] public Image thisImg;
    [FormerlySerializedAs("RankImg")][SerializeField] public Image rankImg;

    // Seq
    [HideInInspector] private static readonly float selectSize = 1.1f;
    [HideInInspector] private static readonly float selectDurTime = 0.1f;
    [HideInInspector] private Sequence signSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        rt.sizeDelta = sizeDelta;
    }

    #endregion

    #region Set

    public void Set_Data(InventoryItemEUIController itemEui)
    {
        thisImg.sprite = itemEui.thisImg.sprite;
        rankImg.sprite = itemEui.rankImg.sprite;
        rankImg.SetNativeSize();
    }

    public void Set_Data(ItemData_UIVisual state)
    {
        thisImg.sprite = state.icon;
        rankImg.sprite = state.rankIcon;
        rankImg.SetNativeSize();
    }

    public void Set_EquipedImg(bool isOn)
    {
        DevTool.Set_AlphaColor(thisImg, isOn ? 0.35f : 1f);
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || ownerUIController == null)
        { return; }

        Play_Selected(selectSize, selectDurTime);

        if (ownerUIController == null)
        {
            return;
        }

        ownerUIController.currentItemBtn = this;

        if (DevTool.Can_CastingTType(ownerUIController, out ModuleUpgradeUIController muui))
            muui.SetOn_Desc(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || ownerUIController == null)
        { return; }

        Play_Selected(1f, selectDurTime);

        ownerUIController.currentItemBtn = null;

        if (DevTool.Can_CastingTType(ownerUIController, out ModuleUpgradeUIController muui))
            muui.SetOff_Desc();
    }

    #endregion

    #region Selected

    private void Play_Selected(float targetScale, float durTime)
    {
        DevTool.Set_KillTween(signSeq);
        signSeq = DOTween.Sequence();

        signSeq.Append(rt.DOScale(targetScale, durTime).SetEase(Ease.Linear));
    }

    #endregion
}
