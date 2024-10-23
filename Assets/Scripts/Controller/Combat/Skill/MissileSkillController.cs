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

    [Header("-- Prefab")]
    [SerializeField] private GameObject Missile_Prefab;

    [Header("-- State")]
    [SerializeField] private float ShotDelay = 0.1f;

    [Header("=== Effect")]
    [SerializeField] private MakeExplosionImage MEI;


    #endregion

    public override void ActiveSkill()
    {
        base.ActiveSkill();
        StartCoroutine(ActualActive());
    }
    

    private IEnumerator ActualActive()
    {
        // 효과

        Transform tf = null;
        for (int i = 0; i < PlayerController.SkillWeapon.Hands.Count; i++)
        {
            if (PlayerController.SkillWeapon.Hands[i].ObjectTF == this.gameObject.transform)
            {
                tf = PlayerController.SkillWeapon.Hands[i].TargetTF;
            }
        }

        // 구현부

        for (int i = 0; i < Tier.ActualState.Value; i++)
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
                    PlayerController.BaseWeapon.KnockbackPower.ActualState.Value,
                    0.4f);

                // Dir
                float angle = PlayerController.SkillWeapon.PitchTF.localRotation.eulerAngles.y;
                Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

                float targetRange = 0.4f;
                if (TryGetComponent(out HaveShadowThing HST))
                { targetRange = HST.TargetRange; }
                missile.SetState_forMissile(this.gameObject.transform.position, bulletState, dir, targetRange);

                // Effect Explosion
                MEI.GenExplosionImgs_Fan(
                    (Vector2)MEI.gameObject.transform.position + (dir * 0.1f),
                    dir, 90f,
                    8, 0.2f, 1.5f,
                    0.35f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f);

                // Effect Shake
                tf.DOShakePosition(ShotDelay, 0.05f, 20, 90, false, true);
            }

            yield return new WaitForSeconds(ShotDelay);
        }
    }
}
