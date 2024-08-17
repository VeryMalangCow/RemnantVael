
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Value

    [Header("=== UI")]
    [SerializeField] private Transform UIParent;

    [Header("=== HUD")]
    [SerializeField] private GameObject PlayerHUDCanvasPrefab;
    [SerializeField] public PlayerHUDController PlayerHUDController;

    [Header("=== Shop")]
    [SerializeField] private GameObject OneOffShopCanvasPrefab;
    [SerializeField] public OneOffShopUIController OneOffShopUIController;

    [Header("=== Data")]
    [SerializeField] public UIController CurrentOpeningUIController;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        PlayerHUDController = SpawnUI<PlayerHUDController>(PlayerHUDCanvasPrefab, true);
        OneOffShopUIController = SpawnUI<OneOffShopUIController>(OneOffShopCanvasPrefab, false);
    }

    private T SpawnUI<T>(GameObject _UIGO, bool _OnOff)
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
