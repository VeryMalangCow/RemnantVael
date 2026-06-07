using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAllyPresencePresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudAllyPresenceView allyPresence;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;

        allyPresence = uiData.hud.AllyPresenceView;
    }

    public void SubscribeOn()
    {
        player.OnStrikePresenceChanged += allyPresence.StrikePresence;
        player.OnUplinkPresenceChanged += allyPresence.UplinkPresence;
        player.OnNeoPresenceChanged += allyPresence.NeoPresence;

        player.OnReputationChanged += allyPresence.SetReputation;

        player.ActPresence();
        player.ActReputation();
    }

    public void SubscribeOff()
    {
        player.OnStrikePresenceChanged -= allyPresence.StrikePresence;
        player.OnUplinkPresenceChanged -= allyPresence.UplinkPresence;
        player.OnNeoPresenceChanged -= allyPresence.NeoPresence;

        player.OnReputationChanged -= allyPresence.SetReputation;
    }
}
