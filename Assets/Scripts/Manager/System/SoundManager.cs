using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : PersistentSingleton<CSVManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Sound")]

    [Space(10)]
    [Header("=== Comp")]
    /// <summary>
    /// 오디오 믹서, 오디오의 타입별로 사운드를 조절할 수 있도록 한다.
    /// </summary>
    [SerializeField] private AudioMixer mAudioMixer;

    #endregion

    #region - Hide

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

    }

    #endregion
}
