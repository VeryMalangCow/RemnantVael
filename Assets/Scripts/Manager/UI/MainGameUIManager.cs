using DG.Tweening;
using System;
using System.Collections;
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

    [Header("=== Prefab")]
    [SerializeField] private HudController hudPrefab;
    [SerializeField] private PauseUIController pauseUiPrefab;
    [SerializeField] private InteractAnnoUIController interactAnnoUiPrefab;
    [SerializeField] private MapIntroUIController mapIntroUiPrefab;
    [Space(5)]
    [SerializeField] private BaseUpgradeUIController buUiPrefab;
    [SerializeField] private ModuleUpgradeUIController muUiPrefab;
    [Space(5)]
    [SerializeField] public AllyBaseUpgradeUIController abuUiPrefab;
    [SerializeField] public AllyModuleUpgradeUIController amuUiPrefab;
    [Space(5)]
    [SerializeField] public BoxLineConnectorUIController puzzleBlcUiPrefab;
    [SerializeField] public NumShapeColorPasswordUIController puzzleNscUiPrefab;
    [SerializeField] public InOrderLockerUIController puzzleIolUiPrefab;
    [Space(5)]
    [SerializeField] public AllyCardUIController allyCardUiPrefab;
    [Space(5)]
    [SerializeField] public PremiumCreditCvtUIController premiumCreditCvtUiPrefab;
    [SerializeField] public ProtoCoreCvtUIController protoCoreCvtUiPrefab;
    [SerializeField] public EtherCoreCvtUIController etherCoreCvtUiPrefab;
    [SerializeField] public OriginCoreCvtUIController originCoreCvtUiPrefab;
    [Space(5)]
    [SerializeField] public BattleProdUIController battleProdUiPrefab;

    public HudController hud { get; private set; }
    public PauseUIController pauseUi { get; private set; }
    public InteractAnnoUIController interactAnnoUi { get; private set; }
    public MapIntroUIController mapIntroUi { get; private set; }


    public BaseUpgradeUIController buUi { get; private set; }
    public ModuleUpgradeUIController muUi { get; private set; }

    public AllyBaseUpgradeUIController abuUi { get; private set; }
    public AllyModuleUpgradeUIController amuUi { get; private set; }


    public BoxLineConnectorUIController puzzleBlcUi { get; private set; }
    public NumShapeColorPasswordUIController puzzleNscUi { get; private set; }
    public InOrderLockerUIController puzzleIolUi { get; private set; }

    public AllyCardUIController allyCardUi { get; private set; }


    public PremiumCreditCvtUIController premiumCreditCvtUi { get; private set; }
    public ProtoCoreCvtUIController protoCoreCvtUi { get; private set; }
    public EtherCoreCvtUIController etherCoreCvtUi { get; private set; }
    public OriginCoreCvtUIController originCoreCvtUi { get; private set; }

    public BattleProdUIController battleProdUi { get; private set; }


    [Header("=== Module")]
    [SerializeField] private InventoryItemEUIController inventoryItemPrefab;
    [SerializeField] private InventorySlotEUIController inventorySlotPrefab;


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
        yield return null;

        PlayerController player = PlayerManager.instance.playerController;
        Color mainClr = player.Get_CorrectColor(eDamageType.Energy, false);
        Color subClr = player.Get_CorrectColor(eDamageType.Energy, true);

        yield return InitAsync(hudPrefab, delegate (HudController ui) { hud = ui; });
        yield return hud.InitAsync(mainClr, subClr);
        yield return InitAsync(pauseUiPrefab, delegate (PauseUIController ui) { pauseUi = ui; });
        yield return pauseUi.InitAsync(mainClr, subClr);

        yield return InitAsync(buUiPrefab, false, delegate (BaseUpgradeUIController ui) { buUi = ui; });
        yield return InitAsync(muUiPrefab, false, delegate (ModuleUpgradeUIController ui) { muUi = ui; });

        yield return InitAsync(abuUiPrefab, false, delegate (AllyBaseUpgradeUIController ui) { abuUi = ui; });
        yield return InitAsync(amuUiPrefab, false, delegate (AllyModuleUpgradeUIController ui) { amuUi = ui; });

        yield return InitAsync(interactAnnoUiPrefab, false, delegate (InteractAnnoUIController ui) { interactAnnoUi = ui; });
        yield return InitAsync(mapIntroUiPrefab, false, delegate (MapIntroUIController ui) { mapIntroUi = ui; });
        yield return InitAsync(allyCardUiPrefab, false, delegate (AllyCardUIController ui) { allyCardUi = ui; });

        yield return InitAsync(puzzleBlcUiPrefab, false, delegate (BoxLineConnectorUIController ui) { puzzleBlcUi = ui; });
        yield return InitAsync(puzzleNscUiPrefab, false, delegate (NumShapeColorPasswordUIController ui) { puzzleNscUi = ui; });
        yield return InitAsync(puzzleIolUiPrefab, false, delegate (InOrderLockerUIController ui) { puzzleIolUi = ui; });

        yield return InitAsync(premiumCreditCvtUiPrefab, false, delegate (PremiumCreditCvtUIController ui) { premiumCreditCvtUi = ui; });
        yield return InitAsync(protoCoreCvtUiPrefab, false, delegate (ProtoCoreCvtUIController ui) { protoCoreCvtUi = ui; });
        yield return InitAsync(etherCoreCvtUiPrefab, false, delegate (EtherCoreCvtUIController ui) { etherCoreCvtUi = ui; });
        yield return InitAsync(originCoreCvtUiPrefab, false, delegate (OriginCoreCvtUIController ui) { originCoreCvtUi = ui; });

        yield return InitAsync(battleProdUiPrefab, false, delegate (BattleProdUIController ui) { battleProdUi = ui; });

        GameManager.instance.Set_BaseOption();
        yield return null;

        PlayerManager.instance.playerController.TestStart();
        PlayerManager.instance.playerController.skillWeapon.TestStart();

    }

    private IEnumerator InitAsync<T>(T tTypePrefab, Action<T> action) where T : UIController
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        T tType = Instantiate(tTypePrefab, uiParent);

        if (tTypePrefab != null)
        {
            tType.Offset();
            if (action != null)
                action(tType);
        }

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"MainGameUIManager : <color=orange>Generate</color> : {tType.name} : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif

        yield return null;
    }

    private IEnumerator InitAsync<T>(T tTypePrefab, bool onOff, Action<T> action) where T : UIController
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        T tType = Instantiate(tTypePrefab, uiParent);
        tType.gameObject.SetActive(onOff);

        if (tTypePrefab != null)
        {
            tType.Offset();
            if (action != null)
                action(tType);
        }

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"MainGameUIManager : <color=orange>Generate</color> : {tType.name} : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif

        yield return null;
    }

    public InventoryItemEUIController GetModuleItemEUIPrefab() => inventoryItemPrefab;
    public InventorySlotEUIController GetModuleSlotEUIPrefab() => inventorySlotPrefab;


    #region Prod

    public void StartProd()
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
    }

    public void EndProd()
    {
        Play_FadeOut(fadeOutTime);
        Play_OffLoadingIcon(fadeOutTime);

        EventManager.instance.TryStart_Event(0);
    }


    #endregion

    #region Set

    public void SetLanguageTxt()
    {
        pauseUi.SetLanguageTxt();

        mapIntroUi.SetLanguageTxt();
        hud.SetLanguageTxt();
        interactAnnoUi.SetLanguageTxt();

        buUi.SetLanguageTxt();
        muUi.SetLanguageTxt();
        ModuleItemManager.instance.Set_DataLanguage();

        abuUi.SetLanguageTxt();
        amuUi.SetLanguageTxt();

        allyCardUi.SetLanguageTxt();
    }

    public void SetColor()
    {
        interactAnnoUi.Offset_ColorComp();

        buUi.Offset_ColorComp();
        muUi.Offset_ColorComp();

        abuUi.Offset_ColorComp();
        amuUi.Offset_ColorComp();
    }

    #endregion

    #region Fade

    public Sequence Play_FadeOut(float durTime) // ¹à¾ÆÁü
    {
        Sequence seq = DOTween.Sequence();

        screenCanvas.gameObject.SetActive(true);

        seq.Append(screenCG.DOFade(0f, durTime));
        seq.Join(hud.cg.DOFade(1f, durTime));

        seq.OnStart(() =>
        {
            screenCG.alpha = 1f;
            hud.cg.alpha = 0f;

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
        seq.Join(hud.cg.DOFade(0f, durTime));

        seq.OnStart(() =>
        {
            screenCG.alpha = 0f;
            hud.cg.alpha = 1f;
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
