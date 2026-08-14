using System.Collections.Generic;
using UnityEngine;

public class AllyDroppingBombController : DroppingBombController
{
    #region Pool
    protected override void RemoveObject()
    {
        BulletManager.instance.RemoveAllyDroppingBomb(this);
    }

    #endregion

    #region Explosion

    protected override void Gen_Explosion()
    {
        Play_ExplosionAttack();
    }

    private void Play_ExplosionAttack()
    {
        AllyExplosionController aec = ExplosionManager.instance.SpawnAllyExplosion();
        aec.Set_State(
            Get_ExlposionState(),
            ac: StaticResourceManager.instance.ExplosionReso.explosionAnimation,
            Get_SpawnTF(),
            this.targetRange);
    }

    private ExplosionState Get_ExlposionState()
    {
        return new ExplosionState(
            new CombatState(
                new CombatOwner(CombatOwnerType.Ally, id),
                new DmgState(DamageType.Physics, state.dmgState.dmg),
                new CriticalState(state.criticalState),
                new KnockbackState(true, state.knockbackState.kbPower, state.knockbackState.kbTime)),
            sizeState,
            new List<bool> { false, false, false, false }); // Fire, Cold, Electricity, Corrosion
    }

    private State_TF2D Get_SpawnTF()
    {
        return new State_TF2D(transform.position, Quaternion.identity, Vector2.one);
    }

    #endregion
}
