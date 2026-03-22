using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ConverterUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    [Space(10)]
    [Header("=== Label")]
    [FormerlySerializedAs("LabelTxt")][SerializeField] protected TMP_Text labelTxt;

    [Space(10)]
    [Header("=== EUI")]
    [FormerlySerializedAs("CvtAcquisitionEUI")][SerializeField] protected CvtAcquisitionEUIController cvtAcquisitionEui;
    [FormerlySerializedAs("AcquisitionItemID")][SerializeField] protected int acquisitionItemId;

    [Space(10)]
    [Header("=== Close")]
    [FormerlySerializedAs("CloseBtn")][SerializeField] protected OwnBtnEUIController closeBtn;

    #endregion

    #region - Hide 

    // String
    [HideInInspector] public static string labelName;

    // Data
    [HideInInspector] protected int acquisitionBookAmount = 1;
    [HideInInspector] protected bool canConvert = false;
    [HideInInspector] private bool convertingNow = false;

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
        closeBtn.Offset();
        closeBtn.ownerUIController = this;

        cvtAcquisitionEui.Offset();
        cvtAcquisitionEui.Offset_Owner(this);
    }

    private void Offset_ColorComp()
    {
        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();

        // Label
        mainColorCompList.Add(labelTxt);

        // Close
        subColorCompList.Add(closeBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());


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

    #region Interact

    public virtual bool Try_Interact()
    {
        if (Is_Interact_CloseBtn()) return true;

        if (currentBtn != null && !convertingNow)
        {
            if (currentBtn == cvtAcquisitionEui.maxBtn)
            {
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                Set_MaxAcquBookAmount();
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.minBtn)
            {
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                Set_MinAcquBookAmount();
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.more1Btn)
            {
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                Set_MoreAcquBookAmount(1);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.more10Btn)
            {
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                Set_MoreAcquBookAmount(10);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.less1Btn)
            {
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                Set_LessAcquBookAmount(1);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.less10Btn)
            {
                SoundManager.instance.Play_2D_SFX_UI("Click_01");
                Set_LessAcquBookAmount(10);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.convertBtn)
            {
                if (canConvert)
                {
                    Play_Convert();
                }
                else
                {
                    Play_Failure();
                }

                return true;
            }
        }

        return false;
    }

    #endregion

    #region Interact (Detail)

    protected bool Is_Interact_CloseBtn()
    {
        if (currentBtn == closeBtn)
        {
            if (!convertingNow) 
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
    protected void Set_MoreAcquBookAmount(int amount)
    {
        Set_AcquBookAmount(acquisitionBookAmount + amount);
    }

    // 감소
    protected void Set_LessAcquBookAmount(int amount)
    {
        Set_AcquBookAmount(acquisitionBookAmount - amount);
    }

    // 이미 가진 아이템
    protected void Set_AcquAmount(int _ItemID)
    {
        cvtAcquisitionEui.Set_PossessionAmountTxt(
            SaveDataManager.instance.jsonData.Get_ItemAmount(acquisitionItemId).ToString());
    }

    // Data
    protected virtual void Set_AcquBookAmount(int amount)
    {
        acquisitionBookAmount = Mathf.Clamp(amount, 1, 999);

        // UI
        cvtAcquisitionEui.Set_AcquisitionAmountTxt(acquisitionBookAmount.ToString());
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
        SoundManager.instance.Play_2D_SFX_UI("Click_Reject");

        convertingNow = true;

        cvtAcquisitionEui.Play_FailComp();
        yield return new WaitForSeconds(0.6f);

        convertingNow = false;
    }

    private IEnumerator Play_Convert_Cor()
    {
        SoundManager.instance.Play_2D_SFX_UI("Make");

        convertingNow = true;

        cvtAcquisitionEui.Play_VisualComp();
        yield return new WaitForSeconds(2.1f);

        Convert();

        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

        cvtAcquisitionEui.Play_SuccessComp();
        yield return new WaitForSeconds(0.6f);


        convertingNow = false;
    }

    protected virtual void Convert()
    {
        SaveDataManager.instance.jsonData.Gain_Item(acquisitionItemId, acquisitionBookAmount);
        Set_AcquAmount(acquisitionItemId);
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");
        Set_AcquAmount(acquisitionItemId);
        Set_AcquBookAmount(1);
    }

    public override void SetOff_ThisPanel()
    {
        if (convertingNow) return;

        SoundManager.instance.Play_2D_SFX_UI("Click_Reject");
        base.SetOff_ThisPanel();
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Close
        DevTool.Get_ComponentTType<TMP_Text>(closeBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(closeBtn, 0)).gameObject).text =
            ResourceManager.instance.Get_StaticWord(28);

        // EUI
        cvtAcquisitionEui.Set_Language(canConvert);
    }

    #endregion
}
