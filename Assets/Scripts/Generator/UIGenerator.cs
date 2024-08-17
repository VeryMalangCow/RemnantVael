
using UnityEngine;

public class UIGenerator : Singleton<UIGenerator>
{
    #region Value


    [Header("=== UI")]
    [SerializeField] private Transform UIParent;
    [SerializeField] private GameObject PlayerHUDCanvasPrefab;
    [SerializeField] private GameObject OneOffShopCanvasPrefab;

    #endregion

    #region Framework

    private void Start()
    {
        UIManager.Instance.PlayerHUDController = SpawnUI<PlayerHUDController>(PlayerHUDCanvasPrefab, true);
        UIManager.Instance.OneOffShopUIController = SpawnUI<OneOffShopUIController>(OneOffShopCanvasPrefab, false);
    }

    #endregion

    #region Generator

    public T SpawnUI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);
        uigo.gameObject.SetActive(_OnOff);

        if (uigo.TryGetComponent(out T spawnUI))
        { return spawnUI; }
        else
        { return default; }
    }

    #endregion
}
