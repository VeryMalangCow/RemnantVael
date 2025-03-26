using DG.Tweening;
using UnityEngine;

public class MainGameUIManager : Singleton<MainGameUIManager>
{
    #region Value

    [Header("=== Class")]
    [SerializeField] private Camera UICamera;
    [SerializeField] public Transform UIParent;
    [SerializeField] private Canvas ScreenCanvas;

    [Header("=== Screen")]
    [SerializeField] private float FadeOutTime = 3f;

    [Header("=== refab")]
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

    [HideInInspector] private CanvasGroup ScreenCG;

    #endregion

    #region Offset

    private void Offset()
    {
        ScreenCG = DevTool.Get_ComponentTType(ScreenCanvas.gameObject, out CanvasGroup cg) ? cg : null;

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

        Sequence startSeq = DOTween.Sequence();

        Start_FadeOut(FadeOutTime);
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

    #region FirstStart

    private Sequence Start_FadeOut(float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(ScreenCG.DOFade(0f, _DurTime)
            .OnComplete(() =>
            {
                PlayerHUD_UIController.gameObject.SetActive(true);
                ScreenCanvas.gameObject.SetActive(false);
            }));

        seq.Append(PlayerHUD_UIController.ThisCG.DOFade(1f, _DurTime));

        seq.OnStart(() =>
            {
                PlayerHUD_UIController.gameObject.SetActive(false);
                ScreenCanvas.gameObject.SetActive(true);
                ScreenCG.alpha = 1f;
                PlayerHUD_UIController.ThisCG.alpha = 0f;
            });

        return seq;
    }

    public Sequence Play_FadeIn(float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(ScreenCG.DOFade(1f, _DurTime));
        seq.Append(PlayerHUD_UIController.ThisCG.DOFade(0f, _DurTime));

        seq.OnStart(() =>
            {
                ScreenCanvas.gameObject.SetActive(true);
                ScreenCG.alpha = 0f;
            })
            .OnComplete(() =>
            {
                PlayerHUD_UIController.gameObject.SetActive(false);
            });

        return seq;
    }

    #endregion
}
