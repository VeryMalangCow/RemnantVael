using UnityEngine;
using UnityEngine.Serialization;

public class AllySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Solar")]

    [Header("=== Owner")]
    [FormerlySerializedAs("ThisAlly")][SerializeField] private FieldUnitAllyController ally;

    [Header("=== Comp")]
    [FormerlySerializedAs("ThisRb")][SerializeField] public Rigidbody2D rb;

    [Header("=== Value")]
    [FormerlySerializedAs("AllyStateMode")][SerializeField] private eAllyStateMode allyStateMode;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Set_ActingByCondition();

        base.LateUpdate();
    }

    #endregion

    #region Set (State)

    public void Set_AllyStateMode(eAllyStateMode stateMode)
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
            case eAllyStateMode.Idle:
                Set_RotSmooth(ally.Get_ForPlayerDir(), Time.deltaTime);
                break;

            case eAllyStateMode.Move:
                Set_RotSmooth(rb.velocity, Time.deltaTime);
                break;

            case eAllyStateMode.Attack:
                Set_RotSmooth(ally.Get_ForEnemyDir(), Time.deltaTime);
                break;
        }
    }

    #endregion
}
