using UnityEngine;
using UnityEngine.Serialization;

public class EliteEnemyController : EnemyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Elite")]

    [Space(10)]
    [Header("=== Data")]
    [FormerlySerializedAs("NameID")][SerializeField] private int nameId;
    public int GetNameID => nameId;

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("PanelRT")][SerializeField] public RectTransform panelRt;

    [Space(10)]
    [Header("=== Item")]
    [FormerlySerializedAs("CoreDropItemPercent")][SerializeField] public CoreDropItemPercent coreDropItemPercent;

    [Space(10)]
    [Header("=== Reso")]
    [FormerlySerializedAs("BattleProdSprite")][SerializeField] public Sprite battleProdSprite;

    #region - Hide

    [HideInInspector] private static readonly Vector2 hudBaseAnchorPos = new Vector2(-812, 290);
    [HideInInspector] private static readonly float hudIntervalY = 80f;

    [HideInInspector] public static bool isDroppedBossKeycard = false;

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        EnemyManager.instance.Add_EliteEnemy(this);

        // Pattern
        Start_PatternFromNone();
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        hud.canvas.worldCamera = MainGameUIManager.instance.uiCamera;
    }

    #endregion

    #region HUD

    public void Set_HUDPanelPos(int index)
    {
        panelRt.anchoredPosition = new Vector2(hudBaseAnchorPos.x, hudBaseAnchorPos.y + (hudIntervalY * index));
    }

    #endregion

    #region Die

    protected override void Set_Die_GenItem()
    {
        base.Set_Die_GenItem();

        if (!isDroppedBossKeycard)
        {
            Gen_KeycardItem(0); // Boss Keycard
            isDroppedBossKeycard = true;
        }

        if (coreDropItemPercent.coreItemPercent != 0 && DevTool.Is_ChanceSuccess(coreDropItemPercent.coreItemPercent))
        {
            Gen_CoreItem(coreDropItemPercent.coreItemID);
        }
    }

    protected override void Set_Die_Extra()
    {
        EnemyManager.instance.Remove_EliteEnemy(this);

        PoolingManager.instance.Set_EnqueueEliteEnemy(this);

        AllyRequestManager.instance.Play_KillEliteEnemy();
    }

    #endregion
}
