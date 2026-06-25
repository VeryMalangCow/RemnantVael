using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StageGridGenerator
{
    #region Class & Struct

    // 방 생성 시 규칙
    public class StageRule
    {
        public int minRoomAmount;
        public int maxRoomAmount;
        public RoomPercent[] percents;
        public int[] bossIndices;
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


    // 방 그리드
    public class RoomGrid
    {
        public int instanceId;
        public int typeId;
        public Vector2Int[] roomPos;
        public RoomGridType roomType;

        public List<GateGrid> gates;

        public RoomGrid(int instanceId, int typeId, int roomPosLenght, RoomGridType roomType)
        {
            this.instanceId = instanceId;
            this.typeId = typeId;
            this.roomPos = new Vector2Int[roomPosLenght];
            gates = new List<GateGrid>();
            this.roomType = roomType;
        }
    }


    // 게이트 그리드
    public struct GateGrid
    {
        public Vector2Int pos;
        public Vector2Int dir;
        public int connectedRoom;

        public GateGrid(Vector2Int pos, Vector2Int dir, int connectedRoom)
        {
            this.pos = pos;
            this.dir = dir;
            this.connectedRoom = connectedRoom;
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
        start, normal, vault, baseShop, allyShop, stPrison, utPrison, ntPrison, boss
    }

    #endregion

    #region Variable

    [Header("=== Rule")]
    [SerializeField] private TextAsset stageRuleCSV;
#if UNITY_EDITOR
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

        string[] lines = stageRuleCSV.text.Split("\n");
        int lineIndex = stageId + 1;
        if (lineIndex < 0 || lineIndex >= lines.Length)
        {
            UnityEngine.Debug.LogError($"StageRule CSV 범위 초과. stageId: {stageId}");
            return false;
        }

        string[] cols = lines[lineIndex].Trim().Split(",");
        int bossIndexColumn = roomTypeAmount + 4;
        if (cols.Length <= bossIndexColumn)
        {
            UnityEngine.Debug.LogError($"StageRule CSV 컬럼 부족. stageId: {stageId}");
            return false;
        }

        rule.minRoomAmount = int.TryParse(cols[1], out int min) ? min : 1;
        rule.maxRoomAmount = int.TryParse(cols[2], out int max) ? max : 10;
        if (rule.maxRoomAmount < rule.minRoomAmount) rule.maxRoomAmount = rule.minRoomAmount;

        rule.percents = new RoomPercent[roomTypeAmount];
        for (int i = 0; i < roomTypeAmount; i++)
            rule.percents[i] = new RoomPercent(i, float.TryParse(cols[i + 3], out float typePercent) ? typePercent : 0);

        string[] bossIndices = cols[roomTypeAmount + 4].Trim().Split("/");
        rule.bossIndices = new int[bossIndices.Length];
        for (int i = 0; i < rule.bossIndices.Length; i++)
            rule.bossIndices[i] = int.TryParse(bossIndices[i], out int bossIndex) ? bossIndex : -1;

        return true;
    }

    #endregion

    #region Total Room

    // Generate <All Type Room>
    private IEnumerator GenerateRoomGrid()
    {
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
                    GenerateGateGrid();
                    success = true;
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
                if (targetStageRule.bossIndices[i] != -1)
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
        RoomGrid room = new RoomGrid(instanceId, type, existLength, roomType);
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

        return true;
    }

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

    // temp Special Rooms 를 복사
    private void ResetTempSpecialRooms()
    {
        tempSpecialRooms.Clear();

        if (tempSpecialRooms.Capacity < specialRooms.Count)
            tempSpecialRooms.Capacity = specialRooms.Count;

        tempSpecialRooms.AddRange(specialRooms);
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

                room.gates.Add(new GateGrid(pos, dir, connectRoom.instanceId));

            }
        }
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

    #endregion

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
        int maxSuccess = 1000;
        int currentSuccess = 0;
        int currentFail = 0;

        int maxTotalTry = 10000;
        int totalTry = 0;

        while (maxSuccess > currentSuccess && totalTry < maxTotalTry)
        {
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
                GenerateGateGrid();
                return true;
            }
        }

        return false;
    }

#endif
}

[System.Serializable]
public class StageTheme
{
    [SerializeField] public StageData lobbyStageData;
    [SerializeField] public List<StageData> allStageData;
    [HideInInspector] public AllPassageMiddleSpriteData passageMiddleSpriteData;

