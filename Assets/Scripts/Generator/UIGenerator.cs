
using UnityEngine;

public class UIGenerator : Singleton<UIGenerator>
{
    #region Value


    [Header("=== UI")]
    [SerializeField] private Transform UIParent;
    [SerializeField] private GameObject PlayerHUD_CanvasPrefab;
    [SerializeField] private GameObject BaseUpgrade_CanvasPrefab;
    [SerializeField] private GameObject ModuleUpgrade_CanvasPrefab;

    #endregion

    #region Framework

    private void Start()
    {
        UIManager.Instance.PlayerHUD_UIController = SpawnUI<PlayerHUDController>(PlayerHUD_CanvasPrefab, true);
        UIManager.Instance.BaseUpgrade_UIController = SpawnUI<BaseUpgradeUIController>(BaseUpgrade_CanvasPrefab, false);
        UIManager.Instance.ModuleUpgrade_UIController = SpawnUI<ModuleUpgradeUIController>(ModuleUpgrade_CanvasPrefab, false);
    }

    #endregion

    #region Generator

    public T SpawnUI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);
        uigo.gameObject.SetActive(_OnOff);
        if(uigo.TryGetComponent(out UIController ui))
        {
            Debug.Log("½ÇÇà");
            ui.Offset_Main();
        }

        if (uigo.TryGetComponent(out T spawnUI))
        { return spawnUI; }
        else
        { return default; }
    }

    #endregion
}
