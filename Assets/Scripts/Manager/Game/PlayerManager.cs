using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Value

    [HideInInspector] public PlayerController PlayerController;

    [Header("=== TF")]
    [SerializeField] private Transform PlayerSpawnParentTF;

    [Header("=== Class")]
    [SerializeField] public CameraController CameraController;

    [Header("=== State")]
    [SerializeField] public BUState<float> AvoidChance;
    [SerializeField] public BUState<float> MaxEP;
    [SerializeField] public BUState<float> TakingDmgMultiple;
    [SerializeField] public BUState<float> SpawnESMultiple;
    [SerializeField] public BUState<float> NeedEP_ForSkillMultiple;
    [SerializeField] public BUState<float> WalkSpeed;
    [SerializeField] public BUState<float> WalkSpeedWhenShotMultiple;
    [SerializeField] public BUState<float> DecEnergyPointMultiple;
    [SerializeField] public BUState<float> BaseDamage;
    [SerializeField] public BUState<float> MuzzleSpeed;
    [SerializeField] public BUState<float> ROF;
    [SerializeField] public BUState<float> CC;
    [SerializeField] public BUState<float> CD;
    [SerializeField] public BUState<float> AccuracyRate;
    [SerializeField] public BUState<float> KnockbackPower;
    [SerializeField] public BUState<float> DashSpeed;
    [SerializeField] public BUState<float> Skill0_MaxCooltime;
    [SerializeField] public BUState<int> Skill0_Tier;
    [SerializeField] public BUState<float> Skill0_Power;

    #endregion

    #region Framework
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        this.PlayerController = UnitManager.Gen_Unit<PlayerController>(GameManager.Instance.DesignatedPlayerPrefab, PlayerSpawnParentTF);
        
        LayerOrderManager.Instance.NeedLayerObjects.Add(PlayerController);
        CameraController.TargetTF = PlayerController.gameObject.transform;
        BaseUpgradeManager.Instance.Offset(PlayerController);

        GameObject spawnedAimGO = Instantiate(PlayerController.AimPrefab, PlayerSpawnParentTF);
        if (spawnedAimGO != null && spawnedAimGO.TryGetComponent(out AimController aim)) 
        {
            InputManager.Instance.AimController = aim;
        }
    }


    #endregion

}