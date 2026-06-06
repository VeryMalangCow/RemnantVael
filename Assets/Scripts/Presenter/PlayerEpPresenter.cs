using System.Diagnostics;
using UnityEngine;

public class PlayerEpPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;
    private HudEpView view;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;
        view = uiData.hud.EpView;
        SubscribeOn();
    }

    public void SubscribeOn()
    {
        player.OnEpChanged += view.SetEnergyGauge;
        player.OnMaxEpChanged += view.SetMaxEnergeGauge;
        player.SetMaxEp();
        player.FullEp();
    }

    public void SubscribeOff()
    {
        player.OnEpChanged -= view.SetEnergyGauge;
        player.OnMaxEpChanged -= view.SetMaxEnergeGauge;
    }
}
