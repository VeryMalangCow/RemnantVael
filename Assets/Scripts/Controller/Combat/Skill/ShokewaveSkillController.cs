
using DG.Tweening;
using UnityEngine;

public class ShockwaveSkillController : ActiveSkillController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Shockwave Skill")]

    [Space(10)]
    [Header("=== Value")]

    [Header("-- Data")]
    [SerializeField] private Vector2 _ColSize = new Vector2(1f, 0.5f);
    [SerializeField] private float _StartSize = 3f; 
    [SerializeField] private float _MaxSize = 7.5f;
    [SerializeField] private float _BiggerTime = 0.3f; 
    [SerializeField] private float _SmallerTime = 0.4f;
    #endregion

    #region Active

    public override void ActiveSkill()
    {
        base.ActiveSkill();
        ActualActive();
    }

    private void ActualActive()
    {
        AttackerState ThisState = new AttackerState(
            eDamageType.Energy, 
            PlayerController.BaseWeapon.BaseDamage.ActualState.Value * Power.ActualState.Value,
            true,
            PlayerController.BaseWeapon.KnockbackPower.ActualState.Value * Tier.ActualState.Value * 25f,
            0.4f,
            PlayerController.BaseWeapon.CC.ActualState.Value,
            PlayerController.BaseWeapon.CD.ActualState.Value); 

        PlayerAttacker pa = PoolingManager.Instance.GetOP_PlayerAttacker();
        pa.SetState_Bigger(transform.position, ThisState, _ColSize, _StartSize, _MaxSize, _BiggerTime, _SmallerTime)
            .OnComplete(() =>
            {
                pa.gameObject.SetActive(false);
                PoolingManager.Instance.PlayerAttackers.Queue.Enqueue(pa);
            });

    }

    #endregion
}
