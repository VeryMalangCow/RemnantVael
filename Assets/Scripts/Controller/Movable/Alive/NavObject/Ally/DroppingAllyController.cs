using System.Collections;
using UnityEngine;

public class DroppingAllyController : NoneUnitAllyController
{

    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float CurrentChargeTime = 0f;
    [SerializeField] protected float DropBottomYPos = 0f;

    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] protected Sprite ThisSprite;


    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] protected float TrailTime;
    [SerializeField] protected float TrailStartWidth;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] protected float LightIntensity;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        Caculate_AttackCharge(Time.deltaTime);
    }


    #endregion

    #region Caculate
    private void Caculate_AttackCharge(float _DeltaTime)
    {
        if (CurrentChargeTime < 1)
        {
            CurrentChargeTime += ActualAllyState.rof.value * _DeltaTime;
        }

        if (Can_Shot())
        {
            HUD.Play_IconRT();
            Shot();
        }
    }

    #endregion

    #region Shot

    protected virtual bool Can_Shot() 
    {
        return CurrentChargeTime >= 1;
    }

    protected virtual void Shot()
    {
        CurrentChargeTime -= 1;
    }

    #endregion

    #region Play

    protected override IEnumerator Play_Main_Cor()
    {
        yield return StartCoroutine(base.Play_Main_Cor());
    }

    #endregion

    #region Sort

    public override void Set_SortingOrder(int _SortingOrder)
    { /* Need Nothing */ }

    #endregion
}
