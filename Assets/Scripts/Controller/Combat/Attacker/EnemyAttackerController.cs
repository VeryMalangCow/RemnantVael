using UnityEngine;

public class EnemyAttackerController : AttackerController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [HideInInspector] public EnemyController enemy;

    #endregion

    #region Pooling

    public override void RemoveObject()
    {
        AttackerManager.instance.RemoveEnemyAttacker(this);
    }

    #endregion

    #region Set State

    // 적의 판정은 조금 줄이도록하는 편법 사용
    public override void Set_State_Juge<T>(AttackerState_Juge<T> state_Juge)
    {
        base.Set_State_Juge(state_Juge);

        if (DevTool.Can_CastingTType(col, out CapsuleCollider2D capsule2D))
            capsule2D.size *= 0.7f;
        else if (DevTool.Can_CastingTType(col, out CircleCollider2D circle2D))
            circle2D.radius *= 0.7f;
    }

    #endregion


    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Player(col);

        base.OnTriggerEnter2D(col);
    }


    protected void Try_Hit_Player(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Player",
            hittedObjList, out PlayerController pc))
        {
            //Damage
            PlayerManager.instance.playerController.Try_Hitted(this);
            hittedObjList.Add(pc);
        }
    }

    #endregion
}
