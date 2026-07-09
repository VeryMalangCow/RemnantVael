using Cysharp.Threading.Tasks;
using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#region Class & Struct

// 방 그리드
public class RoomGrid
{
    public int instanceId;
    public int typeId;
    public Vector2Int[] roomPos;
    public RoomGridType roomType;

    public List<GateGrid> gates;

    public int eliteIndex = -1;

    public RoomGrid(int instanceId, int typeId, int roomPosLength, RoomGridType roomType)
    {
        this.instanceId = instanceId;
        this.typeId = typeId;
        this.roomPos = new Vector2Int[roomPosLength];
        this.gates = new List<GateGrid>();
        this.roomType = roomType;
    }


    public RoomGrid(RoomGrid other)
    {
        instanceId = other.instanceId;
        typeId = other.typeId;
        roomType = other.roomType;

        roomPos = (Vector2Int[])other.roomPos.Clone();

        gates = new List<GateGrid>(other.gates.Count);
        for (int i = 0; i < other.gates.Count; i++)
            gates.Add(new GateGrid(other.gates[i]));
    }
}

public class BossRoomGrid : RoomGrid
{
    public BossRoomIndex bossRoomIndex;

    public BossRoomGrid(int instanceId, int typeId, int roomPosLenght, RoomGridType roomType, BossRoomIndex bossRoomIndex) : base(instanceId, typeId, roomPosLenght, roomType)
    {
        this.bossRoomIndex = bossRoomIndex;
    }

    public BossRoomGrid(BossRoomGrid other) : base(other)
    {
        bossRoomIndex = other.bossRoomIndex;
    }
}

public struct BossRoomIndex
{
    public int bossIndex;
    public int nextStageIndex;

    public BossRoomIndex(int bossIndex, int nextStageIndex)
    {
        this.bossIndex = bossIndex;
        this.nextStageIndex = nextStageIndex;
    }
}


// 게이트 그리드
public class GateGrid
{
    public Vector2Int pos;
    public Vector2Int dir;

    public int ownerRoom;
    public int connectedRoom;

    public GateGrid connectedGate;
    public GateGrid(Vector2Int pos, Vector2Int dir, int ownerRoom, int connectedRoom)
    {
        this.pos = pos;
        this.dir = dir;

        this.ownerRoom = ownerRoom;
        this.connectedRoom = connectedRoom;

        this.connectedGate = null;
    }

    public GateGrid(GateGrid other)
    {
        pos = other.pos;
        dir = other.dir;
        connectedRoom = other.connectedRoom;
    }
}


// 예약 방 구조
public struct ReserveRoom
{
    public int typeId;
    public RoomGridType roomType;

    public ReserveRoom(int typeId, RoomGridType roomType)
    {
        this.typeId = typeId;
        this.roomType = roomType;
    }
}

public enum RoomGridType
{
    start, normal, vault, baseShop, allyShop, stPrison, utPrison, ntPrison, elite, boss
}

#endregion

[Serializable]
public class StageGridGenerator
{
    #region Class & Struct

    // 방 생성 시 규칙
    public class StageRule
    {
        public int minRoomAmount;
        public int maxRoomAmount;
        public RoomPercent[] percents;

        // Boss Enemy
        public BossRoomIndex[] bossIndices;

        // Elite Enemy
        public int eliteAmount;
        public int[] eliteIndices;

        // Normal Enemy
        public int[] enemyIndices;
    }

    public struct RoomPercent
    {
        public int typeIndex;
        public float percent;

        public RoomPercent(int typeIndex, float percent)
        {
            this.typeIndex = typeIndex;
            this.percent = percent;
        }
    }

    #endregion

    #region Variable

    [Header("=== Rule")]
    [SerializeField] private TextAsset stageRuleCSV;
#if UNITY_EDITOR
    [Header("=== Editor Tool")]
    [SerializeField] private SaveDataManager saveDataManager;
    [SerializeField] private int testTargetStageId;
#endif
    public StageRule targetStageRule = new StageRule();

    // 실제 방 데이터들
    public readonly List<RoomGrid> allRoomGrids = new List<RoomGrid>();

    // 정적 데이터
    private static int roomTypeAmount = 8;
    private static Vector2Int[][] ownGridStaticData;
    private static Vector2Int[][] roundOwnGridStaticData;

    // 생성 과정 동적 데이터
    // 실제 배치 + 테두리 (Hashset을 사용해 중복을 "절대" 방지)
    private HashSet<Vector2Int> existPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> roundPositions = new HashSet<Vector2Int>();

    // 실제 좌표와 해당 좌표가 포함되어있는 방
    private Dictionary<Vector2Int, RoomGrid> existPosDict = new Dictionary<Vector2Int, RoomGrid>();


    // 캐시 최적화 (New 방지 = GC Alloc 최소화) => Capacity : 현재 방의 크기 최대치와 Round 최대치를 생각한 값
    // For Init
    private readonly HashSet<Vector2Int> roundPosSetCache = new HashSet<Vector2Int>(8);
    private readonly List<Vector2Int> roundPosListCache = new List<Vector2Int>(8);
    // For Round Cadidate
    private readonly HashSet<Vector2Int> baseCandidateSetCache = new HashSet<Vector2Int>(32);
    private readonly List<Vector2Int> baseCandidateListCache = new List<Vector2Int>(32);
    // For Add
    private readonly Vector2Int[] tempAddExistPositions = new Vector2Int[4];
    private readonly Vector2Int[] tempAddRoundPositions = new Vector2Int[8];


    // Spaical Room을 위한 캐시
    private List<ReserveRoom> specialRooms = new List<ReserveRoom>();
    private List<ReserveRoom> tempSpecialRooms = new List<ReserveRoom>();
    private HashSet<Vector2Int> specialCandidateSetCache = new HashSet<Vector2Int>(8);
    private List<Vector2Int> specialCandidateSortedListCache = new List<Vector2Int>(8);

    // 일반 특수방 랜덤 시도용 임시 리스트
    private readonly List<Vector2Int> specialCandidateTryListCache = new List<Vector2Int>(8);

    private static readonly Vector2Int[] FourDirs =
    {
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down
    };

    // Boss
    private List<BossRoomIndex> bossRoomIndices = new List<BossRoomIndex>(4);

    // Elite
    private List<RoomGrid> eliteRoomCandidateRoomCache = new List<RoomGrid>(4);
    private List<int> eliteRoomIndices = new List<int>(4);

    // Record (Snapshot)
#if UNITY_EDITOR
    private readonly StageGridSnapshotRecorder snapshotRecorder = new();
    public StageGridSnapshotRecorder SnapshotRecorder => snapshotRecorder;

    private void RecordSnapshot(StageGridSnapshotStep step, int targetRoomId = -1,
        Vector2Int focusPosition = default, string description = "",
        List<Vector2Int> candidates = null)
    {
        snapshotRecorder.Record(step, allRoomGrids, targetRoomId,
            focusPosition, description, candidates);
    }
#endif

    // Generate
#if UNITY_EDITOR

    public void GenerateGridRoomDataTest() // Test
    {
        InitRoomData();

        TestGenerateGridData();
    }

    private void TestGenerateGridData()
    {
        if (!SetStageRule(targetStageRule, testTargetStageId))
            return;

        allRoomGrids.Clear();
        int maxSuccess = 100;
        int currentSuccess = 0;
        int currentFail = 0;

        int maxTotalTry = 1000;
        int totalTry = 0;

        while (maxSuccess > currentSuccess && totalTry < maxTotalTry)
        {
            snapshotRecorder.Clear();

            totalTry++;
            bool success = GenerateRoomGridTest();
            if (success)
            {
                currentSuccess++;
            }
            else
            {
                currentFail++;
            }
        }
        UnityEngine.Debug.Log(
            $"Try:[<color=gray>{totalTry}</color>] " +
            $"Fail:[<color=red>{currentFail}</color>] " +
            $"Success:[<color=blue>{currentSuccess}</color>] " +
            $"Average:[<color=yellow>1 in {((float)totalTry / currentSuccess):F2}</color>] " +
            $"Percent:[<color=yellow>{((float)currentSuccess / totalTry) * 100f:F2}%</color>]");
    }

    private bool GenerateRoomGridTest()
    {
        ClearGridCaches();
        allRoomGrids.Clear();

        int targetRoomAmount = UnityEngine.Random.Range(targetStageRule.minRoomAmount, targetStageRule.maxRoomAmount + 1);
        BuildSpecialRooms(saveDataManager.jsonData.gameProgressData);

        int targetNormalRoomAmount = targetRoomAmount - specialRooms.Count;
        if (targetNormalRoomAmount <= 0)
        {
            UnityEngine.Debug.Log($"일반 방 개수가 부족 / targetRoomAmount: {targetRoomAmount}, specialRooms: {specialRooms.Count}");
            return false;
        }

        ResetTempSpecialRooms();
        if (GenerateNormalRoomGrid(targetNormalRoomAmount, out int nextRoomId))
        {
            if (GenerateSpecialRoomGrid(nextRoomId))
            {
                if (GenerateEliteRoomGrid())
                {
                    GenerateGateGrid();
                    return true;
                }
            }
        }

        return false;
    }

#endif

    #endregion

    #region Init

