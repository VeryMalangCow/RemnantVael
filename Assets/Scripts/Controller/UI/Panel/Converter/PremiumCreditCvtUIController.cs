using System.Collections;
using UniRx;
using UnityEngine;

public class PremiumCreditCvtUIController : ConverterUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private CvtMaterialEUIController C_CvtMaterialEUI;
    [SerializeField] private CvtMaterialEUIController EP_CvtMaterialEUI;

    #endregion

    #region - Hide

    [HideInInspector] private static int Need_Credit = 10;
    [HideInInspector] private static float Need_EP = 1;

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
        C_CvtMaterialEUI.Offset();
        EP_CvtMaterialEUI.Offset();
    }

    public void Offset_Subscribe()
    {
        PlayerManager.Instance.PlayerController.CurrentCredit
            .Subscribe(_Value =>
            {
                C_CvtMaterialEUI.Set_PossessionAmountTxt(_Value.ToString());
            }); 
        
        PlayerManager.Instance.PlayerController.Get_CurrentEP()
            .Subscribe(_Value =>
            {
                EP_CvtMaterialEUI.Set_PossessionAmountTxt(_Value.ToString());
            });
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Label
        LabelName = ResourceManager.Instance.Get_StaticWord(124) + " " +
            ResourceManager.Instance.Get_StaticWord(125);
        LabelTxt.text = LabelName;

        C_CvtMaterialEUI.Set_Language();
        EP_CvtMaterialEUI.Set_Language();

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

    // EP으로 생성가능한 최대 수
    private int Get_Acquisitable_EP(float _CurrentEP)
    {
        int result = 0;
        if (_CurrentEP > 0)
        {
            result = (int)_CurrentEP;
        }
        return result;
    }

    // 최대
    protected override void Set_MaxAcquBookAmount()
    {
        // Data
        PlayerController pc = PlayerManager.Instance.PlayerController;

        int currentPossibilityCredit = 
            Get_Acquisitable_Credit(pc.CurrentCredit.Value);

        int currentPossibilityEP =
            Get_Acquisitable_EP(pc.Get_CurrentEP().Value - Need_EP);

        int result = currentPossibilityCredit < currentPossibilityEP ? currentPossibilityCredit : currentPossibilityEP;
        Set_AcquBookAmount(result);
    }

    // 세팅
    protected override void Set_AcquBookAmount(int _Amount)
    {
        base.Set_AcquBookAmount(_Amount);

        // Data
        PlayerController pc = PlayerManager.Instance.PlayerController;
        Debug.Assert(pc, "Player is Null");

        int needCredit = AcquisitionBookAmount * Need_Credit;
        C_CvtMaterialEUI.Set_NecessaryAmountTxt(needCredit.ToString());
        bool canCvtByCredit = needCredit <= pc.CurrentCredit.Value;
        C_CvtMaterialEUI.Set_Condition(canCvtByCredit);

        float needEP = AcquisitionBookAmount * Need_EP;
        EP_CvtMaterialEUI.Set_NecessaryAmountTxt(needEP.ToString());
        bool canCvtByEP = needEP <= (pc.Get_CurrentEP().Value - Need_EP);
        EP_CvtMaterialEUI.Set_Condition(canCvtByEP);

        CanConvert = canCvtByCredit && canCvtByEP;
        CvtAcquisitionEUI.Set_AbleConvertVisual(CanConvert);
    }

    #endregion

    #region Convert

    protected override void Convert()
    {
        base.Convert(); // Gain

        // Lost
        PlayerController pc = PlayerManager.Instance.PlayerController;
        pc.Add_CurrentCredit(-(AcquisitionBookAmount * Need_Credit));
        pc.Add_CurrentEP(-(AcquisitionBookAmount * Need_EP));

        Set_AcquAmount(AcquisitionItemID);
        Set_AcquBookAmount(AcquisitionBookAmount);
    }

    #endregion

    #region Play

    protected override void Play_Failure()
    {
        base.Play_Failure();

        C_CvtMaterialEUI.Play_Failure();
        EP_CvtMaterialEUI.Play_Failure();
    }

    protected override void Play_Convert()
    {
        base.Play_Convert();

        StartCoroutine(Play_Convert_Cor());
    }

    private IEnumerator Play_Convert_Cor()
    {
        yield return new WaitForSeconds(2.1f);

        C_CvtMaterialEUI.Play_Convert();
        EP_CvtMaterialEUI.Play_Convert();
    }

    #endregion
}
