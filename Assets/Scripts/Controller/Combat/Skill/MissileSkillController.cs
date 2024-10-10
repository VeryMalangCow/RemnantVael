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
    [SerializeField] private float SpreadTime = 1f;

    #endregion

    public override void ActiveSkill()
    {
        base.ActiveSkill();

        if (CanActive())
        {
            AllActiveEffect();
            StartCoroutine(ActualActive());
        }
    }

    private IEnumerator ActualActive()
    {
        // ±¸ÇöºÎ

        for (int i = 0; i < Tier.ActualState.Value; i++)
        {
            MissileBulletController missile = PoolingManager.Instance.GetOP_Missile();

            if (missile != null)
            {
                BulletState bulletState = new BulletState(
                    eDamageType.Physics,
                    PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.ActualState.Value * Power.ActualState.Value,
                    0.5f,
                    2f);

                missile.SetState_forMissile(this.gameObject.transform.position, bulletState);
            }

            yield return new WaitForSeconds(ShotDelay);
        }
    }
}
