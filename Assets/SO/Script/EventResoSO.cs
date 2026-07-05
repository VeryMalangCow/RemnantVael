using UnityEngine;

[CreateAssetMenu(fileName = "EventResoSO", menuName = "ScriptableObject/EventResoSO")]
public class EventResoSO : ScriptableObject
{
    [Space(10)]
    public Texture2D[] cutsceneTextures;
    public Sprite[] cutsceneSprites;
    [Space(10)]
    public Texture2D[] dialogueTextures;
    public Sprite[] dialogueSprites;
    [Space(10)]
    public Texture2D[] infoTextures;
    public Sprite[] infoSprites;
}
