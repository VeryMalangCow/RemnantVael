using UnityEngine;

public class EnemyStateUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State")]

    [Space(10)]
    [Header("=== Gage")]
    [SerializeField] public ModifyReductionFocusProgressBar HP_ProgressBar;
    [SerializeField] public ModifyReductionFocusProgressBar SP_ProgressBar;
    [SerializeField] public ModifyReductionFocusProgressBar EP_ProgressBar;

    [HideInInspector] public EnemyHUDController EnemyHUD;

    #endregion

    #region Offset

    public void Offset(EnemyHUDController _EnemyHUD)
    {
        EnemyHUD = _EnemyHUD;

        HP_ProgressBar.Offset();
        SP_ProgressBar.Offset();
        EP_ProgressBar.Offset();
    }

    #endregion
}
