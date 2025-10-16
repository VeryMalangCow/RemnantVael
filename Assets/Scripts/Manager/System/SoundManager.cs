using System.Collections.Generic;
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
        Play_2D_BGM("TitleLobby_BGM");
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

        #region Explosion

        string explosionPath = sfxPath + "Explosion/";

        SFXAudioDict.Add("Explosion", Resources.Load<AudioClip>(explosionPath + "Explosion"));

        #endregion

        #region Status

        string statusPath = sfxPath + "Status/";

        SFXAudioDict.Add("Cold_Status", Resources.Load<AudioClip>(statusPath + "Cold_Status"));
        SFXAudioDict.Add("Corrosion_Status", Resources.Load<AudioClip>(statusPath + "Corrosion_Status"));
        SFXAudioDict.Add("Electricity_Status", Resources.Load<AudioClip>(statusPath + "Electricity_Status"));
        SFXAudioDict.Add("Fire_Status", Resources.Load<AudioClip>(statusPath + "Fire_Status"));



        #endregion

        #region Build

        string buildPath = sfxPath + "Build/";

        SFXAudioDict.Add("Build_PowerOn", Resources.Load<AudioClip>(buildPath + "Build_PowerOn"));
        SFXAudioDict.Add("Build_Damaged", Resources.Load<AudioClip>(buildPath + "Build_Damaged"));

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

        #endregion

        #endregion

        #region BGM

        BGMAudioDict.Add("TitleLobby_BGM", Resources.Load<AudioClip>(bgmPath + "TitleLobby_BGM"));

        #region Main Game (Stage)

        string stagePath = bgmPath + "Stage/";

        BGMAudioDict.Add("Stage99_BGM", Resources.Load<AudioClip>(stagePath + "Stage99_BGM"));
        for (int i = 0; i < ResourceManager.KindOfMapAmount; i++)
        {
            BGMAudioDict.Add($"Stage{DevTool.Get_LengthString(i, 2)}_BGM", Resources.Load<AudioClip>(stagePath + $"Stage{DevTool.Get_LengthString(i, 2)}_BGM"));
        }

        #endregion

        #endregion

        #region Comp

        ThisBgmAudioSource = DevTool.Get_ComponentTType<AudioSource>(gameObject.transform.GetChild(0).gameObject);
        ThisSfxASQueueSet.Offset();

        #endregion
    }

    #endregion

    #region Get

    private AudioClip Get_BgmAudioClip(string _ClipName)
    {
        return BGMAudioDict[_ClipName];
    }
    private AudioClip Get_SfxAudioClip(string _ClipName)
    {
        return SFXAudioDict[_ClipName];
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

    #region Play (SFX)

    public void Play_2D_SFX(AudioSource _AudioSource, string _ClipName)
    {
        AudioClip ac = Get_SfxAudioClip(_ClipName);
        _AudioSource.PlayOneShot(ac);
    }

    public void Play_2D_SFX_Random(AudioSource _AudioSource, string _ClipName, int _Amount)
    {
        Play_2D_SFX(_AudioSource, _ClipName + DevTool.Get_LengthString(Random.Range(0, _Amount), 2));
    }

    public void Play_2D_SFX(string _ClipName)
    {
        Play_2D_SFX(ThisSfxASQueueSet.Get_AS(), _ClipName);
    }

    #endregion

    #region Play (BGM)

    public void Play_2D_BGM(string _ClipName)
    {
        AudioClip ac = Get_BgmAudioClip(_ClipName);
        ThisBgmAudioSource.clip = ac;
        ThisBgmAudioSource.Play();
    }

    #endregion
}
