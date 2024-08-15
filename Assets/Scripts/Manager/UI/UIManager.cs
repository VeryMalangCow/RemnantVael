
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Value

    [Header("=== UI")]
    [SerializeField] private Transform UIParent;

    [Header("=== HUD")]
    [SerializeField] private GameObject PlayerHUDCanvasPrefab;
    [SerializeField] public PlayerHUDController PlayerHUDController;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameObject playerHUDGO = Instantiate(PlayerHUDCanvasPrefab, UIParent);
        if (playerHUDGO.TryGetComponent(out PlayerHUDController PHUDC))
        { PlayerHUDController = PHUDC; }
    }

    #endregion
}
