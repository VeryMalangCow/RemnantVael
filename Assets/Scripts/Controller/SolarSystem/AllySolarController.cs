
using UnityEngine;

public class AllySolarController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Solar")]

    [Header("=== Owner")]
    [SerializeField] private AllyController ThisAlly;

    [Header("=== Comp")]
    [SerializeField] public Rigidbody2D ThisRb;

    [Header("=== Value")]
    [SerializeField] private eAllyStateMode AllyStateMode;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Set_ActingByCondition();

        base.LateUpdate();
    }

    #endregion

    #region Set (State)

    public void Set_AllyStateMode(eAllyStateMode _StateMode)
    {
        if (AllyStateMode == _StateMode) return;

        AllyStateMode = _StateMode;
    }

    #endregion

    #region Set (Acting)

    private void Set_ActingByCondition()
    {
        switch (AllyStateMode)
        {
            case eAllyStateMode.Idle:
                Set_RotSmooth(ThisAlly.Get_ForPlayerDir(), Time.deltaTime);
                break;

            case eAllyStateMode.Move:
                Set_RotSmooth(ThisRb.velocity, Time.deltaTime);
                break;

            case eAllyStateMode.Attack:
                Set_RotSmooth(ThisAlly.Get_ForEnemyDir(), Time.deltaTime);
                break;
        }
    }

    #endregion
}