    // Init
    public IEnumerator Initialize()
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif
        InitRoomData();
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"StageManager : <color=orange>RoomData Init</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    #endregion

    #region Generate

    // 맵의 그리드 생성
    public IEnumerator GenerateGridData(int targetStageId)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        if (!SetStageRule(targetStageRule, targetStageId))
            yield break;

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"StageManager : <color=orange>Set StageRule</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        allRoomGrids.Clear();
        yield return GenerateRoomGrid();
    }

    #region Init

    // Init
    private void InitRoomData()
    {
        ownGridStaticData = new Vector2Int[8][]
        {
            new Vector2Int[1] { new Vector2Int(0, 0) },
            new Vector2Int[2] { new Vector2Int(0, 0), new Vector2Int(1, 0) },
            new Vector2Int[2] { new Vector2Int(0, 0), new Vector2Int(0, 1) },
            new Vector2Int[4] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
            new Vector2Int[3] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1) },
            new Vector2Int[3] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) },
            new Vector2Int[3] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
            new Vector2Int[3] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) }
        };

        roundOwnGridStaticData = new Vector2Int[8][];
        for (int i = 0; i < roundOwnGridStaticData.Length; i++)
            roundOwnGridStaticData[i] = GetInitRoundPositions(ownGridStaticData[i]);

        roundPosSetCache.Clear();
        roundPosListCache.Clear();
    }

    // CSV to Stage Rule (By ID)
    private bool SetStageRule(StageRule rule, int stageId)
    {
        if (stageId == 99) return true;

        if (stageRuleCSV == null)
        {
            UnityEngine.Debug.LogError("stageRuleCSV가 없습니다.");
            return false;
        }

        int i;

        #region Room

        // 방의 확률과 개수 구하기
        string[] lines = stageRuleCSV.text.Split("\n");
        int lineIndex = stageId + 1;
        if (lineIndex < 0 || lineIndex >= lines.Length)
        {
            UnityEngine.Debug.LogError($"StageRule CSV 범위 초과. stageId: {stageId}");
            return false;
        }

        string[] cols = lines[lineIndex].Trim().Split(",");
        int lastIndexColumn = roomTypeAmount + 8;
        if (cols.Length <= lastIndexColumn)
        {
            UnityEngine.Debug.LogError($"StageRule CSV 컬럼 부족. stageId: {stageId}");
            return false;
        }

        rule.minRoomAmount = int.TryParse(cols[1], out int min) ? min : 1;
        rule.maxRoomAmount = int.TryParse(cols[2], out int max) ? max : 10;
        if (rule.maxRoomAmount < rule.minRoomAmount) rule.maxRoomAmount = rule.minRoomAmount;

        rule.percents = new RoomPercent[roomTypeAmount];
        for (i = 0; i < roomTypeAmount; i++)
            rule.percents[i] = new RoomPercent(i, float.TryParse(cols[i + 3], out float typePercent) ? typePercent : 0);

        #endregion

        #region Boss Enemy + NextStage

        // 보스 방 숫자만큼에 랜덤한 BossIndices 초기화
        string[] bossIndices = cols[roomTypeAmount + 4].Trim().Split("/");
        string[] nextStageIndices = cols[roomTypeAmount + 5].Trim().Split("/");

        if (bossIndices.Length < nextStageIndices.Length)
        {
            UnityEngine.Debug.LogError($"StageRule CSV에 NextStageIndex가 Boss 종류보다 많습니다.");
            return false;
        }

        Shuffle(bossIndices);

        int bossRoomAmount = bossIndices.Length;
        rule.bossIndices = new BossRoomIndex[bossRoomAmount];
        int bossIndex, nextStageIndex;
        for (i = 0; i < bossRoomAmount; i++)
        {
            bossIndex = int.TryParse(bossIndices[i], out int _bossIndex) ? _bossIndex : -1;
            nextStageIndex = int.TryParse(nextStageIndices[i], out int _nextStageIndex) ? _nextStageIndex : -1;
            rule.bossIndices[i] = new BossRoomIndex(bossIndex, nextStageIndex);
        }

        #endregion

        #region Elite Enemy

        // Elite
        int eliteAmount = int.TryParse(cols[roomTypeAmount + 6].Trim(), out int _eliteAmount) ? _eliteAmount : -1;
        rule.eliteAmount = eliteAmount;
        string[] eliteIndices = cols[roomTypeAmount + 7].Trim().Split("/");
        if (eliteAmount > eliteIndices.Length)
        {
            UnityEngine.Debug.LogError($"StageRule CSV에 Elite Room 요청보다 Elite 종류가 적습니다.");
            return false;
        }

        Shuffle(eliteIndices);
        rule.eliteIndices = new int[eliteAmount];
        for (i = 0; i < eliteAmount; i++)
            rule.eliteIndices[i] = int.TryParse(eliteIndices[i], out int eliteIndex) ? eliteIndex : -1;

        #endregion

        #region Normal Enemy

        // Enemy

        string[] enemyIndices = cols[roomTypeAmount + 8].Trim().Split("/");
        rule.enemyIndices = new int[enemyIndices.Length];
        for (i = 0; i < enemyIndices.Length; i++)
            rule.enemyIndices[i] = int.TryParse(enemyIndices[i], out int enemyIndex) ? enemyIndex : -1;

        #endregion

        return true;
    }


    #endregion

    #region Total Room

    // Generate <All Type Room>
    private IEnumerator GenerateRoomGrid()
    {
#if UNITY_EDITOR
        snapshotRecorder.Clear();
#endif
        int targetRoomAmount = UnityEngine.Random.Range(targetStageRule.minRoomAmount, targetStageRule.maxRoomAmount + 1);
        BuildSpecialRooms(SaveDataManager.instance.jsonData.gameProgressData);

        int targetNormalRoomAmount = targetRoomAmount - specialRooms.Count;

        if (targetNormalRoomAmount <= 0)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"일반 방 개수가 부족 / targetRoomAmount: {targetRoomAmount}, specialRooms: {specialRooms.Count}");
#endif
            yield break;
        }
        bool success = false;
        int maxTry = 120, perTry = 30, currentTry = 0;
        while (!success)
        {
            allRoomGrids.Clear();
            ClearGridCaches();
            ResetTempSpecialRooms();

            if (GenerateNormalRoomGrid(targetNormalRoomAmount, out int nextRoomId))
            {
                if (GenerateSpecialRoomGrid(nextRoomId))
                {
                    if (GenerateEliteRoomGrid())
                    {
                        GenerateGateGrid();
#if UNITY_EDITOR
                        RecordSnapshot(StageGridSnapshotStep.Complete);
#endif
                        success = true;
                    }
#if UNITY_EDITOR
                    else
                    {
                        UnityEngine.Debug.Log("<color=red>Room Elite Grid Generate FAIL</color>");
                    }
#endif
                }
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.Log("<color=red>Room Special Grid Generate FAIL</color>");
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.Log("<color=red>Room Normal Grid Generate FAIL</color>");
            }
#endif
            if (success)
                break;

            currentTry++;

            if (maxTry <= currentTry) // 시도 초과
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("<color=red>ROOM GENERATE FAIL (Over Try)</color>");
#endif
                yield break;
            }
            else
            {
                if (currentTry % perTry == 0)
                    yield return null;
            }
        }
#if UNITY_EDITOR
        UnityEngine.Debug.Log("<color=green>ROOM GENERATE COMPLETE</color>");
