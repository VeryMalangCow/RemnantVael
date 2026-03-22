using UnityEngine;
using UnityEngine.Serialization;

public class EnemyStateUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State")]

    [Space(10)]
    [Header("=== Gage")]
    [FormerlySerializedAs("HP_ProgressBar")][SerializeField] public ProgressBarEUIController hpProgressBar;
    [FormerlySerializedAs("SP_ProgressBar")][SerializeField] public ProgressBarEUIController spProgressBar;
    [FormerlySerializedAs("EP_ProgressBar")][SerializeField] public ProgressBarEUIController epProgressBar;

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
