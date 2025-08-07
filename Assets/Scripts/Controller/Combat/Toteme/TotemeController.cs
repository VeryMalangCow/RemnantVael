using UnityEngine;
using System.Collections;

public abstract class TotemeController : DroppingDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Toteme")]
    [SerializeField] protected string PoolingString = "";

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private int PlayerBuffID;
    [SerializeField] private int AllyBuffID;

    #endregion

    #region - Hide

    #endregion

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

    public void Set_State(
        float _DroppingTime, float _TopYPos, float _BottomYPos,
        BulletState_PosAndRot _State_PosAndRot,
        BulletState_Size _State_Size)
    {
        base.Set_State_Base(null, _DroppingTime, _TopYPos, _BottomYPos);
        Set_State_PosAndRot(_State_PosAndRot);
        Set_State_ShadowSize(_State_Size);

        SetOn_State();
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot _State_PosAndRot)
    {
        this.transform.position = _State_PosAndRot.SpawnPos + (_State_PosAndRot.Dir * _State_PosAndRot.Dis);
        this.transform.localRotation = DevTool.Get_RotFromDir(_State_PosAndRot.Dir);

        DevTool.Add_RotZValue(transform, _State_PosAndRot.SpreadAngle);
    }


    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();

        base.SetOn_State();
    }

    #endregion

    #region Active

    protected override void Active()
    {
        SetOff_Trail();

        StartCoroutine(Play_BuffArea_Cor());
    }

    private IEnumerator Play_BuffArea_Cor()
    {
        yield return new WaitForSeconds(2f);

        Remove_Object();
    }

    #endregion

    #region Sorting Order

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        ThisTrail.sortingOrder = _SortingOrder - 1;
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

        Remove_Condition();
        Reset_State();

        this.gameObject.SetActive(false);
    }

    #endregion
}
