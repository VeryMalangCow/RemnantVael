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
        Debug.Log("<color=orange>Aim Round Presenter Set</color>");
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
