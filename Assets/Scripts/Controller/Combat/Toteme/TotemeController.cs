using UnityEngine.Rendering.Universal;
using UnityEngine;

public class TotemeController : DroppingDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Toteme")]
    [SerializeField] protected string PoolingString = "";

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected TrailRenderer ThisTrail;
    [SerializeField] protected Light2D ThisLight;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region State

    
    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();
        SetOn_Light();

        base.SetOn_State();
    }

    #endregion

    #region Active

    protected override void Active()
    {
        ThisTrail.enabled = false;
    }

    #endregion

    #region Light

    protected virtual void SetOn_Light()
    {

    }

    private void SetOff_Light()
    {

    }

    #endregion

    #region Trail

    protected virtual void SetOn_Trail()
    {
        ThisTrail.Clear();

        ThisTrail.emitting = true;
        ThisTrail.enabled = true;
    }

    private void SetOff_Trail()
    {
        ThisTrail.emitting = false;
        ThisTrail.enabled = false;
    }

    #endregion
}
