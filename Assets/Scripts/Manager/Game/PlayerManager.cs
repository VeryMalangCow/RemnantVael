using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [Header("=== Prefab")]
    [HideInInspector] public PlayerController PlayerController;
    [SerializeField] private Transform PlayerSpawnParentTF;

    [Header("=== Class")]
    [SerializeField] private CameraController CameraController;


    #endregion

    #region Framework

    private void Start()
    {
        this.PlayerController = UnitGenerator.Instance.GenerateUnit<PlayerController>(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF);
        LayerOrderManager.Instance.MovableObjects.Add(PlayerController);
        CameraController.TargetTF = PlayerController.gameObject.transform;
    }

    #endregion

}