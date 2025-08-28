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

    [HideInInspector] private static readonly Vector2 HUDBaseAnchorPos = new Vector2(-812, 290);
    [HideInInspector] private static readonly float HUDIntervalY = 80f;

    [HideInInspector] public static bool IsDroppedBossKeycard = false;

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        EnemyManager.Instance.Add_EliteEnemy(this);

        // Pattern
        Start_PatternFromNone();
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        HUD.ThisCanvas.worldCamera = MainGameUIManager.Instance.UICamera;
    }

    #endregion

    #region HUD

    public void Set_HUDPanelPos(int _Index)
    {
        PanelRT.anchoredPosition = new Vector2(HUDBaseAnchorPos.x, HUDBaseAnchorPos.y + (HUDIntervalY * _Index));
    }

    #endregion

    #region Die

    protected override void Set_Die_GenItem()
    {
        base.Set_Die_GenItem();

        if (!IsDroppedBossKeycard)
        {
            Gen_KeycardItem(0); // Boss Keycard
            IsDroppedBossKeycard = true;
        }

        if (EnemyDropItemPercent.CoreItemPercent != 0 && DevTool.Is_ChanceSuccess(EnemyDropItemPercent.CoreItemPercent))
        {
            Gen_CoreItem(EnemyDropItemPercent.CoreItemID);
        }
    }

    protected override void Set_Die_Extra()
    {
        EnemyManager.Instance.Remove_EliteEnemy(this);

        PoolingManager.Instance.Set_EnqueueEliteEnemy(this);
    }

    #endregion
}
