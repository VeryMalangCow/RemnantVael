using UnityEngine;

public class TitlePlayerManager : Singleton<TitlePlayerManager>
{
    [SerializeField] private Transform PlayerParentTF;
    [HideInInspector] public TitlePlayerController PlayerController;

    private void Start()
    {
        this.PlayerController = UnitManager.Gen_Unit<TitlePlayerController>(GameManager.Instance.TitlePlayerPrefab, PlayerParentTF);
    }
}
