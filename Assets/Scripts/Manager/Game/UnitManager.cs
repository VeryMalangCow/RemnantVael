using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{

    [SerializeField] private List<BulletController> CurrentBullets = new List<BulletController>();
    [SerializeField] private List<DroppingBombController> CurrentBombs = new List<DroppingBombController>();
    [SerializeField] private List<TotemeController> CurrentTotemes = new List<TotemeController>();
    [SerializeField] private List<AttackerController> CurrentAttackers = new List<AttackerController>();

    public void Add_Unit(BulletController _Unit) => DevTool.Add_InList(CurrentBullets, _Unit);
    public void Remove_Unit(BulletController _Unit) => DevTool.Remove_InList(CurrentBullets, _Unit);


    public void Add_Unit(DroppingBombController _Unit) => DevTool.Add_InList(CurrentBombs, _Unit);
    public void Remove_Unit(DroppingBombController _Unit) => DevTool.Remove_InList(CurrentBombs, _Unit);

    public void Add_Unit(TotemeController _Unit) => DevTool.Add_InList(CurrentTotemes, _Unit);
    public void Remove_Unit(TotemeController _Unit) => DevTool.Remove_InList(CurrentTotemes, _Unit);

    public void Add_Unit(AttackerController _Unit) => DevTool.Add_InList(CurrentAttackers, _Unit);
    public void Remove_Unit(AttackerController _Unit) => DevTool.Remove_InList(CurrentAttackers, _Unit);



    public void RemoveUnits()
    {
        for (int i = 0; i < CurrentBullets.Count; i++)
            CurrentBullets[i].RemoveForce_Object();

        for (int i = 0; i < CurrentBombs.Count; i++)
            CurrentBombs[i].RemoveForce_Object();

        for (int i = 0; i < CurrentTotemes.Count; i++)
            CurrentTotemes[i].RemoveForce_Object();

        for (int i = 0; i < CurrentAttackers.Count; i++)
            CurrentAttackers[i].RemoveForce_Object();

        CurrentBullets.Clear();
        CurrentBombs.Clear();
        CurrentTotemes.Clear();
        CurrentAttackers.Clear();
    }



    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private GameObject[] TestGO;

    public void Test_Cor()
    {
        for (int i = 0; i < TestGO.Length; i++)
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

    [Space(5)]
    [Header("-- Core")]
    [SerializeField] private List<IDWithClass<Sprite>> CoreSprites;
    [HideInInspector] private Dictionary<int, Sprite> CoreSpriteDict;

    #endregion

    #region Sprite

    [Space(10)]
    [Header("=== Sprite")]

    [Space(5)]
    [Header("-- Build")]
    [SerializeField] public Sprite BuildingDurFrame;
    [SerializeField] public Sprite BuildingDurInner;

    [Space(5)]
    [Header("-- Enemy")]
    [SerializeField] public List<Sprite> EnemyPhaseIconList;

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
    [SerializeField] public Sprite AllyNullIcon;

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

    [Space(5)]
    [Header("-- Cvt")]
    [SerializeField] public CoupleData<Sprite> CvtMaterialConditionIcon;

    [Space(5)]
    [Header("-- Request")]
    [SerializeField] public List<Sprite> RequestRankSpriteList;
    [SerializeField] private List<SpriteTypeName> RequestRewardSpriteList;

    #endregion

    #region Anim

    [Space(10)]
    [Header("=== Anim")]

    [Space(5)]
    [Header("-- ModuleItem")]
    [SerializeField] public List<AnimationClip> ModuleItemOutlinerAC;

    [Space(5)]
    [Header("-- Keycard")]
    [SerializeField] public AnimationClip KeycardItemOutlinerAC;
    [SerializeField] public List<Color> KeycardItemOutlinerColorList;

    [Space(5)]
    [Header("-- Core")]
    [SerializeField] public AnimationClip CoreItemOutlinerAC;

    [Space(5)]
    [Header("-- Expl")]
    [SerializeField] public AnimationClip ExplosionAC;
    [SerializeField] public List<AnimationClip> AttributeExplosionACList;


    [Space(5)]
    [Header("-- Shop / Actual")]

    [Space(3)]
    [Header("* Player")]
    [SerializeField] public CoupleData<AnimationClip> BUShop_OnOffAC;
    [SerializeField] public AnimationClip BUShop_BrokenAC;
    [SerializeField] public CoupleData<AnimationClip> MUShop_OnOffAC;
    [SerializeField] public AnimationClip MUShop_BrokenAC;

    [Space(3)]
    [Header("* Ally")]
    [SerializeField] public CoupleData<AnimationClip> AllyBUShop_OnOffAC;
    [SerializeField] public AnimationClip AllyBUShop_BrokenAC;
    [SerializeField] public CoupleData<AnimationClip> AllyMUShop_OnOffAC;
    [SerializeField] public AnimationClip AllyMUShop_BrokenAC;

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

    #region Puzzle

    [Space(10)]
    [Header("=== Puzzle : NSC")]
    [SerializeField] public List<NSCAnswerSpriteSet> AllNSCAnswerSpriteSet;

    #endregion

    #region Converter

    [Space(10)]
    [Header("=== Converter")]
    [SerializeField] public ConverterReso ConverterReso;

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

    #endregion

    #region - Hide

    // Request Reward
    [HideInInspector] public Dictionary<string, Sprite> RequestRewardDict = new Dictionary<string, Sprite>();

    #endregion

    #endregion

    #region Offset

    private void Set_DictData()
    {
        CoreSpriteDict = new Dictionary<int, Sprite>();
        for (int i = 0; i < CoreSprites.Count; i++)
        {
            CoreSpriteDict.Add(CoreSprites[i].ID, CoreSprites[i].TypeClass);
        }
        CoreSprites = null;

        for (int i = 0; i < RequestRewardSpriteList.Count; i++) 
        {
            RequestRewardDict.Add(RequestRewardSpriteList[i].Name, RequestRewardSpriteList[i].Sprite);
        }
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        Set_DictData();
    }


    #endregion

    #region Get (NSC)

    public Sprite Get_NSCAnswerSprite(int _ShapeIndex, int _NumIndex)
    {
        return AllNSCAnswerSpriteSet[_ShapeIndex].AllAnswerSet[_NumIndex];
    }

    #endregion

    #region Get Core Item

    public Sprite Get_CoreSprite(int _ID)
    {
        return CoreSpriteDict.ContainsKey(_ID) ? CoreSpriteDict[_ID] : null;
    }

    #endregion
}
