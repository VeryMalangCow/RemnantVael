using System.Collections;
using UniRx;
using UnityEngine;

public class EtherCoreCvtUIController : ConverterUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Cvt. Premium Credit")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private CvtMaterialEUIController cbCvtMaterialEui;
    [SerializeField] private CvtMaterialEUIController cCvtMaterialEui;
    [SerializeField] private CvtMaterialEUIController protoCvtMaterialEui;

    #endregion

    #region - Hide

    [HideInInspector] private static int need_ChargedBettery = 10;
    [HideInInspector] private static int need_Credit = 50;
    [HideInInspector] private static int need_ProtoC = 3;

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
        cbCvtMaterialEui.Offset();
        cCvtMaterialEui.Offset();
        protoCvtMaterialEui.Offset();
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
        labelName = ResourceManager.instance.Get_StaticWord(119) + " " +
            ResourceManager.instance.Get_StaticWord(125);
        labelTxt.text = labelName;

        cbCvtMaterialEui.Set_Language();
        cCvtMaterialEui.Set_Language();
        protoCvtMaterialEui.Set_Language();
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

    // ProtoC으로 생성가능한 최대 수
    private int Get_Acquisitable_ProtoC(int currentProtoC)
    {
        int result = 0;
        if (currentProtoC > 0)
        {
            result = currentProtoC / need_ProtoC;
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

        int currentPossibilityProtoC =
            Get_Acquisitable_ProtoC(SaveDataManager.instance.jsonData.Get_ItemAmount(1));

        int result = currentPossibilityCredit < currentPossibilityCB ? currentPossibilityCredit : currentPossibilityCB;
        result = result < currentPossibilityProtoC ? result : currentPossibilityProtoC;

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

        float needProtoC = acquisitionBookAmount * need_ProtoC;
        protoCvtMaterialEui.Set_NecessaryAmountTxt(needProtoC.ToString());
        bool canCvtByProtoC = needProtoC <= (SaveDataManager.instance.jsonData.Get_ItemAmount(1));
        protoCvtMaterialEui.Set_Condition(canCvtByProtoC);

        canConvert = canCvtByCredit && canCvtByCB && canCvtByProtoC;
        cvtAcquisitionEui.Set_AbleConvertVisual(canConvert);
    }

    // 이미 가진 아이템
    private void Set_AcquAmount_Core()
    {
        protoCvtMaterialEui.Set_PossessionAmountTxt(
            SaveDataManager.instance.jsonData.Get_ItemAmount(1).ToString());
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
        SaveDataManager.instance.jsonData.Use_Item(1, acquisitionBookAmount * need_ProtoC);

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
        protoCvtMaterialEui.Play_Failure();
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
        protoCvtMaterialEui.Play_Convert();
    }

    #endregion
}
