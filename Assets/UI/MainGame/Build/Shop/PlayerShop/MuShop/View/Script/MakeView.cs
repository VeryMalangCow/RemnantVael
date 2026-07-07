using TMPro;
using UnityEngine;

public class MakeView : ForgeElementView
{
    [Header("=== Comp")]
    [SerializeField] private TMP_Text preview_NeedMs_ForMake;
    [SerializeField] private TMP_Text preview_NeedCb_ForMake;

    public override void Offset(ModuleUpgradeUIController ui, OwnBtnEUIController _panelBtn)
    {
        preview_NeedMs_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        preview_NeedCb_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();

        base.Offset(ui, _panelBtn);
    }
    public override void ResetPanel()
    {
        preview_NeedMs_ForMake.text = ModuleItemManager.Get_MS_ForMake().ToString();
        preview_NeedCb_ForMake.text = ModuleItemManager.Get_CB_ForMake().ToString();
    }
}
