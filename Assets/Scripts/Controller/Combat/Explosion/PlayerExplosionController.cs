using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionController : ExplosionController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player Explosion")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private List<Animator> ThisAttributeATList;

    [HideInInspector] private AnimatorOverrideController FireAOC;
    [HideInInspector] private AnimatorOverrideController ColdAOC;
    [HideInInspector] private AnimatorOverrideController ElectricityAOC;
    [HideInInspector] private AnimatorOverrideController CorrosionAOC;

    #endregion

    #endregion

    #region Reset (State)

    protected override void Reset_State()
    {
        base.Reset_State();

        FireAOC = null;
        ColdAOC = null;
        ElectricityAOC = null;
        CorrosionAOC = null;
    }

    #endregion

    #region Set (State)

    public override void Set_State_Extra()
    {
        base.Set_State_Extra();

        for (int i = 0; i < ThisAttributeATList.Count; i++)
        {
            if (State.Get_AttributeCondition()[i])
            {
                ThisAttributeATList[i].gameObject.SetActive(true);

                DevTool.Set_Anim(ref Get_IndexAOC(i), ThisAttributeATList[i], ResourceManager.Instance.Get_AttributeExplosionAC(i));
                ThisAttributeATList[i].speed = AnimSpeed;
            }
            else
            {
                ThisAttributeATList[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Condition (Attribute)

    private ref AnimatorOverrideController Get_IndexAOC(int _Index)
    {
        switch (_Index)
        {
            case 0:
                return ref FireAOC;
            case 1:
                return ref ColdAOC;
            case 2:
                return ref ElectricityAOC;
            case 3:
                return ref CorrosionAOC;

            default:
                return ref FireAOC;
        }
    }


    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.PlayerExplosions.Enqueue(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_Enemy(_Col);

        base.OnTriggerEnter2D(_Col);
    }

    protected void Try_Hit_Enemy(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "Enemy",
            HittedObjectList, out EnemyController ec))
        {
            //Damage
            ec.Try_Hitted(this);
            HittedObjectList.Add(ec);
        }
    }

    #endregion
}
