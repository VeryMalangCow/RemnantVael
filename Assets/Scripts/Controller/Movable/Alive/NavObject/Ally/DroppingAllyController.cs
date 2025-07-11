using System.Collections;
using UnityEngine;

public class DroppingAllyController : NoneUnitAllyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float ChargeSpeed = 1f;
    [SerializeField] private float CurrentChargeTime = 0f;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        Caculate_AttackCharge(Time.deltaTime);
    }


    #endregion

    #region Play

    protected override IEnumerator Play_Main_Cor()
    {
        yield return StartCoroutine(base.Play_Main_Cor());
    }

    #endregion

    #region Attack

    private void Caculate_AttackCharge(float _DeltaTime)
    {
        if (CurrentChargeTime < 1)
        {
            CurrentChargeTime += ChargeSpeed * _DeltaTime;
        }

        if (Can_Attack())
        {
            CurrentChargeTime -= 1;
            Fire();
        }
    }

    private bool Can_Attack()
    {
        return CurrentChargeTime >= 1 && EnemyManager.Instance.CurrentEnemyList.Count > 0;
    }

    private void Fire()
    {

    }

    #endregion
}
