using UniRx;
using UnityEngine;

public class BuffDmgController : BuffController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dmg Add")]

    [Space(10)]
    [SerializeField] private BuffState<float> Multiple = new BuffState<float>();

    #endregion

    #region Framework


    protected override void Start()
    {
        base.Start();

        CurrentBuffCharge
            .Subscribe(_Value =>
            {
                Multiple.ActualValue = Multiple.BaseValue * CurrentBuffCharge.Value;
            });
    }


    #endregion

    #region Buff

    public override void Gain_Buff()
    {
        base.Gain_Buff();

        PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.GainBuff(Multiple); 
        PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.SetBuffedState();
    }

    public override void Reduct_Buff()
    {
        base.Reduct_Buff();

        PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.SetBuffedState();
    }

    public override void End_Buff()
    {
        base.End_Buff();

        PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.RemoveBuff(Multiple);
        PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage.SetBuffedState();
    }

    #endregion

}
