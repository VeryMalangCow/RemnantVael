using UnityEngine;

public class PlayerBoostPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;
    private HudBoostView view;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;

        view = uiData.hud.BoostView;
    }

    private void SetBoost(int value)
    {
        view.SetBoostLvUI(value);
    }

    public void SubscribeOn()
    {
        player.OnBoostLvChanged += SetBoost;

        player.SetBoostLv(0);
    }

    public void SubscribeOff()
    {
        player.OnBoostLvChanged -= SetBoost;
    }
}
