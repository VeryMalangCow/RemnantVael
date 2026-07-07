using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ProtoCoreCvtUIController : ConverterUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    private CvtMaterialEUIController cbCvtMaterialEui;
    private CvtMaterialEUIController cCvtMaterialEui;
    private static int need_ChargedBettery = 5;
    private static int need_Credit = 25;

    #endregion

    #region Init

    public override IEnumerator InitAsync(Color mainClr, Color subClr)
    {
        yield return base.InitAsync(mainClr, subClr);

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        cbCvtMaterialEui = cvtMaterialEuis[0];
        cCvtMaterialEui = cvtMaterialEuis[1];
        cvtMaterialEuis = null;

        cbCvtMaterialEui.Offset();
        cCvtMaterialEui.Offset();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Proto Core</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    #endregion

    public void SetChargedBetteryUI(int value)
    {
        cbCvtMaterialEui.Set_PossessionAmountTxt(value.ToString());
    }

    #region Language

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        var words = StaticResourceManager.instance.staticWords;

        // Label
        labelName = $"{words.GetLanguage(118)} {words.GetLanguage(125)}";
        labelTxt.text = labelName;

        cbCvtMaterialEui.Set_Language();
        cCvtMaterialEui.Set_Language();
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

    // CB으로 생성가능한 최대 수
    private int Get_Acquisitable_ChargedBettery(int currentCb)
    {
        int result = 0;
        if (currentCb > 0)
        {
            result = currentCb / need_ChargedBettery;
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

        int currentPossibilityCB =
            Get_Acquisitable_ChargedBettery(pc.chargedBettery);

        int result = currentPossibilityCredit < currentPossibilityCB ? currentPossibilityCredit : currentPossibilityCB;
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

        float needCB = acquisitionBookAmount * need_ChargedBettery;
        cbCvtMaterialEui.Set_NecessaryAmountTxt(needCB.ToString());
        bool canCvtByCB = needCB <= pc.chargedBettery;
        cbCvtMaterialEui.Set_Condition(canCvtByCB);

        canConvert = canCvtByCredit && canCvtByCB;
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
        pc.UseChargedBettery(acquisitionBookAmount * need_ChargedBettery);

        Set_AcquAmount(acquisitionItemId);
        Set_AcquBookAmount(acquisitionBookAmount);
    }

    #endregion

    #region Play

    protected override void Play_Failure()
    {
        base.Play_Failure();

        cbCvtMaterialEui.Play_Failure();
        cCvtMaterialEui.Play_Failure();
    }

    protected override void Play_Convert()
    {
        base.Play_Convert();

        StartCoroutine(Play_Convert_Cor());
    }

    private IEnumerator Play_Convert_Cor()
    {
        yield return new WaitForSeconds(2.1f);

        cbCvtMaterialEui.Play_Convert();
        cCvtMaterialEui.Play_Convert();
    }

    #endregion
}