#endif
        ClearGridCaches();
        for (int i = 0; i < allRoomGrids.Count; i++)
        {
            if (allRoomGrids[i] is BossRoomGrid boss)
            {
                var data = boss.bossRoomIndex;
                UnityEngine.Debug.Log($"Boss: {data.bossIndex} / nextStage: {data.nextStageIndex}");
            }

            if (allRoomGrids[i].roomType == RoomGridType.elite)
            {
                var data = allRoomGrids[i].eliteIndex;
                UnityEngine.Debug.Log($"<color=cyan>{allRoomGrids[i].eliteIndex}</color>");
            }
        }
    }

    private void BuildSpecialRooms(GameProgressJsonData saveData)
    {
        specialRooms.Clear();

        if (saveData.usableBU || saveData.usableMU) specialRooms.Add(new ReserveRoom(0, RoomGridType.baseShop));
        if (saveData.usableABU || saveData.usableAMU) specialRooms.Add(new ReserveRoom(0, RoomGridType.allyShop));
        if (saveData.usableVault) specialRooms.Add(new ReserveRoom(0, RoomGridType.vault));
        if (saveData.usableSTPrison) specialRooms.Add(new ReserveRoom(0, RoomGridType.stPrison));
        if (saveData.usableUTPrison) specialRooms.Add(new ReserveRoom(0, RoomGridType.utPrison));
        if (saveData.usableNTPrison) specialRooms.Add(new ReserveRoom(0, RoomGridType.ntPrison));
        if (targetStageRule.bossIndices != null)
        {
            for (int i = 0; i < targetStageRule.bossIndices.Length; i++)
            {
                var indexData = targetStageRule.bossIndices[i];
                if (indexData.bossIndex != -1 && indexData.nextStageIndex != -1)
                    specialRooms.Add(new ReserveRoom(3, RoomGridType.boss));
            }
        }
    }

    private void ClearGridCaches()
    {
        existPositions.Clear();
        roundPositions.Clear();

        existPosDict.Clear();

        baseCandidateSetCache.Clear();
        baseCandidateListCache.Clear();

        specialCandidateSetCache.Clear();
        specialCandidateSortedListCache.Clear();
    }

    #endregion

    #region Normal Room

    // Generate <Normal Room Grid>
    private bool GenerateNormalRoomGrid(int targetRoomAmount, out int nextRoomId)
    {
        // Data Set
        int currentRoomAmount = 1;
        existPositions.Clear();
        roundPositions.Clear();

        existPosDict.Clear();

        int failCount = 0, failMaxLimit = 30;

        // 처음방 생성
        if (!TryPlaceNormalRoomAtBasePos(0, Vector2Int.zero, 0))
        {
            nextRoomId = 0;
            return false;
        }
        allRoomGrids[0].roomType = RoomGridType.start;

        nextRoomId = 1;

        while (currentRoomAmount < targetRoomAmount)
        {
            bool success = false;

            int type = GetRandomRoomType(targetStageRule.percents);
            if (type < 0)
            {
                UnityEngine.Debug.LogWarning("유효한 방 타입(모양)을 선택하지 못했습니다.");
                return false;
            }

            if (TryPlaceNormalRoomAtRandomCandidate(type, currentRoomAmount))
            {
                success = true;
            }

            if (!success)
            {
                failCount++;
                if (failMaxLimit <= failCount)
                {
                    UnityEngine.Debug.LogWarning("방을 생성하지 못했습니다.");
                    return false;
                }

                continue;
            }

            failCount = 0;
            currentRoomAmount++;
            nextRoomId = currentRoomAmount;
        }

        return true;
    }

    // round 후보들을 선정해서, 랜덤한 순서로 배치 시도 <Normal>
    private bool TryPlaceNormalRoomAtRandomCandidate(int type, int instanceId)
    {
        Vector2Int[] ownGrid = ownGridStaticData[type];

        baseCandidateSetCache.Clear();
        baseCandidateListCache.Clear();

        // 테두리 순회 (배치 가능성이 존재하는 모든 테두리)
        foreach (Vector2Int roundPos in roundPositions)
        {
            // 방의 좌표에 따른 순회
            // => 해당 좌표와 중앙 말고도 주변 모든 각도를 체크하기 위한 값으로 baseCandidateSetCache 초기화
            for (int i = 0; i < ownGrid.Length; i++)
            {
                Vector2Int basePos = roundPos - ownGrid[i];
                baseCandidateSetCache.Add(basePos);
            }
        }

        // Set -> List
        if (baseCandidateListCache.Capacity < baseCandidateSetCache.Count)
            baseCandidateListCache.Capacity = baseCandidateSetCache.Count;

        foreach (Vector2Int basePos in baseCandidateSetCache)
            baseCandidateListCache.Add(basePos);

#if UNITY_EDITOR
        RecordSnapshot(StageGridSnapshotStep.NormalCandidate, instanceId, Vector2Int.zero, "Candidate Generated", baseCandidateListCache);
#endif

        // 후보들을 순회
        while (baseCandidateListCache.Count > 0)
        {
            // 랜덤 값을 추출
            // => 마지막 인덱스와 바꿈 (밀림/당김 현상 제거)
            int randomIndex = UnityEngine.Random.Range(0, baseCandidateListCache.Count);
            Vector2Int selectedBasePos = baseCandidateListCache[randomIndex];
            int lastIndex = baseCandidateListCache.Count - 1;
            baseCandidateListCache[randomIndex] = baseCandidateListCache[lastIndex];
            baseCandidateListCache.RemoveAt(lastIndex);

            // 삽입 시도
            if (TryPlaceNormalRoomAtBasePos(type, selectedBasePos, instanceId))
                return true;
        }

        return false;
    }

    // Add Exist Positions <Normal>
    private bool TryPlaceNormalRoomAtBasePos(int type, Vector2Int pos, int instanceId)
    {
        int i;
        // 실제 좌표
        GetRelativeExistPos(type, pos, out int existLength);
        GetRelativeRoundPos(type, pos, out int roundLength);

        // 실제 방을 추가할 수 있는가?
        for (i = 0; i < existLength; i++)
            if (existPositions.Contains(tempAddExistPositions[i]))
                return false;

        // 인접한 방이 1개인가?
        int currentAdjacency = 0, maxAdjacencyLimit = 1;
        for (i = 0; i < roundLength; i++)
        {
            if (existPositions.Contains(tempAddRoundPositions[i]))
                currentAdjacency++;

            if (currentAdjacency > maxAdjacencyLimit)
                return false;
        }

        // 실제 방 추가
        RoomGrid room = new RoomGrid(instanceId, type, existLength, RoomGridType.normal);

        allRoomGrids.Add(room);
        for (i = 0; i < existLength; i++)
        {
            room.roomPos[i] = tempAddExistPositions[i];
            existPositions.Add(tempAddExistPositions[i]);
            existPosDict.Add(tempAddExistPositions[i], room);
        }

        // 라운드 위치 추가 (방이 존재한 위치는 제외)
        for (i = 0; i < roundLength; i++)
            if (!existPositions.Contains(tempAddRoundPositions[i]))
                roundPositions.Add(tempAddRoundPositions[i]);

        // 현재 실제 방 추가 위치는 다시 삭제하기
        for (i = 0; i < existLength; i++)
            roundPositions.Remove(tempAddExistPositions[i]);

#if UNITY_EDITOR
        RecordSnapshot(StageGridSnapshotStep.Normal, room.instanceId, pos);
#endif

        return true;
    }

    #endregion

    #region Special Room


    // Generate <Special Room Grid>
    private bool GenerateSpecialRoomGrid(int startRoomId)
    {
        int nextRoomId = startRoomId;

        CollectSpecialCandidatePositions();
        BuildSortedSpecialCandidateListByDistance();

        // 1. 보스방 먼저 배치
        for (int i = tempSpecialRooms.Count - 1; i >= 0; i--)
        {
            if (tempSpecialRooms[i].roomType != RoomGridType.boss)
                continue;

            if (specialCandidateSortedListCache.Count < tempSpecialRooms.Count)
                return false;

            if (!TryPlaceBossRoom(tempSpecialRooms[i], nextRoomId))
                return false;

            tempSpecialRooms.RemoveAt(i);
            nextRoomId++;
        }

        // 2. 나머지 특수방 배치
        while (tempSpecialRooms.Count > 0)
        {
            if (specialCandidateSortedListCache.Count < tempSpecialRooms.Count)
                return false;

            int index = tempSpecialRooms.Count - 1;

            if (!TryPlaceRandomSpecialRoom(tempSpecialRooms[index], nextRoomId))
                return false;

            tempSpecialRooms.RemoveAt(index);
            nextRoomId++;
        }

        return true;
    }

    // 현재 라운드에서 특수방이 배치될 수 있는 라운드를 선출
    private void CollectSpecialCandidatePositions()
    {
        specialCandidateSetCache.Clear();

        int i, adjacentSide;
        bool isCandidate;

        // 기존 라운드를 순회
        foreach (Vector2Int roundPos in roundPositions)
        {
            adjacentSide = 0;
            isCandidate = true;

            for (i = 0; i < FourDirs.Length; i++)
            {
                Vector2Int checkPos = roundPos + FourDirs[i];

                if (!existPosDict.TryGetValue(checkPos, out RoomGrid adjacentRoom))
                    continue;

                // 이미 특수방과 붙은 후보는 제외
                if (IsSpecialRoomType(adjacentRoom.roomType))
                {
                    isCandidate = false;
                    break;
                }

                adjacentSide++;

                if (adjacentSide > 1)
                {
                    isCandidate = false;
                    break;
                }
            }

            if (isCandidate && adjacentSide == 1)
                specialCandidateSetCache.Add(roundPos);
        }
    }

    // 특수방인가?
    private bool IsSpecialRoomType(RoomGridType roomType)
    {
        return roomType != RoomGridType.normal;
    }

    // 생성할 때, 문 하나만 생성이 될 것인가?
    private bool CanConnectBySingleGateOnly(int existLength)
    {
        int adjacentSide = 0;

        for (int i = 0; i < existLength; i++)
        {
            Vector2Int cellPos = tempAddExistPositions[i];

            for (int j = 0; j < FourDirs.Length; j++)
            {
                Vector2Int checkPos = cellPos + FourDirs[j];

                if (!existPosDict.TryGetValue(checkPos, out RoomGrid adjacentRoom))
                    continue;

                // 특수방끼리 인접 금지
                if (IsSpecialRoomType(adjacentRoom.roomType))
                    return false;

                adjacentSide++;

                // 문 후보가 2개 이상이면 실패
                if (adjacentSide > 1)
                    return false;
            }
        }

        return adjacentSide == 1;
    }


    private void InvalidateSpecialCandidatesAroundPlacedRoom(int existLength, int roundLength)
    {
        for (int i = specialCandidateSortedListCache.Count - 1; i >= 0; i--)
        {
            Vector2Int candidate = specialCandidateSortedListCache[i];

            bool remove = false;

            // 추가된 방의 실제 좌표 제거
            for (int j = 0; j < existLength; j++)
            {
                if (candidate == tempAddExistPositions[j])
                {
                    remove = true;
                    break;
                }
            }

            if (remove)
            {
                specialCandidateSortedListCache.RemoveAt(i);
                continue;
            }

            // 추가된 방의 테두리 좌표 제거
            for (int j = 0; j < roundLength; j++)
            {
                if (candidate == tempAddRoundPositions[j])
                {
                    remove = true;
                    break;
                }
            }

            if (remove)
                specialCandidateSortedListCache.RemoveAt(i);
        }
    }


    // 실제 기본 위치에 배치 시도
    private bool TryPlaceSpecialRoomAtBasePos(int type, Vector2Int basePos, int instanceId, RoomGridType roomType)
    {
        int i;

        GetRelativeExistPos(type, basePos, out int existLength);
        GetRelativeRoundPos(type, basePos, out int roundLength);

        // 1. 겹침 검사
        for (i = 0; i < existLength; i++)
        {
            if (existPositions.Contains(tempAddExistPositions[i]))
                return false;
        }

        // 2. 게이트가 정확히 1개인지 검사
        if (!CanConnectBySingleGateOnly(existLength))
            return false;

        // 3. 실제 방 추가
        RoomGrid room;
        if (roomType == RoomGridType.boss)
        {
            room = new BossRoomGrid(instanceId, type, existLength, roomType, bossRoomIndices[bossRoomIndices.Count - 1]);
            bossRoomIndices.RemoveAt(bossRoomIndices.Count - 1);
        }
        else
        {
            room = new RoomGrid(instanceId, type, existLength, roomType);
        }

#if UNITY_EDITOR
        RecordSnapshot(StageGridSnapshotStep.SpecialCandidate, -1, default, "Special Candidate Generated", specialCandidateSetCache.ToList());
#endif

        allRoomGrids.Add(room);

        for (i = 0; i < existLength; i++)
        {
            room.roomPos[i] = tempAddExistPositions[i];
            existPositions.Add(tempAddExistPositions[i]);
            existPosDict.Add(tempAddExistPositions[i], room);
        }

        // 4. roundPositions 갱신
        for (i = 0; i < roundLength; i++)
        {
            if (!existPositions.Contains(tempAddRoundPositions[i]))
                roundPositions.Add(tempAddRoundPositions[i]);
        }

        for (i = 0; i < existLength; i++)
        {
            roundPositions.Remove(tempAddExistPositions[i]);
        }

        // 5. 특수방 후보 리스트에서 불가능해진 좌표 제거
        InvalidateSpecialCandidatesAroundPlacedRoom(existLength, roundLength);

#if UNITY_EDITOR
        RecordSnapshot(roomType == RoomGridType.boss ? StageGridSnapshotStep.BossRoom : StageGridSnapshotStep.SpecialRoom, room.instanceId, basePos);
#endif

        return true;
    }


    // 후보 중에 배치 시도
    private bool TryPlaceSpecialRoomAtCandidate(ReserveRoom reserveRoom, Vector2Int candidatePos, int instanceId)
    {
        int type = reserveRoom.typeId;
        // ReserveRoom의 실제 필드명이 다르면 이 부분만 맞춰 바꾸면 됨.
        // 예: reserveRoom.typeIndex, reserveRoom.roomTypeIndex 등

        Vector2Int[] ownGrid = ownGridStaticData[type];

        for (int i = 0; i < ownGrid.Length; i++)
        {
            Vector2Int basePos = candidatePos - ownGrid[i];

            if (TryPlaceSpecialRoomAtBasePos(type, basePos, instanceId, reserveRoom.roomType))
                return true;
        }

        return false;
    }


    // 방 배치
    private bool TryPlaceBossRoom(ReserveRoom reserveRoom, int instanceId)
    {
        for (int i = specialCandidateSortedListCache.Count - 1; i >= 0; i--)
        {
            Vector2Int candidatePos = specialCandidateSortedListCache[i];

            if (TryPlaceSpecialRoomAtCandidate(reserveRoom, candidatePos, instanceId))
                return true;
        }

        return false;
    }

    private bool TryPlaceRandomSpecialRoom(ReserveRoom reserveRoom, int instanceId)
    {
        specialCandidateTryListCache.Clear();

        if (specialCandidateTryListCache.Capacity < specialCandidateSortedListCache.Count)
            specialCandidateTryListCache.Capacity = specialCandidateSortedListCache.Count;

        specialCandidateTryListCache.AddRange(specialCandidateSortedListCache);

        while (specialCandidateTryListCache.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, specialCandidateTryListCache.Count);

            Vector2Int candidatePos = specialCandidateTryListCache[randomIndex];

            int lastIndex = specialCandidateTryListCache.Count - 1;
            specialCandidateTryListCache[randomIndex] = specialCandidateTryListCache[lastIndex];
            specialCandidateTryListCache.RemoveAt(lastIndex);

            if (TryPlaceSpecialRoomAtCandidate(reserveRoom, candidatePos, instanceId))
                return true;
        }

        return false;
    }


    // 특수방 후보 라운드를 Set -> Sorted List로 변경
    private void BuildSortedSpecialCandidateListByDistance()
    {
        specialCandidateSortedListCache.Clear();

        if (specialCandidateSortedListCache.Capacity < specialCandidateSetCache.Count)
            specialCandidateSortedListCache.Capacity = specialCandidateSetCache.Count;

        specialCandidateSortedListCache.AddRange(specialCandidateSetCache);

        specialCandidateSortedListCache.Sort(GridDistanceAscendingComparer);
    }

    // 거리 측정
    private static readonly IComparer<Vector2Int> GridDistanceAscendingComparer = new GridDistanceComparer();
    private sealed class GridDistanceComparer : IComparer<Vector2Int>
    {
        public int Compare(Vector2Int a, Vector2Int b)
        {
            int disA = Mathf.Abs(a.x) + Mathf.Abs(a.y);
            int disB = Mathf.Abs(b.x) + Mathf.Abs(b.y);

            return disA.CompareTo(disB);
        }
    }

    // temp Special Rooms 를 복사 및 Boss Index 복사
    private void ResetTempSpecialRooms()
    {
        tempSpecialRooms.Clear();

        if (tempSpecialRooms.Capacity < specialRooms.Count)
            tempSpecialRooms.Capacity = specialRooms.Count;

        tempSpecialRooms.AddRange(specialRooms);

        // Boss
        bossRoomIndices.Clear();

        if (bossRoomIndices.Capacity < targetStageRule.bossIndices.Length)
            bossRoomIndices.Capacity = targetStageRule.bossIndices.Length;

        for (int i = 0; i < targetStageRule.bossIndices.Length; i++)
            bossRoomIndices.Add(targetStageRule.bossIndices[i]);

        // Elite
        eliteRoomIndices.Clear();

        if (eliteRoomIndices.Capacity < targetStageRule.eliteIndices.Length)
            eliteRoomIndices.Capacity = targetStageRule.eliteIndices.Length;

        for (int i = 0; i < targetStageRule.eliteIndices.Length; i++)
            eliteRoomIndices.Add(targetStageRule.eliteIndices[i]);
    }


    #endregion

    #region Elite Room

    // Elite Room Set
    private bool GenerateEliteRoomGrid()
    {
        if (targetStageRule.eliteAmount <= 0)
            return true;

        eliteRoomCandidateRoomCache.Clear();

        // 조건을 만족하는 Normal Room 수집
        for (int i = 0; i < allRoomGrids.Count; i++)
        {
            RoomGrid room = allRoomGrids[i];

            if (!IsValidEliteRoom(room))
                continue;

            eliteRoomCandidateRoomCache.Add(room);
        }

        if (eliteRoomCandidateRoomCache.Count < targetStageRule.eliteAmount)
            return false;

        Shuffle(eliteRoomCandidateRoomCache);
        for (int i = 0; i < targetStageRule.eliteAmount; i++)
        {
            RoomGrid room = eliteRoomCandidateRoomCache[i];
            room.roomType = RoomGridType.elite;
            room.eliteIndex = eliteRoomIndices[eliteRoomIndices.Count - 1];
            eliteRoomIndices.RemoveAt(eliteRoomIndices.Count - 1);
        }

#if UNITY_EDITOR
        RecordSnapshot(StageGridSnapshotStep.EliteRoom);
#endif

        return true;
    }

    // 엘리트 적 방 조건에 맞는가
    private bool IsValidEliteRoom(RoomGrid room)
    {
        if (room.roomType != RoomGridType.normal)
            return false;

        if (room.typeId != 0)
            return false;

        return true;
    }

    #endregion

    #region Gate

    // Generate <Gate Grid>
    private void GenerateGateGrid()
    {
        for (int i = 0; i < allRoomGrids.Count; i++)
        {
            RoomGrid room = allRoomGrids[i];
            room.gates.Clear();
            SetRoomGateGrid(room);
        }

        ConnectFacingGates();

#if UNITY_EDITOR
        RecordSnapshot(StageGridSnapshotStep.Gate, -1, default, "Generate Gate");
#endif
    }
    
    // 하나의 Room에서 연결 구조 계산
    private void SetRoomGateGrid(RoomGrid room)
    {
        Vector2Int pos, checkPos, dir;
        for (int i = 0; i < room.roomPos.Length; i++)
        {
            pos = room.roomPos[i];

            for (int j = 0; j < FourDirs.Length; j++)
            {
                dir = FourDirs[j];
                checkPos = pos + dir;

                if (!existPosDict.TryGetValue(checkPos, out RoomGrid connectRoom))
                    continue;

                // 같은 방이면 제외
                if (connectRoom == room)
                    continue;

                room.gates.Add(new GateGrid(pos, dir, room.instanceId, connectRoom.instanceId));

            }
        }
    }

    // 서로 마주보는 게이트 연결
    private void ConnectFacingGates()
    {
        for (int i = 0; i < allRoomGrids.Count; i++)
        {
            RoomGrid room = allRoomGrids[i];

            for (int j = 0; j < room.gates.Count; j++)
            {
                GateGrid gate = room.gates[j];

                if (gate.connectedGate != null)
                    continue;

                RoomGrid connectedRoom = allRoomGrids[gate.connectedRoom];

                if (connectedRoom == null)
                    continue;

                Vector2Int oppositeGatePos = gate.pos + gate.dir;
                Vector2Int oppositeGateDir = -gate.dir;

                GateGrid oppositeGate = FindGate(connectedRoom, oppositeGatePos, oppositeGateDir, room.instanceId);

                if (oppositeGate == null)
                    continue;

                gate.connectedGate = oppositeGate;
                oppositeGate.connectedGate = gate;
            }
        }
    }

    private GateGrid FindGate(RoomGrid room, Vector2Int pos, Vector2Int dir, int connectedRoomId)
    {
        for (int i = 0; i < room.gates.Count; i++)
        {
            GateGrid gate = room.gates[i];

            if (gate.pos != pos)
                continue;

            if (gate.dir != dir)
                continue;

            if (gate.connectedRoom != connectedRoomId)
                continue;

            return gate;
        }

        return null;
    }

    #endregion

    #region Room Caculator

    // Find Room Type -> Random
    private int GetRandomRoomType(RoomPercent[] roomPercents)
    {
        if (roomPercents == null || roomPercents.Length == 0)
            return -1;

        int i;

        float totalWeight = 0f;
        int lastValidType = -1; // 마지막 타입 체크

        for (i = 0; i < roomPercents.Length; i++)
        {
            if (roomPercents[i].percent <= 0f)
                continue;

            if (roomPercents[i].typeIndex < 0 || roomPercents[i].typeIndex >= roomTypeAmount)
                continue;

            totalWeight += roomPercents[i].percent;
            lastValidType = roomPercents[i].typeIndex;
        }

        if (totalWeight <= 0f)
            return -1;

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        int _typeIndex;
        for (i = 0; i < roomPercents.Length; i++)
        {
            float weight = roomPercents[i].percent;

            if (weight <= 0f)
                continue;

            _typeIndex = roomPercents[i].typeIndex;
            if (_typeIndex < 0 || _typeIndex >= roomTypeAmount)
                continue;

            currentWeight += weight;

            if (randomValue < currentWeight)
                return _typeIndex;
        }

        return lastValidType;
    }

    // round Pos Set
    private void GetRoundPositions(Vector2Int[] ownGrid)
    {
        roundPosSetCache.Clear();
        roundPosListCache.Clear();
        for (int i = 0; i < ownGrid.Length; i++)
        {
            roundPosSetCache.Add(ownGrid[i] + Vector2Int.left);
            roundPosSetCache.Add(ownGrid[i] + Vector2Int.up);
            roundPosSetCache.Add(ownGrid[i] + Vector2Int.right);
            roundPosSetCache.Add(ownGrid[i] + Vector2Int.down);
        }
        for (int i = 0; i < ownGrid.Length; i++)
        {
            roundPosSetCache.Remove(ownGrid[i]);
        }

        if (roundPosListCache.Capacity < roundPosSetCache.Count)
            roundPosListCache.Capacity = roundPosSetCache.Count;

        roundPosListCache.AddRange(roundPosSetCache);
    }
    // + Init
    private Vector2Int[] GetInitRoundPositions(Vector2Int[] ownGrid)
    {
        GetRoundPositions(ownGrid);
        return roundPosListCache.ToArray();
    }



    // Relative Pos
    private void GetRelativeExistPos(int type, Vector2Int pos, out int length)
    {
        Vector2Int[] originPos = ownGridStaticData[type];
        length = originPos.Length;
        for (int i = 0; i < length; i++)
            tempAddExistPositions[i] = originPos[i] + pos;
    }

    private void GetRelativeRoundPos(int type, Vector2Int pos, out int length)
    {
        Vector2Int[] originPos = roundOwnGridStaticData[type];
        length = originPos.Length;
        for (int i = 0; i < length; i++)
            tempAddRoundPositions[i] = originPos[i] + pos;
    }


    #endregion

    #region Shuffle
    public static void Shuffle<T>(T[] array)
    {
        if (array == null || array.Length <= 1)
            return;

        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
    }

    public static void Shuffle<T>(List<T> list)
    {
        if (list == null || list.Count <= 1)
            return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }


    #endregion

    #endregion
}

