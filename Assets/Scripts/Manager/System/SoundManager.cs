using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : PersistentSingleton<SoundManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Sound")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private AudioMixer masterAudioMixer;

    [Space(5)]
    [Header("-- BGM")]
    [SerializeField] private AudioSource thisBaseBgmAudioSource;
    [SerializeField] private AudioSource thisExtraBgmAudioSource;

    [Space(5)]
    [Header("-- SFX")]
    [SerializeField] private ASQueueSet thisSfxASQueueSet;

    #endregion

    #region - Hide

    [HideInInspector] public float bgmVolume = 0.5f;
    [HideInInspector] public float sfxVolume = 0.5f;

    [HideInInspector] private Dictionary<string, AudioClip> bgmAudioDict = new Dictionary<string, AudioClip>();
    [HideInInspector] private Dictionary<string, AudioClip> sfxAudioDict = new Dictionary<string, AudioClip>();

    [HideInInspector] private static readonly int cutsceneSoundAmount = 1;

    [HideInInspector] private Sequence bgmCastingSeq;
    [HideInInspector] public static bool isPlayingBaseBGM = true;

    #endregion

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Offset();
        Debug.Log("SoundManager : Offset Complete");
    }

    #endregion

    #region Offset

    private void Offset()
    {
        #region Base Path

        string basePath = "Sound/Source/";
        string bgmPath = basePath + "BGM/";
        string sfxPath = basePath + "SFX/";

        #endregion

        #region SFX

        #region Player

        string playerPath = sfxPath + "Player/";

        Add_SFX(playerPath, "Player_Dash");
        Add_SFX(playerPath, "Player_Avoid");
        Add_SFX(playerPath, "Player_Hitted");
        Add_SFX(playerPath, "Player_Killed");

        #region Player00

        string player00Path = playerPath + "Player00/";

        // Shot Type Amount
        for (int i = 0; i < 2; i++)
            Add_SFX(player00Path, $"Player00_Shot_{DevTool.Get_LengthString(i, 2)}");
        
        // Missile Shot Type Amount
        for (int i = 0; i < 2; i++)
            Add_SFX(player00Path, $"Player00_MShot_{DevTool.Get_LengthString(i, 2)}");
        
        #endregion

        #endregion

        #region Enemy

        string enemyPath = sfxPath + "Enemy/";

        Add_SFX(enemyPath, "Enemy_Hitted");
        Add_SFX(enemyPath, "Enemy_Killed");

        // Attack
        for (int i = 0; i < 2; i++)
        {
            string index = DevTool.Get_LengthString(i, 2);

            Add_SFX(enemyPath, $"Enemy_Attack_Sword_{index}");
            Add_SFX(enemyPath, $"Enemy_Attack_Bullet_{index}");
            Add_SFX(enemyPath, $"Enemy_Attack_Thrust_{index}");
        }

        #endregion

        #region Combat

        string explosionPath = sfxPath + "Combat/";

        Add_SFX(explosionPath, "Combat_Explosion");

        #endregion

        #region Status

        string statusPath = sfxPath + "Status/";

        Add_SFX(statusPath, "Status_Fire");
        Add_SFX(statusPath, "Status_Cold");
        Add_SFX(statusPath, "Status_Electricity");
        Add_SFX(statusPath, "Status_Corrosion");

        #endregion

        #region Build

        string buildPath = sfxPath + "Build/";

        // Gate
        Add_SFX(buildPath, "Build_EnterGate");
        Add_SFX(buildPath, "Build_UseKeycard");

        Add_SFX(buildPath, "Build_PowerOn");
        Add_SFX(buildPath, "Build_Damaged");

        Add_SFX(buildPath, "Build_Repair");
        Add_SFX(buildPath, "Build_Replacement");
        Add_SFX(buildPath, "Build_Enchance");

        Add_SFX(buildPath, "Build_PrisonUnlock");

        Add_SFX(buildPath, "Build_BreakFieldObj");

        #endregion

        #region Room

        string roomPath = sfxPath + "Room/";

        Add_SFX(roomPath, "Room_Start_KillAll");
        Add_SFX(roomPath, "Room_Start_Safe");
        Add_SFX(roomPath, "Room_Start_Prison");

        Add_SFX(roomPath, "Room_Complete_KillAll");

        Add_SFX(roomPath, "Room_BattleWin");

        #endregion

        #region Item

        string itemPath = sfxPath + "Item/";

        // Absorb
        for (int i = 0; i < 2; i++)
            Add_SFX(itemPath, $"Item_Absorb_{DevTool.Get_LengthString(i, 2)}");
        
        // Interact
        for (int i = 0; i < 2; i++)
            Add_SFX(itemPath, $"Item_Interact_{DevTool.Get_LengthString(i, 2)}");

        #endregion

        #region UI

        string uiPath = sfxPath + "UI/";

        // Click
        for (int i = 0; i < 2; i++)
            Add_SFX(uiPath, $"UI_Click_{DevTool.Get_LengthString(i, 2)}");


        Add_SFX(uiPath, $"UI_Click_Approve");
        Add_SFX(uiPath, $"UI_Click_Reject");

        Add_SFX(uiPath, $"UI_Equip");
        Add_SFX(uiPath, $"UI_Unequip");

        Add_SFX(uiPath, $"UI_Decomposition");
        Add_SFX(uiPath, $"UI_Fusion");
        Add_SFX(uiPath, $"UI_Make");

        Add_SFX(uiPath, $"UI_Reroll");

        Add_SFX(uiPath, $"UI_StartBattleProd");
        Add_SFX(uiPath, $"UI_StartBossBattleProd");

        #endregion

        #endregion

        #region BGM

        Add_BGM(bgmPath, "TitleLobby");

        #region Main Game

        string stagePath = bgmPath + "Stage/";

        Add_BGM(stagePath, "Stage_99");

        for (int i = 0; i < ResourceManager.kindOfMapAmount; i++)
            Add_BGM(stagePath, $"Stage_{DevTool.Get_LengthString(i, 2)}");


        string battlePath = bgmPath + "Battle/";

        Add_BGM(battlePath, "Elite");
        Add_BGM(battlePath, "Boss");

        #endregion

        #region Cutscene

        string cutscenePath = bgmPath + "Cutscene/";

        for (int i = 0; i < cutsceneSoundAmount; i++)
            Add_BGM(cutscenePath, $"Cutscene_{DevTool.Get_LengthString(i, 3)}");

        #endregion

        thisBaseBgmAudioSource.volume = 1f;
        thisBaseBgmAudioSource.Play();
        thisExtraBgmAudioSource.volume = 0f;
        thisExtraBgmAudioSource.Pause();

        #endregion

        #region Comp

        thisSfxASQueueSet.Offset();

        #endregion
    }


    void Add_SFX(string path, string name) => sfxAudioDict.Add(name, Resources.Load<AudioClip>(path + name));
    void Add_BGM(string path, string name) => bgmAudioDict.Add(name, Resources.Load<AudioClip>(path + name));


    #endregion

    #region Play (SFX)

    #region Module

    private void Play_2D_SFX(AudioSource audioSource, string clipName)
    {
        audioSource.PlayOneShot(sfxAudioDict[clipName]);
    }

    private void Play_2D_SFX_Random(AudioSource audioSource, string clipName, int amount)
    {
        Play_2D_SFX(audioSource, $"{clipName}_{DevTool.Get_LengthString(Random.Range(0, amount), 2)}");
    }

    private void Play_2D_SFX(string clipName)
    {
        Play_2D_SFX(thisSfxASQueueSet.Get_T(), clipName);
    }

    #endregion

    #region Detail

    // Player
    public void Play_2D_SFX_Player_Random(AudioSource audioSource, int id, string name, int amount)
    {
        Play_2D_SFX_Random(audioSource, $"Player{DevTool.Get_LengthString(id, 2)}_{name}", amount);
    }

    public void Play_2D_SFX_Player(string name)
    {
        Play_2D_SFX("Player_" + name);
    }
    public void Play_2D_SFX_Player(AudioSource audioSource, string name)
    {
        Play_2D_SFX(audioSource, "Player_" + name);
    }

    // Enemy
    public void Play_2D_SFX_Enemy(string name)
    {
        Play_2D_SFX("Enemy_" + name);
    }
    public void Play_2D_SFX_Enemy(AudioSource audioSource, string name)
    {
        Play_2D_SFX(audioSource, "Enemy_" + name);
    }
    public void Play_2D_SFX_EnemyAttack_Random(AudioSource audioSource, string name, int amount)
    {
        Play_2D_SFX_Random(audioSource, $"Enemy_Attack_{name}", amount);
    }

    // Combat
    public void Play_2D_SFX_Combat(AudioSource audioSource, string name)
    {
        Play_2D_SFX(audioSource, "Combat_" + name);
    }

    // Item
    public void Play_2D_SFX_Item_Random(AudioSource audioSource, string name, int amount)
    {
        Play_2D_SFX_Random(audioSource, $"Item_{name}", amount);
    }

    // UI
    public void Play_2D_SFX_UI(string name)
    {
        Play_2D_SFX("UI_" + name);
    }

    // Build
    public void Play_2D_SFX_Build(string name)
    {
        Play_2D_SFX("Build_" + name);
    }

    // Room
    public void Play_2D_SFX_Room(string name)
    {
        Play_2D_SFX("Room_" + name);
    }

    // Status
    public void Play_2D_SFX_Status(string name)
    {
        Play_2D_SFX("Status_" + name);
    }

    #endregion

    #endregion

    #region Play (BGM)

    #region Module

    public void Pause_2D_BGM()
    {
        thisBaseBgmAudioSource.Pause();
    }

    private void Play_2D_BGM(string clipName)
    {
        thisBaseBgmAudioSource.clip = bgmAudioDict[clipName];
        thisBaseBgmAudioSource.Play();
    }

    public void Play_2D_ExtraBGM(string clipName)
    {
        thisExtraBgmAudioSource.clip = bgmAudioDict[clipName];
        thisExtraBgmAudioSource.Play();
    }

    #endregion

    #region Cast

    public void CastBGM_ToExtra()
    {
        isPlayingBaseBGM = false;

        thisBaseBgmAudioSource.Pause();
        thisBaseBgmAudioSource.volume = 0f;

        thisExtraBgmAudioSource.Play(); 
        thisExtraBgmAudioSource.volume = 1f;
    }

    public void CastBGM_ToBase()
    {
        isPlayingBaseBGM = true;

        thisBaseBgmAudioSource.Play();
        thisBaseBgmAudioSource.volume = 1f;

        thisExtraBgmAudioSource.Pause();
        thisExtraBgmAudioSource.volume = 0f;
    }

    #endregion

    #region Element

    public void Play_2D_BGM_Title()
    {
        Play_2D_BGM("TitleLobby");
    }

    public void Play_2D_BGM_Stage(int id)
    {
        Play_2D_BGM($"Stage_{DevTool.Get_LengthString(id, 2)}");
    }

    public void Play_2D_BGM_Cutscene(int id)
    {
        Play_2D_BGM($"Cutscene_{DevTool.Get_LengthString(id, 3)}");
    }

    #endregion

    #endregion

    #region Set

    private void Set_MasterVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        masterAudioMixer.SetFloat("Master", dB);
    }

    public void Set_MasterVolume(float startV, float targetV, float durTime)
    {
        float v = startV;
        DOTween.To(() => v, _v => v = _v, targetV, durTime)
            .OnStart(() => Set_MasterVolume(startV))
            .OnUpdate(() => Set_MasterVolume(v));
    }

    public void Set_BgmVolume(float value)
    {
        bgmVolume = value;
        SaveDataManager.instance.jsonData.optionData.bgmVolume = bgmVolume;

        float dB = Mathf.Log10(Mathf.Clamp(bgmVolume, 0.0001f, 1f)) * 20f;
        masterAudioMixer.SetFloat("BGM", dB);
    }

    public void Set_SfxVolume(float value)
    {
        sfxVolume = value;
        SaveDataManager.instance.jsonData.optionData.sfxVolume = sfxVolume;

        float dB = Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20f;
        masterAudioMixer.SetFloat("SFX", dB);
    }

    #endregion

}
