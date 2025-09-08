using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ConverterUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] protected TMP_Text LabelTxt;

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] protected CvtAcquisitionEUIController CvtAcquisitionEUI;
    [SerializeField] protected int AcquisitionItemID;

    [Space(10)]
    [Header("=== Close")]
    [SerializeField] protected OwnBtnEUIController CloseBtn;

    #endregion

    #region - Hide 

    // String
    [HideInInspector] public static string LabelName;

    // Data
    [HideInInspector] protected int AcquisitionBookAmount = 1;
    [HideInInspector] protected bool CanConvert = false;
    [HideInInspector] private bool ConvertingNow = false;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Basic();
        Offset_ColorComp();

        Set_LanguageTxt();
    }

    private void Offset_Basic()
    {
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        CvtAcquisitionEUI.Offset();
        CvtAcquisitionEUI.Offset_Owner(this);
    }

    private void Offset_ColorComp()
    {
        MainColorCompList = new List<Component>();
        SubColorCompList = new List<Component>();

        // Label
        MainColorCompList.Add(LabelTxt);

        // Close
        SubColorCompList.Add(CloseBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());


        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    #endregion

    #region Interact

    public virtual bool Try_Interact()
    {
        if (Is_Interact_CloseBtn()) return true;

        if (CurrentBtn != null && !ConvertingNow)
        {
            if (CurrentBtn == CvtAcquisitionEUI.MaxBtn)
            {
                Set_MaxAcquBookAmount();
                return true;
            }
            else if (CurrentBtn == CvtAcquisitionEUI.MinBtn)
            {
                Set_MinAcquBookAmount();
                return true;
            }
            else if (CurrentBtn == CvtAcquisitionEUI.More1Btn)
            {
                Set_MoreAcquBookAmount(1);
                return true;
            }
            else if (CurrentBtn == CvtAcquisitionEUI.More10Btn)
            {
                Set_MoreAcquBookAmount(10);
                return true;
            }
            else if (CurrentBtn == CvtAcquisitionEUI.Less1Btn)
            {
                Set_LessAcquBookAmount(1);
                return true;
            }
            else if (CurrentBtn == CvtAcquisitionEUI.Less10Btn)
            {
                Set_LessAcquBookAmount(10);
                return true;
            }
            else if (CurrentBtn == CvtAcquisitionEUI.ConvertBtn)
            {
                if (CanConvert)
                    Play_Convert();
                else
                    Play_Failure();

                return true;
            }
        }

        return false;
    }

    #endregion

    #region Interact (Detail)

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

    #region Acquisition

    // 최대
    protected abstract void Set_MaxAcquBookAmount();

    // 최소 (1)
    protected void Set_MinAcquBookAmount()
    {
        Set_AcquBookAmount(1);
    }

    // 증가
    protected void Set_MoreAcquBookAmount(int _Amount)
    {
        Set_AcquBookAmount(AcquisitionBookAmount + _Amount);
    }

    // 감소
    protected void Set_LessAcquBookAmount(int _Amount)
    {
        Set_AcquBookAmount(AcquisitionBookAmount - _Amount);
    }

    // 이미 가진 아이템
    protected void Set_AcquAmount(int _ItemID)
    {
        CvtAcquisitionEUI.Set_PossessionAmountTxt(
            SaveDataManager.Instance.JsonData.Get_ItemAmount(AcquisitionItemID).ToString());
    }

    // Data
    protected virtual void Set_AcquBookAmount(int _Amount)
    {
        AcquisitionBookAmount = Mathf.Clamp(_Amount, 1, 999);

        // UI
        CvtAcquisitionEUI.Set_AcquisitionAmountTxt(AcquisitionBookAmount.ToString());
    }

    #endregion

    #region Convert

    protected virtual void Play_Failure()
    {
        StartCoroutine(Play_Failure_Cor());
    }
    protected virtual void Play_Convert()
    {
        StartCoroutine(Play_Convert_Cor());
    }


    private IEnumerator Play_Failure_Cor()
    {
        ConvertingNow = true;

        CvtAcquisitionEUI.Play_FailComp();
        yield return new WaitForSeconds(0.6f);

        ConvertingNow = false;
    }

    private IEnumerator Play_Convert_Cor()
    {
        ConvertingNow = true;

        CvtAcquisitionEUI.Play_VisualComp();
        yield return new WaitForSeconds(2.1f);

        Convert();

        CvtAcquisitionEUI.Play_SuccessComp();
        yield return new WaitForSeconds(0.6f);

        ConvertingNow = false;
    }

    protected virtual void Convert()
    {
        SaveDataManager.Instance.JsonData.Gain_Item(AcquisitionItemID, AcquisitionBookAmount);
        Set_AcquAmount(AcquisitionItemID);
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();
        Set_AcquAmount(AcquisitionItemID);
        Set_AcquBookAmount(1);
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Close
        DevTool.Get_ComponentTType<TMP_Text>(CloseBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(CloseBtn, 0)).gameObject).text =
            ResourceManager.Instance.Get_StaticWord(28);

        // EUI
        CvtAcquisitionEUI.Set_Language(CanConvert);
    }

    #endregion
}
