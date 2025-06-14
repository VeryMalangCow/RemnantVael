using DG.Tweening;
using UnityEngine;

public class MainGameUIManager : Singleton<MainGameUIManager>
{
    #region Value

    [Header("=== Class")]
    [SerializeField] public Camera UICamera;
    [SerializeField] public Transform UIParent;
    [SerializeField] private Canvas ScreenCanvas;
    [SerializeField] private Canvas LoadingIconCanvas;

    [Header("=== Screen")]
    [SerializeField] public float FadeOutTime = 3f;

    [Header("=== Prefab")]
    [SerializeField] private GameObject PlayerHUD_CanvasPrefab;

    [SerializeField] private GameObject BaseUpgrade_CanvasPrefab;
    [SerializeField] private GameObject ModuleUpgrade_CanvasPrefab;

    [SerializeField] private GameObject AllyBaseUpgrade_CanvasPrefab;
    [SerializeField] private GameObject AllyModuleUpgrade_CanvasPrefab;

    [SerializeField] private GameObject OutMainGame_CanvasPrefab;
    [SerializeField] private GameObject InteractAnno_CanvasPrefab;
    [SerializeField] private GameObject MapIntro_CanvasPrefab;

    [SerializeField] private GameObject AllyCard_CanvasPrefab;

    [SerializeField] private GameObject Puzzle_BoxLineConnector_CanvasPrefab;
    [SerializeField] private GameObject Puzzle_NumShapeColorPassword_CanvasPrefab;
    [SerializeField] private GameObject Puzzle_InOrderLocker_CanvasPrefab;

    // Controller
    [HideInInspector] public PlayerHUDController PlayerHUD_UIController;

    [HideInInspector] public BaseUpgradeUIController BaseUpgrade_UIController;
    [HideInInspector] public ModuleUpgradeUIController ModuleUpgrade_UIController;

    [HideInInspector] public AllyBaseUpgradeUIController AllyBaseUpgrade_UIController;
    [HideInInspector] public AllyModuleUpgradeUIController AllyModuleUpgrade_UIController;

    [HideInInspector] public OutMainGameUIController OutMainGame_UIController;
    [HideInInspector] public InteractAnnoUIController InteractAnno_UIController;
    [HideInInspector] public MapIntroUIController MapIntro_UIController;

    [HideInInspector] public AllyCardUIController AllyCard_UIController;

    [HideInInspector] public BoxLineConnectorUIController BoxLineConnector_UIController;
    [HideInInspector] public NumShapeColorPasswordUIController NumShapeColorPassword_UIController;
    [HideInInspector] public InOrderLockerUIController InOrderLocker_UIController;

    // Current
    [HideInInspector] public static UIController CurrentOpening_UIController;

    // Production
    [HideInInspector] private CanvasGroup ScreenCG;
    [HideInInspector] private CanvasGroup LoadingIconCG;
    [HideInInspector] private RectTransform LoadingIconRT;
    [HideInInspector] private Tween CogwheelTween = null;
    #endregion

    #region Offset

    private void Offset()
    {
        ScreenCG = DevTool.Get_ComponentTType(ScreenCanvas.gameObject, out CanvasGroup cg) ? cg : null;
        LoadingIconCG = DevTool.Get_ComponentTType(LoadingIconCanvas.gameObject, out CanvasGroup iconCg) ? iconCg : null;
        LoadingIconRT = DevTool.Get_ComponentTType(LoadingIconCanvas.gameObject.transform.GetChild(0).gameObject, out RectTransform iconRt) ? iconRt : null;

        CogwheelTween = LoadingIconRT
            .DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        PlayerHUD_UIController
            = Gen_UI<PlayerHUDController>(PlayerHUD_CanvasPrefab, true);

        OutMainGame_UIController
            = Gen_UI<OutMainGameUIController>(OutMainGame_CanvasPrefab, false);

        BaseUpgrade_UIController
            = Gen_UI<BaseUpgradeUIController>(BaseUpgrade_CanvasPrefab, false);
        ModuleUpgrade_UIController
            = Gen_UI<ModuleUpgradeUIController>(ModuleUpgrade_CanvasPrefab, false);

        AllyBaseUpgrade_UIController
            = Gen_UI<AllyBaseUpgradeUIController>(AllyBaseUpgrade_CanvasPrefab, false);
        AllyModuleUpgrade_UIController
            = Gen_UI<AllyModuleUpgradeUIController>(AllyModuleUpgrade_CanvasPrefab, false);

        InteractAnno_UIController
            = Gen_UI<InteractAnnoUIController>(InteractAnno_CanvasPrefab, false);

        MapIntro_UIController
            = Gen_UI<MapIntroUIController>(MapIntro_CanvasPrefab, false);

        AllyCard_UIController
            = Gen_UI<AllyCardUIController>(AllyCard_CanvasPrefab, false);

        BoxLineConnector_UIController
            = Gen_UI<BoxLineConnectorUIController>(Puzzle_BoxLineConnector_CanvasPrefab, false);
        NumShapeColorPassword_UIController
            = Gen_UI<NumShapeColorPasswordUIController>(Puzzle_NumShapeColorPassword_CanvasPrefab, false);
        InOrderLocker_UIController
            = Gen_UI<InOrderLockerUIController>(Puzzle_InOrderLocker_CanvasPrefab, false);


        Sequence startSeq = DOTween.Sequence();

        Play_FadeOut(FadeOutTime);
        Play_OffLoadingIcon(FadeOutTime);
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    #endregion

    #region Gen

    private T Gen_UI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);

