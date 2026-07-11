using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeMemoryCleanup
{
    static PlayModeMemoryCleanup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.delayCall += () =>
            {
                AddressablesManager.ReleaseAll();
                Resources.UnloadUnusedAssets();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Debug.Log("Memory Cleanup");
            };
        }
    }
}