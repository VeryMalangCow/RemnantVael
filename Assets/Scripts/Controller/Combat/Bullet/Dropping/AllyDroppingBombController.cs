using System.Collections.Generic;
using UnityEngine;

public class AllyDroppingBombController : DroppingBombController
{
    #region Pooling

    protected override void PoolingSet()
    {
        PoolingManager.Instance.DroppingAllyBullet.Enqueue(this);
    }

    #endregion

    #region Explosion

    protected override void Gen_Explosion()
    {
        Play_ExplosionAttack();
    }

    private void Play_ExplosionAttack()
    {
        AllyExplosionController aec = PoolingManager.Instance.Get_OP_AllyExplosion();
        aec.Set_State(
            Get_ExlposionState(),
            _AC: ResourceManager.Instance.ExplosionAC,
            Get_SpawnTF(),
            this.TargetRange);
    }

    private ExplosionState Get_ExlposionState()
    {
        return new ExplosionState(
            new CombatState(
                new CombatOwner(eCombatOwner.Ally, ID),
                new DmgState(eDamageType.Physics, State.DmgState.Dmg),
                new CriticalState(State.CriticalState),
                new KnockbackState(true, State.KnockbackState.KBPower, State.KnockbackState.KBTime)),
            SizeState,
            new List<bool> { false, false, false, false }); // Fire, Cold, Electricity, Corrosion
    }

    private State_TF2D Get_SpawnTF()
    {
        return new State_TF2D(transform.position, Quaternion.identity, Vector2.one);
    }

    #endregion
}
