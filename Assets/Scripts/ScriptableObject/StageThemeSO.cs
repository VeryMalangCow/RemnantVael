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

    public List<Sprite> stageAllSprites;
    public SerializableArray<Sprite>[] fieldObjSprites;

}
