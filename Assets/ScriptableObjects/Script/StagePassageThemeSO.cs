using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StagePassageThemeSO", menuName = "ScriptableObject/StagePassageThemeSO")]
public class StagePassageThemeSO : ScriptableObject
{
    public int beforeStageId;
    public int afterStageId;
    [Space(10)]
    public Material mapMaterial;
    public Texture2D passageTexture;
    public List<Sprite> sprites;
}
