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
    [SerializeField] private List<Animator> atList;

    [HideInInspector] private AnimatorOverrideController fireAoc;
    [HideInInspector] private AnimatorOverrideController coldAoc;
    [HideInInspector] private AnimatorOverrideController electricityAoc;
    [HideInInspector] private AnimatorOverrideController corrosionAoc;

    #endregion

    #endregion

    #region Reset (State)

    protected override void Reset_State()
    {
        base.Reset_State();

        fireAoc = null;
        coldAoc = null;
        electricityAoc = null;
        corrosionAoc = null;
    }

    #endregion

    #region Set (State)

    public override void Set_State_Extra()
    {
        base.Set_State_Extra();

        for (int i = 0; i < atList.Count; i++)
        {
            if (state.Get_AttributeCondition()[i])
            {
                atList[i].gameObject.SetActive(true);

                DevTool.Set_Anim(ref Get_IndexAOC(i), atList[i], ResourceManager.instance.Get_AttributeExplosionAC(i));
                atList[i].speed = animSpeed;
            }
            else
            {
                atList[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Condition (Attribute)

    private ref AnimatorOverrideController Get_IndexAOC(int index)
    {
        switch (index)
        {
            case 0:
                return ref fireAoc;
            case 1:
                return ref coldAoc;
            case 2:
                return ref electricityAoc;
            case 3:
                return ref corrosionAoc;

            default:
                return ref fireAoc;
        }
    }


    #endregion

    #region Remove

    public override void RemoveObject()
    {
        ExplosionManager.instance.RemovePlayerExplosion(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Enemy(col);

        base.OnTriggerEnter2D(col);
    }

    protected void Try_Hit_Enemy(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Enemy",
            hittedObjectList, out EnemyController ec))
        {
            //Damage
            ec.Try_Hitted(this);
            hittedObjectList.Add(ec);
        }
    }

    #endregion
}
