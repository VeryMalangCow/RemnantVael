using UnityEngine;

public class EnemyAttackerController : AttackerController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [HideInInspector] public EnemyController Enemy;

    #endregion

    #region Set State

    // 적의 판정은 조금 줄이도록하는 편법 사용
    public override void Set_State_Juge<T>(AttackerState_Juge<T> _State_Juge)
    {
        base.Set_State_Juge(_State_Juge);

        if (DevTool.Can_CastingTType(ThisCol, out CapsuleCollider2D capsule2D))
            capsule2D.size *= 0.8f;
        else if (DevTool.Can_CastingTType(ThisCol, out CircleCollider2D circle2D))
            circle2D.radius *= 0.8f;
    }

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.EnemyAttackers.Queue.Enqueue(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_Player(_Col);

        base.OnTriggerEnter2D(_Col);
    }


    protected void Try_Hit_Player(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "Player",
            HittedObjectList, out PlayerController pc))
        {
            //Damage
            PlayerManager.Instance.PlayerController.Try_Hitted(this);
            HittedObjectList.Add(pc);
        }
    }

    #endregion
}
