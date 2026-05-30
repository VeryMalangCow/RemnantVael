using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MainGameUIManager : Singleton<MainGameUIManager>, IMainGameInitializer
{
    #region Value
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    #region - Inspector

    [Header("=== Class")]
    [SerializeField] public Camera uiCamera;
    [SerializeField] public Transform uiParent;
    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private Canvas loadingIconCanvas;

    [Header("=== Screen")]
    [SerializeField] public float fadeOutTime = 2f;

    [Header("=== Save")]
    [SerializeField] private CanvasGroup saveDataCG;

    [Header("=== Dead")]
    [SerializeField] private TMP_Text deadTxt;
    [SerializeField] private TMP_Text endGameTxt;

    #endregion

    #region - Hide

    // Controller

    [HideInInspector] public PlayerHUDController playerHud;

    [HideInInspector] public BaseUpgradeUIController baseUpgradeUi;
    [HideInInspector] public ModuleUpgradeUIController moduleUpgradeUi;

    [HideInInspector] public AllyBaseUpgradeUIController allyBaseUpgradeUi;
    [HideInInspector] public AllyModuleUpgradeUIController allyModuleUpgradeUi;

    [HideInInspector] public OutMainGameUIController outMainGameUi;
    [HideInInspector] public InteractAnnoUIController interactAnnoUi;
    [HideInInspector] public MapIntroUIController mapIntroUi;

    [HideInInspector] public AllyCardUIController allyCardUi;

    [HideInInspector] public BoxLineConnectorUIController boxLineConnectorUi;
    [HideInInspector] public NumShapeColorPasswordUIController numShapeColorPasswordUi;
    [HideInInspector] public InOrderLockerUIController inOrderLockerUi;

    [HideInInspector] public PremiumCreditCvtUIController premiumCreditCvtUi;
    [HideInInspector] public ProtoCoreCvtUIController protoCoreCvtUi;
    [HideInInspector] public EtherCoreCvtUIController etherCoreCvtUi;
    [HideInInspector] public OriginCoreCvtUIController originCoreCvtUi;

    [HideInInspector] public BattleProdUIController battleProdUi;

    // Current
    [HideInInspector] public static UIController currentOpeningUi;

    // Production
    [HideInInspector] private CanvasGroup screenCG;
    [HideInInspector] private CanvasGroup loadingIconCG;
    [HideInInspector] private RectTransform loadingIconRT;
    [HideInInspector] private Tween cogwheelTween = null;

    #endregion

    #endregion

    // Init
    public IEnumerator Initialize()
    {
        screenCG = DevTool.Get_ComponentTType(screenCanvas.gameObject, out CanvasGroup cg) ? cg : null;
        screenCG.alpha = 1;
        loadingIconCG = DevTool.Get_ComponentTType(loadingIconCanvas.gameObject, out CanvasGroup iconCg) ? iconCg : null;
        loadingIconCG.alpha = 1;
        loadingIconRT = DevTool.Get_ComponentTType(loadingIconCanvas.gameObject.transform.GetChild(0).gameObject, out RectTransform iconRt) ? iconRt : null;
        cogwheelTween = loadingIconRT
            .DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        saveDataCG.gameObject.SetActive(false);

        yield return null;

        var reso = ResourceManager.instance;

        yield return InitAsync(reso.playerHUD_CanvasPrefab, true, delegate(PlayerHUDController ui) { playerHud = ui; });
        yield return InitAsync(reso.outMainGame_CanvasPrefab, false, delegate (OutMainGameUIController ui) { outMainGameUi = ui; });

        yield return InitAsync(reso.baseUpgrade_CanvasPrefab, false, delegate (BaseUpgradeUIController ui) { baseUpgradeUi = ui; });
        yield return InitAsync(reso.moduleUpgrade_CanvasPrefab, false, delegate (ModuleUpgradeUIController ui) { moduleUpgradeUi = ui; });

        yield return InitAsync(reso.allyBaseUpgrade_CanvasPrefab, false, delegate (AllyBaseUpgradeUIController ui) { allyBaseUpgradeUi = ui; });
        yield return InitAsync(reso.allyModuleUpgrade_CanvasPrefab, false, delegate (AllyModuleUpgradeUIController ui) { allyModuleUpgradeUi = ui; });

        yield return InitAsync(reso.interactAnno_CanvasPrefab, false, delegate (InteractAnnoUIController ui) { interactAnnoUi = ui; });
        yield return InitAsync(reso.mapIntro_CanvasPrefab, false, delegate (MapIntroUIController ui) { mapIntroUi = ui; });
        yield return InitAsync(reso.allyCard_CanvasPrefab, false, delegate (AllyCardUIController ui) { allyCardUi = ui; });

        yield return InitAsync(reso.puzzle_BoxLineConnector_CanvasPrefab, false, delegate (BoxLineConnectorUIController ui) { boxLineConnectorUi = ui; });
        yield return InitAsync(reso.puzzle_NumShapeColorPassword_CanvasPrefab, false, delegate (NumShapeColorPasswordUIController ui) { numShapeColorPasswordUi = ui; });
        yield return InitAsync(reso.puzzle_InOrderLocker_CanvasPrefab, false, delegate (InOrderLockerUIController ui) { inOrderLockerUi = ui; });

        yield return InitAsync(reso.cvt_PremiumCredit_CanvasPrefab, false, delegate (PremiumCreditCvtUIController ui) { premiumCreditCvtUi = ui; });
        yield return InitAsync(reso.cvt_ProtoCore_CanvasPrefab, false, delegate (ProtoCoreCvtUIController ui) { protoCoreCvtUi = ui; });
        yield return InitAsync(reso.cvt_EtherCore_CanvasPrefab, false, delegate (EtherCoreCvtUIController ui) { etherCoreCvtUi = ui; });
        yield return InitAsync(reso.cvt_OriginCore_CanvasPrefab, false, delegate (OriginCoreCvtUIController ui) { originCoreCvtUi = ui; });

        yield return InitAsync(reso.battleProd_CanvasPrefab, false, delegate (BattleProdUIController ui) { battleProdUi = ui; });

        GameManager.instance.Set_BaseOption();
        yield return null;

        PlayerManager.instance.playerController.TestStart();
        PlayerManager.instance.playerController.skillWeapon.TestStart();

    }

    private IEnumerator InitAsync<T>(GameObject uiGo, bool onOff, Action<T> action) where T : UIController
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        GameObject uigo = Instantiate(uiGo, uiParent);
        sw.Stop();
        UnityEngine.Debug.Log($"MainGameUIManager : Generate : {uigo.name} : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

        uigo.gameObject.SetActive(onOff);
        yield return null;

        if (uiGo != null && DevTool.Get_ComponentTType(uigo, out UIController uiController))
        {
            uiController.Offset(uiCamera);
            T tComp = uiController as T;
            if (action != null && tComp != null) action(tComp);
        }
        yield return null;
    }

    #region Offset

    public void EndProd()
    {
        Play_FadeOut(fadeOutTime);
        Play_OffLoadingIcon(fadeOutTime);
    }


    #endregion

    #region Set

    public void Set_LanguageTxt()
    {
        outMainGameUi.Set_LanguageTxt();

        mapIntroUi.Set_LanguageTxt();
        playerHud.Set_LanguageTxt();
        interactAnnoUi.Set_LanguageTxt();

        baseUpgradeUi.Set_LanguageTxt();
        moduleUpgradeUi.Set_LanguageTxt();
        ModuleItemManager.instance.Set_DataLanguage();

        allyBaseUpgradeUi.Set_LanguageTxt();
        allyModuleUpgradeUi.Set_LanguageTxt();

        allyCardUi.Set_LanguageTxt();
    }

    public void Set_Color()
    {
        outMainGameUi.Offset_ColorComp();

        playerHud.Offset_ColorComp();
        interactAnnoUi.Offset_ColorComp();

        baseUpgradeUi.Offset_ColorComp();
        moduleUpgradeUi.Offset_ColorComp();

        allyBaseUpgradeUi.Offset_ColorComp();
        allyModuleUpgradeUi.Offset_ColorComp();
    }

    #endregion

    #region Fade

    public Sequence Play_FadeOut(float durTime) // ¹à¾ÆÁü
    {
        Sequence seq = DOTween.Sequence();

        screenCanvas.gameObject.SetActive(true);

        seq.Append(screenCG.DOFade(0f, durTime));
        seq.Join(playerHud.cg.DOFade(1f, durTime));

        seq.OnStart(() =>
        {
            screenCG.alpha = 1f;
            playerHud.cg.alpha = 0f;

        })
        .OnComplete(() =>
        {
            screenCanvas.gameObject.SetActive(false);
        });

        return seq;
    }

    public Sequence Play_FadeIn(float durTime) // ¾îµÎ¿öÁü
    {
        Sequence seq = DOTween.Sequence();

        screenCanvas.gameObject.SetActive(true);

        seq.Append(screenCG.DOFade(1f, durTime));
        seq.Join(playerHud.cg.DOFade(0f, durTime));

        seq.OnStart(() =>
        {
            screenCG.alpha = 0f;
            playerHud.cg.alpha = 1f;
        })
        .OnComplete(() =>
        {

        });

        return seq;
    }

    #endregion

    #region Loading

    public Sequence Play_OnLoadingIcon(float durTime) // ³ªÅ¸³ª±â
    {
        Sequence seq = DOTween.Sequence();

        loadingIconCanvas.gameObject.SetActive(true);

        seq.Append(loadingIconCG.DOFade(1f, durTime));

        seq.OnStart(() =>
        {
            cogwheelTween.Play();
            loadingIconCG.alpha = 0f;
        })
        .OnComplete(() =>
        {

        });

        return seq;
    }

    public Sequence Play_OffLoadingIcon(float durTime) // »ç¶óÁö±â
    {
        Sequence seq = DOTween.Sequence();

        loadingIconCanvas.gameObject.SetActive(true);

        seq.Append(loadingIconCG.DOFade(0f, durTime));

        seq.OnStart(() =>
        {
            loadingIconCG.alpha = 1f;
        })
        .OnComplete(() =>
        {
            cogwheelTween.Pause();
            loadingIconCanvas.gameObject.SetActive(false);
        });

        return seq;
    }

    #endregion

    #region SaveData

    public void Play_SaveData()
    {
        Sequence seq = DOTween.Sequence();

        saveDataCG.alpha = 0f;
        saveDataCG.gameObject.SetActive(true);

        seq.Append(saveDataCG.DOFade(1f, 0.3f));
        seq.Append(saveDataCG.DOFade(0f, 0.3f));
        seq.Append(saveDataCG.DOFade(1f, 0.3f));
        seq.Append(saveDataCG.DOFade(0f, 0.3f));
        seq.Append(saveDataCG.DOFade(1f, 0.3f));
        seq.AppendInterval(1.5f);
        seq.Append(saveDataCG.DOFade(0f, 1.5f));
        seq.OnComplete(() =>
        {
            saveDataCG.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Player Dead

    public void Play_DeadProd()
    {
        UnityEngine.Debug.Log("Á×À½");
        StartCoroutine(Play_DeadProd_Cor());
    }

    private IEnumerator Play_DeadProd_Cor(float fadeInTime = 2f, float txtFadeInTime = 1f, float stayTime = 2f)
    {
        EventManager.instance.Set_Input(false);
        Play_FadeIn(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        deadTxt.DOFade(1f, txtFadeInTime);
        DOTween.To(() => deadTxt.characterSpacing, x => deadTxt.characterSpacing = x, 20, txtFadeInTime);

        yield return new WaitForSeconds(txtFadeInTime + stayTime);

        deadTxt.DOFade(0f, txtFadeInTime);

        yield return new WaitForSeconds(txtFadeInTime);

        LoadingSceneManager.instance.Play_LoadScene("MainGame");
    }

    public void Play_EndGameProd()
    {
        UnityEngine.Debug.Log("Á¾·á");
        StartCoroutine(Play_EndGameProd_Cor());
    }

    private IEnumerator Play_EndGameProd_Cor(float fadeInTime = 2f, float txtFadeInTime = 1f, float stayTime = 2f)
    {
        yield return new WaitForSeconds(0.5f);

        EventManager.instance.Set_Input(false);
        Play_FadeIn(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        endGameTxt.DOFade(1f, txtFadeInTime);
        DOTween.To(() => endGameTxt.characterSpacing, x => endGameTxt.characterSpacing = x, 20, txtFadeInTime);

        yield return new WaitForSeconds(txtFadeInTime + stayTime);

        endGameTxt.DOFade(0f, txtFadeInTime);

        yield return new WaitForSeconds(txtFadeInTime);

        LoadingSceneManager.instance.Play_LoadScene("TitleLobby");
    }


    #endregion
}