    private MapReso[] stageMapReso;
    private MapReso passageMapReso;

    private Sprite[][][] mapFieldObjList_Data;

    private static int eachKindOfMapAmount = 2;
    public static int kindOfMapAmount = 2;
    public static int kindOfFieldObjType = 3;

    [HideInInspector] private GameObject[] fieldObjArray;

    public IEnumerator Initialize()
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        // Offset
        lobbyStageData.Offset(GetAsset_MapReso("Sprite/Map/", "MapLobby"));

        test();
        for (int i = 0; i < allStageData.Count; i++)
        {
            allStageData[i].Offset(stageMapReso[i]);
        }
        // => ResoucreManager에서 리소스를 가져오고 난 다음, 호출문
        passageMiddleSpriteData = new AllPassageMiddleSpriteData(passageMapReso);

        // Field
        List<Sprite[][]> mapFieldObjList_Data = new List<Sprite[][]>();
        for (int i = 0; i < stageMapReso.Length; i++)
        {
            mapFieldObjList_Data.Add(Get_FieldObj(stageMapReso[i]));
        }
        this.mapFieldObjList_Data = mapFieldObjList_Data.ToArray();

        // Field Obj
        string path = "Prefab/FieldObj/";
        fieldObjArray = new GameObject[kindOfFieldObjType];

        for (int i = 0; i < kindOfFieldObjType; i++)
            fieldObjArray[i] = GetAsset<GameObject>(path, $"FieldObj_T{DevTool.Get_LengthString(i, 2)}");

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"StageManager : <color=orange>SpriteOffset</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }
    private T GetAsset<T>(string path, string fileName) where T : UnityEngine.Object
    => Resources.Load<T>(path + fileName);
    // Get
    public GameObject Get_RandomFieldObj_Prefab() => fieldObjArray[UnityEngine.Random.Range(0, fieldObjArray.Length)];

    public Sprite Get_RandomFieldObjSprite(int stageID, int typeID)
    {
        if (stageID >= mapFieldObjList_Data.Length) UnityEngine.Debug.Log("Stage ID : " + stageID);

        Sprite[] spriteArr = mapFieldObjList_Data[stageID][typeID];
        return spriteArr[UnityEngine.Random.Range(0, spriteArr.Length)];
    }

    private void test()
    {
        string path = $"Sprite/Map/";

        List<MapReso> resos = new List<MapReso>();
        for (int i = 0; i < kindOfMapAmount; i++)
        {
            string stageName = $"Map{DevTool.Get_LengthString(i, 2)}";
            resos.Add(GetAsset_MapReso("Sprite/Map/", stageName));
        }
        stageMapReso = resos.ToArray();

        string passageName = $"MapPassage/";
        passageMapReso = GetAsset_MapReso(path, passageName);

        // Kind of Map / Type / List
        List<Sprite[][]> mapFieldObjList_Data = new List<Sprite[][]>();
        for (int i = 0; i < stageMapReso.Length; i++)
        {
            mapFieldObjList_Data.Add(Get_FieldObj(stageMapReso[i]));
        }
        this.mapFieldObjList_Data = mapFieldObjList_Data.ToArray();
    }

    private Sprite[][] Get_FieldObj(MapReso reso) // Type / SpriteList
    {
        List<List<Sprite>> result = new List<List<Sprite>>();

        for (int i = 0; i < kindOfFieldObjType; i++)
        {
            result.Add(new List<Sprite>());
        }

        for (int i = 0; i < reso.mapResoElements.Length; i++)
        {
            string[] name = reso.mapResoElements[i].sprite.name.Split("_");
            if (name[1] == "FieldObj")
            {
                int type = Int32.Parse(name[2].Substring(1, 2));
                result[type].Add(reso.mapResoElements[i].sprite);
            }
        }

        List<Sprite[]> result2 = new List<Sprite[]>();
        for (int i = 0; i < result.Count; i++)
        {
            result2.Add(result[i].ToArray());
        }

        return result2.ToArray();
    }

    private T[] GetAsset_Arr<T>(string path, string fileName = "") where T : UnityEngine.Object
    => Resources.LoadAll<T>(path + fileName);

    private MapReso GetAsset_MapReso(string path, string name)
    {
        List<MapResoElement> mapResoElements = new List<MapResoElement>();

        for (int i = 0; i < eachKindOfMapAmount; i++)
        {
            Sprite[] spriteArr = GetAsset_Arr<Sprite>(path + name + "/", $"{name}_{DevTool.Get_LengthString(i, 3)}");

            for (int j = 0; j < spriteArr.Length; j++)
            {
                mapResoElements.Add(new MapResoElement(spriteArr[j], i));
            }
        }

        return new MapReso(mapResoElements.ToArray());
    }
}

