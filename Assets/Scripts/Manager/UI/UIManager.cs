
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
        GameObject playerHUDGO = Instantiate(PlayerHUDCanvasPrefab, UIParent);
        if(playerHUDGO.TryGetComponent(out PlayerHUDController PHUDC))
        { PlayerHUDController = PHUDC; }
    }

    private void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerManager.Instance.PlayerController.AddCurrentEP(-30);
        }
    }

    #endregion
}
