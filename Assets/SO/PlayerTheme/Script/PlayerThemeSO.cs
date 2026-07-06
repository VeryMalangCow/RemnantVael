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
}