[System.Serializable]
public class StageObjectGenerator
{
    #region Variable

    // Comp
    [SerializeField] private Transform stageParentTf;

    private StagePrefabSO stagePrefab;

    // Cell Size
    private static readonly Vector2Int offsetRoomSize = new Vector2Int(22, 14);

    public List<RoomController> currentAllRoomController { get; private set; } = new List<RoomController>();
    public List<EntranceRuleController> currentAllEntranceRoomController = new List<EntranceRuleController>();

    #endregion

    #region Init

    public IEnumerator Initialize(StagePrefabSO stagePrefab)
    {
        this.stagePrefab = stagePrefab;
        yield return null;
    }

    #endregion

    public IEnumerator GenStage(StageTheme stageTheme, int stageId, List<StageGridGenerator.RoomGrid> allRoomGrids)
    {
        ResetData();

        currentStageData = GetCollectStageData(stageTheme, stageId);
        if (currentStageData == null) 
            yield break;

        // 로비 시작 방
        if (stageId == 99) 
        {
            yield return GenerateLobbyStage();
        }
        else // 전투 스테이지 시작 방
        {
            yield return GenerateGamePlayStage(currentStageData, allRoomGrids);
            EliteEnemyController.isDroppedBossKeycard = false;
        }

        // Entrance 활성화
        //Set_EntranceIndex(stageId);

        // 게이트 활성화
        //Set_GateActiveOn();

        // UI 셋
        MainGameUIManager.instance.mapIntroUi.Play_IntroLabel();
        MainGameUIManager.instance.hud.MinimapView.Gen_Minimap();
        MainGameUIManager.instance.hud.MinimapView.SetOnMapIcon(stageId);
        MainGameUIManager.instance.hud.MinimapView.SetStageDescription();

        // Sound (BGM) 시작
        SoundManager.instance.Play_2D_BGM_Stage(currentStageData.infoData.stageId);

        if (stageId == 99)
            Play_LobbyStart_Cor();

        yield return null;
    }

    private void Play_LobbyStart_Cor()
    {
        PlayerManager.instance.playerController.Set_StartStage();
        EventManager.instance.Set_Input(true);
    }


    #region Generate - Lobby

    // Lobby 스테이지 생성
    private IEnumerator GenerateLobbyStage()
    {
        int instanceId = 0;

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        GenLobbyStartRoom(roomTypeId: 0, instanceId++, Vector2Int.zero);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Generate : Lobby Stage : Center : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        GenLobbyEntranceRoom(instanceId++, Vector2Int.up);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Generate : Lobby Stage : Center : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms");
#endif
        yield return null;
    }

