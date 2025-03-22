using UnityEngine;

public class EnemyHUDController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> HUD")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public Canvas ThisCanvas;
    [SerializeField] public EnemyStateUIController StateUI;
    [SerializeField] public EnemyBuffUIController TemporaryBuffUI;
    [SerializeField] public EnemyBuffUIController PermanentBuffUI;

    #endregion

    #region Offset

    public void Offset(EnemyController _Enemy)
    {
        StateUI.Offset(this);
        TemporaryBuffUI.Offset(this);
        PermanentBuffUI.Offset(this);
    }

    #endregion
}
