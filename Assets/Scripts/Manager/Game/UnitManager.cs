using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    #region Material

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] public Material ModuleM_000_Explosion;
    [SerializeField] public Material ModuleM_000_Hitted;
    [SerializeField] public Material EnemyM_000_Explosion;

    #endregion

    #region Sprite

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public Sprite BuildingDurFrame;
    [SerializeField] public Sprite BuildingDurInner;

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
    [SerializeField] public AnimationClip Operator_OverriderRerollAC;
    [SerializeField] public AnimationClip Operator_EnergyUpgradeAC;

    [Space(5)]
    [Header("-- Operator / State")]
    [SerializeField] public CoupleData<AnimationClip> Operator_LightAC;

    #endregion

    #region Txt

    [Space(10)]
    [Header("=== Font")]
    [SerializeField] public List<LanguageTxt> LanguageTxtList;

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

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        Set_RainbowColorDotween();
    }

    #endregion

    #region Set

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
}
