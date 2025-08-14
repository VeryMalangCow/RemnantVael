using UnityEngine;

public class EliteEnemyController : EnemyController
{
    #region Value


    #region - Hide

    [HideInInspector] private static readonly Vector2 HUDBaseAnchorPos = new Vector2(-780, 300); 


    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        HUD.ThisCanvas.worldCamera = MainGameUIManager.Instance.UICamera;
    }

    #endregion
}