[Serializable]
public class StageObjectGenerator
{
    #region Variable

    // Comp
    [SerializeField] private Transform stageParentTf;

    private StageResoSO stagePrefab;
    private BuildResoSO buildPrefab;

    // Cell Size
    public static readonly Vector2Int offsetRoomSize = new Vector2Int(22, 14);

    public List<RoomController> currentAllRoomController { get; private set; } = new List<RoomController>();
    public List<EntranceRuleController> currentAllEntranceRoomController { get; private set; } = new List<EntranceRuleController>();

    // Stage Data
    public StageThemeSO currentStageThemeSO { get; private set; }
    private StageThemeSO beforeStageThemeSO;
    private StageThemeSO afterStageThemeSO;

    private StagePassageThemeSO currentStagePassageThemeSO;

    public int currentStageId { get { return currentStageThemeSO != null ? currentStageThemeSO.stageId : -1; } }
    public int beforeStageId { get { return beforeStageThemeSO != null ? beforeStageThemeSO.stageId : -1; } }
    public int afterStageId { get { return afterStageThemeSO != null ? afterStageThemeSO.stageId : -1; } }

    // Enemy
    private List<int> normalEnemyIndices;

    // Language
    public string[] stageNames;
    public string[] stageDescs;

#if UNITY_EDITOR
    private string generatorLogger;
#endif
    #endregion

