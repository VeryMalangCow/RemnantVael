using UnityEngine;

public class AllyBuffManager : Singleton<AllyBuffManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Buff Value")]

    [Space(5)]
    [Header("-- Sync")]

    [Header("* Dmg")]
    [SerializeField] public OriginalAllyBuff Sync005_OriginalBuff;
    [SerializeField] public OriginalAllyBuff Sync008_OriginalBuff;

    [Header("* Rof")]
    [SerializeField] public OriginalAllyBuff Sync007_OriginalBuff;

    [Header("* CD")]
    [SerializeField] public OriginalAllyBuff Sync006_OriginalBuff;


    #endregion

    #region Offset



    #endregion

}
