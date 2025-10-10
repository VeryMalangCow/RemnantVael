using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUIManager : Singleton<MainGameUIManager>
{
    #region Value

    #region - Inspector

    [Header("=== Class")]
    [SerializeField] public Camera UICamera;
    [SerializeField] public Transform UIParent;
    [SerializeField] private Canvas ScreenCanvas;
    [SerializeField] private Canvas LoadingIconCanvas;

    [Header("=== Screen")]
    [SerializeField] public float FadeOutTime = 3f;

    [Header("=== Save")]
    [SerializeField] private CanvasGroup SaveDataCG;

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

    [SerializeField] private GameObject Cvt_PremiumCredit_CanvasPrefab;
    [SerializeField] private GameObject Cvt_ProtoCore_CanvasPrefab;
    [SerializeField] private GameObject Cvt_EtherCore_CanvasPrefab;
    [SerializeField] private GameObject Cvt_OriginCore_CanvasPrefab;

    [SerializeField] private CanvasGroup BattleProd_CG;
    [SerializeField] private RectTransform BattleProd_PlayerRT;
    [SerializeField] private Image BattleProd_PlayerImg;
    [SerializeField] private RectTransform BattleProd_EnemyRT;
    [SerializeField] private Image BattleProd_EnemyImg;

    [SerializeField] private List<Sprite> KeyCardSpriteList;

    #endregion

    #region - Hide

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

    [HideInInspector] public PremiumCreditCvtUIController PremiumCreditCvt_UIController;
    [HideInInspector] public ProtoCoreCvtUIController ProtoCoreCvt_UIController;
    [HideInInspector] public EtherCoreCvtUIController EtherCoreCvt_UIController;
    [HideInInspector] public OriginCoreCvtUIController OriginCoreCvt_UIController;

    // Current
    [HideInInspector] public static UIController CurrentOpening_UIController;

    // Production
    [HideInInspector] private CanvasGroup ScreenCG;
    [HideInInspector] private CanvasGroup LoadingIconCG;
    [HideInInspector] private RectTransform LoadingIconRT;
    [HideInInspector] private Tween CogwheelTween = null;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        ScreenCG = DevTool.Get_ComponentTType(ScreenCanvas.gameObject, out CanvasGroup cg) ? cg : null;
        LoadingIconCG = DevTool.Get_ComponentTType(LoadingIconCanvas.gameObject, out CanvasGroup iconCg) ? iconCg : null;
        LoadingIconRT = DevTool.Get_ComponentTType(LoadingIconCanvas.gameObject.transform.GetChild(0).gameObject, out RectTransform iconRt) ? iconRt : null;

        SaveDataCG.gameObject.SetActive(false);

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

        PremiumCreditCvt_UIController
            = Gen_UI<PremiumCreditCvtUIController>(Cvt_PremiumCredit_CanvasPrefab, false);
        ProtoCoreCvt_UIController
            = Gen_UI<ProtoCoreCvtUIController>(Cvt_ProtoCore_CanvasPrefab, false);
        EtherCoreCvt_UIController
            = Gen_UI<EtherCoreCvtUIController>(Cvt_EtherCore_CanvasPrefab, false);
        OriginCoreCvt_UIController
            = Gen_UI<OriginCoreCvtUIController>(Cvt_OriginCore_CanvasPrefab, false);

        Sequence startSeq = DOTween.Sequence();

        Play_FadeOut(FadeOutTime);
        Play_OffLoadingIcon(FadeOutTime);

        Offset_BattleProd();
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

    public void Set_Color()
    {
        OutMainGame_UIController.Offset_ColorComp();

        PlayerHUD_UIController.Offset_ColorComp();
        InteractAnno_UIController.Offset_ColorComp();

        BaseUpgrade_UIController.Offset_ColorComp();
        ModuleUpgrade_UIController.Offset_ColorComp();

        AllyBaseUpgrade_UIController.Offset_ColorComp();
        AllyModuleUpgrade_UIController.Offset_ColorComp();
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

    #region SaveData

    public void Play_SaveData()
    {
        Sequence seq = DOTween.Sequence();

        SaveDataCG.alpha = 0f;
        SaveDataCG.gameObject.SetActive(true);

        seq.Append(SaveDataCG.DOFade(1f, 0.3f));
        seq.Append(SaveDataCG.DOFade(0f, 0.3f));
        seq.Append(SaveDataCG.DOFade(1f, 0.3f));
        seq.Append(SaveDataCG.DOFade(0f, 0.3f));
        seq.Append(SaveDataCG.DOFade(1f, 0.3f));
        seq.AppendInterval(1.5f);
        seq.Append(SaveDataCG.DOFade(0f, 1.5f));
        seq.OnComplete(() =>
        {
            SaveDataCG.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Get

    public Sprite Get_KeyCardSprite(int _Key)
    {
        return KeyCardSpriteList[_Key];
    }

    public int Get_KindOfKeyCardAmount()
    {
        return KeyCardSpriteList.Count;
    }

    #endregion

    #region BattleProd

    private void Offset_BattleProd()
    {
        BattleProd_CG.gameObject.SetActive(false);
        BattleProd_CG.alpha = 0;
        BattleProd_PlayerRT.anchoredPosition = new Vector2(-100, 0);
        BattleProd_EnemyRT.anchoredPosition = new Vector2(100, 0);
    }

    public void Play_BattleOnProd(Sprite _PlayerImg, Sprite _EnemyImg, out float _DurTime)
    {
        BattleProd_CG.gameObject.SetActive(true);
        BattleProd_PlayerImg.sprite = _PlayerImg;
        BattleProd_PlayerImg.SetNativeSize();
        BattleProd_EnemyImg.sprite = _EnemyImg;
        BattleProd_EnemyImg.SetNativeSize();

        Sequence seq = DOTween.Sequence();

        float time0 = 0.2f;
        seq.Append(BattleProd_CG.DOFade(1f, time0));
        float time1 = 1f;
        seq.Append(BattleProd_PlayerRT.DOAnchorPosX(650f, time1).SetEase(Ease.Linear));
        //seq.Join(BattleProd_EnemyRT.DOAnchorPosX(-650f, time1).SetEase(Ease.Linear));
        float time2 = 3f;
        seq.Append(BattleProd_PlayerRT.DOAnchorPosX(700, time2).SetEase(Ease.OutQuad));
        //seq.Join(BattleProd_EnemyRT.DOAnchorPosX(-700, time2).SetEase(Ease.OutQuad));

        _DurTime = time0 + time1 + time2;
    }

    public void Play_BattleOffProd(out float _DurTime)
    {
        _DurTime = 0.25f;
        BattleProd_CG.DOFade(0f, _DurTime)
            .OnComplete(() => Offset_BattleProd())
            .SetUpdate(true);
    }

    #endregion
}