    // 로비 방 생성
    private void GenLobbyStartRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomRuleLobbyPrefab, room.gameObject.transform);

        SetLobbyRoomToWorld(room, roomRule, instanceId, pos);
    }

    private void GenLobbyEntranceRoom(int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomLobbyEntrancePrefab, stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomEntranceRuleLobbyPrefab, room.gameObject.transform);

        currentAllEntranceRoomController.Add(roomRule);

        roomRule.Set_EntranceRuleInLobby();

        SetLobbyRoomToWorld(room, roomRule, instanceId, pos);
    }

    #endregion

    #region Generate - GamePlay

    private IEnumerator GenerateGamePlayStage(StageData stageData, List<StageGridGenerator.RoomGrid> allRoomGrids)
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        string s = "";
#endif
        int instanceId = 0;

        int i, entranceId = 0;
        for (i = 0; i < allRoomGrids.Count; i++)
        {
#if UNITY_EDITOR
            sw.Restart();
#endif
            StageGridGenerator.RoomGrid grid = allRoomGrids[i];
            switch (grid.roomType)
            {
                case StageGridGenerator.RoomGridType.normal: GenNormalRoom(grid.typeId, instanceId++, grid.roomPos[0]); break;
                case StageGridGenerator.RoomGridType.start: GenStartRoom(grid.typeId, instanceId++, grid.roomPos[0]); break;
                case StageGridGenerator.RoomGridType.boss: GenEntranceRoom(grid.typeId, instanceId++, grid.roomPos[0], entranceId++); break;
                case StageGridGenerator.RoomGridType.vault: GenVaultRoom(grid.typeId, instanceId++, grid.roomPos[0]); break;
                case StageGridGenerator.RoomGridType.baseShop: GenShopRoom(grid.typeId, instanceId++, grid.roomPos[0]); break;
                case StageGridGenerator.RoomGridType.allyShop: GenAllyShopRoom(grid.typeId, instanceId++, grid.roomPos[0]); break;
                case StageGridGenerator.RoomGridType.stPrison: GenPrisonRoom(grid.typeId, instanceId++, grid.roomPos[0], 0); break;
                case StageGridGenerator.RoomGridType.utPrison: GenPrisonRoom(grid.typeId, instanceId++, grid.roomPos[0], 1); break;
                case StageGridGenerator.RoomGridType.ntPrison: GenPrisonRoom(grid.typeId, instanceId++, grid.roomPos[0], 2); break;
            }

#if UNITY_EDITOR
            sw.Stop();
            s += $"<color=red>{sw.Elapsed.TotalMilliseconds:F2}</color>ms / ";
#endif
            yield return null;
        }
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"Generate : GamePlay Stage : {s}");
#endif
    }

    #endregion

    #region Generate - GamePlay - Room


    // 시작 방 생성
    private void GenStartRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomStartPrefab, room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);
    }

    // 기본 방 생성
    private void GenNormalRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(Get_CorrectRandomRoomRule(roomTypeId), room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);
    }

    // 통과 방 생성
    private void GenEntranceRoom(int roomTypeId, int instanceId, Vector2Int pos, int entranceId)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomEntrancePrefabs[entranceId], room.gameObject.transform);

        currentAllEntranceRoomController.Add(roomRule);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);
    }

    // 금고 방 생성
    private void GenVaultRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomVaultPrefab, room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);

        roomRule.SetVault(
            UnityEngine.Object.Instantiate(DevTool.Get_RandomInList(stagePrefab.vaultPrefabs)),
            UnityEngine.Object.Instantiate(stagePrefab.repairOperPrefab, roomRule.inRoom_RepairOperactorParentTf),
            UnityEngine.Object.Instantiate(stagePrefab.vaultRerollOperPrefab, roomRule.inRoom_RerollOperactorParentTf),
            UnityEngine.Object.Instantiate(stagePrefab.vaultUpgradeOperPrefab, roomRule.inRoom_UpgradeOperactorParentTf));
    }

    // 상점 방 생성
    private void GenShopRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomShopPrefab, room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);

        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (data.usableBU)
            roomRule.SetBuShop(
                UnityEngine.Object.Instantiate(stagePrefab.buPrefab),
                UnityEngine.Object.Instantiate(stagePrefab.repairOperPrefab));
        if (data.usableMU)
            roomRule.SetMuShop(
                UnityEngine.Object.Instantiate(stagePrefab.muPrefab),
                UnityEngine.Object.Instantiate(stagePrefab.repairOperPrefab));
    }

    // 동료 상점 방 생성
    private void GenAllyShopRoom(int roomTypeId, int instanceId, Vector2Int pos)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomAllyShopPrefab, room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);

        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (data.usableABU)
            roomRule.SetAbuShop(
                UnityEngine.Object.Instantiate(stagePrefab.abuPrefab),
                UnityEngine.Object.Instantiate(stagePrefab.repairOperPrefab));
        if (data.usableAMU)
            roomRule.SetAmuShop(
                UnityEngine.Object.Instantiate(stagePrefab.amuPrefab),
                UnityEngine.Object.Instantiate(stagePrefab.repairOperPrefab));
    }

    // 감옥 방 생성
    private void GenPrisonRoom(int roomTypeId, int instanceId, Vector2Int pos, int prisonTypeId)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPrefabs[roomTypeId], stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomPrisonPrefab, room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, instanceId, pos);

        roomRule.SetPrison(
            UnityEngine.Object.Instantiate(stagePrefab.prisonPrefabs[prisonTypeId]),
            UnityEngine.Object.Instantiate(stagePrefab.prisonPayOperPrefab),
            UnityEngine.Object.Instantiate(stagePrefab.prisonPuzzleOperPrefab));

    }

    // 통로 방 생성
    private void GenPassageRoom(int nextStageId)
    {
        var room = UnityEngine.Object.Instantiate(stagePrefab.roomPassagePrefab, stageParentTf);
        var roomRule = UnityEngine.Object.Instantiate(stagePrefab.roomPassageRulePrefab, room.gameObject.transform);

        SetGamePlayRoomToWorld(room, roomRule, 0, Vector2Int.zero);

        roomRule.Set_ElevatorData(nextStageId);
    }


    #endregion


    #region Data

    // 올바른 Stage Data 구하기
    public StageData GetCollectStageData(StageTheme stageTheme, int stageId)
    {
        if (stageTheme == null)
            return null;

        // Lobby
        if (stageId == 99)
            return stageTheme.lobbyStageData;

        // Game
        StageData stageData = stageTheme.allStageData[stageId];
        if (stageId == stageData.infoData.stageId)
            return stageData;

        // 만약 Id가 올바르지않다면 순회해서 탐색
        for (int i = 0; i < stageTheme.allStageData.Count; i++)
        {
            stageData = stageTheme.allStageData[i];
            if (stageData.infoData.stageId == stageId)
                return stageData;
        }

        return null;
    }

    // 생성 전에, 전 스테이지 정보 데이터 초기화
    private void ResetData()
    {
        currentAllRoomController.Clear();

        MainGameUIManager.instance.hud.MinimapView.AllRemoveMinimapCell();

        beforeStageId = -1;
        afterStageId = -1;

        beforeStageData = null;
        afterStageData = null;
    }

    #endregion

    #region Pos

    private void SetLobbyRoomToWorld(RoomController room, RoomRuleController roomRule, int instanceId, Vector2Int gridPos)
    {
        currentAllRoomController.Add(room);

        room.Offset(instanceId, roomRule);
        room.gameObject.transform.position = GetGridVecToWorldPos(room.roomVec[0] + gridPos);
    }

    private void SetGamePlayRoomToWorld(RoomController room, RoomRuleController roomRule, int instanceId, Vector2Int gridPos)
    {
        currentAllRoomController.Add(room);

        room.Offset(instanceId, roomRule);
        room.gameObject.transform.position = GetGridVecToWorldPos(room.roomVec[0] + gridPos);

        room.Spawn_FieldObj();
    }

    private Vector2 GetGridVecToWorldPos(Vector2 gridVec)
        => gridVec * offsetRoomSize;


    #endregion

    #region Random Rule

    // 인덱스가 같은 RoomRule 찾기 (마지막엔 랜덤)
    private RoomRuleController Get_CorrectRandomRoomRule(int typeId)
    {
        RoomRuleController[] roomRuleList = stagePrefab.roomRulePrefabs[typeId].array;
        return roomRuleList[UnityEngine.Random.Range(0, roomRuleList.Length)];
    }

    #endregion


    #region Temp - Variable

    [Header("=== Current")]

    // 클리어와 클리어 전 머터리얼 셋 
    [HideInInspector] public StageData currentStageData;

    // Passage
    [HideInInspector] private int beforeStageId = -1;
    [HideInInspector] private int afterStageId = -1;
    [HideInInspector] private StageData beforeStageData;
    [HideInInspector] private StageData afterStageData;

    #endregion


    #region Stage

    // 통로 스테이지 생성
    public void GenPassageStage(int nextStageId)
    {
        // 전에 있는 데이터를 제거
        ResetData();

        // Gen
        GenPassageRoom(nextStageId);

        // 게이트 활성화
        Set_GateActiveOn();

        // UI 셋
        MainGameUIManager.instance.hud.MinimapView.Gen_Minimap();
        MainGameUIManager.instance.hud.MinimapView.SetOffMapIcon();

        // Sound (BGM) 시작
        //SoundManager.Instance.Play_2D_BGM("Stage" + DevTool.Get_LengthString(stageData.InfoData.StageID, 2) + "_BGM");
    }


    #endregion

    #region Set

    #region Gate

    // 게이트에 모든 짝꿍 게이트 지정과 세팅
    private void Set_ParterAllGate()
    {
        List<GateController> allGate = Get_AllGate(currentAllRoomController);

        for (int i = 0; i < allGate.Count - 1; i++)
        {
            // 이미 파트너 게이트가 있다면
            if (allGate[i].parterGate != null) continue;

            for (int j = i + 1; j < allGate.Count; j++)
            {
                if (Is_PartnerGate(allGate[i], allGate[j]))
                {
                    allGate[i].parterGate = allGate[j];
                    allGate[j].parterGate = allGate[i];
                }
            }
        }
    }

    // 반대편에 방에 존재하는 게이트인지 + 서로 바라보고 있는지
    private bool Is_PartnerGate(GateController gate1, GateController gate2)
        => ((gate1.roomPosGate + gate1.gateDir) == gate2.roomPosGate) && (gate1.gateDir * -1) == gate2.gateDir;


    // 현재 게이트 모두 활성화
    private void Set_GateActiveOn()
    {
        Set_ParterAllGate();
        List<GateController> allGate = Get_AllGate(currentAllRoomController);
        for (int i = 0; i < allGate.Count; i++)
        {
            if (allGate[i].parterGate != null)
            {
                // 게이트 활성화
                allGate[i].Set_ExistDoorState(true);

                // 게이트가 특정 방의 게이트라면 특정 필요 키카드 삽입
                int needKeyCardID = allGate[i].thisRoom.roomRule.Get_NeedKeyCardID();
                if (needKeyCardID != -1)
                {
                    allGate[i].Set_NeedKeyCard(needKeyCardID);
                    allGate[i].parterGate.Set_NeedKeyCard(needKeyCardID);
                }

                // Next Map Icon
                allGate[i].Set_NextMap();
            }
            else
            {
                // 게이트 비활성화
                allGate[i].Set_ExistDoorState(false);
            }
        }
    }

    #endregion

    #region SR

    public void Set_PassageMiddleSprite(StageTheme stageTheme, SpriteRenderer sr, string key)
    {
        Sprite data = stageTheme.passageMiddleSpriteData.Get_CorrectSprite(key, out int materialIndex);
        if (data == null) return;

        sr.sprite = data;
        sr.material = ResourceManager.instance.Get_PassageMiddleMaterial(materialIndex);
    }

    public void Set_CurrentMapSprite(SpriteRenderer sr, string spriteKey)
        => Set_MapUnclearSprite(currentStageData, sr, spriteKey);


    public void Set_BeforeMapSprite(SpriteRenderer sr, string spriteKey)
        => Set_MapClearSprite(beforeStageData, sr, spriteKey);


    public void Set_AfterMapSprite(SpriteRenderer sr, string spriteKey)
        => Set_MapClearSprite(afterStageData, sr, spriteKey);




    private void Set_MapUnclearSprite(StageData stageData, SpriteRenderer sr, string spriteKey)
    {
        if (!stageData.mapSpriteReso.mapSprite.ContainsKey(spriteKey)) { UnityEngine.Debug.Log(spriteKey); return; }

        SpriteMaterial spriteMatrial = stageData.mapSpriteReso.mapSprite[spriteKey];
        sr.sprite = spriteMatrial.sprite;
        sr.material = stageData.mapMaterialUnclear[spriteMatrial.materialIndex];
    }

    private void Set_MapClearSprite(StageData stageData, SpriteRenderer sr, string spriteKey)
    {
        if (!stageData.mapSpriteReso.mapSprite.ContainsKey(spriteKey)) { UnityEngine.Debug.Log(spriteKey); return; }

        SpriteMaterial spriteMatrial = stageData.mapSpriteReso.mapSprite[spriteKey];
        sr.sprite = spriteMatrial.sprite;
        sr.material = stageData.mapMaterialClear[spriteMatrial.materialIndex];
    }

    #endregion

    #region Anim

    public void Set_StageDoorAnim(GateController gate, SpriteRenderer sr, Vector2Int doorDir)
    {
        List<StageDoorAnim> doorAnim = currentStageData.mapDoorAnim;
        for (int i = 0; i < doorAnim.Count; i++)
            if (doorAnim[i].dir == doorDir)
            {
                gate.ac = doorAnim[i].doorAnim;
                sr.material = currentStageData.mapMaterialUnclear[doorAnim[i].materialIndex];
            }
    }



    #endregion

    #endregion


    #region Gate 

    private List<GateController> Get_AllGate(List<RoomController> roomList)
    {
        List<GateController> allGate = new List<GateController>();
        for (int i = 0; i < roomList.Count; i++)
            allGate.AddRange(roomList[i].inRoom_AllGate);

        return allGate;
    }

    #endregion

    #region Minimap

    public CoupleData<Sprite> Get_CorrectMinimapIcon(RoomRuleController roomRule)
    {
        switch (roomRule)
        {
            case VaultRuleController:
                return ResourceManager.instance.vault_Icon;

            case EntranceRuleController:
                return ResourceManager.instance.elevator_Icon;

            case ShopRuleController:
                return ResourceManager.instance.shop_Icon;

            case AllyShopRuleController:
                return ResourceManager.instance.allyShop_Icon;

            case PrisonRuleController prisonRule:
                if (prisonRule.prison is StrikeTeamPrisonController)
                    return ResourceManager.instance.st_Prison_Icon;
                else if (prisonRule.prison is UplinkTeamPrisonController)
                    return ResourceManager.instance.ut_Prison_Icon;
                else if (prisonRule.prison is NeoTeamPrisonController)
                    return ResourceManager.instance.nt_Prison_Icon;
                else
                    return null;

            default:
                return null;
        }

    }

    #endregion

    #region Vault

    public GameObject Get_VaultCorrectType(Type typeVault)
    {
        for (int i = 0; i < ResourceManager.instance.vaultPrefabArr.Length; i++)
            if (DevTool.Get_ComponentTType<VaultController>(ResourceManager.instance.vaultPrefabArr[i]).GetType() == typeVault)
                return ResourceManager.instance.vaultPrefabArr[i];

        return null;
    }

    #endregion

    #region Play (Spawn another Passage Stage)

    public void Play_GenPassageStage(StageTheme stageTheme, int beforeStageId, int afterStageId)
    {
        this.beforeStageId = beforeStageId;
        this.afterStageId = afterStageId;
        UnityEngine.Debug.Log($"{beforeStageId} -> {afterStageData}");

        beforeStageData = GetCollectStageData(stageTheme, this.beforeStageId);
        afterStageData = GetCollectStageData(stageTheme, this.afterStageId);

        GenPassageStage(afterStageId);
    }


    public int Get_BeforeStageID()
    {
        return beforeStageId;
    }

    public int Get_AfterStageID()
    {
        return afterStageId;
    }

    #endregion

   


}

