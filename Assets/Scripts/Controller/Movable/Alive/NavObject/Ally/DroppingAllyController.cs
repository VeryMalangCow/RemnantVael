using System.Collections;
using UnityEngine;

public class DroppingAllyController : NoneUnitAllyController
{

    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float currentChargeTime = 0f;
    [SerializeField] protected float dropBottomYPos = 0f;

    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] protected Sprite sprite;


    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] protected float trailTime;
    [SerializeField] protected float trailStartWidth;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] protected float lightIntensity;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        Caculate_AttackCharge(Time.deltaTime);
    }


    #endregion

    #region Caculate
    private void Caculate_AttackCharge(float deltaTime)
    {
        if (currentChargeTime < 1)
        {
            currentChargeTime += actualAllyState.rof.value * deltaTime;
        }

        if (Can_Shot())
        {
            hud.Play_IconRT();
            Shot();
        }
    }

    #endregion

    #region Shot

    protected virtual bool Can_Shot() 
    {
        return currentChargeTime >= 1;
    }

    protected virtual void Shot()
    {
        currentChargeTime -= 1;
    }

    #endregion

    #region Play

    protected override IEnumerator Play_Main_Cor()
    {
        yield return StartCoroutine(base.Play_Main_Cor());
    }

    #endregion

    #region Sort

    public override void Set_SortingOrder(int sortingOrder)
    { /* Need Nothing */ }

    #endregion
}
