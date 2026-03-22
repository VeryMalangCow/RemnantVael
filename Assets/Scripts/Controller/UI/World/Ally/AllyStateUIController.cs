using UnityEngine;
using UnityEngine.Serialization;

public class AllyStateUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State")]

    [Space(10)]
    [Header("=== Gage")]
    [FormerlySerializedAs("HP_ProgressBar")][SerializeField] public ProgressBarEUIController hpProgressBar;
    [FormerlySerializedAs("SP_ProgressBar")][SerializeField] public ProgressBarEUIController spProgressBar;
    [FormerlySerializedAs("EP_ProgressBar")][SerializeField] public ProgressBarEUIController epProgressBar;

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
