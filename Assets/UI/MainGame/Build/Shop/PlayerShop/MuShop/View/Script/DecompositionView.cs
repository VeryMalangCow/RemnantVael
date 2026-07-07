using TMPro;
using UnityEngine;

public class DecompositionView : ForgeElementView
{
    [Header("=== Comp")]
    [SerializeField] public InventorySlotEUIController thisSlot;
    [SerializeField] private TMP_Text previewGainMs;
    [SerializeField] private TMP_Text previewGainBc;


    public override void Offset(ModuleUpgradeUIController ui, OwnBtnEUIController _panelBtn)
    {
        thisSlot.ownerUIController = ui;
        thisSlot.Offset();
        thisSlot.item.Offset();
        thisSlot.item.ownerUIController = ui;

        thisSlot.Set_ForgeSelectedTxt(true);

        base.Offset(ui, _panelBtn);
    }

    public override void ResetPanel()
    {
        thisSlot.item.gameObject.SetActive(false);

        previewGainBc.text = "-";
        previewGainMs.text = "-";
    }

    // ºÐÇØ ½½·Ô UI ¼Â
    public void Set_DecompositionUI(bool setActive, string gainBc, string gainMs)
    {
        thisSlot.item.gameObject.SetActive(setActive);
        previewGainBc.text = gainBc;
        previewGainMs.text = gainMs;
    }

    public void SetItemData(InventoryItemEUIController itemEui)
    {
        thisSlot.item.Set_Data(itemEui);
    }

}
