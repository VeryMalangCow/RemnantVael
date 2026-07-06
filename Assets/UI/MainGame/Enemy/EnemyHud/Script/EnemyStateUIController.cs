using UnityEngine;

public class EnemyStateUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State")]

    [Space(10)]
    [Header("=== Gage")]
    [SerializeField] public ProgressBarEUIController hpProgressBar;
    [SerializeField] public ProgressBarEUIController spProgressBar;
    [SerializeField] public ProgressBarEUIController epProgressBar;

    [HideInInspector] public EnemyHUDController enemyHud;

    #endregion

    #region Offset

    public void Offset(EnemyHUDController enemyHud)
    {
        this.enemyHud = enemyHud;

        hpProgressBar.Offset();
        spProgressBar.Offset();
        epProgressBar.Offset();
    }

    #endregion
}
