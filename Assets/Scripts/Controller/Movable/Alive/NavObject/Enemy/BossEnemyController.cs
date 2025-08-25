using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemyController : EnemyController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Boss")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public RectTransform PanelRT;
    [SerializeField] private Image ThisHUDIcon;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private List<BossPhaseData> BossPhaseData;

    #endregion

    #region - Hide

    [HideInInspector] private static readonly Vector2 HUDBaseAnchorPos = new Vector2(-40, 290);
    [HideInInspector] private int CurrentPhase = 0;

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        EnemyManager.Instance.SetOn_BossEnemy(this);
        CurrentPhase = 0;
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

    public void Set_HUDPanelPos()
    {
        PanelRT.anchoredPosition = HUDBaseAnchorPos;
    }

    #endregion

    #region Die

    protected override void Set_Die_Extra()
    {
        EnemyManager.Instance.SetOff_BossEnemy();

        PoolingManager.Instance.Set_EnqueueBossEnemy(this);

        Destroy(this.gameObject);
    }

    #endregion
}
