using UnityEngine;

public class EliteEnemyController : EnemyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Elite")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public RectTransform PanelRT;

    #region - Hide

    [HideInInspector] private static readonly Vector2 HUDBaseAnchorPos = new Vector2(-812, 300);
    
    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        HUD.ThisCanvas.worldCamera = MainGameUIManager.Instance.UICamera;
        PanelRT.anchoredPosition = HUDBaseAnchorPos;
    }

    #endregion


    #region Die

    protected override void Set_Die_Enqueue()
    {
        PoolingManager.Instance.Set_EnqueueEliteEnemy(this);
    }

    #endregion
}
