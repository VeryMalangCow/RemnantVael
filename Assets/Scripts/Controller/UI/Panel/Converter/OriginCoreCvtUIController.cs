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
    [SerializeField] private CvtMaterialEUIController cbCvtMaterialEui;
    [SerializeField] private CvtMaterialEUIController cCvtMaterialEui;
    [SerializeField] private CvtMaterialEUIController etherCvtMaterialEui;

    #endregion

    #region - Hide

    [HideInInspector] private static int need_ChargedBettery = 20;
    [HideInInspector] private static int need_Credit = 120;
    [HideInInspector] private static int need_EtherC = 5;

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
        cbCvtMaterialEui.Offset();
        cCvtMaterialEui.Offset();
        etherCvtMaterialEui.Offset();
    }

    public void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.currentChargedBettery
            .Subscribe(_Value =>
            {
                cbCvtMaterialEui.Set_PossessionAmountTxt(_Value.ToString());
            });

        PlayerManager.instance.playerController.currentCredit
            .Subscribe(_Value =>
            {
                cCvtMaterialEui.Set_PossessionAmountTxt(_Value.ToString());
            });
    }

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        // Label
        labelName = ResourceManager.instance.Get_StaticWord(120) + " " +
            ResourceManager.instance.Get_StaticWord(125);
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
            Get_Acquisitable_Credit(pc.currentCredit.Value);

        int currentPossibilityCB =
            Get_Acquisitable_ChargedBettery(pc.currentChargedBettery.Value);

        int currentPossibilityEtherC =
            Get_Acquisitable_EtherC(SaveDataManager.instance.jsonData.Get_ItemAmount(2));

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
        Debug.Assert(pc, "Player is Null");

        int needCredit = acquisitionBookAmount * need_Credit;
        cCvtMaterialEui.Set_NecessaryAmountTxt(needCredit.ToString());
        bool canCvtByCredit = needCredit <= pc.currentCredit.Value;
        cCvtMaterialEui.Set_Condition(canCvtByCredit);

        float needCB = acquisitionBookAmount * need_ChargedBettery;
        cbCvtMaterialEui.Set_NecessaryAmountTxt(needCB.ToString());
        bool canCvtByCB = needCB <= (pc.currentChargedBettery.Value);
        cbCvtMaterialEui.Set_Condition(canCvtByCB);

        float needEtherC = acquisitionBookAmount * need_EtherC;
        etherCvtMaterialEui.Set_NecessaryAmountTxt(needEtherC.ToString());
        bool canCvtByEtherC = needEtherC <= (SaveDataManager.instance.jsonData.Get_ItemAmount(2));
        etherCvtMaterialEui.Set_Condition(canCvtByEtherC);

        canConvert = canCvtByCredit && canCvtByCB && canCvtByEtherC;
        cvtAcquisitionEui.Set_AbleConvertVisual(canConvert);
    }

    // 이미 가진 아이템
    private void Set_AcquAmount_Core()
    {
        etherCvtMaterialEui.Set_PossessionAmountTxt(
            SaveDataManager.instance.jsonData.Get_ItemAmount(2).ToString());
    }

    #endregion

    #region Convert

    protected override void Convert()
    {
        base.Convert(); // Gain

        // Lost
        PlayerController pc = PlayerManager.instance.playerController;
        pc.Add_CurrentCredit(-(acquisitionBookAmount * need_Credit));
        pc.Use_ChargedBettery(acquisitionBookAmount * need_ChargedBettery);
        SaveDataManager.instance.jsonData.Use_Item(2, acquisitionBookAmount * need_EtherC);

        Set_AcquAmount_Core();
        Set_AcquAmount(acquisitionItemId);
        Set_AcquBookAmount(acquisitionBookAmount);
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
