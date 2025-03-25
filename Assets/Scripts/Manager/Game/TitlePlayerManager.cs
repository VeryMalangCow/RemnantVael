using UnityEngine;

public class TitlePlayerManager : Singleton<TitlePlayerManager>
{
    [SerializeField] private Transform PlayerParentTF;
    [HideInInspector] public TitlePlayerController PlayerController;

    private void Start()
    {
        Gen_Player();
    }


    #region Gen

    private void Gen_Player()
    {
        this.PlayerController =
            DevTool.Get_ComponentTType<TitlePlayerController>(
                Instantiate(GameManager.Instance.TitlePlayerPrefab, PlayerParentTF));
    }

    #endregion
}
