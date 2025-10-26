using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    #region - Current Unit

    [HideInInspector] private List<BulletController> CurrentBullets = new List<BulletController>();
    [HideInInspector] private List<DroppingBombController> CurrentBombs = new List<DroppingBombController>();
    [HideInInspector] private List<TotemeController> CurrentTotemes = new List<TotemeController>();
    [HideInInspector] private List<AttackerController> CurrentAttackers = new List<AttackerController>();

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

    #endregion

    #region - Generator

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

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private GameObject[] TestGO;

    public void Test_Cor()
    {
        for (int i = 0; i < TestGO.Length; i++)
            TestGO[i].gameObject.SetActive(true);
    }

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

}
