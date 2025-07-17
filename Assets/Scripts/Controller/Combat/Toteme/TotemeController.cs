using UnityEngine.Rendering.Universal;
using UnityEngine;

public abstract class TotemeController : DroppingDepthController
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

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        DevTool.Add_InList(LayerOrderManager.Instance.NeedSortingObjects, this);
    }

    protected void OnDisable()
    {
        DevTool.Remove_InList(LayerOrderManager.Instance.NeedSortingObjects, this);
    }

    #endregion

    #region Reset

    public void Reset_State()
    {
        Reset_BaseToteme();
    }

    private void Reset_BaseToteme()
    {
        transform.position = new Vector3(1000, 0, 0);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

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
        SetOff_Trail();


    }

    #endregion

    #region Sorting Order

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        ThisTrail.sortingOrder = _SortingOrder - 1;
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

    #region Remove

    protected abstract void Remove_Condition();

    protected void Remove_Object()
    {
        SetOff_Trail();
        SetOff_Light();

        Remove_Condition();
        Reset_State();

        this.gameObject.SetActive(false);
    }

    #endregion
}
