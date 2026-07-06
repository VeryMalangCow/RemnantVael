using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildPrefabSO", menuName = "ScriptableObject/BuildPrefabSO")]
public class BuildResoSO : ScriptableObject
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
    public Material operMaterial;
    public Material operIconMaterial;
    [Space(10)]
    public OperatorBuild<RepairOperatorController> repairOperPrefab;
    public OperatorBuild<VaultRerollOperatorController> vaultRerollOperPrefab;
    public OperatorBuild<VaultUpgradeOperatorController> vaultUpgradeOperPrefab;
    public OperatorBuild<PrisonPayOperatorController> prisonPayOperPrefab;
    public OperatorBuild<PrisonPuzzleOperatorController> prisonPuzzleOperPrefab;

    [Space(40)]
    public CoupleData<Sprite> cvtMaterialConditionIcon;
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

    [Space(40)]
    public Sprite durablityFrameSprite;
    public Sprite durablityInnerSprite;

    [Space(40)]
    public Material explosionMaterial;
    public Material durabilityMaterial;
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
    public Material panelMaterial;
    public Material iconMaterial;
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
    public Material iconMaterial;
}

[Serializable]
public class PrisonBuild
{
    public PrisonEachBuild[] builds;

    public Sprite[] rateIcons;
    public CoupleData<AnimationClip> onOffAnimation;
    public CoupleData<AnimationClip> onOffUpsideAnimation;
    public CoupleData<AnimationClip> stateAnimation;
    public CoupleData<Material> material;
    public Material iconMaterial;
    public Material allyMaterial;
    [Space(10)]
    public Sprite spaceBarSprite;
    public Color lockedClr;
    public static Color unlockedClr;

    [Serializable]
    public class PrisonEachBuild
    {
        public PrisonController prefab;
        public PrisonAllySprite allySprite;
        public CoupleData<Sprite> teamIcon;
    }

    [Space(10)]
    public Sprite[] nscNumSpriteArr;
    public Sprite[] nscShapeSpriteArr;
    public Color[] nscClrArr;
    public Sprite nscClrSprite;

    public SerializableArray<Sprite>[] allNscAnswerSprites;
}

[System.Serializable]
public class NSCAnswerSpriteSet
{
    public int shapeIndex;
    public Sprite[] allAnswerSet;
}


[Serializable]
public class OperatorBuild<T>
{
    public T prefab;
    public AnimationClip animation;
    public Material panelMaterial;
    public Sprite annoIcon;
    public Material annoMaterial;
}

[Serializable]
public class ConverterBuild<T>
{
    public T prefab;
    public AnimationClip animation;
    public Material material;
}
