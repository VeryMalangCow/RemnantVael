using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIconSO", menuName = "ScriptableObject/ItemIconSO")]
public class ItemIconSO : ScriptableObject
{
    [Space(10)]
    [Header("=== Module")]

    [Space(5)]
    [Header("-- Rank")]
    public Sprite[] rankIcons;
    public Sprite[] descRankIcons;

    [Space(5)]
    [Header("-- Item")]
    public Texture2D[] moduleItemTextures;
    public List<Sprite> moduleItemSprites;

    [Space(5)]
    [Header("-- Synhrony")]
    public Texture2D[] moduleSynhronyTextures;
    public List<Sprite> moduleSynhronySprites;

    [Space(5)]
    [Header("-- Vfx")]
    public AnimationClip[] moduleOutlineAnimations;




    [Space(30)]
    [Header("=== Core")]
    public Sprite[] coreSprites;
    public AnimationClip coreShiningAnimation;




    [Space(30)]
    [Header("=== Keycard")]
    public keycardIcon[] keycardIcons;
    public AnimationClip keycardOutlineAnimation;
}

[System.Serializable]
public class keycardIcon
{
    public Sprite sprite;
    public Color clr;
}