    #region Init

    public IEnumerator Initialize(StageResoSO stagePrefab, BuildResoSO buildPrefab)
    {
        this.stagePrefab = stagePrefab;
        this.buildPrefab = buildPrefab;
        yield return null;
    }

    #endregion

    #region Addressables

    private IEnumerator LoadStageThemeSO(int stageId, System.Action<StageThemeSO> onComplete)
    {
        StageThemeSO result = null;

        yield return UniTask.ToCoroutine(async () =>
        {
            result = await AddressablesManager.LoadAsync<StageThemeSO>(StageAddress.Get(stageId));
        });

        onComplete?.Invoke(result);
    }
    private IEnumerator LoadStagePassageThemeSO(int beforeId, int afterId, System.Action<StagePassageThemeSO> onComplete)
    {
        StagePassageThemeSO result = null;

        yield return UniTask.ToCoroutine(async () =>
        {
            result = await AddressablesManager.LoadAsync<StagePassageThemeSO>(StagePassageAddress.Get(beforeId, afterId));
        });

        onComplete?.Invoke(result);
    }

    #endregion

    #region Generate

    // Lobby Or GamePlay 스테이지 생성
    public IEnumerator GenStage(int stageId, List<RoomGrid> allRoomGrids, List<int> normalEnemyIndices)
    {
        ResetData();

        if (currentStageThemeSO != null)
            AddressablesManager.Release(StageAddress.Get(currentStageThemeSO.stageId));
        yield return LoadStageThemeSO(stageId, so => currentStageThemeSO = so);
        if (currentStageThemeSO == null) yield break;
        stageNames = currentStageThemeSO.GetStageNames();
        stageDescs = currentStageThemeSO.GetStageDescs();

        // 로비 시작 방
        if (stageId == 99)
        {
            yield return GenerateLobbyStage();
        }
        else // 전투 스테이지 시작 방
        {
            if (normalEnemyIndices != null)
                this.normalEnemyIndices = normalEnemyIndices;

            yield return GenerateGamePlayStage(allRoomGrids);
            EliteEnemyController.isDroppedBossKeycard = false;
        }

        // 게이트 활성화
        if (stageId == 99)
            TryConnectLobbyGate();
        else
            TryConnectGate(allRoomGrids);

        // UI 셋
        MainGameUIManager.instance.hud.MinimapView.Gen_Minimap();
        MainGameUIManager.instance.hud.MinimapView.SetOnMapIcon(stageId);

        MainGameUIManager.instance.mapIntroUi.Play_IntroLabel();
        MainGameUIManager.instance.hud.MinimapView.SetStageDescription();

        // Sound (BGM) 시작
        SoundManager.instance.PlayCurrentStageBgm();

        // 로비는 시작 엘레베이터가 없기에 직접 인풋 키기
        if (stageId == 99)
        {
            PlayerManager.instance.playerController.Set_StartStage();
            EventManager.instance.Set_Input(true);
        }

        yield return null;
    }

