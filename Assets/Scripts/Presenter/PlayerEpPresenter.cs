using System.Diagnostics;
using UnityEngine;

public class PlayerEpPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudEpView view;

    private PremiumCreditCvtUIController premiumCreditCvt;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;

        view = uiData.hud.EpView;
        premiumCreditCvt = uiData.premiumCreditCvt;
    }

    public void SubscribeOn()
    {
        player.OnEpChanged += view.SetEpUI;
        player.OnMaxEpChanged += view.SetMaxEpUI;

        player.OnEpChanged += premiumCreditCvt.SetEpUI;

        player.SetMaxEp();
        player.FullEp();
    }

    public void SubscribeOff()
    {
        player.OnEpChanged -= view.SetEpUI;
        player.OnMaxEpChanged -= view.SetMaxEpUI;

        player.OnEpChanged -= premiumCreditCvt.SetEpUI;
    }
}
