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

    private void SetStrikeTeamPresence(int current, int max)
    {
        allyPresence.StrikePresence(current, max);
    }

    private void SetUplinkTeamPresence(int current, int max)
    {
        allyPresence.UplinkPresence(current, max);
    }

    private void SetNeoTeamPresence(int current, int max)
    {
        allyPresence.NeoPresence(current, max);
    }

    private void SetReputation(float value)
    {
        allyPresence.SetReputation(value);
    }

    public void SubscribeOn()
    {
        player.OnStrikePresenceChanged += SetStrikeTeamPresence;
        player.OnUplinkPresenceChanged += SetUplinkTeamPresence;
        player.OnNeoPresenceChanged += SetNeoTeamPresence;

        player.OnReputationChanged += SetReputation;

        player.ActPresence();
        player.ActReputation();
    }

    public void SubscribeOff()
    {
        player.OnStrikePresenceChanged -= SetStrikeTeamPresence;
        player.OnUplinkPresenceChanged -= SetUplinkTeamPresence;
        player.OnNeoPresenceChanged -= SetNeoTeamPresence;

        player.OnReputationChanged -= SetReputation;
    }
}
