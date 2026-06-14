using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;
    private SaveDataManager saveData;

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


    private void SetCredit(int value)
    {
        lootableItemView.SetCreditUI(value);
        premiumCreditCvt.SetCreditUI(value);
        etherCoreCvt.SetCreditUI(value);
        originCoreCvt.SetCreditUI(value);
    }

    private void SetOverrider(int value)
    {
        lootableItemView.SetOverriderUI(value);
        allyBaseUpgrade.SetOverriderUI(value);
    }

    private void SetModuleShard(int value)
    {
        lootableItemView.SetModuleShardUI(value);
        moduleUpgrade.SetModuleShard(value);
    }

    private void SetKey(Dictionary<int, int> dict)
    {
        keyView.SetKeyItem(dict);
    }

    private void SetHighItem(int id, int amount)
    {
        tabItemView.SetHighLvItemUI(id, amount);
    }

    public void SubscribeOn()
    {
        player.OnCreditChanged += SetCredit;
        player.OnOverriderChanged += SetOverrider;
        player.OnModuleShardChanged += SetModuleShard;
        player.OnKeycardChanged += SetKey;
        saveData.OnHighItemChanged += SetHighItem;

        player.SetCreditUI();
        player.SetOverriderUI();
        player.SetModuleShardUI();
    }

    public void SubscribeOff()
    {
        player.OnCreditChanged -= SetCredit;
        player.OnOverriderChanged -= SetOverrider;
        player.OnModuleShardChanged -= SetModuleShard;
        player.OnKeycardChanged -= SetKey;
        saveData.OnHighItemChanged -= SetHighItem;
    }
}
