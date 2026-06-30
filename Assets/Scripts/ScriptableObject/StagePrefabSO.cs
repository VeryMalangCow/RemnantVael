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
    public StartRuleController roomStartPrefab;
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
    [Header("=== Field Obj")]
    [SerializeField] public DestructibleObjectController[] fieldObjPrefabs;
}
