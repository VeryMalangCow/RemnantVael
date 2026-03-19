using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class MainGameUIManager : Singleton<MainGameUIManager>
{
    #region Value

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
    [HideInInspector] public PlayerHUDController playerHUD_UIController;

    [HideInInspector] public BaseUpgradeUIController baseUpgrade_UIController;
    [HideInInspector] public ModuleUpgradeUIController moduleUpgrade_UIController;

    [HideInInspector] public AllyBaseUpgradeUIController allyBaseUpgrade_UIController;
    [HideInInspector] public AllyModuleUpgradeUIController allyModuleUpgrade_UIController;

    [HideInInspector] public OutMainGameUIController outMainGame_UIController;
    [HideInInspector] public InteractAnnoUIController interactAnno_UIController;
    [HideInInspector] public MapIntroUIController mapIntro_UIController;

    [HideInInspector] public AllyCardUIController allyCard_UIController;

    [HideInInspector] public BoxLineConnectorUIController boxLineConnector_UIController;
    [HideInInspector] public NumShapeColorPasswordUIController numShapeColorPassword_UIController;
    [HideInInspector] public InOrderLockerUIController inOrderLocker_UIController;

    [HideInInspector] public PremiumCreditCvtUIController premiumCreditCvt_UIController;
    [HideInInspector] public ProtoCoreCvtUIController protoCoreCvt_UIController;
    [HideInInspector] public EtherCoreCvtUIController etherCoreCvt_UIController;
    [HideInInspector] public OriginCoreCvtUIController originCoreCvt_UIController;

    [HideInInspector] public BattleProdUIController battleProd_UIController;

    // Current
    [HideInInspector] public static UIController currentOpening_UIController;

    // Production
    [HideInInspector] private CanvasGroup screenCG;
    [HideInInspector] private CanvasGroup loadingIconCG;
    [HideInInspector] private RectTransform loadingIconRT;
    [HideInInspector] private Tween cogwheelTween = null;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        screenCG = DevTool.Get_ComponentTType(screenCanvas.gameObject, out CanvasGroup cg) ? cg : null;
        loadingIconCG = DevTool.Get_ComponentTType(loadingIconCanvas.gameObject, out CanvasGroup iconCg) ? iconCg : null;
        loadingIconRT = DevTool.Get_ComponentTType(loadingIconCanvas.gameObject.transform.GetChild(0).gameObject, out RectTransform iconRt) ? iconRt : null;

        saveDataCG.gameObject.SetActive(false);

        cogwheelTween = loadingIconRT
            .DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        playerHUD_UIController
            = Gen_UI<PlayerHUDController>(ResourceManager.instance.playerHUD_CanvasPrefab, true);

        outMainGame_UIController
            = Gen_UI<OutMainGameUIController>(ResourceManager.instance.outMainGame_CanvasPrefab, false);

        baseUpgrade_UIController
            = Gen_UI<BaseUpgradeUIController>(ResourceManager.instance.baseUpgrade_CanvasPrefab, false);
        moduleUpgrade_UIController
            = Gen_UI<ModuleUpgradeUIController>(ResourceManager.instance.moduleUpgrade_CanvasPrefab, false);

        allyBaseUpgrade_UIController
            = Gen_UI<AllyBaseUpgradeUIController>(ResourceManager.instance.allyBaseUpgrade_CanvasPrefab, false);
        allyModuleUpgrade_UIController
            = Gen_UI<AllyModuleUpgradeUIController>(ResourceManager.instance.allyModuleUpgrade_CanvasPrefab, false);

        interactAnno_UIController
            = Gen_UI<InteractAnnoUIController>(ResourceManager.instance.interactAnno_CanvasPrefab, false);

        mapIntro_UIController
            = Gen_UI<MapIntroUIController>(ResourceManager.instance.mapIntro_CanvasPrefab, false);

        allyCard_UIController
            = Gen_UI<AllyCardUIController>(ResourceManager.instance.allyCard_CanvasPrefab, false);

        boxLineConnector_UIController
            = Gen_UI<BoxLineConnectorUIController>(ResourceManager.instance.puzzle_BoxLineConnector_CanvasPrefab, false);
        numShapeColorPassword_UIController
            = Gen_UI<NumShapeColorPasswordUIController>(ResourceManager.instance.puzzle_NumShapeColorPassword_CanvasPrefab, false);
        inOrderLocker_UIController
            = Gen_UI<InOrderLockerUIController>(ResourceManager.instance.puzzle_InOrderLocker_CanvasPrefab, false);

        premiumCreditCvt_UIController
            = Gen_UI<PremiumCreditCvtUIController>(ResourceManager.instance.cvt_PremiumCredit_CanvasPrefab, false);
        protoCoreCvt_UIController
            = Gen_UI<ProtoCoreCvtUIController>(ResourceManager.instance.cvt_ProtoCore_CanvasPrefab, false);
        etherCoreCvt_UIController
            = Gen_UI<EtherCoreCvtUIController>(ResourceManager.instance.cvt_EtherCore_CanvasPrefab, false);
        originCoreCvt_UIController
            = Gen_UI<OriginCoreCvtUIController>(ResourceManager.instance.cvt_OriginCore_CanvasPrefab, false);

        battleProd_UIController
            = Gen_UI<BattleProdUIController>(ResourceManager.instance.battleProd_CanvasPrefab, false);

        Sequence startSeq = DOTween.Sequence();

        Play_FadeOut(fadeOutTime);
        Play_OffLoadingIcon(fadeOutTime);
        screenCG.alpha = 1f;
    }


    #endregion

    #region Framework

    private void Start()
    {
        Offset();
        GameManager.instance.Set_BaseOption();
    }

    #endregion

    #region Gen

    private T Gen_UI<T>(GameObject uiGo, bool onOff)
    {
        GameObject uigo = Instantiate(uiGo, uiParent);

        uigo.gameObject.SetActive(onOff);
        if (DevTool.Get_ComponentTType(uigo, out UIController uiController) &&
            DevTool.Get_ComponentTType(uigo, out Canvas uiCanvas))
        {
            uiController.Offset();
            uiCanvas.worldCamera = uiCamera;
        }

        return DevTool.Get_ComponentTType(uigo, out T tType) ? tType : default;
    }

    #endregion

    #region Set

    public void Set_LanguageTxt()
    {
        outMainGame_UIController.Set_LanguageTxt();

        mapIntro_UIController.Set_LanguageTxt();
        playerHUD_UIController.Set_LanguageTxt();
        interactAnno_UIController.Set_LanguageTxt();

        baseUpgrade_UIController.Set_LanguageTxt();
        moduleUpgrade_UIController.Set_LanguageTxt();
        ModuleItemManager.instance.Set_DataLanguage();

        allyBaseUpgrade_UIController.Set_LanguageTxt();
        allyModuleUpgrade_UIController.Set_LanguageTxt();

        allyCard_UIController.Set_LanguageTxt();
    }

    public void Set_Color()
    {
        outMainGame_UIController.Offset_ColorComp();

        playerHUD_UIController.Offset_ColorComp();
        interactAnno_UIController.Offset_ColorComp();

        baseUpgrade_UIController.Offset_ColorComp();
        moduleUpgrade_UIController.Offset_ColorComp();

        allyBaseUpgrade_UIController.Offset_ColorComp();
        allyModuleUpgrade_UIController.Offset_ColorComp();
    }

    #endregion

    #region Fade

    public Sequence Play_FadeOut(float durTime) // ¹à¾ÆÁü
    {
        Sequence seq = DOTween.Sequence();

        screenCanvas.gameObject.SetActive(true);

        seq.Append(screenCG.DOFade(0f, durTime));
        seq.Join(playerHUD_UIController.ThisCG.DOFade(1f, durTime));

        seq.OnStart(() =>
        {
            screenCG.alpha = 1f;
            playerHUD_UIController.ThisCG.alpha = 0f;

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
        seq.Join(playerHUD_UIController.ThisCG.DOFade(0f, durTime));

        seq.OnStart(() =>
        {
            screenCG.alpha = 0f;
            playerHUD_UIController.ThisCG.alpha = 1f;
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
        Debug.Log("Á×À½");
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
        Debug.Log("Á¾·á");
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
