using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllyPrefabSO", menuName = "ScriptableObject/AllyPrefabSO")]
public class AllyPrefabSO : ScriptableObject
{
    [Header("=== Ally Card")]
    public SerializableArray<Texture2D>[] allyCardTextures;
    public SerializableArray<Sprite>[] allyCardIcons;

    public AllyCardSpriteSet[] allyCardSpriteSets;
    public Sprite allyNullIcon;

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
}

[System.Serializable]
public class AllyCardSpriteSet
{
    public Sprite frame;
    public Sprite light;
    public Sprite bg;
    public Color clr;
}

