using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundManager : PersistentSingleton<SoundManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Sound")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private AudioMixer MasterAudioMixer;

    [SerializeField] private AudioSource ThisBgmAudioSource;
    [SerializeField] private ASQueueSet ThisSfxASQueueSet;

    #endregion

    #region - Hide

    [HideInInspector] public float BVolume = 0.5f;
    [HideInInspector] public float SVolume = 0.5f;

    [HideInInspector] private Dictionary<string, AudioClip> BGMAudioDict = new Dictionary<string, AudioClip>();
    [HideInInspector] private Dictionary<string, AudioClip> SFXAudioDict = new Dictionary<string, AudioClip>();

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

        SFXAudioDict.Add("Player_Dash", Resources.Load<AudioClip>(playerPath + "Player_Dash"));
        SFXAudioDict.Add("Player_Avoid", Resources.Load<AudioClip>(playerPath + "Player_Avoid"));
        SFXAudioDict.Add("Player_Hitted", Resources.Load<AudioClip>(playerPath + "Player_Hitted"));
        SFXAudioDict.Add("Player_Killed", Resources.Load<AudioClip>(playerPath + "Player_Killed"));

        #region Player00

        string player00Path = playerPath + "Player00/";

        // Shot Type Amount
        for (int i = 0; i < 2; i++)
        {
            string name = $"Player00_Shot_{DevTool.Get_LengthString(i, 2)}";
            SFXAudioDict.Add(name, Resources.Load<AudioClip>(player00Path + name));
        }

        // Missile Shot Type Amount
        for (int i = 0; i < 2; i++)
        {
            string name = $"Player00_MShot_{DevTool.Get_LengthString(i, 2)}";
            SFXAudioDict.Add(name, Resources.Load<AudioClip>(player00Path + name));
        }

        #endregion

        #endregion

        #region Enemy

        string enemyPath = sfxPath + "Enemy/";

        SFXAudioDict.Add("Enemy_Hitted", Resources.Load<AudioClip>(enemyPath + "Enemy_Hitted"));
        SFXAudioDict.Add("Enemy_Killed", Resources.Load<AudioClip>(enemyPath + "Enemy_Killed"));

        #endregion

        #region Combat

        string explosionPath = sfxPath + "Combat/";

        SFXAudioDict.Add("Combat_Explosion", Resources.Load<AudioClip>(explosionPath + "Combat_Explosion"));

        #endregion

        #region Status

        string statusPath = sfxPath + "Status/";

        SFXAudioDict.Add("Status_Fire", Resources.Load<AudioClip>(statusPath + "Status_Fire"));
        SFXAudioDict.Add("Status_Cold", Resources.Load<AudioClip>(statusPath + "Status_Cold"));
        SFXAudioDict.Add("Status_Electricity", Resources.Load<AudioClip>(statusPath + "Status_Electricity"));
        SFXAudioDict.Add("Status_Corrosion", Resources.Load<AudioClip>(statusPath + "Status_Corrosion"));



        #endregion

        #region Build

        string buildPath = sfxPath + "Build/";

        // Gate
        SFXAudioDict.Add("Build_EnterGate", Resources.Load<AudioClip>(buildPath + "Build_EnterGate"));
        SFXAudioDict.Add("Build_UseKeycard", Resources.Load<AudioClip>(buildPath + "Build_UseKeycard"));

        // Interact - Shop
        SFXAudioDict.Add("Build_PowerOn", Resources.Load<AudioClip>(buildPath + "Build_PowerOn"));
        SFXAudioDict.Add("Build_Damaged", Resources.Load<AudioClip>(buildPath + "Build_Damaged"));

        // Interact - Operator
        SFXAudioDict.Add("Build_Repair", Resources.Load<AudioClip>(buildPath + "Build_Repair"));
        SFXAudioDict.Add("Build_Replacement", Resources.Load<AudioClip>(buildPath + "Build_Replacement"));
        SFXAudioDict.Add("Build_Enchance", Resources.Load<AudioClip>(buildPath + "Build_Enchance"));

        // Interact - Prison
        SFXAudioDict.Add("Build_PrisonUnlock", Resources.Load<AudioClip>(buildPath + "Build_PrisonUnlock"));

        #endregion

        #region Room

        string roomPath = sfxPath + "Room/";

        SFXAudioDict.Add("Room_Start_KillAll", Resources.Load<AudioClip>(roomPath + "Room_Start_KillAll"));
        SFXAudioDict.Add("Room_Start_Safe", Resources.Load<AudioClip>(roomPath + "Room_Start_Safe"));
        SFXAudioDict.Add("Room_Start_Prison", Resources.Load<AudioClip>(roomPath + "Room_Start_Prison"));

        SFXAudioDict.Add("Room_Complete_KillAll", Resources.Load<AudioClip>(roomPath + "Room_Complete_KillAll"));

        #endregion

        #region UI

        string uiPath = sfxPath + "UI/";

        SFXAudioDict.Add("UI_Click", Resources.Load<AudioClip>(uiPath + "UI_Click"));
        SFXAudioDict.Add("UI_Click_Approve", Resources.Load<AudioClip>(uiPath + "UI_Click_Approve"));
        SFXAudioDict.Add("UI_Click_Reject", Resources.Load<AudioClip>(uiPath + "UI_Click_Reject"));

        SFXAudioDict.Add("UI_Equip", Resources.Load<AudioClip>(uiPath + "UI_Equip"));
        SFXAudioDict.Add("UI_Unequip", Resources.Load<AudioClip>(uiPath + "UI_Unequip"));

        SFXAudioDict.Add("UI_Decomposition", Resources.Load<AudioClip>(uiPath + "UI_Decomposition"));
        SFXAudioDict.Add("UI_Fusion", Resources.Load<AudioClip>(uiPath + "UI_Fusion"));
        SFXAudioDict.Add("UI_Make", Resources.Load<AudioClip>(uiPath + "UI_Make"));

        SFXAudioDict.Add("UI_Reroll", Resources.Load<AudioClip>(uiPath + "UI_Reroll"));
        SFXAudioDict.Add("UI_StartBattleProd", Resources.Load<AudioClip>(uiPath + "UI_StartBattleProd"));

        #endregion

        #endregion

        #region BGM

        BGMAudioDict.Add("TitleLobby", Resources.Load<AudioClip>(bgmPath + "TitleLobby"));

        #region Main Game (Stage)

        string stagePath = bgmPath + "Stage/";

        BGMAudioDict.Add("Stage99", Resources.Load<AudioClip>(stagePath + "Stage99"));
        for (int i = 0; i < ResourceManager.KindOfMapAmount; i++)
        {
            string id = DevTool.Get_LengthString(i, 2);
            BGMAudioDict.Add($"Stage{id}", Resources.Load<AudioClip>(stagePath + $"Stage{id}"));
        }

        #endregion

        #endregion

        #region Comp

        ThisBgmAudioSource = DevTool.Get_ComponentTType<AudioSource>(gameObject.transform.GetChild(0).gameObject);
        ThisSfxASQueueSet.Offset();

        #endregion
    }

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
        Play_2D_SFX(ThisSfxASQueueSet.Get_AS(), _ClipName);
    }

    #endregion

    #region Detail

    // Player
    public void Play_2D_SFX_Player_Random(AudioSource _AudioSource, int _ID, string _Name, int _Amount)
    {
        Play_2D_SFX_Random(_AudioSource, $"Player{DevTool.Get_LengthString(_ID, 2)}_{_Name}", 2);
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

    // Combat
    public void Play_2D_SFX_Combat(AudioSource _AudioSource, string _Name)
    {
        Play_2D_SFX(_AudioSource, "Combat_" + _Name);
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

    public void Play_2D_BGM(string _ClipName)
    {
        ThisBgmAudioSource.clip = BGMAudioDict[_ClipName];
        ThisBgmAudioSource.Play();
    }
    public void Play_2D_BGM_Stage(int _ID)
    {
        Play_2D_BGM($"Stage{DevTool.Get_LengthString(_ID, 2)}");
    }

    #endregion
    
    #region Set

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
