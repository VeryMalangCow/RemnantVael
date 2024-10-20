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
        if (CanActive())
        {
            base.ActiveSkill();
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
                    1.5f,
                    3.5f);

                // Dir
                float angle = PlayerManager.Instance.PlayerController.SkillWeapon.PitchTF.localRotation.eulerAngles.y;
                Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

                float targetRange = 0.4f;
                if (TryGetComponent(out HaveShadowThing HST))
                { targetRange = HST.TargetRange; }
                missile.SetState_forMissile(this.gameObject.transform.position, bulletState, dir, targetRange);

                // Effect
                
                //Vector2 dir = (InputManager.Instance.MousePosByWorld - (Vector2)MEI.transform.position).normalized;
                MEI.GenExplosionImgs_Fan(
                    MEI.gameObject.transform.position,
                    dir, 4f,
                    6, 0.4f, 0.55f,
                    0.35f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f);
            }

            yield return new WaitForSeconds(ShotDelay);
        }
    }
}
