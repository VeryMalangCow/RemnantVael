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

    #endregion

    #region - Hide

    [HideInInspector] public float BVolume = 0.5f;
    [HideInInspector] public float SVolume = 0.5f;

    [HideInInspector] private AudioSource ThisBgmAudioSource;
    [HideInInspector] private AudioSource ThisSfxAudioSource;

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
        string basePath = "Sound/Source/";


        string bgmPath = "BGM/";

        string stagePath = "Stage/";


        string sfxPath = "SFX/";

        string playerPath = "Player/";
        string enemyPath = "Enemy/";


        SFXAudioDict.Add("Player_Hitted", Resources.Load<AudioClip>(basePath + sfxPath + playerPath + "Player_Hitted"));
        SFXAudioDict.Add("Player00_Shot", Resources.Load<AudioClip>(basePath + sfxPath + playerPath + "Player00/Player00_Shot"));

        SFXAudioDict.Add("Enemy_Hitted", Resources.Load<AudioClip>(basePath + sfxPath + enemyPath + "Enemy_Hitted"));

        BGMAudioDict.Add("Stage99_BGM", Resources.Load<AudioClip>(basePath + bgmPath + stagePath + "Stage99_BGM"));
        for (int i = 0; i < ResourceManager.KindOfMapAmount; i++)
        {
            BGMAudioDict.Add($"Stage{DevTool.Get_LengthString(i, 2)}_BGM", Resources.Load<AudioClip>(basePath + bgmPath + stagePath + $"Stage{DevTool.Get_LengthString(i, 2)}_BGM"));
        }

        ThisBgmAudioSource = DevTool.Get_ComponentTType<AudioSource>(gameObject.transform.GetChild(0).gameObject);
        ThisSfxAudioSource = DevTool.Get_ComponentTType<AudioSource>(gameObject.transform.GetChild(1).gameObject);
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

    public void Play_2D_SFX(string _ClipName)
    {
        Play_2D_SFX(ThisSfxAudioSource, _ClipName);
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
