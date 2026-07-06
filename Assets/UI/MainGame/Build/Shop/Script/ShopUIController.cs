using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ShopUIController : PanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] protected TMP_Text labelTxt;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] private DurablityEUIController durEuiPrefab;
    public DurablityEUIController durEui { get; private set; }
    [SerializeField] private MessageWindowEUIController msgEuiPrefab;
    public MessageWindowEUIController msgEui { get; private set; }

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainTmps;
    [SerializeField] private TMP_Text[] subTmps;

    [Space(10)]
    [Header("=== Close")]
    [SerializeField] protected OwnBtnEUIController closeBtn;

    public static string labelName;
    public static List<string> tabBtnTxtList;

    #endregion

    #region Init

    public virtual IEnumerator InitAsync(Color mainClr, Color subClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        // dur
        durEui = Instantiate(durEuiPrefab, transform);
        durEui.Offset();

        // msg
        msgEui = Instantiate(msgEuiPrefab, transform);
        msgEui.Offset(); 
        msgEui.Reset_Data();

        // data
        for (int i = 0; i < panelTabList.Count; i++)
        {
            panelTabList[i].Offset();
            panelTabList[i].tabBtn.ownerUIController = this;
        }

        // Close
        closeBtn.Offset();
        closeBtn.ownerUIController = this;

        SetColor(mainClr, subClr);

#if UNITY_EDITOR        
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Dur + Msg + DataSet</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

#endregion

    #region Color

    private void SetColor(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainTmps);
        DevTool.SetColorTmps(subClr, subTmps);

        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();   

        // Tab Btn
        mainColorCompList.AddRange(Get_AllTabBtn_Txt());
        subColorCompList.AddRange(Get_AllTabBtn_Img());

        // Set Color
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;
    }

    #endregion

    #region Mono

    protected virtual void OnEnable()
    {
        foreach (TabEUIController MET in panelTabList)
        {
            MET.Reset_ScrollBar();
        }

        if (msgEui != null)
            msgEui.Reset_Data();
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

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

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
