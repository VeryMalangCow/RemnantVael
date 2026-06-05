using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class HudLootableItemsView : MonoBehaviour
{
    [SerializeField] private LootableItemEUIController creditEui;
    [SerializeField] private LootableItemEUIController overriderEui;
    [SerializeField] private LootableItemEUIController moduleShardEui;

    public IEnumerator Init()
    {
        Stopwatch sw = Stopwatch.StartNew();

        creditEui.Offset();
        overriderEui.Offset();
        moduleShardEui.Offset();

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>Betteries View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    public void SetCredit(int value)
    { 
        creditEui.PlayAmount(value);
    }

    public void SetOverrider(int value)
    {
        overriderEui.PlayAmount(value);
    }

    public void SetModuleShard(int value)
    {
        moduleShardEui.PlayAmount(value);
    }
}
