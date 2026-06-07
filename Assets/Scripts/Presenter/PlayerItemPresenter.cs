using UnityEngine;

public class PlayerItemPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;
    public SaveDataManager saveData;

    private HudLootableItemView lootableItemView;
    private HudKeyView keyView;
    private HudTabItemView tabItemView;

    private ModuleUpgradeUIController moduleUpgrade;

    private AllyBaseUpgradeUIController allyBaseUpgrade;

    private PremiumCreditCvtUIController premiumCreditCvt;
    private EtherCoreCvtUIController etherCoreCvt;
    private OriginCoreCvtUIController originCoreCvt;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;
        saveData = ownerData.saveData;


        lootableItemView = uiData.hud.LootableItemsView;
        keyView = uiData.hud.KeyView;
        tabItemView = uiData.hud.TabItemView;

        moduleUpgrade = uiData.moduleUpgrade;

        allyBaseUpgrade = uiData.allyBaseUpgrade;

        premiumCreditCvt = uiData.premiumCreditCvt;
        etherCoreCvt = uiData.etherCoreCvt;
        originCoreCvt = uiData.originCoreCvt;
    }

    public void SubscribeOn()
    {
        player.OnCreditChanged += lootableItemView.SetCreditUI;
        player.OnOverriderChanged += lootableItemView.SetOverriderUI;
        player.OnModuleShardChanged += lootableItemView.SetModuleShardUI;

        player.OnCreditChanged += premiumCreditCvt.SetCreditUI;
        player.OnCreditChanged += etherCoreCvt.SetCreditUI;
        player.OnCreditChanged += originCoreCvt.SetCreditUI;

        player.OnModuleShardChanged += moduleUpgrade.SetModuleShard;

        player.OnOverriderChanged += allyBaseUpgrade.SetOverriderUI;

        player.OnKeycardChanged += keyView.SetKeyItem;

        saveData.OnHighItemChanged += tabItemView.SetHighLvItemUI;

        player.SetCreditUI();
        player.SetOverriderUI();
        player.SetModuleShardUI();
    }

    public void SubscribeOff()
    {
        player.OnCreditChanged -= lootableItemView.SetCreditUI;
        player.OnOverriderChanged -= lootableItemView.SetOverriderUI;
        player.OnModuleShardChanged -= lootableItemView.SetModuleShardUI;

        player.OnCreditChanged -= premiumCreditCvt.SetCreditUI;
        player.OnCreditChanged -= etherCoreCvt.SetCreditUI;
        player.OnCreditChanged -= originCoreCvt.SetCreditUI;

        player.OnModuleShardChanged -= moduleUpgrade.SetModuleShard;

        player.OnOverriderChanged -= allyBaseUpgrade.SetOverriderUI;

        player.OnKeycardChanged -= keyView.SetKeyItem;

        saveData.OnHighItemChanged -= tabItemView.SetHighLvItemUI;
    }
}
