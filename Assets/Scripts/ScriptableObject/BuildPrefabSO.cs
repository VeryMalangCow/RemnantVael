using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildPrefabSO", menuName = "ScriptableObject/BuildPrefabSO")]
public class BuildPrefabSO : ScriptableObject
{
    public AnimationClip brokenStateAnimation;
    public CoupleData<AnimationClip> needChargeBetteryOnOffStateAnimation;
    [Space(10)]
    public VaultBuild vaultPrefab;
    public DestructableBuild<BaseUpgradeController> buPrefab;
    public DestructableBuild<ModuleUpgradeController> muPrefab;
    public DestructableBuild<AllyBaseUpgradeController> abuPrefab;
    public DestructableBuild<AllyModuleUpgradeController> amuPrefab;
    public PrisonBuild prisonPrefab;

    [Space(40)]

    public CoupleData<AnimationClip> operOnOffAnimation;
    public CoupleData<AnimationClip> operLightAnimation; 
    [Space(10)]
    public OperatorBuild<RepairOperatorController> repairOperPrefab;
    public OperatorBuild<VaultRerollOperatorController> vaultRerollOperPrefab;
    public OperatorBuild<VaultUpgradeOperatorController> vaultUpgradeOperPrefab;
    public OperatorBuild<PrisonPayOperatorController> prisonPayOperPrefab;
    public OperatorBuild<PrisonPuzzleOperatorController> prisonPuzzleOperPrefab;

    [Space(40)]

    public ConverterBuild<ConverterController> preminumCreditCvtPrefab;
    public ConverterBuild<ConverterController> protoCoreCvtPrefab;
    public ConverterBuild<ConverterController> EtherCoreCvtPrefab;
    public ConverterBuild<ConverterController> originCoreCvtPrefab;
    public ConverterBuild<ConverterController> GetConverter(int id)
    {
        switch (id)
        {
            case 0: return preminumCreditCvtPrefab;
            case 1: return protoCoreCvtPrefab;
            case 2: return EtherCoreCvtPrefab;
            case 3: return originCoreCvtPrefab;

            default: return null;
        }
    } 

}

[Serializable]
public class VaultBuild
{
    public VaultController[] prefabs;
    public AnimationClip[] onAnimations;
    public AnimationClip[] brokenAnimations;
    [Space(10)]
    public AnimationClip moduleIconAnimation;
    public AnimationClip betteryShardIconAnimation;
    public AnimationClip jouleIconAnimation;
    [Space(10)]
    public CoupleData<AnimationClip> stateIconAnimation;
    [Space(10)]
    public Material material;
}

[Serializable]
public class DestructableBuild<T>
{
    public T prefab; 
    [Space(10)]
    public CoupleData<AnimationClip> onOffAniamtion;
    public AnimationClip brokenAnimation;
    [Space(10)]
    public Material material;
}

[Serializable]
public class PrisonBuild
{
    public PrisonController[] prefabs;
    public Sprite[] rateIcons;
    public CoupleData<AnimationClip> onOffAnimation;
    public CoupleData<AnimationClip> onOffUpsideAnimation;
    public CoupleData<AnimationClip> stateAnimation;
    public CoupleData<Material> material;
}

[Serializable]
public class OperatorBuild<T>
{
    public T prefab;
    public AnimationClip animation;
}

[Serializable]
public class ConverterBuild<T>
{
    public T prefab;
    public AnimationClip animation;
}
