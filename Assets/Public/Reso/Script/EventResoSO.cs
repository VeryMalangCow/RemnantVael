using UnityEngine;

[CreateAssetMenu(fileName = "EventResoSO", menuName = "ScriptableObject/EventResoSO")]
public class EventResoSO : ScriptableObject
{
    [Space(10)]
    [Header("=== Event")]
    public TextAsset eventIdCsv;
    public TextAsset eventElementCsv;


    [Space(10)]
    [Header("=== Cutscene")]
    public Texture2D[] cutsceneTextures;
    public Sprite[] cutsceneSprites;

    public AudioClip[] cutsceneAudios;

    public TextAsset cutsceneIdCsv;
    public TextAsset[] cutsceneElementCsvs;

    [Space(10)]
    [Header("=== Dialogue")]
    public Texture2D[] dialogueTextures;
    public Sprite[] dialogueSprites;

    public TextAsset dialogueIdCsv;
    public TextAsset[] dialogueElementCsvs;


    [Space(10)]
    [Header("=== Info")]
    public Texture2D[] infoTextures;
    public Sprite[] infoSprites;
    [Space(10)]
    public TextAsset infoNameCsv;
    public TextAsset infoDescCsv;
    public TextAsset infoKeyCsv;
}