[System.Serializable]
public class StageData
{
    public string mapIndexName;

    [Space(5)]
    public StageInfo infoData;

    [Space(5)]
    public StageRoom roomData;

    [Space(5)]
    public List<Material> mapMaterialUnclear;
    public List<Material> mapMaterialClear;
    public List<StageDoorAnim> mapDoorAnim;

    [HideInInspector] public List<Sprite> allMapSprite;
    [HideInInspector] public StageMapSprite mapSpriteReso;

    public void Offset(MapReso reso)
    {
        mapSpriteReso.Offset(reso, mapIndexName);
    }
}


public class StageManager : Singleton<StageManager>, IMainGameInitializer
{
    #region Init - Variable

    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [SerializeField] public int targetStageId = -1;

    #endregion

    #region Init - Initialize

    // Init
    public IEnumerator Initialize()
    {
        yield return stageTheme.Initialize();
        yield return stageGridGenerator.Initialize();
        yield return stageObjectGenerator.Initialize(stagePrefab);

        yield return GenerateStage(targetStageId);
    }

    #endregion

    [Space(10)]

    #region Stage Resource - Variable

    [SerializeField] private StagePrefabSO stagePrefab;
    [SerializeField] public StageTheme stageTheme;

    #endregion


    #region  Stage Grid Generator - Variable

