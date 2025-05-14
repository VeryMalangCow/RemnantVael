using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private List<GameObject> TestGO;

    private IEnumerator Test_Cor()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < TestGO.Count; i++)
            TestGO[i].gameObject.SetActive(true);
    }

    #region Material

    [Space(10)]
    [Header("=== Material")]

    [Space(5)]
    [Header("-- Module")]
    [SerializeField] public Material ModuleM_000_Explosion;
    [SerializeField] public Material ModuleM_000_Hitted;

    [Space(5)]
    [Header("-- Enemy")]
    [SerializeField] public Material EnemyM_000_Explosion;

    [Space(5)]
    [Header("-- Build")]
    [SerializeField] public Material Build_000;
    [SerializeField] public CoupleData<Material> Prison_OnOffMaterial;

    #endregion

    #region Sprite

    [Space(10)]
    [Header("=== Sprite")]

    [Space(5)]
    [Header("-- Build")]
    [SerializeField] public Sprite BuildingDurFrame;
    [SerializeField] public Sprite BuildingDurInner;

    [Space(5)]
    [Header("-- Prison")]
    [SerializeField] public List<Sprite> PrisonRateIconList;
    [SerializeField] public CoupleData<Sprite> StrikeTeamIcon;
    [SerializeField] public CoupleData<Sprite> UplinkTeamIcon;
    [SerializeField] public CoupleData<Sprite> NeoTeamIcon;
    [SerializeField] public PrisonAllySprite StrikeTeamAllySprites;
    [SerializeField] public PrisonAllySprite UplinkTeamAllySprites;
    [SerializeField] public PrisonAllySprite NeoTeamAllySprites;

    [Space(5)]
    [Header("-- Ally Card")]
    [SerializeField] public List<Sprite> AllyCardFrameList;
    [SerializeField] public List<Sprite> AllyCardLightList;
    [SerializeField] public List<Sprite> AllyCardBGList;
    [SerializeField] public List<Color> AllyCardColorList;

    [Space(5)]
    [Header("-- Key")]
    [SerializeField] public Sprite SpaceBarSprite;
    [SerializeField] public Sprite MLBSprite;
    [SerializeField] public Sprite MRBSprite;

    [Space(10)]
    [Header("-- Puzzle Color")]
    [SerializeField] public Color LockedClr;
    [SerializeField] public Color UnlockedClr;

    [Space(5)]
    [Header("-- NSC")]
    [SerializeField] public List<Sprite> NSC_NumSpriteList;
    [SerializeField] public List<Sprite> NSC_ShapeSpriteList;
    [SerializeField] public Sprite NSC_ColorSprite;
    [SerializeField] public List<Color> NSC_ColorList;

    #endregion

    #region Color

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] public Color RandomColor = Color.red;
    [HideInInspector] private Sequence RandomColorSetSeq;

    #endregion

    #region Anim

    [Space(10)]
    [Header("=== Anim")]

    [Space(5)]
    [Header("-- ModuleItem")]
    [SerializeField] public List<AnimationClip> ModuleItemOutlinerAC;

    [Space(5)]
    [Header("-- Shop / Actual")]
    [SerializeField] public CoupleData<AnimationClip> BUShop_OnOffAC;
    [SerializeField] public CoupleData<AnimationClip> MUShop_OnOffAC;
    [SerializeField] public AnimationClip BUShop_BrokenAC;
    [SerializeField] public AnimationClip MUShop_BrokenAC;

    [Header("-- Shop / State")]
    [SerializeField] public AnimationClip BrokenStateAC;
    [SerializeField] public CoupleData<AnimationClip> NeedChargeBettery_OnOffStateAC;

    [Space(5)]
    [Header("-- Vault / Actual")]
    [SerializeField] public List<CoupleData<AnimationClip>> Vault_AC;
    [SerializeField] public List<AnimationClip> Vault_BrokenAC;
    [SerializeField] public AnimationClip Vault_ModuleIconAC;
    [SerializeField] public AnimationClip Vault_BSIconAC;
    [SerializeField] public AnimationClip Vault_JIconAC;

    [Space(5)]
    [Header("-- Vault / State")]
    [SerializeField] public CoupleData<AnimationClip> Vault_StateAC;

    [Space(5)]
    [Header("-- Operator / Actual")]
    [SerializeField] public CoupleData<AnimationClip> Operator_OnOffAC;
    [SerializeField] public AnimationClip Operator_RepairAC;
    [SerializeField] public AnimationClip Operator_RerollAC;
    [SerializeField] public AnimationClip Operator_UpgradeAC;
    [SerializeField] public AnimationClip Operator_AllyAC;

    [Space(5)]
    [Header("-- Operator / State")]
    [SerializeField] public CoupleData<AnimationClip> Operator_LightAC;

    [Space(5)]
    [Header("-- Prison / Actual")]
    [SerializeField] public CoupleData<AnimationClip> Prison_OnOffAC;
    [SerializeField] public CoupleData<AnimationClip> Prison_OnOffUpsideAC;

    [Space(5)]
    [Header("-- Prison / State")]
    [SerializeField] public CoupleData<AnimationClip> Prison_StateAC;

    #endregion

    #region Txt

    [Space(10)]
    [Header("=== Font")]
    [SerializeField] public List<LanguageTxt> LanguageTxtList;

    #endregion

    #region Puzzle

    [Space(10)]
    [Header("=== Puzzle : NSC")]
    [SerializeField] public List<NSCAnswerSpriteSet> AllNSCAnswerSpriteSet;

    #endregion

    #region Generator

    [Space(10)]
    [Header("=== Generator")]

    [Space(5)]
    [Header("-- Explosion")]
    [SerializeField] public PlayerExplImgGenerator Player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator Build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator Enemy_ExplImgGenerator;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] public OnceTimeAnimGenerator OnceTime_AnimGenerator;

    #endregion

    #region - Hide

    // string
    [HideInInspector] public string RatingString;
    [HideInInspector] public List<string> PrisonRateStringList;
    [HideInInspector] public string StrikeTeamString;
    [HideInInspector] public string UplinkTeamString;
    [HideInInspector] public string NeoTeamString;
    [HideInInspector] public List<string> AllyCardRateList;

    // Language
    [HideInInspector] private HashSet<LanguageTxtController> AllLanguageTxtController = new HashSet<LanguageTxtController>();

    // Prison
    [HideInInspector] public HashSet<PrisonController> AllPrison = new HashSet<PrisonController>();

    #endregion

    #endregion


    #region Framework

    protected override void Awake()
    {
        base.Awake();

        Set_RainbowColorDotween();
        Set_LanguageTxt();

        StartCoroutine(Test_Cor());
    }

    #endregion

    #region Set (Color)

    // 무지개 컬러 Dotween
    private void Set_RainbowColorDotween()
    {
        RandomColorSetSeq = DOTween.Sequence();
        RandomColor = Color.red;

        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 1, 0, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 1, 0, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 1, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 0, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 0, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 0, 0, 1), 0.5f).SetEase(Ease.Linear));

        RandomColorSetSeq.SetLoops(-1, LoopType.Restart);
    }

    #endregion

    #region Language

    public void Add_LanguageTxt(LanguageTxtController _LangTxt)
    {
        AllLanguageTxtController.Add(_LangTxt);
    }

    public void Set_LanguageFont(int _LangID)
    {
        if (GameManager.LanguageID == _LangID) return;
        GameManager.LanguageID = _LangID;

        // Change String
        Set_LanguageTxt();

        // Change Font Asset
        foreach (LanguageTxtController ltc in AllLanguageTxtController)
            ltc.Set_Font(GameManager.LanguageID);

        // Change UI
        MainGameUIManager.Instance.Set_LanguageTxt();

        // Change PrisonInfo
        foreach (PrisonController prison in AllPrison)
            prison.Set_LanguageTxt();
    }

    private void Set_LanguageTxt()
    {
        RatingString = ResourceManager.Instance.Get_StaticWord(69);
        PrisonRateStringList = new List<string>
        {
            ResourceManager.Instance.Get_StaticWord(64),
            ResourceManager.Instance.Get_StaticWord(65),
            ResourceManager.Instance.Get_StaticWord(66),
            ResourceManager.Instance.Get_StaticWord(67),
            ResourceManager.Instance.Get_StaticWord(68)
        };

        StrikeTeamString = $"{ResourceManager.Instance.Get_StaticWord(61)}<size=85%> ({ResourceManager.Instance.Get_StaticWord(71)})</size>";
        UplinkTeamString = $"{ResourceManager.Instance.Get_StaticWord(62)}<size=85%> ({ResourceManager.Instance.Get_StaticWord(72)})</size>";
        NeoTeamString = $"{ResourceManager.Instance.Get_StaticWord(63)}<size=85%> ({ResourceManager.Instance.Get_StaticWord(73)})</size>";

        AllyCardRateList = new List<string>
        {
            ResourceManager.Instance.Get_StaticWord(76),
            ResourceManager.Instance.Get_StaticWord(77),
            ResourceManager.Instance.Get_StaticWord(78),
            ResourceManager.Instance.Get_StaticWord(79),
            ResourceManager.Instance.Get_StaticWord(80),
            ResourceManager.Instance.Get_StaticWord(81)
        };
    }

    #endregion

    #region Get (NSC)

    public Sprite Get_NSCAnswerSprite(int _ShapeIndex, int _NumIndex)
    {
        return AllNSCAnswerSpriteSet[_ShapeIndex].AllAnswerSet[_NumIndex];
    }

    #endregion
}
