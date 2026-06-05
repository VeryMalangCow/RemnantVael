using UnityEngine;

public class PlayerBetteryPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudBetteryShardView betteryShardView;
    private HudBetteriesView betteriesView;

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

        SubscribeOn();
        Debug.Log("<color=orange>Bettery Presenter Set</color>");
    }

    public void SubscribeOn()
    {
        player.OnBetteryShardChanged += betteryShardView.SetBetteryShard;
        player.OnEmptyBetteryChanged += betteriesView.SetEmptyBettery;
        player.OnChargedBetteryChanged += betteriesView.SetChargedBettery;

        player.OnEmptyBetteryChanged += baseUpgrade.SetEmptyBettery;
        player.OnChargedBetteryChanged += baseUpgrade.SetChargedBettery;
        player.OnEmptyBetteryChanged += moduleUpgrade.SetEmptyBettery;
        player.OnChargedBetteryChanged += moduleUpgrade.SetChargedBettery;


        player.OnChargedBetteryChanged += allyBaseUpgrade.SetChargedBettery;
        player.OnChargedBetteryChanged += allyModuleUpgrade.SetChargedBettery;

        player.OnChargedBetteryChanged += protoCoreCvt.SetChargedBettery;
        player.OnChargedBetteryChanged += etherCoreCvt.SetChargedBettery;
        player.OnChargedBetteryChanged += originCoreCvt.SetChargedBettery;

        player.SetBetteryShardUI();
        player.SetEmptyBetteryUI();
        player.SetChargedBetteryUI();
    }

    public void SubscribeOff()
    {
        player.OnBetteryShardChanged -= betteryShardView.SetBetteryShard;
        player.OnEmptyBetteryChanged -= betteriesView.SetEmptyBettery;
        player.OnChargedBetteryChanged -= betteriesView.SetChargedBettery;

        player.OnEmptyBetteryChanged -= baseUpgrade.SetEmptyBettery;
        player.OnChargedBetteryChanged -= baseUpgrade.SetChargedBettery;
        player.OnEmptyBetteryChanged -= moduleUpgrade.SetEmptyBettery;
        player.OnChargedBetteryChanged -= moduleUpgrade.SetChargedBettery;


        player.OnChargedBetteryChanged -= allyBaseUpgrade.SetChargedBettery;
        player.OnChargedBetteryChanged -= allyModuleUpgrade.SetChargedBettery;

        player.OnChargedBetteryChanged -= protoCoreCvt.SetChargedBettery;
        player.OnChargedBetteryChanged -= etherCoreCvt.SetChargedBettery;
        player.OnChargedBetteryChanged -= originCoreCvt.SetChargedBettery;
    }
}
