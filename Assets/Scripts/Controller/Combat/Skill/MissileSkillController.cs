using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MissileSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private HaveShadowThing ThisHST;

    [Header("-- State")]
    [SerializeField] private float ShotDelay = 0.1f;


    #endregion

    #region Active

    public override void ActiveSkill()
    {
        base.ActiveSkill();
        StartCoroutine(ActualActive());
    }
    

    private IEnumerator ActualActive()
    {
        // 효과
        InputManager.Instance.AimController.SetOnSkill(0, true);


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
            MissileBulletController missile = PoolingManager.Instance.GetOP_Missile();

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
                    PlayerController.BaseWeapon.BaseDamage.ActualState.Value * Power.ActualState.Value,
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

                missile.SetState_forMissile(this.gameObject.transform.position, bulletState, dir, targetRange);


                // Effect Explosion -> Physics DMG
                ExplosionEffect_Fan((Vector2)ThisHST.TargetObject.gameObject.transform.position + (dir * 0.1f), ThisHST.ThisSR.sortingOrder + 1, isCritical, dir);
                
                

                // Effect Shake
                tf.DOShakePosition(ShotDelay, 0.05f, 20, 90, false, true);
            }

            yield return new WaitForSeconds(ShotDelay);
        }

        InputManager.Instance.AimController.SetOnSkill(0, false);
    }

    #endregion

    #region Effect

    private void ExplosionEffect_Fan(Vector2 _SpawndPos, int _SortLayer, bool _IsCritical, Vector2 _Dir)
    {
        int index = 0;
        if (!_IsCritical)
        { index = 0; }
        else
        { index = 1; }

        PlayerController.PlayerMEI.GenExplosionImgs_Fan(
            _SpawndPos, _SortLayer,
            _Dir, 90f,
            4, 0.2f, 1.5f,
            1.0f, 0.05f, 0.1f,
            0.5f, 0.5f, 1.0f,
            index, PlayerController.ThisPlayerSmokeMaterial);
    }

    #endregion
}
