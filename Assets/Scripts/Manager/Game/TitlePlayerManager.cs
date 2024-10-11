using UnityEngine;

public class TitlePlayerManager : Singleton<TitlePlayerManager>
{
    [SerializeField] private Transform PlayerParentTF;
    [SerializeField] private InputTitleManager InputManager;
    [HideInInspector] public TitlePlayerController PlayerController;

    private void Start()
    {
        this.PlayerController = UnitManager.GenerateUnit<TitlePlayerController>(GameManager.Instance.TitlePlayerPrefab, PlayerParentTF);
        InputManager.OnEnableInput();
    }
}
