using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySyncIconEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public int id;
    [SerializeField] private Image iconImg;

    [Space(5)]
    [Header("-- Progress")]
    [SerializeField] private Image progressImg;
    [SerializeField] private TMP_Text progressTxt;
    [SerializeField] private TMP_Text progressMaxTxt;

    [Space(5)]
    [Header("-- State")]
    [SerializeField] private Image applyStateImg;
    [SerializeField] private Image connectStateImg;

    [Space(2)]
    [SerializeField] private CanvasGroup completelyCg;

    [HideInInspector] public RectTransform rt;

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
        progressMaxTxt.text = $"/{AllyController.syncMax}";

        Set_ConnectUI(false);
        Set_Color();
    }

    #endregion

    #region Set

    public void Set_UI(int id, int amount)
    {
        this.id = id;

        MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(id);
        
        iconImg.sprite = MDC.thisIcon;
        progressImg.sprite = MainGameUIManager.instance.amuUi.profileDetailEui.Get_SyncProgressSprite(amount);
        progressTxt.text = amount.ToString();
        float progressing = (float)amount / AllyController.syncMax;
        DevTool.Set_AlphaColor(progressTxt, progressing);

        applyStateImg.gameObject.SetActive(progressing >= 1 ? true : false);
    }

    public void Set_ConnectUI(bool isConnect)
    {
        connectStateImg.gameObject.SetActive(isConnect);
    }

    public void Set_Completely(bool isCompletely)
    {
        completelyCg.gameObject.SetActive(isCompletely);
    }


    public void Set_Color()
    {
        connectStateImg.color = PlayerManager.instance.playerController.Get_CorrectColor(DamageType.Energy, false);
        List<Image> list = DevTool.Get_ChildList<Image>(completelyCg.gameObject.transform);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].color = PlayerManager.instance.playerController.Get_CorrectColor(DamageType.Energy, false);
        }
    }

    #endregion
}
