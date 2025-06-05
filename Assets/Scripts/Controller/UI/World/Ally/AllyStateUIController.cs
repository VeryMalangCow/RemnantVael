using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyStateUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State")]

    [Space(10)]
    [Header("=== Gage")]
    [SerializeField] public ProgressBarEUIController HP_ProgressBar;
    [SerializeField] public ProgressBarEUIController SP_ProgressBar;
    [SerializeField] public ProgressBarEUIController EP_ProgressBar;

    [HideInInspector] public AllyHUDController AllyHUD;

    #endregion

    #region Offset

    public void Offset(AllyHUDController _EnemyHUD)
    {
        AllyHUD = _EnemyHUD;

        HP_ProgressBar.Offset();
        SP_ProgressBar.Offset();
        EP_ProgressBar.Offset();
    }

    #endregion
}