    // Passage 스테이지 생성
    public IEnumerator GenPassageStage(int beforeStageId, int afterStageId)
    {
        // 전에 있는 데이터를 제거
        ResetData();

#if UNITY_EDITOR
        UnityEngine.Debug.Log($"{beforeStageId} -> {afterStageId}");
#endif
        yield return LoadStageThemeSO(beforeStageId, so => beforeStageThemeSO = so);
        yield return LoadStageThemeSO(afterStageId, so => afterStageThemeSO = so);

        AddressablesManager.Release(StageAddress.Get(currentStageId));

        // Gen
        yield return GenPassageRoom(afterStageId);

        // 게이트 활성화
        ResetAllGateState();

        // UI 셋
        MainGameUIManager.instance.hud.MinimapView.Gen_Minimap();
        MainGameUIManager.instance.hud.MinimapView.SetOffMapIcon();

        // Sound (BGM) 시작
        //SoundManager.Instance.Play_2D_BGM("Stage" + DevTool.Get_LengthString(stageData.InfoData.StageID, 2) + "_BGM");

        yield return null;
    }


    #endregion

    #region Generate - Lobby

    // Lobby 스테이지 생성
    private IEnumerator GenerateLobbyStage()
    {
#if UNITY_EDITOR
        generatorLogger = "Lobby Generate\n";
#endif
        yield return GenLobbyStartRoom(0, 0, Vector2Int.zero);
        yield return GenLobbyEntranceRoom(1, Vector2Int.up);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(generatorLogger);
        generatorLogger = null;
#endif
    }

    #endregion

    #region Generate - Lobby - Room

    // 로비 방 생성
    private IEnumerator GenLobbyStartRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Center</color>\n";
#endif
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomRuleLobbyPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetLobbyRoomToWorld(room, roomRule, instanceId, 0, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }

    private IEnumerator GenLobbyEntranceRoom(int instanceId, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Entrance</color>\n";
#endif
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomLobbyEntrancePrefab, stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomEntranceRuleLobbyPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        currentAllEntranceRoomController.Add(roomRule);

        roomRule.SetEntranceRuleInLobby();
        roomRule.SetElevatorData(0);

        SetLobbyRoomToWorld(room, roomRule, instanceId, 0, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }

    #endregion

    #region Generate - GamePlay

    private IEnumerator GenerateGamePlayStage(List<RoomGrid> allRoomGrids)
    {
#if UNITY_EDITOR
        generatorLogger = "GamePlay Generate\n";
#endif

        int entranceId = 0;
        for (int i = 0; i < allRoomGrids.Count; i++)
        {
            RoomGrid grid = allRoomGrids[i];
            Vector2Int pivot = grid.roomPos[0];
            switch (grid.roomType)
            {
                case RoomGridType.normal: yield return GenNormalRoom(grid, pivot); break;
                case RoomGridType.start: yield return GenStartRoom(grid, pivot); break;
                case RoomGridType.elite: yield return GenEliteRoom(grid, pivot); break;
                case RoomGridType.boss: yield return GenEntranceRoom(grid, pivot, entranceId++); break;
                case RoomGridType.vault: yield return GenVaultRoom(grid, pivot); break;
                case RoomGridType.baseShop: yield return GenShopRoom(grid, pivot); break;
                case RoomGridType.allyShop: yield return GenAllyShopRoom(grid, pivot); break;
                case RoomGridType.stPrison: yield return GenPrisonRoom(grid, pivot, 0); break;
                case RoomGridType.utPrison: yield return GenPrisonRoom(grid, pivot, 1); break;
                case RoomGridType.ntPrison: yield return GenPrisonRoom(grid, pivot, 2); break;
            }

        }
        EliteEnemyController.isDroppedBossKeycard = false;

#if UNITY_EDITOR
        UnityEngine.Debug.Log(generatorLogger);
        generatorLogger = null;
#endif
    }

    #endregion

    #region Generate - GamePlay - Room

    // 시작 방 생성
    private IEnumerator GenStartRoom(RoomGrid grid, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Start</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomStartPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms \n\n";
#endif
        yield return null;
    }

    // 기본 방 생성
    private IEnumerator GenNormalRoom(RoomGrid grid, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Normal</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(GetCorrectRandomRoomRule(gridTypeId), room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);

        roomRule.SetEnemyId(normalEnemyIndices);

#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }

    // 엘리트 적 방 생성
    private IEnumerator GenEliteRoom(RoomGrid grid, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Elite</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(GetCorrectRandomEliteRoomRule(), room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);

        roomRule.SetEnemyId(normalEnemyIndices, eliteId: grid.eliteIndex);

#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }


    // 통과 방 생성
    private IEnumerator GenEntranceRoom(RoomGrid grid, Vector2Int pos, int entranceId)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Entrance</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomEntrancePrefabs[entranceId], room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        currentAllEntranceRoomController.Add(roomRule);

        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);

        if (grid is BossRoomGrid bossGrid)
        {
            var indexData = bossGrid.bossRoomIndex;
            roomRule.SetElevatorData(indexData.nextStageIndex);
            roomRule.SetEnemyId(normalEnemyIndices, bossId: indexData.bossIndex);
        }
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }

    // 금고 방 생성
    private IEnumerator GenVaultRoom(RoomGrid grid, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Vault</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomVaultPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        roomRule.SetVault(
            UnityEngine.Object.Instantiate(DevTool.Get_RandomInList(buildPrefab.vaultPrefab.prefabs)),
            UnityEngine.Object.Instantiate(buildPrefab.repairOperPrefab.prefab, roomRule.inRoom_RepairOperactorParentTf),
            UnityEngine.Object.Instantiate(buildPrefab.vaultRerollOperPrefab.prefab, roomRule.inRoom_RerollOperactorParentTf),
            UnityEngine.Object.Instantiate(buildPrefab.vaultUpgradeOperPrefab.prefab, roomRule.inRoom_UpgradeOperactorParentTf));
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Object</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;

    }

    // 상점 방 생성
    private IEnumerator GenShopRoom(RoomGrid grid, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Shop</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomShopPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (data.usableBU)
            roomRule.SetBuShop(
                UnityEngine.Object.Instantiate(buildPrefab.buPrefab.prefab),
                UnityEngine.Object.Instantiate(buildPrefab.repairOperPrefab.prefab));
        if (data.usableMU)
            roomRule.SetMuShop(
                UnityEngine.Object.Instantiate(buildPrefab.muPrefab.prefab),
                UnityEngine.Object.Instantiate(buildPrefab.repairOperPrefab.prefab));
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Object (BU + MU)</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }

    // 동료 상점 방 생성
    private IEnumerator GenAllyShopRoom(RoomGrid grid, Vector2Int pos)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>AllyShop</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomAllyShopPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (data.usableABU)
            roomRule.SetAbuShop(
                UnityEngine.Object.Instantiate(buildPrefab.abuPrefab.prefab),
                UnityEngine.Object.Instantiate(buildPrefab.repairOperPrefab.prefab));
        if (data.usableAMU)
            roomRule.SetAmuShop(
                UnityEngine.Object.Instantiate(buildPrefab.amuPrefab.prefab),
                UnityEngine.Object.Instantiate(buildPrefab.repairOperPrefab.prefab));
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Object</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;


    }

    // 감옥 방 생성
    private IEnumerator GenPrisonRoom(RoomGrid grid, Vector2Int pos, int prisonTypeId)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger += "<color=yellow>Prison</color>\n";
#endif
        int gridTypeId = grid.typeId;
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[gridTypeId], stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomPrisonPrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetGamePlayRoomToWorld(room, roomRule, grid.instanceId, gridTypeId, pos);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        roomRule.SetPrison(
            UnityEngine.Object.Instantiate(buildPrefab.prisonPrefab.builds[prisonTypeId].prefab),
            UnityEngine.Object.Instantiate(buildPrefab.prisonPayOperPrefab.prefab),
            UnityEngine.Object.Instantiate(buildPrefab.prisonPuzzleOperPrefab.prefab));
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Object</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms\n\n";
#endif
        yield return null;
    }

    #endregion

    #region Generate - Passage - Room

    // 통로 방 생성
    private IEnumerator GenPassageRoom(int nextStageId)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        generatorLogger = "Passage Generate\n";
        generatorLogger += "<color=yellow>Passage</color>\n";
#endif
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPassagePrefab, stageParentTf);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Room</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomPassageRulePrefab, room.gameObject.transform);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Rule</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;

        yield return LoadStagePassageThemeSO(beforeStageId, afterStageId, so => currentStagePassageThemeSO = so);
        SetPassageRoomToWorld(room, roomRule, 0, 1, Vector2Int.zero);

        roomRule.Set_ElevatorData(nextStageId);
#if UNITY_EDITOR
        sw.Stop();
        generatorLogger += $"<color=orange>Init</color> <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
        yield return null;

#if UNITY_EDITOR
        UnityEngine.Debug.Log(generatorLogger);
        generatorLogger = null;
