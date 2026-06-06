using UnityEngine;

public class PlayerBoostPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;
    private HudBoostView view;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;
        view = uiData.hud.BoostView;
        SubscribeOn();
    }

    public void SubscribeOn()
    {
        player.OnBoostLvChanged += view.SetBoostLvUI;

        player.SetBoostLv(0);
    }

    public void SubscribeOff()
    {
        player.OnBoostLvChanged -= view.SetBoostLvUI;
    }
}
