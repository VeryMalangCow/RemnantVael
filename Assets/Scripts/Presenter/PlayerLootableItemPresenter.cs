using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLootableItemPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudLootableItemsView view;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;

        view = uiData.hud.LootableItemsView;

        SubscribeOn();
        Debug.Log("<color=orange>Lootable Item Presenter Set</color>");
    }

    public void SubscribeOn()
    {
        player.OnCreditChanged += view.SetCredit;
        player.OnOverriderChanged += view.SetOverrider;
        player.OnModuleShardChanged += view.SetModuleShard;

        player.SetCreditUI();
        player.SetOverriderUI();
        player.SetModuleShardUI();
    }

    public void SubscribeOff()
    {
        player.OnCreditChanged -= view.SetCredit;
        player.OnOverriderChanged -= view.SetOverrider;
        player.OnModuleShardChanged -= view.SetModuleShard;

    }
}
