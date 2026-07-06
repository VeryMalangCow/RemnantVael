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

    private void SetEp(float current, float max)
    {
        view.SetEpUI(current, max);
        premiumCreditCvt.SetEpUI(current, max);
    }

    private void SetMaxEp(float current, float max)
    {
        view.SetMaxEpUI(current, max);
    }

    public void SubscribeOn()
    {
        player.OnEpChanged += SetEp;
        player.OnMaxEpChanged += SetMaxEp;

        player.SetMaxEp();
        player.FullEp();
    }

    public void SubscribeOff()
    {
        player.OnEpChanged -= SetEp;
        player.OnMaxEpChanged -= SetMaxEp;
    }
}
