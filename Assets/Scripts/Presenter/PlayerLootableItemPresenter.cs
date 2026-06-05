using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLootableItemPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudLootableItemView lootableItemView;
    private HudKeyView keyView;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;

        lootableItemView = uiData.hud.LootableItemsView;

        SubscribeOn();
        Debug.Log("<color=orange>Lootable Item Presenter Set</color>");
    }

    public void SubscribeOn()
    {
        player.OnCreditChanged += lootableItemView.SetCredit;
        player.OnOverriderChanged += lootableItemView.SetOverrider;
        player.OnModuleShardChanged += lootableItemView.SetModuleShard;

        player.SetCreditUI();
        player.SetOverriderUI();
        player.SetModuleShardUI();
    }

    public void SubscribeOff()
    {
        player.OnCreditChanged -= lootableItemView.SetCredit;
        player.OnOverriderChanged -= lootableItemView.SetOverrider;
        player.OnModuleShardChanged -= lootableItemView.SetModuleShard;

    }
}
