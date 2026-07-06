using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class HudLootableItemView : MonoBehaviour
{
    [SerializeField] private LootableItemEUIController creditEui;
    [SerializeField] private LootableItemEUIController overriderEui;
    [SerializeField] private LootableItemEUIController moduleShardEui;

    public void Init()
    {
        creditEui.Offset();
        overriderEui.Offset();
        moduleShardEui.Offset();

        gameObject.SetActive(true);
    }

    public void SetCreditUI(int value)
    { 
        creditEui.PlayAmount(value);
    }

    public void SetOverriderUI(int value)
    {
        overriderEui.PlayAmount(value);
    }

    public void SetModuleShardUI(int value)
    {
        moduleShardEui.PlayAmount(value);
    }
}
