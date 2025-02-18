using UnityEngine;

public class EnemyHUDController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> HUD")]
    [HideInInspector] private EnemyController Enemy;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public Canvas ThisCanvas;
    [SerializeField] public EnemyStateUIController StateUI;
    [SerializeField] public EnemyBuffUIController BuffUI;

    #endregion

    #region Offset

    public void Offset(EnemyController _Enemy)
    {
        Enemy = _Enemy;

        StateUI.Offset(this);
        BuffUI.Offset(this);
    }

    #endregion
}
