using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakeView : ForgeElementView
{
    [Header("=== Comp")]
    [SerializeField] private TMP_Text preview_NeedMs_ForMake;
    [SerializeField] private TMP_Text preview_NeedCb_ForMake;

    public override void Offset(ModuleUpgradeUIController ui, OwnBtnEUIController _panelBtn, string btnName, string btnDesc)
    {
        preview_NeedMs_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        preview_NeedCb_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();

        base.Offset(ui, _panelBtn, btnName, btnDesc);
    }
    public override void ResetPanel()
    {
        preview_NeedMs_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        preview_NeedCb_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();
    }
}
