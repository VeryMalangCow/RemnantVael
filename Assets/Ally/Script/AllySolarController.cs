using UnityEngine;

public class AllySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Solar")]

    [SerializeField] private FieldUnitAllyController ally;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private AllyStateMode allyStateMode;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Set_ActingByCondition();

        base.LateUpdate();
    }

    #endregion

    #region Set (State)

    public void Set_AllyStateMode(AllyStateMode stateMode)
    {
        if (allyStateMode == stateMode) return;

        allyStateMode = stateMode;
    }

    #endregion

    #region Set (Acting)

    private void Set_ActingByCondition()
    {
        switch (allyStateMode)
        {
            case AllyStateMode.Idle:
                Set_RotSmooth(ally.Get_ForPlayerDir(), Time.deltaTime);
                break;

            case AllyStateMode.Move:
                Set_RotSmooth(rb.velocity, Time.deltaTime);
                break;

            case AllyStateMode.Attack:
                Set_RotSmooth(ally.Get_ForEnemyDir(), Time.deltaTime);
                break;
        }
    }

    #endregion
}
