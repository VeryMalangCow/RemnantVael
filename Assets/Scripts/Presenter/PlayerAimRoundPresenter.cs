using UnityEngine;

public class PlayerAimRoundPresenter : MonoBehaviour, IPresentable
{
    private PlayerWeaponController weapon;
    private AimRoundController aimRound;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        weapon = ownerData.player.baseWeapon;
        aimRound = uiData.aimRound;
        SubscribeOn();
    }

    public void SubscribeOn()
    {
        weapon.OnAccChanged += aimRound.Set_AngleRoundValue;
        weapon.SetAccAimRound();
    }

    public void SubscribeOff()
    {
        weapon.OnAccChanged -= aimRound.Set_AngleRoundValue;
    }
}
