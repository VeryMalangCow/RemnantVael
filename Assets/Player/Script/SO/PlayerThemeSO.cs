using UnityEngine;

[CreateAssetMenu(fileName = "PlayerThemeSO", menuName = "ScriptableObject/PlayerThemeSO")]
public class PlayerThemeSO : ScriptableObject
{
    [Header("=== Data")]
    public int id;

    [Header("=== Prefab")]
    public PlayerController player;

    [Space(30)]
    [Header("=== Audio")]
    public AudioClip[] audios;

    [Space(30)]
    [Header("=== CSV")]
    public TextAsset nameCsv;
    public TextAsset skillNameCsv;
    public TextAsset skillDescCsv;

    public string[] GetNames()
        => CSVReader.GetCsvLine(nameCsv.text, 1);

    public string[] GetSkillNames(int index)
        => CSVReader.GetCsvLine(skillNameCsv.text, index + 1);

    public string[][] GetSkillDescs(int index)
    {
        string[][] result = new string[PlayerManager.skillAmount][];
        for (int i = 0; i < PlayerManager.skillAmount; i++)
        {
            result[i] = CSVReader.GetCsvLine(skillDescCsv.text, (PlayerManager.skillAmount * index) + (i + 1));

        }
        return result;
    }
}
