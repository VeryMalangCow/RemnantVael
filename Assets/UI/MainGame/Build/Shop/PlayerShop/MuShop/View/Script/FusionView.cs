using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FusionView : ForgeElementView
{
    [Header("=== Comp")]
    [SerializeField] public List<InventorySlotEUIController> fusionSlotList;
    [SerializeField] public TMP_Text preview_NeedMs_ForFusion;

    public override void Offset(ModuleUpgradeUIController ui, OwnBtnEUIController _panelBtn, string btnName, string btnDesc)
    {
        for (int i = 0; i < fusionSlotList.Count; i++)
        {
            fusionSlotList[i].ownerUIController = ui;
            fusionSlotList[i].Offset();
            fusionSlotList[i].item.Offset();
            fusionSlotList[i].item.ownerUIController = ui;

            fusionSlotList[i].Set_ForgeSelectedTxt(true, i);
        }
        base.Offset(ui, _panelBtn, btnName, btnDesc);

    }
    public override void ResetPanel()
    {
        for (int i = 0; i < fusionSlotList.Count; i++)
            fusionSlotList[i].item.gameObject.SetActive(false);

        preview_NeedMs_ForFusion.text = "-";
    }
}
