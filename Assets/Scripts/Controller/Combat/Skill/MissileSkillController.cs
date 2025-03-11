using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private DepthController ThisHST;

    [Header("-- State")]
    [SerializeField] private float ShotDelay = 0.1f;
    [SerializeField] private readonly float SpreadAngleLimit = 10;


    #endregion

    #region Active

    public override void Active_Skill()
    {
        base.Active_Skill();
        StartCoroutine(Play_ActualActive_Cor());
    }
    
    private IEnumerator Play_ActualActive_Cor()
    {
        // 효과
        InputManager.Instance.AimController.Set_ActivingSkill(0, true);


        Transform tf = null;
        for (int i = 0; i < PlayerController.SkillWeapon.Hands.Count; i++)
        {
            if (PlayerController.SkillWeapon.Hands[i].ObjectTF == this.gameObject.transform)
            {
                tf = PlayerController.SkillWeapon.Hands[i].TargetTF;
            }
        }

        // 구현부

        for (int i = 0; i < Tier.ActualState.Value + 1; i++)
        {
            MissileBulletController missile = PoolingManager.Instance.Get_OP_Missile();

            if (missile != null)
            {
                float rcc = UnityEngine.Random.Range(0f, 1f);

                bool isCritical = false;
                if (rcc < PlayerController.BaseWeapon.CC.ActualState.Value)
                {
                    isCritical = true;
                }

                DmgState dmgState = new DmgState(eDamageType.Physics, PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value);
                CriticalState criticalState = new CriticalState(PlayerController.BaseWeapon.CC.ActualState.Value, PlayerController.BaseWeapon.CD.ActualState.Value);
                KnockbackState knockbackState = new KnockbackState(true, PlayerController.BaseWeapon.KnockbackPower.ActualState.Value, 0.4f);

                BulletState bulletState = new BulletState(new CombatState(dmgState, criticalState, knockbackState), true, 1.5f, 3.5f);

                // Dir
                float angle = PlayerController.SkillWeapon.PitchTF.localRotation.eulerAngles.y;
                Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

                float targetRange = ThisHST.TargetRange;

                // Sorting Layer
                missile.ThisSR.sortingOrder = ThisHST.ThisSR.sortingOrder - 1;

                float randomSpreadAngle = Random.Range(-SpreadAngleLimit, SpreadAngleLimit);

                BulletState_PosAndRot posAndRot = new BulletState_PosAndRot(this.gameObject.transform.position, dir, randomSpreadAngle);
                BulletState_Size? size = null;
                State_Anim? anim = null;
                missile.Set_State(bulletState, posAndRot, size, anim, targetRange);


                // Effect Explosion -> Physics DMG
                UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Skill0(
                    PlayerController.Get_ID(),
                    (Vector2)ThisHST.TargetObject.gameObject.transform.position + (dir * 0.1f),
                    dir,
                    isCritical);



                // Effect Shake
                tf.DOShakePosition(ShotDelay, 0.05f, 20, 90, false, true);
                PlayerManager.Instance.CameraController.Play_ShotAnim(ShotDelay, bulletState.DmgState.Dmg * 0.5f);
            }

            yield return new WaitForSeconds(ShotDelay);
        }

        InputManager.Instance.AimController.Set_ActivingSkill(0, false);
        Set_EndUI();
    }

    #endregion
}
