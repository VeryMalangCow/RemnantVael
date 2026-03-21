using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DroppingAllyController : NoneUnitAllyController
{

    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Value")]
    [FormerlySerializedAs("CurrentChargeTime")][SerializeField] private float currentChargeTime = 0f;
    [FormerlySerializedAs("DropBottomYPos")][SerializeField] protected float dropBottomYPos = 0f;

    [Space(10)]
    [Header("=== Bullet")]
    [FormerlySerializedAs("ThisSprite")][SerializeField] protected Sprite sprite;


    [Space(10)]
    [Header("=== Trail")]
    [FormerlySerializedAs("TrailTime")][SerializeField] protected float trailTime;
    [FormerlySerializedAs("TrailStartWidth")][SerializeField] protected float trailStartWidth;

    [Space(10)]
    [Header("=== Light")]
    [FormerlySerializedAs("LightIntensity")][SerializeField] protected float lightIntensity;

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
