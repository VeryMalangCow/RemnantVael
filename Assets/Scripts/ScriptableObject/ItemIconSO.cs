using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIconSO", menuName = "ScriptableObject/ItemIconSO")]
public class ItemIconSO : ScriptableObject
{
    public Sprite[] rankIcons;
    public Sprite[] descRankIcons;

    [Space(10)]
    [Header("=== Module Item")]
    public Texture2D[] moduleItemTextures;
    public List<Sprite> moduleItemSprites;

    [Space(10)]
    [Header("=== Module Synhrony")]
    public Texture2D[] moduleSynhronyTextures;
    public List<Sprite> moduleSynhronySprites;

    [Space(10)]
    [Header("=== Core")]
    public Sprite[] coreSprites;
}