    [SerializeField] public StageGridGenerator stageGridGenerator;

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


    #region Stage Object Generator - Variable

    [SerializeField] public StageObjectGenerator stageObjectGenerator;

    [SerializeField] public List<RoomController> currentAllRoomController => stageObjectGenerator.currentAllRoomController;
    [SerializeField] private List<EntranceRuleController> currentAllEntranceRoomController => stageObjectGenerator.currentAllEntranceRoomController;

    #endregion

    #region Stage Object Generator - Generate

    private IEnumerator GenerateRoomObjects(int targetStageId)
    {
        RemoveStageObject();
        yield return stageObjectGenerator.GenStage(stageTheme, targetStageId, stageGridGenerator.allRoomGrids);

        // 처음 스타트맵
        StartCurrentRoom(GetCorrectRoom(0));
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

        currentSetSprites.Clear();
        currentSetAnims.Clear();

        currentRoomController = null;
    }

    #endregion


    #region Generate

    public void GenerateStageNext(int targetStageId)
    {
        this.targetStageId = targetStageId;
        StartCoroutine(GenerateStage(targetStageId));
    }

    private IEnumerator GenerateStage(int targetStageId)
    {
        if (targetStageId != 99)
            yield return GenerateGridRoomData(targetStageId);

        yield return GenerateRoomObjects(targetStageId);
    }

