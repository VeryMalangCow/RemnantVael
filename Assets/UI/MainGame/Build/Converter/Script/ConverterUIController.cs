using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public abstract class ConverterUIController : SinglePanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt.")]
    [SerializeField] private Transform cvtEuiParentTf;

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] private CvtMaterialEUIController cvtMaterialEuiPrefab;
    [SerializeField] private CvtMaterialData[] cvtMaterialDatas;
    protected CvtMaterialEUIController[] cvtMaterialEuis;

    [System.Serializable]
    public class CvtMaterialData
    {
        public float posY;
        public Sprite icon;
        public float rotateZ;
    }

    [Space(10)]
    [Header("=== Acquisition")]
    [SerializeField] private CvtAcquisitionEUIController cvtAcquisitionEuiPrefab;
    [SerializeField] private Sprite acquisitionItemIcon;
    protected CvtAcquisitionEUIController cvtAcquisitionEui;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected TMP_Text labelTxt;
    [SerializeField] protected OwnBtnEUIController closeBtn;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainTmps;
    [SerializeField] private TMP_Text[] subTmps;

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] protected int acquisitionItemId;

    // String
    [HideInInspector] public static string labelName;

    // Data
    [HideInInspector] protected int acquisitionBookAmount = 1;
    [HideInInspector] protected bool canConvert = false;
    [HideInInspector] private bool convertingNow = false;

    #endregion

    #region Init

    public virtual IEnumerator InitAsync(Color mainClr, Color subClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        closeBtn.Offset();
        closeBtn.ownerUIController = this;

        SetColor(mainClr, subClr);

        int len = cvtMaterialDatas.Length;
        cvtMaterialEuis = new CvtMaterialEUIController[len];
        for (int i = 0; i < len; i++)
        {
            cvtMaterialEuis[i] = Instantiate(cvtMaterialEuiPrefab, cvtEuiParentTf);
            cvtMaterialEuis[i].Init(cvtMaterialDatas[i]);
        }

        cvtAcquisitionEui = Instantiate(cvtAcquisitionEuiPrefab, cvtEuiParentTf);
        cvtAcquisitionEui.Offset();
        cvtAcquisitionEui.Init(this, acquisitionItemIcon);

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Data Set + Material EUI + Acquisition EUI</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    #endregion

    #region Color

    private void SetColor(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainTmps);
        DevTool.SetColorTmps(subClr, subTmps);
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
                SoundManager.instance.PlayUiSfx("Click01");
                Set_MaxAcquBookAmount();
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.minBtn)
            {
                SoundManager.instance.PlayUiSfx("Click01");
                Set_MinAcquBookAmount();
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.more1Btn)
            {
                SoundManager.instance.PlayUiSfx("Click01");
                Set_MoreAcquBookAmount(1);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.more10Btn)
            {
                SoundManager.instance.PlayUiSfx("Click01");
                Set_MoreAcquBookAmount(10);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.less1Btn)
            {
                SoundManager.instance.PlayUiSfx("Click01");
                Set_LessAcquBookAmount(1);
                return true;
            }
            else if (currentBtn == cvtAcquisitionEui.less10Btn)
            {
                SoundManager.instance.PlayUiSfx("Click01");
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
            SaveDataManager.instance.GetItemAmount(acquisitionItemId).ToString());
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
        SoundManager.instance.PlayUiSfx("Reject");

        convertingNow = true;

        cvtAcquisitionEui.Play_FailComp();
        yield return new WaitForSeconds(0.6f);

        convertingNow = false;
    }

    private IEnumerator Play_Convert_Cor()
    {
        SoundManager.instance.PlayUiSfx("Make");

        convertingNow = true;

        cvtAcquisitionEui.Play_VisualComp();
        yield return new WaitForSeconds(2.1f);

        Convert();

        SoundManager.instance.PlayUiSfx("Approve");

        cvtAcquisitionEui.Play_SuccessComp();
        yield return new WaitForSeconds(0.6f);


        convertingNow = false;
    }

    protected virtual void Convert()
    {
        SaveDataManager.instance.GainHighLvItem(acquisitionItemId, acquisitionBookAmount);
        Set_AcquAmount(acquisitionItemId);
    }

    #endregion

    #region Panel

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        SoundManager.instance.PlayUiSfx("Approve");
        Set_AcquAmount(acquisitionItemId);
        Set_AcquBookAmount(1);
    }

    public override void SetOff_ThisPanel()
    {
        if (convertingNow) return;

        SoundManager.instance.PlayUiSfx("Reject");
        base.SetOff_ThisPanel();
    }

    #endregion

    #region Language

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        // Close
        DevTool.Get_ComponentTType<TMP_Text>(closeBtn.gameObject.transform.GetChild(0).gameObject).text =
            StaticResourceManager.instance.staticWords.GetLanguage(28);

        // EUI
        cvtAcquisitionEui.Set_Language(canConvert);
    }

    #endregion
}
