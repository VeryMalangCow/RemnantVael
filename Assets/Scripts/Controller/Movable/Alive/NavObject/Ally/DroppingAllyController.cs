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

    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] private Sprite BulletSprite;
    [SerializeField] private Vector2 BulletObjSize;

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
            Debug.Log(ActualAllyState.Rof.Value);
            CurrentChargeTime += ActualAllyState.Rof.Value * _DeltaTime;
        }

        if (Can_Attack())
        {
            CurrentChargeTime -= 1;

            EnemyController targetEnemy = PlayerManager.Instance.Get_PingedEnemy();
            if (targetEnemy == null)
            {
                targetEnemy = EnemyManager.Instance.CurrentEnemyList[
                    UnityEngine.Random.Range(0, EnemyManager.Instance.CurrentEnemyList.Count)];
            }

            Fire(PoolingManager.Instance.Get_OP_DroppingAllyBullet(), targetEnemy.transform.position);
        }
    }

    private bool Can_Attack()
    {
        return CurrentChargeTime >= 1 && EnemyManager.Instance.CurrentEnemyList.Count > 0;
    }

    private void Fire(AllyDroppingBombController _Bullet, Vector2 _TargetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        _Bullet.Set_State(
            Get_BulletState(), _DroppingTime: ActualAllyState.MuzzleSpeed.Value, _TopYPos: 5f,
            _State_PosAndRot: Get_BulletState_PosAndRot(_TargetPos),
            _State_Size: Get_BulletState_Shadow_Size());

        _Bullet.ThisSR.color = this.ThisExtraColor;

        // Sync
        ActiveAlly_Fire(null, _Bullet);

        // ÀÌ¹ÌÁö
        _Bullet.ThisSR.sprite = BulletSprite;
    }

    #endregion

    #region State (Bullet)

    private CombatState Get_BulletState()
    {
        return new CombatState(
            new CombatOwner(eCombatOwner.Ally, ID),
            new DmgState(eDamageType.Physics, ActualAllyState.Dmg.Value),
            new CriticalState(ActualAllyState.CC.Value, 1 + ActualAllyState.CD.Value),
            new KnockbackState(false, 0, 0));
    }

    private BulletState_PosAndRot Get_BulletState_PosAndRot(Vector2 _TargetPos)
    {
        return new BulletState_PosAndRot(
            _TargetPos,
            Vector2.zero,
            0);
    }

    private BulletState_Size Get_BulletState_Shadow_Size()
    {
        return new BulletState_Size(
            BulletObjSize * ActualAllyState.AttackSize.Value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