        uigo.gameObject.SetActive(_OnOff);
        if (DevTool.Get_ComponentTType(uigo, out UIController uiController) &&
            DevTool.Get_ComponentTType(uigo, out Canvas uiCanvas))
        {
            uiController.Offset();
            uiCanvas.worldCamera = UICamera;
        }

        return DevTool.Get_ComponentTType(uigo, out T tType) ? tType : default;
    }

    #endregion

    #region Set

    public void Set_LanguageTxt()
    {
        OutMainGame_UIController.Set_LanguageTxt();

        MapIntro_UIController.Set_LanguageTxt();
        PlayerHUD_UIController.Set_LanguageTxt();
        InteractAnno_UIController.Set_LanguageTxt();

        BaseUpgrade_UIController.Set_LanguageTxt();
        ModuleUpgrade_UIController.Set_LanguageTxt();
        ModuleItemManager.Instance.Set_DataLanguage();

        AllyBaseUpgrade_UIController.Set_LanguageTxt();
        AllyModuleUpgrade_UIController.Set_LanguageTxt();

        AllyCard_UIController.Set_LanguageTxt();
    }

    #endregion

    #region Fade

    public Sequence Play_FadeOut(float _DurTime) // ¹à¾ÆÁü
    {
        Sequence seq = DOTween.Sequence();

        ScreenCanvas.gameObject.SetActive(true);

        seq.Append(ScreenCG.DOFade(0f, _DurTime));
        seq.Join(PlayerHUD_UIController.ThisCG.DOFade(1f, _DurTime));

        seq.OnStart(() =>
        {
            ScreenCG.alpha = 1f;
            PlayerHUD_UIController.ThisCG.alpha = 0f;

        })
        .OnComplete(() =>
        {
            ScreenCanvas.gameObject.SetActive(false);
        });

        return seq;
    }

    public Sequence Play_FadeIn(float _DurTime) // ¾îµÎ¿öÁü
    {
        Sequence seq = DOTween.Sequence();

        ScreenCanvas.gameObject.SetActive(true);

        seq.Append(ScreenCG.DOFade(1f, _DurTime));
        seq.Join(PlayerHUD_UIController.ThisCG.DOFade(0f, _DurTime));

        seq.OnStart(() =>
        {
            ScreenCG.alpha = 0f;
            PlayerHUD_UIController.ThisCG.alpha = 1f;
        })
        .OnComplete(() =>
        {

        });

        return seq;
    }

    #endregion

    #region Loading

    public Sequence Play_OnLoadingIcon(float _DurTime) // ³ªÅ¸³ª±â
    {
        Sequence seq = DOTween.Sequence();

        LoadingIconCanvas.gameObject.SetActive(true);

        seq.Append(LoadingIconCG.DOFade(1f, _DurTime));

        seq.OnStart(() =>
        {
            CogwheelTween.Play();
            LoadingIconCG.alpha = 0f;
        })
        .OnComplete(() =>
        {

        });

        return seq;
    }

    public Sequence Play_OffLoadingIcon(float _DurTime) // »ç¶óÁö±â
    {
        Sequence seq = DOTween.Sequence();

        LoadingIconCanvas.gameObject.SetActive(true);

        seq.Append(LoadingIconCG.DOFade(0f, _DurTime));

        seq.OnStart(() =>
        {
            LoadingIconCG.alpha = 1f;
        })
        .OnComplete(() =>
        {
            CogwheelTween.Pause();
            LoadingIconCanvas.gameObject.SetActive(false);
        });

        return seq;
    }

    #endregion
}
