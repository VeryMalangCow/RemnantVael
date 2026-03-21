using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MissileSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Skill")]

    [Header("=== State")]
    [SerializeField] private readonly float shotDelay = 0.1f;
    [SerializeField] private readonly float spreadAngleLimit = 10;


    #endregion

    #region Active

    public override void Active_Skill()
    {
        base.Active_Skill();

        StartCoroutine(Play_ActualActive_Cor());
    }

    #endregion

    #region Actual Active

    private IEnumerator Play_ActualActive_Cor()
    {
        Start_SkillUI();
        // ==========

        
        for (int i = 0; i < tier.actualState.Value + 1; i++)
        {
            Play_ShotEachMissile();
            yield return new WaitForSeconds(shotDelay);
        }


        // ==========
        End_SkillUI();
    }

    private void Play_ShotEachMissile()
    {
        MissileBulletController missile = PoolingManager.instance.Get_OP_Missile();

        if (missile != null)
        {
            BulletState bulletState = Get_CurrentBulletState();

            // Dir
            float angle = playerController.skillWeapon.pitchTf.localRotation.eulerAngles.y;
            Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

            // State
            missile.Set_State(
                bulletState,
                State_PosAndRot(dir), 
                state_Size: null, 
                state_Anim: null,
                state_Effect: null,
                depthController.targetRange);

            // Effect Explosion -> Physics DMG
            UnitManager.instance.player_ExplImgGenerator.Expl_Player_Skill0(
                playerController.Get_ID(),
                (Vector2)depthController.targetObject.gameObject.transform.position + (dir * 0.1f),
                dir,
                bulletState.isCritical);

            // Effect Shake
            depthController.transform.DOShakePosition(shotDelay, 0.05f, 20, 90, false, true);
            PlayerManager.instance.cameraController.Play_ShotAnim(shotDelay, bulletState.dmgState.dmg * 0.5f);

            // Sound
            SoundManager.instance.Play_2D_SFX_Player_Random(
                playerController.Get_AS(), playerController.Get_ID(), "MShot", 2);
        }

    }

    #endregion

    #region Get

    // 플레이어의 현재 총알 스탯을 가져오기
    private BulletState Get_CurrentBulletState()
    {
        return new BulletState(
            new CombatState(
                new CombatOwner(eCombatOwner.Player),
                new DmgState(eDamageType.Physics, playerController.baseWeapon.baseDamage.buffedState * power.actualState.Value * 1.5f),
                new CriticalState(playerController.baseWeapon.cc.actualState.Value, playerController.baseWeapon.cd.buffedState),
                new KnockbackState(true, playerController.baseWeapon.kbPower.actualState.Value * 1.5f, 0.4f)),
            checkIsCritical: true, 
            muzzleSpeed: playerController.baseWeapon.muzzleSpeed.actualState.Value * 1.5f, 
            aliveTime: 3.5f);
    }

    private BulletState_PosAndRot State_PosAndRot(Vector2 dir)
    {
        return new BulletState_PosAndRot(
            this.gameObject.transform.position, 
            dir, 
            DevTool.Get_RandomValueBaseZero(spreadAngleLimit * 2f));
    }

    #endregion
}
