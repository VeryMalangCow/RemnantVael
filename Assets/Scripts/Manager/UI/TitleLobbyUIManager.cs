using UnityEngine;

public class TitleLobbyUIManager : Singleton<TitleLobbyUIManager>
{
    #region Value

    [Header("=== UI_Camera")]
    [SerializeField] private Camera UICamera;

    [Header("=== UI_Prefab")]
    [SerializeField] private Transform UIParent;
    [SerializeField] private GameObject TitleLobby_CanvasPrefab;

    // Controller
    [HideInInspector] public TitleLobbyUIController TitleLobby_UIController;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        TitleLobby_UIController = SpawnUI<TitleLobbyUIController>(TitleLobby_CanvasPrefab, true);
    }


    public T SpawnUI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);
        uigo.gameObject.SetActive(_OnOff);
        if (uigo.TryGetComponent(out UIController ui))
        {
            ui.Offset_Main();
        }
        if (uigo.TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = UICamera;
        }

        if (uigo.TryGetComponent(out T spawnUI))
        { return spawnUI; }
        else
        { return default; }
    }

    #endregion
}
