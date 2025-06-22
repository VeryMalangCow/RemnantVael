using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySyncIconEUIController : ElementUIController
{
    #region Value

    [SerializeField] private Image ThisIconImg;
    [SerializeField] private Image ProgressImg;
    [SerializeField] private TMP_Text ProgressTxt;
    [SerializeField] private TMP_Text ProgressMaxTxt;

    [HideInInspector] public RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
        ProgressMaxTxt.text = $"/{AllyController.SyncMax}";
    }

    #endregion

    #region Set

    public void Set_UI(int _ID, int _Amount)
    {
        MainChipData MDC = ModuleItemManager.Instance.Get_CorrectMainChip(_ID);
        
        ThisIconImg.sprite = MDC.ThisIcon;
        ProgressImg.sprite = MainGameUIManager.Instance.AllyModuleUpgrade_UIController.PickedPanelSyncProgressSpriteList[_Amount - 1];
        ProgressTxt.text = _Amount.ToString();
        DevTool.Set_AlphaColor(ProgressImg, (float)_Amount / AllyController.SyncMax);
        DevTool.Set_AlphaColor(ProgressTxt, (float)_Amount / AllyController.SyncMax);
    }

    #endregion
}
