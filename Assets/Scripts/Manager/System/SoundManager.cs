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
    [SerializeField] private AudioMixer MasterAudioMixer;

    [Space(5)]
    [Header("-- BGM")]
    [SerializeField] private AudioSource ThisBaseBgmAudioSource;
    [SerializeField] private AudioSource ThisExtraBgmAudioSource;

    [Space(5)]
    [Header("-- SFX")]
    [SerializeField] private ASQueueSet ThisSfxASQueueSet;

    #endregion

    #region - Hide

    [HideInInspector] public float BVolume = 0.5f;
    [HideInInspector] public float SVolume = 0.5f;

    [HideInInspector] private Dictionary<string, AudioClip> BGMAudioDict = new Dictionary<string, AudioClip>();
    [HideInInspector] private Dictionary<string, AudioClip> SFXAudioDict = new Dictionary<string, AudioClip>();

    [HideInInspector] private static readonly int CutsceneSoundAmount = 1;

    [HideInInspector] private Sequence BgmCastingSeq;
    [HideInInspector] public static bool IsPlayingBaseBGM = true;

    #endregion

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Offset();
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

        for (int i = 0; i < ResourceManager.KindOfMapAmount; i++)
            Add_BGM(stagePath, $"Stage_{DevTool.Get_LengthString(i, 2)}");


        string battlePath = bgmPath + "Battle/";

        Add_BGM(battlePath, "Elite");
        Add_BGM(battlePath, "Boss");

        #endregion

        #region Cutscene

        string cutscenePath = bgmPath + "Cutscene/";

        for (int i = 0; i < CutsceneSoundAmount; i++)
            Add_BGM(cutscenePath, $"Cutscene_{DevTool.Get_LengthString(i, 3)}");

        #endregion

        ThisBaseBgmAudioSource.volume = 1f;
        ThisBaseBgmAudioSource.Play();
        ThisExtraBgmAudioSource.volume = 0f;
        ThisExtraBgmAudioSource.Pause();

        #endregion

        #region Comp

        ThisSfxASQueueSet.Offset();

        #endregion
    }


    void Add_SFX(string _Path, string _Name) => SFXAudioDict.Add(_Name, Resources.Load<AudioClip>(_Path + _Name));
    void Add_BGM(string _Path, string _Name) => BGMAudioDict.Add(_Name, Resources.Load<AudioClip>(_Path + _Name));


    #endregion

    #region Play (SFX)

    #region Module

    private void Play_2D_SFX(AudioSource _AudioSource, string _ClipName)
    {
        _AudioSource.PlayOneShot(SFXAudioDict[_ClipName]);
    }

    private void Play_2D_SFX_Random(AudioSource _AudioSource, string _ClipName, int _Amount)
    {
        Play_2D_SFX(_AudioSource, $"{_ClipName}_{DevTool.Get_LengthString(Random.Range(0, _Amount), 2)}");
    }

    private void Play_2D_SFX(string _ClipName)
    {
        Play_2D_SFX(ThisSfxASQueueSet.Get_T(), _ClipName);
    }

    #endregion

    #region Detail

    // Player
    public void Play_2D_SFX_Player_Random(AudioSource _AudioSource, int _ID, string _Name, int _Amount)
    {
        Play_2D_SFX_Random(_AudioSource, $"Player{DevTool.Get_LengthString(_ID, 2)}_{_Name}", _Amount);
    }

    public void Play_2D_SFX_Player(string _Name)
    {
        Play_2D_SFX("Player_" + _Name);
    }
    public void Play_2D_SFX_Player(AudioSource _AudioSource, string _Name)
    {
        Play_2D_SFX(_AudioSource, "Player_" + _Name);
    }

    // Enemy
    public void Play_2D_SFX_Enemy(string _Name)
    {
        Play_2D_SFX("Enemy_" + _Name);
    }
    public void Play_2D_SFX_Enemy(AudioSource _AudioSource, string _Name)
    {
        Play_2D_SFX(_AudioSource, "Enemy_" + _Name);
    }
    public void Play_2D_SFX_EnemyAttack_Random(AudioSource _AudioSource, string _Name, int _Amount)
    {
        Play_2D_SFX_Random(_AudioSource, $"Enemy_Attack_{_Name}", _Amount);
    }

    // Combat
    public void Play_2D_SFX_Combat(AudioSource _AudioSource, string _Name)
    {
        Play_2D_SFX(_AudioSource, "Combat_" + _Name);
    }

    // Item
    public void Play_2D_SFX_Item_Random(AudioSource _AudioSource, string _Name, int _Amount)
    {
        Play_2D_SFX_Random(_AudioSource, $"Item_{_Name}", _Amount);
    }

    // UI
    public void Play_2D_SFX_UI(string _Name)
    {
        Play_2D_SFX("UI_" + _Name);
    }

    // Build
    public void Play_2D_SFX_Build(string _Name)
    {
        Play_2D_SFX("Build_" + _Name);
    }

    // Room
    public void Play_2D_SFX_Room(string _Name)
    {
        Play_2D_SFX("Room_" + _Name);
    }

    // Status
    public void Play_2D_SFX_Status(string _Name)
    {
        Play_2D_SFX("Status_" + _Name);
    }

    #endregion

    #endregion

    #region Play (BGM)

    #region Module

    public void Pause_2D_BGM()
    {
        ThisBaseBgmAudioSource.Pause();
    }

    private void Play_2D_BGM(string _ClipName)
    {
        ThisBaseBgmAudioSource.clip = BGMAudioDict[_ClipName];
        ThisBaseBgmAudioSource.Play();
    }

    public void Play_2D_ExtraBGM(string _ClipName)
    {
        ThisExtraBgmAudioSource.clip = BGMAudioDict[_ClipName];
        ThisExtraBgmAudioSource.Play();
    }

    #endregion

    #region Cast

    public void CastBGM_ToExtra()
    {
        IsPlayingBaseBGM = false;

        ThisBaseBgmAudioSource.Pause();
        ThisBaseBgmAudioSource.volume = 0f;

        ThisExtraBgmAudioSource.Play(); 
        ThisExtraBgmAudioSource.volume = 1f;
    }

    public void CastBGM_ToBase()
    {
        IsPlayingBaseBGM = true;

        ThisBaseBgmAudioSource.Play();
        ThisBaseBgmAudioSource.volume = 1f;

        ThisExtraBgmAudioSource.Pause();
        ThisExtraBgmAudioSource.volume = 0f;
    }

    #endregion

    #region Element

    public void Play_2D_BGM_Title()
    {
        Play_2D_BGM("TitleLobby");
    }

    public void Play_2D_BGM_Stage(int _ID)
    {
        Play_2D_BGM($"Stage_{DevTool.Get_LengthString(_ID, 2)}");
    }

    public void Play_2D_BGM_Cutscene(int _ID)
    {
        Play_2D_BGM($"Cutscene_{DevTool.Get_LengthString(_ID, 3)}");
    }

    #endregion

    #endregion

    #region Set

    private void Set_MasterVolume(float _Value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(_Value, 0.0001f, 1f)) * 20f;
        MasterAudioMixer.SetFloat("Master", dB);
    }

    public void Set_MasterVolume(float _StartV, float _TargetV, float _DurTime)
    {
        float v = _StartV;
        DOTween.To(() => v, _v => v = _v, _TargetV, _DurTime)
            .OnStart(() => Set_MasterVolume(_StartV))
            .OnUpdate(() => Set_MasterVolume(v));
    }

    public void Set_BgmVolume(float _Value)
    {
        BVolume = _Value;
        SaveDataManager.Instance.JsonData.OptionData.BGMVolume = BVolume;

        float dB = Mathf.Log10(Mathf.Clamp(BVolume, 0.0001f, 1f)) * 20f;
        MasterAudioMixer.SetFloat("BGM", dB);
    }

    public void Set_SfxVolume(float _Value)
    {
        SVolume = _Value;
        SaveDataManager.Instance.JsonData.OptionData.SFXVolume = SVolume;

        float dB = Mathf.Log10(Mathf.Clamp(SVolume, 0.0001f, 1f)) * 20f;
        MasterAudioMixer.SetFloat("SFX", dB);
    }

    #endregion

}
