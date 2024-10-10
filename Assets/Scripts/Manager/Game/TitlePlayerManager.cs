using UnityEngine;

public class TitlePlayerManager : Singleton<TitlePlayerManager>
{
    [SerializeField] private Transform PlayerParentTF;
    [SerializeField] private InputTitleManager InputManager;
    [HideInInspector] public TitleLobbyPlayerController PlayerController;

    private void Start()
    {
        GameObject playerGO = Instantiate(GameManager.Instance.DesignatedPlayerPrefab, PlayerParentTF);
        if (playerGO.TryGetComponent(out TitleLobbyPlayerController Player))
        {
            PlayerController = Player;
            InputManager.gameObject.SetActive(true);
        }
    }
}
