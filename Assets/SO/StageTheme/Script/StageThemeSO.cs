using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageThemeSO", menuName = "ScriptableObject/StageThemeSO")]
public class StageThemeSO : ScriptableObject
{
    [Header("=== Data")]
    public int stageId;

    [Space(30)]
    [Header("=== Material")]
    public List<Material> mapMaterialUnclear;
    public List<Material> mapMaterialClear;

    [Space(30)]
    [Header("=== Anim")]
    public List<StageDoorAnim> mapDoorAnim;

    [Space(30)]
    [Header("=== Sprite")]
    public Texture2D[] stageTextures;

    public List<SpriteMaterial> stageAllSprites;
    public SerializableArray<SpriteMaterial>[] fieldObjSprites;

    [Space(10)]
    [Header("=== Sound")]
    public AudioClip stageBgm;
}

[System.Serializable]
public class SpriteMaterial
{
    public Sprite sprite;
    public int materialIndex;

    public SpriteMaterial(Sprite sprite, int materialIndex)
    {
        this.sprite = sprite;
        this.materialIndex = materialIndex;
    }
}

[System.Serializable]
public class StageDoorAnim
{
    public Vector2Int dir;
    public AnimationClip doorAnim;
}