    #endregion

    [Space(10)]

    #region Variable

    // Nav
    [SerializeField] private NavMeshSurface navMesh;

    private HashSet<BuildSetSpriteController> currentSetSprites = new HashSet<BuildSetSpriteController>();
    private HashSet<BuildSetAnimController> currentSetAnims = new HashSet<BuildSetAnimController>();

    public RoomController currentRoomController;

    #endregion


    public StageData GetCurrentStageData()
        => stageObjectGenerator.GetCollectStageData(stageTheme, targetStageId);
    
    // Start Room
    public void StartCurrentRoom(RoomController targetRoom)
    {
        StartCoroutine(StartCurrentRoomCor(targetRoom));
    }

    private IEnumerator StartCurrentRoomCor(RoomController targetRoom)
    {
        if (targetRoom == null) yield break;

        // 필요없는 유닛 제거
        currentSetSprites.Clear();
        currentSetAnims.Clear();

        // 현재 방 선택
        currentRoomController = targetRoom;

        DevTool.Set_Active(currentAllRoomController, false);

        currentRoomController.gameObject.SetActive(true);
        currentRoomController.Set_SortingStaticObjects();

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
            ResourceManager.instance.Get_EnemyName(id),
            out float durTime);

        StartCoroutine(StartBattleRoom(gate, durTime));
    }

    public void StartBossRoom(GateController gate, int id)
    {
        MainGameUIManager.instance.battleProdUi.Play_BattleOnProd(
            EnemyManager.instance.GetBossProdSprite(id),
            ResourceManager.instance.Get_EnemyName(id),
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

        if (EnemyManager.instance.currentEnemyList.Count <= 0)
        {
            currentRoomController.PlaySet_RoomStateComplete();

            // 상호작용 UI 변경 (문이나 아이템에 붙어있을 때, 상황을 바꾸어줌)
            PlayerManager.instance.playerController.SetInteractable();

            // Minimap
            MainGameUIManager.instance.hud.MinimapView.Set_State();
        }
    }



    #region Room


    // id에 맞는 방
    public RoomController GetCorrectRoom(int id)
    {
        if (currentAllRoomController == null) return null;

        RoomController room = currentAllRoomController[id];
        if (room.Get_ID() == id)
            return room;

        for (int i = 0; i < currentAllRoomController.Count; i++)
        {
            room = currentAllRoomController[i];
            if (room.Get_ID() == id)
                return room;
        }

        return null;
    }

    #endregion


    public void Add_SetSprite(BuildSetSpriteController setSprite)
    {
        currentSetSprites.Add(setSprite);
    }

    public void Add_SetAnim(BuildSetAnimController setAnim)
    {
        currentSetAnims.Add(setAnim);
    }
    public void Set_SetAnimClearly()
    {
        if (currentSetAnims == null || currentSetAnims.Count <= 0) return;

        foreach (BuildSetAnimController setAnim in currentSetAnims)
        {
            if (DevTool.Get_ComponentTType(setAnim.gameObject, out SpriteRenderer sr))
            {
                int index = stageObjectGenerator.currentStageData.mapMaterialUnclear.IndexOf(sr.sharedMaterial);
                if (index == -1)
                { UnityEngine.Debug.Log(sr.material.name + " / " + sr.gameObject.transform.parent.gameObject.name); continue; }
                sr.material = stageObjectGenerator.currentStageData.mapMaterialClear[index];
            }
        }
    }
    public void Set_SetSpriteClearly()
    {
        if (currentSetSprites == null || currentSetSprites.Count <= 0) return;

        foreach (BuildSetSpriteController setSprite in currentSetSprites)
        {
            if (DevTool.Get_ComponentTType(setSprite.gameObject, out SpriteRenderer sr))
            {
                int index = stageObjectGenerator.currentStageData.mapMaterialUnclear.IndexOf(sr.sharedMaterial);
                if (index == -1)
                { UnityEngine.Debug.Log(sr.gameObject.name + " / " + sr.gameObject.transform.parent.gameObject.name); continue; }
                sr.material = stageObjectGenerator.currentStageData.mapMaterialClear[index];
            }
        }
    }

    public void Set_PassageMiddleSprite(SpriteRenderer sr, string key)
    {
        stageObjectGenerator.Set_PassageMiddleSprite(stageTheme, sr, key);
    }

    public void Play_GenPassageStage(int afterStageID)
    {
        stageObjectGenerator.Play_GenPassageStage(stageTheme, targetStageId, afterStageID);
    }
}