#endif
    }


    #endregion

    #region Connect Gate

    // 모든 문의 연결을 끊고, 제거하기
    private void ResetAllGateState()
    {
        for (int i = 0; i < currentAllRoomController.Count; i++)
        {
            RoomController room = currentAllRoomController[i];

            for (int j = 0; j < room.inRoom_AllGate.Count; j++)
            {
                GateController gate = room.inRoom_AllGate[j];

                gate.Set_ExistDoorState(false, null);
            }
        }
    }


    // 로비 게이트 초기화
    private void TryConnectLobbyGate()
    {
        ResetAllGateState();

        if (currentAllRoomController == null || currentAllRoomController.Count < 2)
            return;

        ConnectGate(
            currentAllRoomController[0], Vector2Int.zero, Vector2Int.up,
            currentAllRoomController[1], Vector2Int.zero, Vector2Int.down);
    }

    private void ConnectGate(
        RoomController roomA, Vector2Int gatePosA, Vector2Int gateDirA,
        RoomController roomB, Vector2Int gatePosB, Vector2Int gateDirB)
    {
        GateController gateA = FindGate(roomA, gatePosA, gateDirA);
        GateController gateB = FindGate(roomB, gatePosB, gateDirB);

        if (gateA == null || gateB == null)
            return;

        ConnectGatePair(gateA, gateB);
    }

    // 게이트 연결 시도
    private void TryConnectGate(List<RoomGrid> allRoomGrids)
    {
        if (allRoomGrids == null || allRoomGrids.Count == 0)
            return;

        ResetAllGateState();

        for (int i = 0; i < allRoomGrids.Count; i++)
        {
            RoomGrid grid = allRoomGrids[i];

            if (grid.instanceId < 0 || grid.instanceId >= currentAllRoomController.Count)
                continue;

            RoomController room = currentAllRoomController[grid.instanceId];

            for (int j = 0; j < grid.gates.Count; j++)
            {
                GateGrid gateGrid = grid.gates[j];

                if (gateGrid.connectedGate == null)
                    continue;

                if (gateGrid.connectedRoom < 0 || gateGrid.connectedRoom >= allRoomGrids.Count)
                    continue;

                Vector2Int localGatePos = gateGrid.pos - grid.roomPos[0];
                GateController gate = FindGate(room, localGatePos, gateGrid.dir);

                if (gate == null)
                    continue;

                if (gate.parterGate != null)
                    continue;

                RoomGrid connectedGrid = allRoomGrids[gateGrid.connectedRoom];

                if (connectedGrid.instanceId < 0 || connectedGrid.instanceId >= currentAllRoomController.Count)
                    continue;

                RoomController connectedRoom = currentAllRoomController[connectedGrid.instanceId];

                GateGrid partnerGateGrid = gateGrid.connectedGate;

                // 연결된 방 기준 로컬 게이트 좌표
                Vector2Int partnerLocalGatePos = partnerGateGrid.pos - connectedGrid.roomPos[0];

                GateController partnerGate = FindGate(
                    connectedRoom,
                    partnerLocalGatePos,
                    partnerGateGrid.dir
                );

                if (partnerGate == null)
                    continue;

                ConnectGatePair(gate, partnerGate);
            }
        }
    }

    // Room 내부 Gate 찾기
    private GateController FindGate(RoomController room, Vector2Int gatePos, Vector2Int gateDir)
    {
        if (room == null || room.inRoom_AllGate == null)
            return null;

        for (int i = 0; i < room.inRoom_AllGate.Count; i++)
        {
            GateController gate = room.inRoom_AllGate[i];

            if (gate.roomPosGate != gatePos || gate.gateDir != gateDir)
                continue;

            return gate;
        }

#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning(
            $"GateController를 찾을 수 없습니다. roomId: {room.id}, pos: {gatePos}, dir: {gateDir}"
        );
#endif

        return null;
    }

    // 실제 게이트 연결
    private void ConnectGatePair(GateController gate, GateController partnerGate)
    {
        gate.Set_ExistDoorState(true, partnerGate);
        partnerGate.Set_ExistDoorState(true, gate);

        int needKeyCardID = gate.thisRoom.roomRule.Get_NeedKeyCardID();

        if (needKeyCardID == -1)
            needKeyCardID = partnerGate.thisRoom.roomRule.Get_NeedKeyCardID();

        if (needKeyCardID != -1)
        {
            gate.Set_NeedKeyCard(needKeyCardID);
            partnerGate.Set_NeedKeyCard(needKeyCardID);
        }

        gate.Set_NextMap();
        partnerGate.Set_NextMap();
    }

    #endregion

    #region Data


    // 생성 전에, 전 스테이지 정보 데이터 초기화
    private void ResetData()
    {
        currentAllRoomController.Clear();
        currentAllEntranceRoomController.Clear();

        MainGameUIManager.instance.hud.MinimapView.AllRemoveMinimapCell();

        AddressablesManager.Release(StageAddress.Get(beforeStageId));
        AddressablesManager.Release(StageAddress.Get(afterStageId));
        AddressablesManager.Release(StagePassageAddress.Get(beforeStageId, afterStageId));

        beforeStageThemeSO = null;
        afterStageThemeSO = null;
    }

    #endregion

    #region Pos

    private void SetLobbyRoomToWorld(RoomController room, RoomRuleController roomRule, int instanceId, int typeId, Vector2Int gridPos)
    {
        currentAllRoomController.Add(room);
        room.Offset(roomRule, instanceId, typeId, gridPos, currentStageThemeSO);
        room.InitVisualSprite();

        roomRule.SetLobbyDontNeedKey();
    }

    private void SetGamePlayRoomToWorld(RoomController room, RoomRuleController roomRule, int instanceId, int typeId, Vector2Int gridPos)
    {
        currentAllRoomController.Add(room);
        room.Offset(roomRule, instanceId, typeId, gridPos, currentStageThemeSO);
        room.InitVisualSprite();
    }

    private void SetPassageRoomToWorld(PassageRoomController room, RoomRuleController roomRule, int instanceId, int typeId, Vector2Int gridPos)
    {
        currentAllRoomController.Add(room);
        room.Offset(roomRule, instanceId, typeId, gridPos,
            beforeStageThemeSO,
            afterStageThemeSO,
            currentStagePassageThemeSO);
        room.InitPassageVisualSprite();
    }


    #endregion

    #region Random Rule

    // 인덱스가 같은 RoomRule 찾기 (마지막엔 랜덤)
    private RoomRuleController GetCorrectRandomRoomRule(int typeId)
    {
        RoomRuleController[] roomRuleList = stagePrefab.roomRulePrefabs[typeId].array;
        return roomRuleList[UnityEngine.Random.Range(0, roomRuleList.Length)];
    }

    // 인덱스가 같은 RoomEliteRule 찾기
    private RoomRuleController GetCorrectRandomEliteRoomRule()
    {
        RoomRuleController[] roomRuleList = stagePrefab.roomElitePrefabs;
        return roomRuleList[UnityEngine.Random.Range(0, roomRuleList.Length)];
    }

    #endregion


    #region Anim

    public AnimationClip GetCurrentStageDoorAnim(Vector2Int doorDir)
    {
        List<StageDoorAnim> anims = currentStageThemeSO.mapDoorAnim;
        for (int i = 0; i < anims.Count; i++)
        {
            if (anims[i].dir == doorDir)
                return anims[i].doorAnim;
        }
        return null;
    }

    #endregion
}


public class StageManager : Singleton<StageManager>, IMainGameInitializer
{
    #region Init - Variable

    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [Space(20)]
    [SerializeField] public int targetStageId = -1;

    #endregion

    #region Init - Initialize

    // Init
    public IEnumerator Initialize()
    {
        yield return stageGridGenerator.Initialize();
        StaticResourceManager staticReso = StaticResourceManager.instance;
        yield return stageObjectGenerator.Initialize(staticReso.StageReso, staticReso.BuildReso);

        yield return GenerateStageCor(targetStageId);
    }

    #endregion


    #region  Stage Grid Generator - Variable

    [SerializeField] private StageGridGenerator stageGridGenerator;
    public StageGridGenerator StageGridGenerator { get { return stageGridGenerator; } }

    // Enemy
    private List<int> currentStageBossEnemyIndices = new List<int>(2);
    private List<int> currentStageEliteEnemyIndices = new List<int>(4);
    private List<int> currentStageNormalEnemyIndices = new List<int>(8);

    #endregion

    #region Stage Grid Generator - Generate
    private IEnumerator GenerateGridRoomData(int targetStageId)
    {
        yield return stageGridGenerator.GenerateGridData(targetStageId);
    }

#if UNITY_EDITOR
    [ContextMenu("GenerateGridRoomData")]
    private void GenerateGridRoomDataTest() // Test
    {
        stageGridGenerator.GenerateGridRoomDataTest();
    }
#endif

    #endregion

    #region Stage Grid Generator - Enemy

    private void SetEnemyIndices()
    {

        SetBossEnemyIndices();
        SetEliteEnemyIndices();
        SetNormalEnemyIndices();
    }

    private void SetBossEnemyIndices()
    {
        currentStageBossEnemyIndices.Clear();
        var bossRoom = stageGridGenerator.targetStageRule.bossIndices;

        if (currentStageBossEnemyIndices.Capacity < bossRoom.Length)
            currentStageBossEnemyIndices.Capacity = bossRoom.Length;

        for (int i = 0; i < bossRoom.Length; i++)
        {
            if (bossRoom[i].bossIndex != -1)
                currentStageBossEnemyIndices.Add(bossRoom[i].bossIndex);
        }
    }

