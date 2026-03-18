using System.Collections;
using UniRx;
using UnityEngine;

public class OriginCoreCvtUIController : ConverterUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private CvtMaterialEUIController CB_CvtMaterialEUI;
    [SerializeField] private CvtMaterialEUIController C_CvtMaterialEUI;
    [SerializeField] private CvtMaterialEUIController EtherC_CvtMaterialEUI;

    #endregion

    #region - Hide

    [HideInInspector] private static int Need_ChargedBettery = 20;
    [HideInInspector] private static int Need_Credit = 120;
    [HideInInspector] private static int Need_EtherC = 5;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_EUI();
        Offset_Subscribe();
    }

    public void Offset_EUI()
    {
        CB_CvtMaterialEUI.Offset();
        C_CvtMaterialEUI.Offset();
        EtherC_CvtMaterialEUI.Offset();
    }

    public void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.CurrentChargedBettery
            .Subscribe(_Value =>
            {
                CB_CvtMaterialEUI.Set_PossessionAmountTxt(_Value.ToString());
            });

        PlayerManager.instance.playerController.CurrentCredit
            .Subscribe(_Value =>
            {
                C_CvtMaterialEUI.Set_PossessionAmountTxt(_Value.ToString());
            });
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Label
        LabelName = ResourceManager.instance.Get_StaticWord(120) + " " +
            ResourceManager.instance.Get_StaticWord(125);
        LabelTxt.text = LabelName;

        CB_CvtMaterialEUI.Set_Language();
        C_CvtMaterialEUI.Set_Language();
        EtherC_CvtMaterialEUI.Set_Language();
    }

    #endregion

    #region Acquisition

    // 크레딧으로 생성가능한 최대 수
    private int Get_Acquisitable_Credit(int _CurrentCredit)
    {
        int result = 0;
        if (_CurrentCredit > 0)
        {
            result = _CurrentCredit / Need_Credit;
        }
        return result;
    }

    // CB으로 생성가능한 최대 수
    private int Get_Acquisitable_ChargedBettery(int _CurrentCB)
    {
        int result = 0;
        if (_CurrentCB > 0)
        {
            result = _CurrentCB / Need_ChargedBettery;
        }
        return result;
    }

    // EtherC으로 생성가능한 최대 수
    private int Get_Acquisitable_EtherC(int _CurrentEtherC)
    {
        int result = 0;
        if (_CurrentEtherC > 0)
        {
            result = _CurrentEtherC / Need_EtherC;
        }
        return result;
    }

    // 최대
    protected override void Set_MaxAcquBookAmount()
    {
        // Data
        PlayerController pc = PlayerManager.instance.playerController;

        int currentPossibilityCredit =
            Get_Acquisitable_Credit(pc.CurrentCredit.Value);

        int currentPossibilityCB =
            Get_Acquisitable_ChargedBettery(pc.CurrentChargedBettery.Value);

        int currentPossibilityEtherC =
            Get_Acquisitable_EtherC(SaveDataManager.instance.jsonData.Get_ItemAmount(2));

        int result = currentPossibilityCredit < currentPossibilityCB ? currentPossibilityCredit : currentPossibilityCB;
        result = result < currentPossibilityEtherC ? result : currentPossibilityEtherC;

        Set_AcquBookAmount(result);
    }

    // 세팅
    protected override void Set_AcquBookAmount(int _Amount)
    {
        base.Set_AcquBookAmount(_Amount);

        // Data
        PlayerController pc = PlayerManager.instance.playerController;
        Debug.Assert(pc, "Player is Null");

        int needCredit = AcquisitionBookAmount * Need_Credit;
        C_CvtMaterialEUI.Set_NecessaryAmountTxt(needCredit.ToString());
        bool canCvtByCredit = needCredit <= pc.CurrentCredit.Value;
        C_CvtMaterialEUI.Set_Condition(canCvtByCredit);

        float needCB = AcquisitionBookAmount * Need_ChargedBettery;
        CB_CvtMaterialEUI.Set_NecessaryAmountTxt(needCB.ToString());
        bool canCvtByCB = needCB <= (pc.CurrentChargedBettery.Value);
        CB_CvtMaterialEUI.Set_Condition(canCvtByCB);

        float needEtherC = AcquisitionBookAmount * Need_EtherC;
        EtherC_CvtMaterialEUI.Set_NecessaryAmountTxt(needEtherC.ToString());
        bool canCvtByEtherC = needEtherC <= (SaveDataManager.instance.jsonData.Get_ItemAmount(2));
        EtherC_CvtMaterialEUI.Set_Condition(canCvtByEtherC);

        CanConvert = canCvtByCredit && canCvtByCB && canCvtByEtherC;
        CvtAcquisitionEUI.Set_AbleConvertVisual(CanConvert);
    }

    // 이미 가진 아이템
    private void Set_AcquAmount_Core()
    {
        EtherC_CvtMaterialEUI.Set_PossessionAmountTxt(
            SaveDataManager.instance.jsonData.Get_ItemAmount(2).ToString());
    }

    #endregion

    #region Convert

    protected override void Convert()
    {
        base.Convert(); // Gain

        // Lost
        PlayerController pc = PlayerManager.instance.playerController;
        pc.Add_CurrentCredit(-(AcquisitionBookAmount * Need_Credit));
        pc.Use_ChargedBettery(AcquisitionBookAmount * Need_ChargedBettery);
        SaveDataManager.instance.jsonData.Use_Item(2, AcquisitionBookAmount * Need_EtherC);

        Set_AcquAmount_Core();
        Set_AcquAmount(AcquisitionItemID);
        Set_AcquBookAmount(AcquisitionBookAmount);
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Set_AcquAmount_Core();
    }

    #endregion

    #region Play

    protected override void Play_Failure()
    {
        base.Play_Failure();

        CB_CvtMaterialEUI.Play_Failure();
        C_CvtMaterialEUI.Play_Failure();
        EtherC_CvtMaterialEUI.Play_Failure();
    }

    protected override void Play_Convert()
    {
        base.Play_Convert();

        StartCoroutine(Play_Convert_Cor());
    }

    private IEnumerator Play_Convert_Cor()
    {
        yield return new WaitForSeconds(2.1f);

        CB_CvtMaterialEUI.Play_Convert();
        C_CvtMaterialEUI.Play_Convert();
        EtherC_CvtMaterialEUI.Play_Convert();
    }

    #endregion
}
