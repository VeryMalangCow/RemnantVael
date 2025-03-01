using DG.Tweening;
using UnityEngine;

public class MainGameUIManager : Singleton<MainGameUIManager>
{
    #region Value

    [Header("=== UI_Camera")]
    [SerializeField] private Camera UICamera;

    [Header("=== UI_Prefab")]
    [SerializeField] public Transform UIParent;
    [SerializeField] private GameObject PlayerHUD_CanvasPrefab;
    [SerializeField] private GameObject BaseUpgrade_CanvasPrefab;
    [SerializeField] private GameObject ModuleUpgrade_CanvasPrefab;
    [SerializeField] private GameObject OutMainGame_CanvasPrefab;
    [SerializeField] private GameObject InteractAnno_CanvasPrefab;
    [SerializeField] private GameObject MapIntro_CanvasPrefab;

    // Controller
    [HideInInspector] public PlayerHUDController PlayerHUD_UIController;
    [HideInInspector] public BaseUpgradeUIController BaseUpgrade_UIController;
    [HideInInspector] public ModuleUpgradeUIController ModuleUpgrade_UIController;
    [HideInInspector] public OutMainGameUIController OutMainGame_UIController;
    [HideInInspector] public InteractAnnoUIController InteractAnno_UIController;
    [HideInInspector] public MapIntroUIController MapIntro_UIController;    

    [HideInInspector] public UIController CurrentOpening_UIController;

    [Header("=== Screen")]
    [SerializeField] private Canvas ScreenCanvas;
    [HideInInspector] private CanvasGroup ScreenCG;
    [SerializeField] private float FadeOutTime = 2f;

    #endregion

    #region Framework

    private void Start()
    {
        PlayerHUD_UIController 
            = Gen_UI<PlayerHUDController>(PlayerHUD_CanvasPrefab, true);

        BaseUpgrade_UIController 
            = Gen_UI<BaseUpgradeUIController>(BaseUpgrade_CanvasPrefab, false);
        ModuleUpgrade_UIController 
            = Gen_UI<ModuleUpgradeUIController>(ModuleUpgrade_CanvasPrefab, false);

        OutMainGame_UIController
            = Gen_UI<OutMainGameUIController>(OutMainGame_CanvasPrefab, false);

        InteractAnno_UIController
            = Gen_UI<InteractAnnoUIController>(InteractAnno_CanvasPrefab, false);

        MapIntro_UIController
            = Gen_UI<MapIntroUIController>(MapIntro_CanvasPrefab, false);

        Start_FirstPlay();
    }

    #endregion

    #region Spawn

    private T Gen_UI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);
        uigo.gameObject.SetActive(_OnOff);
        if (uigo.TryGetComponent(out UIController ui))
        {
            ui.Offset_Main();
        }
        if (uigo.TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = UICamera;
        }

        if (uigo.TryGetComponent(out T spawnUI))
        { return spawnUI; }
        else
        { return default; }
    }

    #endregion

    #region FirstStart

    private void Start_FirstPlay()
    {
        if (ScreenCG == null && ScreenCanvas.TryGetComponent(out CanvasGroup CG))
        { ScreenCG = CG; }

        Sequence firstSeq = DOTween.Sequence();

        ScreenCG.alpha = 1f;

        firstSeq.Append(ScreenCG.DOFade(0f, FadeOutTime));

        firstSeq
            .OnComplete(() =>
            {
                ScreenCanvas.gameObject.SetActive(false);
            });
    }

    public void Play_Dark(float _DurTime)
    {
        ScreenCanvas.gameObject.SetActive(true);

        Sequence firstSeq = DOTween.Sequence();

        firstSeq.Append(ScreenCG.DOFade(1f, FadeOutTime));
    }

    #endregion
}