    private void SetEliteEnemyIndices()
    {
        currentStageEliteEnemyIndices.Clear();
        var eliteRoom = stageGridGenerator.targetStageRule.eliteIndices;

        if (currentStageEliteEnemyIndices.Capacity < eliteRoom.Length)
            currentStageEliteEnemyIndices.Capacity = eliteRoom.Length;

        for (int i = 0; i < eliteRoom.Length; i++)
        {
            if (eliteRoom[i] != -1)
                currentStageEliteEnemyIndices.Add(eliteRoom[i]);
        }
    }

    private void SetNormalEnemyIndices()
    {
        currentStageNormalEnemyIndices.Clear();
        var bossRoom = stageGridGenerator.targetStageRule.enemyIndices;

        if (currentStageNormalEnemyIndices.Capacity < bossRoom.Length)
            currentStageNormalEnemyIndices.Capacity = bossRoom.Length;

        for (int i = 0; i < bossRoom.Length; i++)
        {
            if (bossRoom[i] != -1)
                currentStageNormalEnemyIndices.Add(bossRoom[i]);
        }
    }

    #endregion

    #region Stage Object Generator - Variable

    [SerializeField] public StageObjectGenerator stageObjectGenerator;
    public StageObjectGenerator StageObjectGenerator { get { return stageObjectGenerator; } }

    [SerializeField] public List<RoomController> currentAllRoomController => stageObjectGenerator.currentAllRoomController;
    [SerializeField] private List<EntranceRuleController> currentAllEntranceRoomController => stageObjectGenerator.currentAllEntranceRoomController;

    #endregion

    #region Stage Object Generator - Generate

    private IEnumerator GenerateRoomObjects(int targetStageId)
    {
        ReturnAllPoolObject();
        RemoveStageObject();
        yield return stageObjectGenerator.GenStage(targetStageId, stageGridGenerator.allRoomGrids, currentStageNormalEnemyIndices);
        yield return CustomGC.CollectAsync();

        StartCurrentRoom(currentAllRoomController[0]);

        if (currentAllRoomController[0].roomRule is StartRuleController start)
        {
            start.elevator.Play_MoveToTarget();
        }
    }
    private IEnumerator GeneratePassageRoomObjects(int afterStageID)
    {
        ReturnAllPoolObject();
        RemoveStageObject();
        yield return stageObjectGenerator.GenPassageStage(targetStageId, afterStageID);

        yield return CustomGC.CollectAsync();

        StartCurrentRoom(currentAllRoomController[0]);

        if (currentAllRoomController[0].roomRule is PassageRuleController passage)
        {
            passage.startingElevator.Play_MoveToTarget();
        }
    }

    private void ReturnAllPoolObject()
    {
        BulletManager.instance.ReturnAll();
        DropItemManager.instance.ReturnAll();
        VfxManager.instance.ReturnAll();
        AttackerManager.instance.ReturnAll();
        ExplosionManager.instance.ReturnAll();
    }

    private void RemoveStageObject()
    {
        for (int i = stageObjectGenerator.currentAllRoomController.Count - 1; i >= 0; i--)
        {
            Destroy(stageObjectGenerator.currentAllRoomController[i].gameObject);
        }

        stageObjectGenerator.currentAllRoomController.Clear();
        stageObjectGenerator.currentAllEntranceRoomController.Clear();

        currentAllRoomController.Clear();
        currentAllEntranceRoomController.Clear();

        currentRoomController = null;
    }

    #endregion


    #region Generate - Func

    // Lobby & Battle
    public void GenerateStage(int targetStageId)
    {
        this.targetStageId = targetStageId;
        StartCoroutine(GenerateStageCor(targetStageId));
    }

    private IEnumerator GenerateStageCor(int targetStageId)
    {
        yield return EnemyManager.instance.DestoryEnemyPoolsAsync();

        if (targetStageId != 99)
        {
            yield return GenerateGridRoomData(targetStageId);
            SetEnemyIndices();
            yield return EnemyManager.instance.SetEnemyPoolsAsync(currentStageNormalEnemyIndices, currentStageEliteEnemyIndices, currentStageBossEnemyIndices);
        }

        yield return GenerateRoomObjects(targetStageId);
    }

    public void GeneratePassageStage(int afterStageID)
    {
        StartCoroutine(GeneratePassageStageCor(afterStageID));
    }

    public IEnumerator GeneratePassageStageCor(int afterStageID)
    {
        yield return EnemyManager.instance.DestoryEnemyPoolsAsync();

        yield return GeneratePassageRoomObjects(afterStageID);
    }

    #endregion

    #region Field Obj
    public SpriteMaterial GetRandomFieldObjSprite(int typeId)
    {
        if (currentStageThemeSO == null || currentStageThemeSO.stageId == 99)
            return null;

        SpriteMaterial[] objSprites = currentStageThemeSO.fieldObjSprites[typeId].array;
        return objSprites[UnityEngine.Random.Range(0, objSprites.Length)];
    }

    public DestructibleObjectController GetRandomFieldObjPrefab()
    {
        var fieldObjs = StaticResourceManager.instance.StageReso.fieldObjPrefabs;
        return fieldObjs[UnityEngine.Random.Range(0, fieldObjs.Length)];
    }

    #endregion

    #region Variable

    [Space(10)]

    // Nav
    [SerializeField] private NavMeshSurface navMesh;

    public StageThemeSO currentStageThemeSO => stageObjectGenerator.currentStageThemeSO;
    public RoomController currentRoomController;

    #endregion

    #region Play

    // Start Room
    public void StartCurrentRoom(RoomController targetRoom)
    {
        StartCoroutine(StartCurrentRoomCor(targetRoom));
    }

    private IEnumerator StartCurrentRoomCor(RoomController targetRoom)
    {
        if (targetRoom == null)
        {
            UnityEngine.Debug.LogWarning("TargetRoom is NULL");
            yield break;
        }

        // 현재 방 선택
        currentRoomController = targetRoom;

        DevTool.Set_Active(currentAllRoomController, false);

        currentRoomController.gameObject.SetActive(true);
        currentRoomController.SetSortingStaticObjects();

        // Ally
        AllyManager.instance.Set_AllAllyPlayerNearPos();
        AllyManager.instance.Stop_AllAllies_Combat();

        // Minimap
        MainGameUIManager.instance.hud.MinimapView.Set_State();

        yield return new WaitForSeconds(0.2f);

        PlayerManager.instance.playerController.SetOn_Trail();
        targetRoom.Play_RoomState();
        navMesh.BuildNavMesh();

        // Minimap
        MainGameUIManager.instance.hud.MinimapView.Set_State();
        MainGameUIManager.instance.hud.MinimapView.Play_Effect();

        // Ally
        AllyManager.instance.Start_AllAllies_Combat();
    }


    // Start Room -> Elite & Boss
    public void StartEliteRoom(GateController gate, int id)
    {
        MainGameUIManager.instance.battleProdUi.Play_BattleOnProd(
            EnemyManager.instance.GetEliteProdSprite(id),
            StaticResourceManager.instance.enemyNames.GetLanguage(id),
            out float durTime);

        StartCoroutine(StartBattleRoom(gate, durTime));
    }

    public void StartBossRoom(GateController gate, int id)
    {
        MainGameUIManager.instance.battleProdUi.Play_BattleOnProd(
            EnemyManager.instance.GetBossProdSprite(id),
            StaticResourceManager.instance.enemyNames.GetLanguage(id),
            out float durTime);

        StartCoroutine(StartBattleRoom(gate, durTime));
    }

    private IEnumerator StartBattleRoom(GateController gate, float durTime)
    {
        EventManager.instance.Set_Input(false);

        yield return new WaitForSeconds(durTime);

        gate.EnterGate();
        MainGameUIManager.instance.battleProdUi.Play_BattleOffProd(out float outDurTime);

        yield return new WaitForSeconds(outDurTime);

        EventManager.instance.Set_Input(true);
    }


    // Complete -> Kill All
    public void CompleteRoomKillAll()
    {
        if (currentRoomController == null || currentRoomController.roomRule.roomType == eRoomType.Completed) return;

        StartCoroutine(CompleteRoomKillAllCor());
    }

    public IEnumerator CompleteRoomKillAllCor()
    {
        yield return new WaitForSeconds(0.5f);

        if (EnemyManager.instance.currentEnemies.Count <= 0)
        {
            currentRoomController.PlaySet_RoomStateComplete();

            // 상호작용 UI 변경 (문이나 아이템에 붙어있을 때, 상황을 바꾸어줌)
            PlayerManager.instance.playerController.SetInteractable();

            // Minimap
            MainGameUIManager.instance.hud.MinimapView.Set_State();
        }
    }

    #endregion

    #region Shift Vault

    public VaultController GetVaultCorrectType(Type typeVault)
    {
        var vaults = StaticResourceManager.instance.BuildReso.vaultPrefab.prefabs;
        for (int i = 0; i < vaults.Length; i++)
        {
            VaultController vault = vaults[i];
            if (vault.GetType() == typeVault)
                return vault;
        }

        return null;
    }

    #endregion
}