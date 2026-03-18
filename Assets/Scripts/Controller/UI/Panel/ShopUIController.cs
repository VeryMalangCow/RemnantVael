using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUIController : PanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] protected TMP_Text LabelTxt;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] public DurablityEUIController ThisDurEUI;
    [SerializeField] public MessageWindowEUIController ThisMsgEUI;

    [Space(10)]
    [Header("=== Close")]
    [SerializeField] protected OwnBtnEUIController CloseBtn;


    #endregion

    #region - Hide 

    // String
    [HideInInspector] public static string LabelName;
    [HideInInspector] public static List<string> TabBtnTxtList;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Basic();
        Offset_ExtraColorComp();
    }

    private void Offset_Basic()
    {
        // Tab
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            ThisPanelTabList[i].Offset();
            ThisPanelTabList[i].ThisTabBtn.OwnerUIController = this;
        }

        // Dur
        ThisDurEUI.Offset();

        // Close
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        // Broken
        ThisMsgEUI.Offset();
    }

    private void Offset_ExtraColorComp()
    {
        MainColorCompList = new List<Component>();
        SubColorCompList = new List<Component>();   

        // Label
        MainColorCompList.Add(LabelTxt);

        // Close
        SubColorCompList.Add(CloseBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

        // Tab Btn
        MainColorCompList.AddRange(Get_AllTabBtn_Txt());
        SubColorCompList.AddRange(Get_AllTabBtn_Img());


        // Set Color
        Color mainClr = PlayerManager.Instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    #endregion

    #region Framework

    protected virtual void OnEnable()
    {
        foreach (TabEUIController MET in ThisPanelTabList)
        {
            MET.Reset_ScrollBar();
        }
    }

    #endregion

    #region Interact (Panel)

    protected bool Is_Interact_Msg()
    {
        if (ThisMsgEUI.gameObject.activeSelf)
        {
            if (ThisMsgEUI.CanPass) ThisMsgEUI.Play_Off(0.5f);

            return true;
        }
        return false;
    }

    protected bool Is_Interact_CloseBtn()
    {
        if (CurrentBtn == CloseBtn)
        {
            SetOff_ThisPanel();
            return true;
        }
        return false;
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Close
        DevTool.Get_ComponentTType<TMP_Text>(CloseBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(CloseBtn, 0)).gameObject).text =
            ResourceManager.Instance.Get_StaticWord(28);

        // Dur
        ThisDurEUI.Set_LanguageTxt();


        for (int i = 0; i < ThisPanelTabList.Count; i++)
            ThisPanelTabList[i].ThisTabBtn.Offset_Txt(TabBtnTxtList[i]);
    }

    #endregion
}
