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
    [SerializeField] private CvtMaterialEUIController cCvtMaterialEui;
    [SerializeField] private CvtMaterialEUIController epCvtMaterialEui;

    #endregion

    #region - Hide

    [HideInInspector] private static int need_Credit = 10;
    [HideInInspector] private static float need_EP = 1;

    #endregion

    #endregion

    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

        Offset_EUI();
        Offset_Subscribe();
    }

    public void Offset_EUI()
    {
        cCvtMaterialEui.Offset();
        epCvtMaterialEui.Offset();
    }

    public void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.currentCredit
            .Subscribe(_Value =>
            {
                cCvtMaterialEui.Set_PossessionAmountTxt(_Value.ToString());
            }); 
        
        PlayerManager.instance.playerController.Get_CurrentEP()
            .Subscribe(_Value =>
            {
                epCvtMaterialEui.Set_PossessionAmountTxt(_Value.ToString());
            });
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Label
        labelName = ResourceManager.instance.Get_StaticWord(124) + " " +
            ResourceManager.instance.Get_StaticWord(125);
        labelTxt.text = labelName;

        cCvtMaterialEui.Set_Language();
        epCvtMaterialEui.Set_Language();

    }

    #endregion

    #region Acquisition

    // 크레딧으로 생성가능한 최대 수
    private int Get_Acquisitable_Credit(int currentCredit)
    {
        int result = 0;
        if (currentCredit > 0)
        {
            result = currentCredit / need_Credit;
        }
        return result;
    }

    // EP으로 생성가능한 최대 수
    private int Get_Acquisitable_EP(float currentEP)
    {
        int result = 0;
        if (currentEP > 0)
        {
            result = (int)currentEP;
        }
        return result;
    }

    // 최대
    protected override void Set_MaxAcquBookAmount()
    {
        // Data
        PlayerController pc = PlayerManager.instance.playerController;

        int currentPossibilityCredit = 
            Get_Acquisitable_Credit(pc.currentCredit.Value);

        int currentPossibilityEP =
            Get_Acquisitable_EP(pc.Get_CurrentEP().Value - need_EP);

        int result = currentPossibilityCredit < currentPossibilityEP ? currentPossibilityCredit : currentPossibilityEP;
        Set_AcquBookAmount(result);
    }

    // 세팅
    protected override void Set_AcquBookAmount(int amount)
    {
        base.Set_AcquBookAmount(amount);

        // Data
        PlayerController pc = PlayerManager.instance.playerController;
        Debug.Assert(pc, "Player is Null");

        int needCredit = acquisitionBookAmount * need_Credit;
        cCvtMaterialEui.Set_NecessaryAmountTxt(needCredit.ToString());
        bool canCvtByCredit = needCredit <= pc.currentCredit.Value;
        cCvtMaterialEui.Set_Condition(canCvtByCredit);

        float needEP = acquisitionBookAmount * need_EP;
        epCvtMaterialEui.Set_NecessaryAmountTxt(needEP.ToString());
        bool canCvtByEP = needEP <= (pc.Get_CurrentEP().Value - need_EP);
        epCvtMaterialEui.Set_Condition(canCvtByEP);

        canConvert = canCvtByCredit && canCvtByEP;
        cvtAcquisitionEui.Set_AbleConvertVisual(canConvert);
    }

    #endregion

    #region Convert

    protected override void Convert()
    {
        base.Convert(); // Gain

        // Lost
        PlayerController pc = PlayerManager.instance.playerController;
        pc.Add_CurrentCredit(-(acquisitionBookAmount * need_Credit));
        pc.Add_CurrentEP(-(acquisitionBookAmount * need_EP));

        Set_AcquAmount(acquisitionItemId);
        Set_AcquBookAmount(acquisitionBookAmount);
    }

    #endregion

    #region Play

    protected override void Play_Failure()
    {
        base.Play_Failure();

        cCvtMaterialEui.Play_Failure();
        epCvtMaterialEui.Play_Failure();
    }

    protected override void Play_Convert()
    {
        base.Play_Convert();

        StartCoroutine(Play_Convert_Cor());
    }

    private IEnumerator Play_Convert_Cor()
    {
        yield return new WaitForSeconds(2.1f);

        cCvtMaterialEui.Play_Convert();
        epCvtMaterialEui.Play_Convert();
    }

    #endregion
}
