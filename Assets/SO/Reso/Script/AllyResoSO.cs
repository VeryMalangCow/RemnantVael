using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllyPrefabSO", menuName = "ScriptableObject/AllyPrefabSO")]
public class AllyResoSO : ScriptableObject
{
    [Header("=== Ally")]
    public static readonly string[] directionOrder = new string[] { "UL", "U", "UR", "R", "DR", "D", "DL", "L" };
    public AllySpriteSet gruntSpriteSet;
    public AllySpriteSet ignisSpriteSet;
    public AllySpriteSet glaciaSpriteSet;
    public AllySpriteSet voltSpriteSet;
    public AllySpriteSet toxSpriteSet;
    public AllySpriteSet GetAllySpriteSet(string name)
    {
        switch (name)
        {
            case "Grunt": return gruntSpriteSet;
            case "Ignis": return ignisSpriteSet;
            case "Glacia": return glaciaSpriteSet;
            case "Volt": return voltSpriteSet;
            case "Tox": return toxSpriteSet;

            default: return null;
        }
    }

    [Header("=== Ally Gun")]
    public AllyGunSpriteSet gruntGunSpriteSet;
    public AllyGunSpriteSet ignisGunSpriteSet;
    public AllyGunSpriteSet glaciaGunSpriteSet;
    public AllyGunSpriteSet voltGunSpriteSet;
    public AllyGunSpriteSet toxGunSpriteSet;
    public AllyGunSpriteSet GetAllyGunSpriteSet(string name)
    {
        switch (name)
        {
            case "Grunt": return gruntGunSpriteSet;
            case "Ignis": return ignisGunSpriteSet;
            case "Glacia": return glaciaGunSpriteSet;
            case "Volt": return voltGunSpriteSet;
            case "Tox": return toxGunSpriteSet;

            default: return null;
        }
    }


    [Header("=== Ally Card")]
    public SerializableArray<Texture2D>[] allyCardTextures;
    public SerializableArray<Sprite>[] allyCardIcons;

    public AllyCardSpriteSet[] allyCardSpriteSets;
    public Sprite allyNullIcon;

    [Header("=== Tuner")]
    public List<Sprite> tunerTypeIcons;

    [Header("=== Requset")]
    public Sprite[] requestRankSprites;
    public Sprite bcSprite;
    public Sprite creditSprite;
    public Sprite epSprite;
    public Sprite Get_AllyRequestReward(string type)
    {
        switch (type)
        {
            case "BC": return bcSprite;
            case "Credit": return creditSprite;
            case "EP": return epSprite;

            default: return null;
        }
    }

    [Space(30)]
    [Header("=== CSV")]
    [Header("-- Request")]
    public TextAsset requestNameCsv;
    public TextAsset requestFailureCsv;
    public TextAsset requestSuccessCsv;

    [Header("-- Card")]
    public AllyCardCsv stCsv;
    public AllyCardCsv utCsv;
    public AllyCardCsv ntCsv;

    [Header("-- Tuner")]
    public TextAsset tunerStateCsv;
}

[System.Serializable]
public class AllyCardSpriteSet
{
    public Sprite frame;
    public Sprite light;
    public Sprite bg;
    public Color clr;
}

[System.Serializable]
public class AllySpriteSet
{
    public List<Sprite> allyIdle;
    public List<Sprite> allyMove;
    public List<Sprite> allyAttack;

    public Material material;
}

[System.Serializable]
public class AllyGunSpriteSet
{
    public List<Sprite> gun;

    public Material material;
}

[System.Serializable]
public class AllyCardCsv
{
    public TextAsset csv;
    public TextAsset nameCsv;
    public TextAsset descCsv;
}