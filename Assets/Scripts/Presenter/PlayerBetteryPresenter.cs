using UnityEngine;

public class PlayerBetteryPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudBetteryShardView betteryShardView;
    private HudBetteryView betteriesView;

    private BaseUpgradeUIController baseUpgrade;
    private ModuleUpgradeUIController moduleUpgrade;

    private AllyBaseUpgradeUIController allyBaseUpgrade;
    private AllyModuleUpgradeUIController allyModuleUpgrade;
    
    private ProtoCoreCvtUIController protoCoreCvt;
    private EtherCoreCvtUIController etherCoreCvt;
    private OriginCoreCvtUIController originCoreCvt;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;


        betteryShardView = uiData.hud.BetteryShardView;
        betteriesView = uiData.hud.BetteriesView;

        baseUpgrade = uiData.baseUpgrade;
        moduleUpgrade = uiData.moduleUpgrade;

        allyBaseUpgrade = uiData.allyBaseUpgrade;
        allyModuleUpgrade = uiData.allyModuleUpgrade;

        protoCoreCvt = uiData.protoCoreCvt;
        etherCoreCvt = uiData.etherCoreCvt;
        originCoreCvt = uiData.originCoreCvt;
    }

    private void SetBetteryShard(int value)
    {
        betteryShardView.SetBetteryShardUI(value);
    }

    private void SetEmptyBettery(int value)
    {
        betteriesView.SetEmptyBetteryUI(value); 
        baseUpgrade.SetEmptyBetteryUI(value); 
        moduleUpgrade.SetEmptyBetteryUI(value);
    }

    private void SetChargedBettery(int value)
    {
        betteriesView.SetChargedBetteryUI(value);
        baseUpgrade.SetChargedBetteryUI(value);
        moduleUpgrade.SetChargedBetteryUI(value);

        allyBaseUpgrade.SetChargedBetteryUI(value);
        allyModuleUpgrade.SetChargedBetteryUI(value);

        protoCoreCvt.SetChargedBetteryUI(value);
        etherCoreCvt.SetChargedBetteryUI(value);
        originCoreCvt.SetChargedBetteryUI(value);
    }

    public void SubscribeOn()
    {
        player.OnBetteryShardChanged += SetBetteryShard;
        player.OnEmptyBetteryChanged += SetEmptyBettery;
        player.OnChargedBetteryChanged += SetChargedBettery;

        player.SetBetteryShardUI();
        player.SetEmptyBetteryUI();
        player.SetChargedBetteryUI();
    }

    public void SubscribeOff()
    {
        player.OnBetteryShardChanged -= SetBetteryShard;
        player.OnEmptyBetteryChanged -= SetEmptyBettery;
        player.OnChargedBetteryChanged -= SetChargedBettery;

    }
}
