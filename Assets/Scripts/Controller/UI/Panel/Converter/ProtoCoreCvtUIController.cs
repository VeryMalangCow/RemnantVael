using System.Collections;
using UniRx;
using UnityEngine;

public class ProtoCoreCvtUIController : ConverterUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private CvtMaterialEUIController CB_CvtMaterialEUI;
    [SerializeField] private CvtMaterialEUIController C_CvtMaterialEUI;

    #endregion

    #region - Hide

    [HideInInspector] private static int Need_ChargedBettery = 5;
    [HideInInspector] private static int Need_Credit = 25;

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
    }

    public void Offset_Subscribe()
    {
        PlayerManager.Instance.playerController.CurrentChargedBettery
            .Subscribe(_Value =>
            {
                CB_CvtMaterialEUI.Set_PossessionAmountTxt(_Value.ToString());
            });

        PlayerManager.Instance.playerController.CurrentCredit
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
        LabelName = ResourceManager.Instance.Get_StaticWord(118) + " " +
            ResourceManager.Instance.Get_StaticWord(125);
        LabelTxt.text = LabelName;

        CB_CvtMaterialEUI.Set_Language();
        C_CvtMaterialEUI.Set_Language();
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

    // 최대
    protected override void Set_MaxAcquBookAmount()
    {
        // Data
        PlayerController pc = PlayerManager.Instance.playerController;

        int currentPossibilityCredit =
            Get_Acquisitable_Credit(pc.CurrentCredit.Value);

        int currentPossibilityCB =
            Get_Acquisitable_ChargedBettery(pc.CurrentChargedBettery.Value);

        int result = currentPossibilityCredit < currentPossibilityCB ? currentPossibilityCredit : currentPossibilityCB;
        Set_AcquBookAmount(result);
    }

    // 세팅
    protected override void Set_AcquBookAmount(int _Amount)
    {
        base.Set_AcquBookAmount(_Amount);

        // Data
        PlayerController pc = PlayerManager.Instance.playerController;
        Debug.Assert(pc, "Player is Null");

        int needCredit = AcquisitionBookAmount * Need_Credit;
        C_CvtMaterialEUI.Set_NecessaryAmountTxt(needCredit.ToString());
        bool canCvtByCredit = needCredit <= pc.CurrentCredit.Value;
        C_CvtMaterialEUI.Set_Condition(canCvtByCredit);

        float needCB = AcquisitionBookAmount * Need_ChargedBettery;
        CB_CvtMaterialEUI.Set_NecessaryAmountTxt(needCB.ToString());
        bool canCvtByCB = needCB <= (pc.CurrentChargedBettery.Value);
        CB_CvtMaterialEUI.Set_Condition(canCvtByCB);

        CanConvert = canCvtByCredit && canCvtByCB;
        CvtAcquisitionEUI.Set_AbleConvertVisual(CanConvert);
    }

    #endregion

    #region Convert

    protected override void Convert()
    {
        base.Convert(); // Gain

        // Lost
        PlayerController pc = PlayerManager.Instance.playerController;
        pc.Add_CurrentCredit(-(AcquisitionBookAmount * Need_Credit));
        pc.Use_ChargedBettery(AcquisitionBookAmount * Need_ChargedBettery);

        Set_AcquAmount(AcquisitionItemID);
        Set_AcquBookAmount(AcquisitionBookAmount);
    }

    #endregion

    #region Play

    protected override void Play_Failure()
    {
        base.Play_Failure();

        CB_CvtMaterialEUI.Play_Failure();
        C_CvtMaterialEUI.Play_Failure();
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
    }

    #endregion
}
