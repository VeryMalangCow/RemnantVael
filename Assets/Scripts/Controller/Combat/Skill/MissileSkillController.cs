using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MissileSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Skill")]

    [Header("=== State")]
    [SerializeField] private readonly float ShotDelay = 0.1f;
    [SerializeField] private readonly float SpreadAngleLimit = 10;


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

        
        for (int i = 0; i < Tier.ActualState.Value + 1; i++)
        {
            Play_ShotEachMissile();
            yield return new WaitForSeconds(ShotDelay);
        }


        // ==========
        End_SkillUI();
    }

    private void Play_ShotEachMissile()
    {
        MissileBulletController missile = PoolingManager.Instance.Get_OP_Missile();

        if (missile != null)
        {
            BulletState bulletState = Get_CurrentBulletState();

            // Dir
            float angle = PlayerController.SkillWeapon.PitchTF.localRotation.eulerAngles.y;
            Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

            float targetRange = DepthController.TargetRange;

            // Sorting Layer
            missile.ThisSR.sortingOrder = DepthController.ThisSR.sortingOrder - 1;

            float randomSpreadAngle = Random.Range(-SpreadAngleLimit, SpreadAngleLimit);

            BulletState_PosAndRot posAndRot = new BulletState_PosAndRot(this.gameObject.transform.position, dir, randomSpreadAngle);
            BulletState_Size? size = null;
            State_Anim? anim = null;
            missile.Set_State(bulletState, posAndRot, size, anim, targetRange);


            // Effect Explosion -> Physics DMG
            UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Skill0(
                PlayerController.Get_ID(),
                (Vector2)DepthController.TargetObject.gameObject.transform.position + (dir * 0.1f),
                dir,
                bulletState.IsCritical);



            // Effect Shake
            DepthController.transform.DOShakePosition(ShotDelay, 0.05f, 20, 90, false, true);
            PlayerManager.Instance.CameraController.Play_ShotAnim(ShotDelay, bulletState.DmgState.Dmg * 0.5f);
        }

    }

    #endregion

    #region Get

    // 플레이어의 현재 총알 스탯을 가져오기
    private BulletState Get_CurrentBulletState()
    {
        return new BulletState(
            new CombatState(
                new DmgState(eDamageType.Physics, PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value * 1.5f),
                new CriticalState(PlayerController.BaseWeapon.CC.ActualState.Value, PlayerController.BaseWeapon.CD.ActualState.Value),
                new KnockbackState(true, PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * 1.5f, 0.4f)),
            _CheckIsCritical: true, 
            _MuzzleSpeed: PlayerController.BaseWeapon.MuzzleSpeed.ActualState.Value * 1.5f, 
            _AliveTime: 3.5f);
    }

    #endregion
}
