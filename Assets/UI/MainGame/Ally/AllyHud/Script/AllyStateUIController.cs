using UnityEngine;

public class AllyStateUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State")]

    [Space(10)]
    [Header("=== Gage")]
    [SerializeField] public ProgressBarEUIController hpProgressBar;
    [SerializeField] public ProgressBarEUIController spProgressBar;
    [SerializeField] public ProgressBarEUIController epProgressBar;

    [HideInInspector] public AllyHUDController allyHud;

    #endregion

    #region Offset

    public void Offset(AllyHUDController enemyHud)
    {
        allyHud = enemyHud;

        hpProgressBar.Offset();
        spProgressBar.Offset();
        epProgressBar.Offset();
    }

    #endregion
}
