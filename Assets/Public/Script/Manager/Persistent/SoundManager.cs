using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [Space(20)]
    [Header("<><><><><> Sound")]

    [Space(10)]
    [Header("=== Mixer")]
    [SerializeField] private AudioMixer masterAudioMixer;

    [Space(5)]
    [Header("-- Bgm")]
    [SerializeField] private AudioSource thisBaseBgmAudioSource;
    [SerializeField] private AudioSource thisExtraBgmAudioSource;
    [HideInInspector] public float bgmVolume = 0.5f;

    [Space(5)]
    [Header("-- SFX")]
    [SerializeField] private ASQueueSet thisSfxASQueueSet;
    [HideInInspector] public float sfxVolume = 0.5f;

    private Dictionary<string, AudioClip> playerAudioDict = new Dictionary<string, AudioClip>(4);

    private Dictionary<string, AudioClip> enemyAudioDict = new Dictionary<string, AudioClip>(2);
    private Dictionary<string, List<AudioClip>> enemyAttackAudioDict = new Dictionary<string, List<AudioClip>>(3);

    private Dictionary<string, AudioClip> statusAudioDict = new Dictionary<string, AudioClip>(4);
    private AudioClip explosionAudioClip;

    private Dictionary<string, List<AudioClip>> itemAudioDict = new Dictionary<string, List<AudioClip>>(2);

    private Dictionary<string, AudioClip> buildAudioDict = new Dictionary<string, AudioClip>(9);

    private Dictionary<string, AudioClip> roomAudioDict = new Dictionary<string, AudioClip>(5);

    private Dictionary<string, AudioClip> uiAudioDict = new Dictionary<string, AudioClip>(12);

    private Dictionary<string, List<AudioClip>> playerThemeAudioDict = new Dictionary<string, List<AudioClip>>();

    public static bool isPlayingBaseBGM = true;



    #region Bgm

    // TitleLobby
    public void PlayTitleLobbyBgm()
    {
        thisBaseBgmAudioSource.clip = StaticResourceManager.instance.SoundReso.titleLobbyBgm;
        thisBaseBgmAudioSource.Play();
    }

    // Stage Bgm
    public void PlayCurrentStageBgm()
    {
        thisBaseBgmAudioSource.clip = StageManager.instance.stageObjectGenerator.currentStageThemeSO.stageBgm;
        thisBaseBgmAudioSource.Play();
    }

    // Cutscene Bgm
    public void PlayCutsceneBgm(int id)
    {
        thisBaseBgmAudioSource.clip = StaticResourceManager.instance.EventReso.cutsceneAudios[id];
        thisBaseBgmAudioSource.Play();
    }

    // Enemy
    public void PlayEliteEnemyBattleBgm()
    {
        thisExtraBgmAudioSource.clip = StaticResourceManager.instance.SoundReso.eliteEnemyBattleBgm;
        thisExtraBgmAudioSource.Play();
    }
    
    public void PlayBossEnemyBattleBgm()
    {
        thisExtraBgmAudioSource.clip = StaticResourceManager.instance.SoundReso.bossEnemyBattleBgm;
        thisExtraBgmAudioSource.Play();
    }

    // Pause
    public void Pause_2D_BGM()
    {
        thisBaseBgmAudioSource.Pause();
    }

    // Cast
    public void CastBgmToExtra()
    {
        isPlayingBaseBGM = false;

        thisBaseBgmAudioSource.Pause();
        thisBaseBgmAudioSource.volume = 0f;

        thisExtraBgmAudioSource.Play();
        thisExtraBgmAudioSource.volume = 1f;
    }

    public void CastBgmToBase()
    {
        isPlayingBaseBGM = true;

        thisBaseBgmAudioSource.Play();
        thisBaseBgmAudioSource.volume = 1f;

        thisExtraBgmAudioSource.Pause();
        thisExtraBgmAudioSource.volume = 0f;
    }
    #endregion

    #region Sfx

    // Player
    public void PlayPlayerSfx(Vector2 pos, string name)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(playerAudioDict[name]);
    }

    public void PlayPlayerRandomThemeSfx(Vector2 pos, string name)
    {
        List<AudioClip> clips = playerThemeAudioDict[name];
        thisSfxASQueueSet.Get_T().PlayOneShot(clips[Random.Range(0, clips.Count - 1)]);
    }

    // Enemy
    public void PlayEnemySfx(Vector2 pos, string name)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(enemyAudioDict[name]);
    }


    // Enemy Attack
    public void PlayEnemyAttackSfxRandom(Vector2 pos, string name)
    {
        List<AudioClip> clips = enemyAttackAudioDict[name];
        thisSfxASQueueSet.Get_T().PlayOneShot(clips[Random.Range(0, clips.Count - 1)]);
    }

    // Status
    public void PlayStatusSfx(Vector2 pos, string name)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(statusAudioDict[name]);
    }

    // Explosion
    public void PlayExplosionSfx(Vector2 pos)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(explosionAudioClip);
    }

    // Item
    public void PlayItemSfxRandom(Vector2 pos, string name)
    {
        List<AudioClip> clips = itemAudioDict[name];
        thisSfxASQueueSet.Get_T().PlayOneShot(clips[Random.Range(0, clips.Count - 1)]);
    }

    // Build
    public void PlayBuildSfx(Vector2 pos, string name)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(buildAudioDict[name]);
    }

    // Room
    public void PlayRoomSfx(Vector2 pos, string name)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(roomAudioDict[name]);
    }

    // UI
    public void PlayUiSfx(string name)
    {
        thisSfxASQueueSet.Get_T().PlayOneShot(uiAudioDict[name]);
    }

    #endregion


    #region Volume

    public void SetMasterVolume(float startV, float targetV, float durTime)
    {
        float v = startV;
        DOTween.To(() => v, _v => v = _v, targetV, durTime)
            .OnStart(() => SetMasterVolume(startV))
            .OnUpdate(() => SetMasterVolume(v));
    }


    private void SetMasterVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        masterAudioMixer.SetFloat("Master", dB);
    }


    public void SetBgmVolume(float value)
    {
        bgmVolume = value;
        SaveDataManager.instance.jsonData.optionData.bgmVolume = bgmVolume;

        float dB = Mathf.Log10(Mathf.Clamp(bgmVolume, 0.0001f, 1f)) * 20f;
        masterAudioMixer.SetFloat("BGM", dB);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        SaveDataManager.instance.jsonData.optionData.sfxVolume = sfxVolume;

        float dB = Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20f;
        masterAudioMixer.SetFloat("SFX", dB);
    }

    #endregion


    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Offset();
        Debug.Log("SoundManager : Offset Complete");
    }

    public void Offset()
    {
        int i;
        string[] names;
        var reso = StaticResourceManager.instance.SoundReso;

        // Player
        var playerSounds = reso.playerSfxs;
        for (i = 0; i < playerSounds.Length; i++)
        {
            names = playerSounds[i].name.Split('_');
            playerAudioDict.Add(names[1], playerSounds[i]);
        }

        // Enemy
        var enemySounds = reso.enemySfxs;
        for (i = 0; i < enemySounds.Length; i++)
        {
            names = enemySounds[i].name.Split('_');
            enemyAudioDict.Add(names[1], enemySounds[i]);
        }

        // Enemy Attack
        var enemyAttackSounds = reso.enemyAttackSfxs;
        for (i = 0; i < enemyAttackSounds.Length; i++)
        {
            names = enemyAttackSounds[i].name.Split('_');
            string name = names[2];
            if (!enemyAttackAudioDict.ContainsKey(name))
                enemyAttackAudioDict.Add(name, new List<AudioClip>(2));
            
            enemyAttackAudioDict[name].Add(enemyAttackSounds[i]);
        }

        // Status
        var statusSounds = reso.statusSfxs;
        for (i = 0; i < statusSounds.Length; i++)
        {
            names = statusSounds[i].name.Split('_');
            statusAudioDict.Add(names[1], statusSounds[i]);
        }

        // Explosion
        explosionAudioClip = reso.explosionAudioSfx;

        // Item
        var itemSounds = reso.itemSfxs;
        for (i = 0; i < itemSounds.Length; i++)
        {
            names = itemSounds[i].name.Split('_');
            string name = names[1];
            if (!itemAudioDict.ContainsKey(name))
                itemAudioDict.Add(name, new List<AudioClip>(2));

            itemAudioDict[name].Add(itemSounds[i]);
        }

        // Build
        var buildSounds = reso.buildSfxs;
        for (i = 0; i < buildSounds.Length; i++)
        {
            names = buildSounds[i].name.Split('_');
            buildAudioDict.Add(names[1], buildSounds[i]);
        }

        // Room
        var roomSounds = reso.roomSfxs;
        for (i = 0; i < roomSounds.Length; i++)
        {
            names = roomSounds[i].name.Split('_');
            roomAudioDict.Add(names[1], roomSounds[i]);
        }

        // UI
        var uiSounds = reso.uiSfxs;
        for (i = 0; i < uiSounds.Length; i++)
        {
            names = uiSounds[i].name.Split('_');
            uiAudioDict.Add(names[1], uiSounds[i]);
        }


        thisBaseBgmAudioSource.volume = 1f;
        thisBaseBgmAudioSource.Play();
        thisExtraBgmAudioSource.volume = 0f;
        thisExtraBgmAudioSource.Pause();

        thisSfxASQueueSet.Offset();
    }

    public void SetPlayerThemeDict(PlayerThemeSO playerTheme)
    {
        playerThemeAudioDict.Clear();

        string[] names;
        for (int i = 0; i < playerTheme.audios.Length; i++)
        {
            names = playerTheme.audios[i].name.Split('_');
            string name = names[1];
            if (!playerThemeAudioDict.ContainsKey(name))
                playerThemeAudioDict.Add(name, new List<AudioClip>());

            playerThemeAudioDict[name].Add(playerTheme.audios[i]);
        }
    }



}
