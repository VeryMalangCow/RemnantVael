using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private List<EnemyBuffIconUIController> BuffIconUIs;

    [HideInInspector] public EnemyHUDController EnemyHUD;

    #endregion

    #region Offset

    public void Offset(EnemyHUDController _EnemyHUD)
    {
        EnemyHUD = _EnemyHUD;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).TryGetComponent(out EnemyBuffIconUIController EBI))
            { 
                BuffIconUIs.Add(EBI);
                EBI.Offset();
            }
        }
    }

    #endregion

    #region Get

    // 사용하지 않는 중인 버프 Icon UI
    public EnemyBuffIconUIController GetBuffIconUI()
    {
        for (int i = 0; i < BuffIconUIs.Count; i++)
        {
            if (!BuffIconUIs[i].UsingNow)
            {
                return BuffIconUIs[i];
            }
        }

        return null;
    }

    #endregion
}
