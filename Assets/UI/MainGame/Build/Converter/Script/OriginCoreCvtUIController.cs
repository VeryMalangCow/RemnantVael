using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class OriginCoreCvtUIController : ConverterUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    private CvtMaterialEUIController cbCvtMaterialEui;
    private CvtMaterialEUIController cCvtMaterialEui;
    private CvtMaterialEUIController etherCvtMaterialEui;
    private static int need_ChargedBettery = 20;
    private static int need_Credit = 120;
    private static int need_EtherC = 5;

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
        etherCvtMaterialEui = cvtMaterialEuis[2];
        cvtMaterialEuis = null;

        cbCvtMaterialEui.Offset();
        cCvtMaterialEui.Offset();
        etherCvtMaterialEui.Offset();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Origin Core</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        SetLanguageTxt();
        yield return null;
    }

    #endregion

    public void SetChargedBetteryUI(int value)
    {
        cbCvtMaterialEui.Set_PossessionAmountTxt(value.ToString());
    }

    public void SetCreditUI(int value)
    {
        cCvtMaterialEui.Set_PossessionAmountTxt(value.ToString());
    }

    #region Language

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        var words = StaticResourceManager.instance.staticWords;

        // Label
        labelName = $"{words.GetLanguage(120)} {words.GetLanguage(125)}";
        labelTxt.text = labelName;

        cbCvtMaterialEui.Set_Language();
        cCvtMaterialEui.Set_Language();
        etherCvtMaterialEui.Set_Language();
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
    private int Get_Acquisitable_ChargedBettery(int currentCB)
    {
        int result = 0;
        if (currentCB > 0)
        {
            result = currentCB / need_ChargedBettery;
        }
        return result;
    }

    // EtherC으로 생성가능한 최대 수
    private int Get_Acquisitable_EtherC(int currentEtherC)
    {
        int result = 0;
        if (currentEtherC > 0)
        {
            result = currentEtherC / need_EtherC;
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

        int currentPossibilityEtherC =
            Get_Acquisitable_EtherC(SaveDataManager.instance.GetItemAmount(2));

        int result = currentPossibilityCredit < currentPossibilityCB ? currentPossibilityCredit : currentPossibilityCB;
        result = result < currentPossibilityEtherC ? result : currentPossibilityEtherC;

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
        bool canCvtByCB = needCB <= (pc.chargedBettery);
        cbCvtMaterialEui.Set_Condition(canCvtByCB);

        float needEtherC = acquisitionBookAmount * need_EtherC;
        etherCvtMaterialEui.Set_NecessaryAmountTxt(needEtherC.ToString());
        bool canCvtByEtherC = needEtherC <= (SaveDataManager.instance.GetItemAmount(2));
        etherCvtMaterialEui.Set_Condition(canCvtByEtherC);

        canConvert = canCvtByCredit && canCvtByCB && canCvtByEtherC;
        cvtAcquisitionEui.Set_AbleConvertVisual(canConvert);
    }

    // 이미 가진 아이템
    private void Set_AcquAmount_Core()
    {
        etherCvtMaterialEui.Set_PossessionAmountTxt(
            SaveDataManager.instance.GetItemAmount(2).ToString());
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
        SaveDataManager.instance.UseHighLvItem(2, acquisitionBookAmount * need_EtherC);

        Set_AcquAmount_Core();
        Set_AcquAmount(acquisitionItemId);
        Set_AcquBookAmount(acquisitionBookAmount);
    }

    #endregion

    #region Panel

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        Set_AcquAmount_Core();
    }

    #endregion

    #region Play

    protected override void Play_Failure()
    {
        base.Play_Failure();

        cbCvtMaterialEui.Play_Failure();
        cCvtMaterialEui.Play_Failure();
        etherCvtMaterialEui.Play_Failure();
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
        etherCvtMaterialEui.Play_Convert();
    }

    #endregion
}
