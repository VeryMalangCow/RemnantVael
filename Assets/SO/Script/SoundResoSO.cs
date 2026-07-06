using UnityEngine;

[CreateAssetMenu(fileName = "SoundResoSO", menuName = "ScriptableObject/SoundResoSO")]
public class SoundResoSO : ScriptableObject
{
    public AudioClip titleLobbyBgm;
    public AudioClip eliteEnemyBattleBgm;
    public AudioClip bossEnemyBattleBgm;

    [Space(30)]
    public AudioClip[] playerSfxs;

    public AudioClip[] enemySfxs;
    public AudioClip[] enemyAttackSfxs;

    public AudioClip explosionAudioSfx;
    public AudioClip[] statusSfxs;

    public AudioClip[] itemSfxs;

    public AudioClip[] buildSfxs;

    public AudioClip[] roomSfxs;

    public AudioClip[] uiSfxs;
}
