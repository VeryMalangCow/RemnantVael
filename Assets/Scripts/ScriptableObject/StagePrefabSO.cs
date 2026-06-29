using UnityEngine;

[CreateAssetMenu(fileName = "StagePrefabSO", menuName = "ScriptableObject/StagePrefabSO")]
public class StagePrefabSO : ScriptableObject
{
    [Header("=== Prefab")]
    [Space(10)]
    [Header("-- Lobby")]
    [Header("* Room")]
    public RoomController roomLobbyEntrancePrefab;
    [Header("* Rule")]
    public RoomRuleController roomRuleLobbyPrefab;
    public EntranceRuleController roomEntranceRuleLobbyPrefab;

    [Space(10)]
    [Header("-- GamePlay")]
    [Header("* Room")]
    public RoomController[] roomPrefabs;
    [Header("* Rule")]
    public SerializableArray<RoomRuleController>[] roomRulePrefabs;
    public RoomRuleController roomStartPrefab;
    public EntranceRuleController[] roomEntrancePrefabs;
    public VaultRuleController roomVaultPrefab;
    public ShopRuleController roomShopPrefab;
    public AllyShopRuleController roomAllyShopPrefab;
    public PrisonRuleController roomPrisonPrefab;

    [Space(10)]
    [Header("-- Passage")]
    [Header("* Room")]
    public PassageRoomController roomPassagePrefab;
    [Header("* Rule")]
    public PassageRuleController roomPassageRulePrefab;

    [Space(10)]
    [Header("=== Build")]
    public VaultController[] vaultPrefabs;
    public BaseUpgradeController buPrefab;
    public ModuleUpgradeController muPrefab;
    public AllyBaseUpgradeController abuPrefab;
    public AllyModuleUpgradeController amuPrefab;
    public PrisonController[] prisonPrefabs;

    [Space(10)]
    [Header("=== Oper")]
    public RepairOperatorController repairOperPrefab;
    public VaultRerollOperatorController vaultRerollOperPrefab;
    public VaultUpgradeOperatorController vaultUpgradeOperPrefab;
    public PrisonPayOperatorController prisonPayOperPrefab;
    public PrisonPuzzleOperatorController prisonPuzzleOperPrefab;

    [Space(10)]
    [Header("=== Field Obj")]
    [SerializeField] public DestructibleObjectController[] fieldObjPrefabs;


}
