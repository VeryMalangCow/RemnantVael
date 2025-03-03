using DG.Tweening;
using System.Collections;
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
        InputManager.Instance.AimController.Set_SkillState(0, true);


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

                BulletState bulletState = new BulletState(
                    eDamageType.Physics,
                    /*PlayerController.BaseWeapon.BaseDamage.ActualState.Value*/ 
                    PlayerController.BaseWeapon.BaseDamage.BuffedState * Power.ActualState.Value,
                    1.5f,
                    3.5f,
                    isCritical,
                    PlayerController.BaseWeapon.CD.ActualState.Value,
                    true,
                    PlayerController.BaseWeapon.KnockbackPower.ActualState.Value,
                    0.4f);

                // Dir
                float angle = PlayerController.SkillWeapon.PitchTF.localRotation.eulerAngles.y;
                Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

                float targetRange = ThisHST.TargetRange;

                // Sorting Layer
                missile.ThisSR.sortingOrder = ThisHST.ThisSR.sortingOrder - 1;

                missile.Set_State_Missile(this.gameObject.transform.position, bulletState, dir, targetRange);


                // Effect Explosion -> Physics DMG
                Gen_ExplosionEffect_Fan((Vector2)ThisHST.TargetObject.gameObject.transform.position + (dir * 0.1f), isCritical, dir);
                
                

                // Effect Shake
                tf.DOShakePosition(ShotDelay, 0.05f, 20, 90, false, true);
            }

            yield return new WaitForSeconds(ShotDelay);
        }

        InputManager.Instance.AimController.Set_SkillState(0, false);
        Set_EndUI();
    }

    #endregion

    #region Effect

    private void Gen_ExplosionEffect_Fan(Vector2 _SpawndPos, bool _IsCritical, Vector2 _Dir)
    {
        int index = 0;
        if (!_IsCritical)
        { index = 0; }
        else
        { index = 1; }

        PlayerController.PlayerMEI.Gen_ExplosionImgs_Fan(
            _SpawndPos,
            _Dir, 90f,
            4, 0.2f, 1.5f,
            1.0f, 0.05f, 0.1f,
            0.5f, 0.5f, 1.0f,
            index, PlayerController.ThisPlayerMaterial_000);
    }

    #endregion
}
