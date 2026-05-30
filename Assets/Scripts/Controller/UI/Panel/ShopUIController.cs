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
    [SerializeField] protected TMP_Text labelTxt;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] public DurablityEUIController durEui;
    [SerializeField] public MessageWindowEUIController msgEui;

    [Space(10)]
    [Header("=== Close")]
    [SerializeField] protected OwnBtnEUIController closeBtn;


    #endregion

    #region - Hide 

    // String
    [HideInInspector] public static string labelName;
    [HideInInspector] public static List<string> tabBtnTxtList;

    #endregion

    #endregion

    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

        Offset_Basic();
        Offset_ExtraColorComp();
    }

    private void Offset_Basic()
    {
        // Tab
        for (int i = 0; i < panelTabList.Count; i++)
        {
            panelTabList[i].Offset();
            panelTabList[i].tabBtn.ownerUIController = this;
        }

        // Dur
        durEui.Offset();

        // Close
        closeBtn.Offset();
        closeBtn.ownerUIController = this;

        // Broken
        msgEui.Offset();
    }

    private void Offset_ExtraColorComp()
    {
        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();   

        // Label
        mainColorCompList.Add(labelTxt);

        // Close
        subColorCompList.Add(closeBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

        // Tab Btn
        mainColorCompList.AddRange(Get_AllTabBtn_Txt());
        subColorCompList.AddRange(Get_AllTabBtn_Img());


        // Set Color
        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;
    }

    #endregion

    #region Framework

    protected virtual void OnEnable()
    {
        foreach (TabEUIController MET in panelTabList)
        {
            MET.Reset_ScrollBar();
        }
    }

    #endregion

    #region Interact (Panel)

    protected bool Is_Interact_Msg()
    {
        if (msgEui.gameObject.activeSelf)
        {
            if (msgEui.canPass) msgEui.Play_Off(0.5f);

            return true;
        }
        return false;
    }

    protected bool Is_Interact_CloseBtn()
    {
        if (currentBtn == closeBtn)
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
        DevTool.Get_ComponentTType<TMP_Text>(closeBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(closeBtn, 0)).gameObject).text =
            ResourceManager.instance.Get_StaticWord(28);

        // Dur
        durEui.Set_LanguageTxt();


        for (int i = 0; i < panelTabList.Count; i++)
            panelTabList[i].tabBtn.Offset_Txt(tabBtnTxtList[i]);
    }

    #endregion
}
