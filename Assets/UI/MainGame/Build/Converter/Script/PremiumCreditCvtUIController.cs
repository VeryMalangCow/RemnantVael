using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PremiumCreditCvtUIController : ConverterUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    private CvtMaterialEUIController cCvtMaterialEui;
    private CvtMaterialEUIController epCvtMaterialEui;
    private static int need_Credit = 10;
    private static float need_EP = 1;

    #endregion

    #region Init

    public override IEnumerator InitAsync(Color mainClr, Color subClr)
    {
        yield return base.InitAsync(mainClr, subClr);

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        cCvtMaterialEui = cvtMaterialEuis[0];
        epCvtMaterialEui = cvtMaterialEuis[1];
        cvtMaterialEuis = null;

        cCvtMaterialEui.Offset();
        epCvtMaterialEui.Offset();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Premium Credit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif

        SetLanguageTxt();
        yield return null;
    }

    #endregion

    #region UI

    public void SetEpUI(float current, float max)
    {
        epCvtMaterialEui.Set_PossessionAmountTxt(max.ToString());
    }

    public void SetCreditUI(int value)
    {
        cCvtMaterialEui.Set_PossessionAmountTxt(value.ToString());
    }

    #endregion

    #region Language

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        var words = StaticResourceManager.instance.staticWords;

        // Label
        labelName = $"{words.GetLanguage(124)} {words.GetLanguage(125)}";
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
            Get_Acquisitable_Credit(pc.credit);

        int currentPossibilityEP =
            Get_Acquisitable_EP(pc.currentEp - need_EP);

        int result = currentPossibilityCredit < currentPossibilityEP ? currentPossibilityCredit : currentPossibilityEP;
        Set_AcquBookAmount(result);
    }

    // 세팅
    protected override void Set_AcquBookAmount(int amount)
    {
        base.Set_AcquBookAmount(amount);

        // Data
        PlayerController pc = PlayerManager.instance.playerController;
        UnityEngine.Debug.Assert(pc, "Player is Null");

        int needCredit = acquisitionBookAmount * need_Credit;
        cCvtMaterialEui.Set_NecessaryAmountTxt(needCredit.ToString());
        bool canCvtByCredit = needCredit <= pc.credit;
        cCvtMaterialEui.Set_Condition(canCvtByCredit);

        float needEP = acquisitionBookAmount * need_EP;
        epCvtMaterialEui.Set_NecessaryAmountTxt(needEP.ToString());
        bool canCvtByEP = needEP <= (pc.currentEp - need_EP);
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
        pc.GainCredit(-(acquisitionBookAmount * need_Credit));
        pc.AddCurrentEp(-(acquisitionBookAmount * need_EP));

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
