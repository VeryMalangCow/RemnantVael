using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySyncIconEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public int ID;
    [SerializeField] private Image ThisIconImg;

    [Space(5)]
    [Header("-- Progress")]
    [SerializeField] private Image ProgressImg;
    [SerializeField] private TMP_Text ProgressTxt;
    [SerializeField] private TMP_Text ProgressMaxTxt;

    [Space(5)]
    [Header("-- State")]
    [SerializeField] private Image ThisApplyStateImg;
    [SerializeField] private Image ThisConnectStateImg;

    [Space(2)]
    [SerializeField] private CanvasGroup CompletelyCG;

    [HideInInspector] public RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
        ProgressMaxTxt.text = $"/{AllyController.SyncMax}";

        Set_ConnectUI(false);
        Set_Color();
    }

    #endregion

    #region Set

    public void Set_UI(int _ID, int _Amount)
    {
        ID = _ID;

        MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(_ID);
        
        ThisIconImg.sprite = MDC.thisIcon;
        ProgressImg.sprite = MainGameUIManager.instance.allyModuleUpgrade_UIController.Get_SyncProgressSprite(_Amount);
        ProgressTxt.text = _Amount.ToString();
        float progressing = (float)_Amount / AllyController.SyncMax;
        DevTool.Set_AlphaColor(ProgressTxt, progressing);

        ThisApplyStateImg.gameObject.SetActive(progressing >= 1 ? true : false);
    }

    public void Set_ConnectUI(bool _IsConnect)
    {
        ThisConnectStateImg.gameObject.SetActive(_IsConnect);
    }

    public void Set_Completely(bool _IsCompletely)
    {
        CompletelyCG.gameObject.SetActive(_IsCompletely);
    }


    public void Set_Color()
    {
        ThisConnectStateImg.color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        List<Image> list = DevTool.Get_ChildList<Image>(CompletelyCG.gameObject.transform);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        }
    }

    #endregion
}
