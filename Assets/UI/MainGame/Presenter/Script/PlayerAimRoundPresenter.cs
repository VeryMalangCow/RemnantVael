using UnityEngine;

public class PlayerAimRoundPresenter : MonoBehaviour, IPresentable
{
    private PlayerWeaponController weapon;
    private AimRoundController aimRound;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        weapon = ownerData.player.baseWeapon;

        aimRound = uiData.aimRound;
    }

    private void SetAcc(float acc)
    {
        aimRound.Set_AngleRoundValue(acc);
    }

    public void SubscribeOn()
    {
        weapon.OnAccChanged += SetAcc;

        weapon.SetAccAimRound();
    }

    public void SubscribeOff()
    {
        weapon.OnAccChanged -= SetAcc;
    }
}
