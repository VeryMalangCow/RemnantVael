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
    [SerializeField] public OriginalAllyBuff sync005_OriginalBuff;
    [SerializeField] public OriginalAllyBuff sync008_OriginalBuff;
    [SerializeField] public OriginalAllyBuff totisToteme_OriginalBuff;

    [Header("* Rof")]
    [SerializeField] public OriginalAllyBuff sync007_OriginalBuff;

    [Header("* CD")]
    [SerializeField] public OriginalAllyBuff sync006_OriginalBuff;


    #endregion

    #region Offset



    #endregion

}
