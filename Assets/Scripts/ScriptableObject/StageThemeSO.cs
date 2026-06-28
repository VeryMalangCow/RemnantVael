using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageThemeSet", menuName = "ScriptableObject/StageThemeSet")]
public class StageThemeSO : ScriptableObject
{
    public int stageId;

    public List<Material> mapMaterialUnclear;
    public List<Material> mapMaterialClear;
    public List<StageDoorAnim> mapDoorAnim;

    public Texture2D[] stageTextures;

    public List<SpriteMaterial> stageAllSprites;
    public SerializableArray<SpriteMaterial>[] fieldObjSprites;

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
