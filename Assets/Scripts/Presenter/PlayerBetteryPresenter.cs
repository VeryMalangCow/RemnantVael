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

    public void SubscribeOn()
    {
        player.OnBetteryShardChanged += betteryShardView.SetBetteryShardUI;
        player.OnEmptyBetteryChanged += betteriesView.SetEmptyBetteryUI;
        player.OnChargedBetteryChanged += betteriesView.SetChargedBetteryUI;

        player.OnEmptyBetteryChanged += baseUpgrade.SetEmptyBetteryUI;
        player.OnChargedBetteryChanged += baseUpgrade.SetChargedBetteryUI;
        player.OnEmptyBetteryChanged += moduleUpgrade.SetEmptyBetteryUI;
        player.OnChargedBetteryChanged += moduleUpgrade.SetChargedBetteryUI;


        player.OnChargedBetteryChanged += allyBaseUpgrade.SetChargedBetteryUI;
        player.OnChargedBetteryChanged += allyModuleUpgrade.SetChargedBetteryUI;

        player.OnChargedBetteryChanged += protoCoreCvt.SetChargedBetteryUI;
        player.OnChargedBetteryChanged += etherCoreCvt.SetChargedBetteryUI;
        player.OnChargedBetteryChanged += originCoreCvt.SetChargedBetteryUI;

        player.SetBetteryShardUI();
        player.SetEmptyBetteryUI();
        //player.SetChargedBetteryUI();
    }

    public void SubscribeOff()
    {
        player.OnBetteryShardChanged -= betteryShardView.SetBetteryShardUI;
        player.OnEmptyBetteryChanged -= betteriesView.SetEmptyBetteryUI;
        player.OnChargedBetteryChanged -= betteriesView.SetChargedBetteryUI;

        player.OnEmptyBetteryChanged -= baseUpgrade.SetEmptyBetteryUI;
        player.OnChargedBetteryChanged -= baseUpgrade.SetChargedBetteryUI;
        player.OnEmptyBetteryChanged -= moduleUpgrade.SetEmptyBetteryUI;
        player.OnChargedBetteryChanged -= moduleUpgrade.SetChargedBetteryUI;


        player.OnChargedBetteryChanged -= allyBaseUpgrade.SetChargedBetteryUI;
        player.OnChargedBetteryChanged -= allyModuleUpgrade.SetChargedBetteryUI;

        player.OnChargedBetteryChanged -= protoCoreCvt.SetChargedBetteryUI;
        player.OnChargedBetteryChanged -= etherCoreCvt.SetChargedBetteryUI;
        player.OnChargedBetteryChanged -= originCoreCvt.SetChargedBetteryUI;
    }
}
