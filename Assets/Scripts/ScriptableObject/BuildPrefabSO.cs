using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildPrefabSO", menuName = "ScriptableObject/BuildPrefabSO")]
public class BuildPrefabSO : ScriptableObject
{
    public VaultPrefab vaultPrefab;
    [Space(20)]

    public BaseUpgradeController buPrefab;
    public ModuleUpgradeController muPrefab;
    public AllyBaseUpgradeController abuPrefab;
    public AllyModuleUpgradeController amuPrefab;
    public PrisonController[] prisonPrefabs;

    [Space(10)]
    [Header("=== Cvt")]
    public PreminumCreditCvtController preminumCreditCvtPrefab;
    public ProtoCoreCvtController protoCoreCvtPrefab;
    public EtherCoreCvtController EtherCoreCvtPrefab;
    public OriginCoreCvtController originCoreCvtPrefab;


    [Space(10)]
    [Header("=== Oper")]
    public RepairOperatorController repairOperPrefab;
    public VaultRerollOperatorController vaultRerollOperPrefab;
    public VaultUpgradeOperatorController vaultUpgradeOperPrefab;
    public PrisonPayOperatorController prisonPayOperPrefab;
    public PrisonPuzzleOperatorController prisonPuzzleOperPrefab;
}

[Serializable]
public class VaultPrefab
{
    public VaultController[] prefabs;
    public AnimationClip[] onAnimations;
    public AnimationClip[] brokenAnimations;
    [Space(10)]
    public AnimationClip moduleIconAC;
    public AnimationClip betteryShardIconAC;
    public AnimationClip jouleIconAC;
    [Space(10)]
    public CoupleData<AnimationClip> stateIconAC;
    [Space(10)]
    public Material material;
}


public class DestructableBuild<T>
{
    public T prefab;
    public CoupleData<AnimationClip> onOffAc;
    public AnimationClip buShop_BrokenAC { get; private set; }
}
