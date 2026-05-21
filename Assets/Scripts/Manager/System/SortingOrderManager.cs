using System.Collections.Generic;
using UnityEngine;

public class SortingOrderManager : Singleton<SortingOrderManager>
{
    #region Struct

    [System.Serializable]
    private struct DepthEntry : System.IEquatable<DepthEntry>
    {
        public DepthController depth;
        public float lastY;
        public bool isRemoved;

        public bool Equals(DepthEntry other)
        {
            return depth == other.depth;
        }
    }

    #endregion

    #region Value

    public readonly static int order_BuildUpper = 1;
    public readonly static int order_SortingObjTop = 5000;
    public readonly static int order_EffectImg = 9999;
    public readonly static int order_BuildLower = 10000;
    public readonly static int order_DoorIcon = 19998;
    public readonly static int order_Explosion = 19999;
    public readonly static int order_Aim = 20000;
    public readonly static int order_DmgTxt = 20001;



    [Header("=== Movable Object")]
    [SerializeField] private List<DepthEntry> sortedObjs = new List<DepthEntry>(256);
    [SerializeField] private List<DepthController> movableDepths = new List<DepthController>(128);

    private List<DepthController> requestAddDepths = new List<DepthController>(64);
    private bool existAddDepth = false;

    private List<DepthController> requestRemoveDepths = new List<DepthController>(64);
    private bool existRemoveDepth = false;

    private int minAddOrRemoveIndex = int.MaxValue;

    private bool isDirty = false;
    private bool isRangeExtended = false;

    private int dirtyStartIndex = int.MaxValue;
    private int dirtyEndIndex = int.MinValue;

    #endregion

    #region MonoBehaviour

    private void Update()
    {
        HandleRemoveSortObjs();
        HandleDirtyAll();
        HandleAddSortObjs();
    }

    private void LateUpdate()
    {
        HandleApplySortingOrder();
    }

    #endregion

    #region Swap

    // 모든 객체 중 움직임이 가능한 객체를 체크 (움직임만 확인)
    private void HandleDirtyAll()
    {
        for (int i = 0; i < movableDepths.Count; i++)
        {
            int sortIndex = movableDepths[i].sortIndex; 
            
            if (sortIndex < 0 || sortIndex >= sortedObjs.Count)
                throw new System.Exception("movableDepths의 sortIndex 범위 오류");

            DepthEntry entry = sortedObjs[sortIndex];

            if (entry.depth != movableDepths[i])
                throw new System.Exception("Depth 움직임을 감지하는데 다른 Depth 접근");

            float currentY = entry.depth.GetPosY();

            if (Mathf.Abs(currentY - entry.lastY) > 0.01f) // 미세한 오차 방지
            {
                entry.lastY = currentY;
                sortedObjs[sortIndex] = entry;
                IsActualDirty(sortIndex, currentY);
            }
        }
    }


    // 신호가 들어오면 실제로 해당 Depth가 Dirty인지 판별 (실제 순서 바뀌었는지)
    private void IsActualDirty(int index, float newY)
    {
        // 해당 인덱스 요소의 Y값 변경
        if (IsDirtyForUp(index))
        {
            SortOrderMoveUpDepth(index);
        }
        else if (IsDirtyForDown(index))
        {
            SortOrderMoveDownDepth(index);
        }
    }

    // 상단과 비교
    private bool IsDirtyForUp(int index)
    {
        // 상단 Depth보다 Y값이 크다면
        return (index > 0 && sortedObjs[index].lastY > sortedObjs[index - 1].lastY);
    }

    // 하단과 비교
    private bool IsDirtyForDown(int index)
    {
        // 하단 Depth보다 Y값이 작으면
        return (index < sortedObjs.Count - 1 && sortedObjs[index].lastY < sortedObjs[index + 1].lastY);
    }
    
    // 상단으로 이동 (올바른 값 찾을 때까지)
    private void SortOrderMoveUpDepth(int index)
    {
        while (IsDirtyForUp(index))
        {
            SwapInOrderList(index, index - 1);
            index--;
        }
    }

    // 하단으로 이동 (올바른 값 찾을 때까지)
    private void SortOrderMoveDownDepth(int index)
    {
        while (IsDirtyForDown(index))
        {
            SwapInOrderList(index, index + 1);
            index++;
        }
    }

    // 두 객체 순서 변동
    private void SwapInOrderList(int indexA, int indexB)
    {
        DepthEntry a = sortedObjs[indexA];
        DepthEntry b = sortedObjs[indexB];

        sortedObjs[indexA] = b;
        sortedObjs[indexA].depth.SetSortIndex(indexA);
        sortedObjs[indexB] = a;
        sortedObjs[indexB].depth.SetSortIndex(indexB);

        UpdateRange(indexA);
        UpdateRange(indexB);

        if (!isDirty) isDirty = true;
    }

    // Start와 End 위치 셋
    private void UpdateRange(int index)
    {
        if (dirtyStartIndex > index) dirtyStartIndex = index;
        if (dirtyEndIndex < index) dirtyEndIndex = index;
    }

    #endregion

    #region Apply Sorting Order Value

    // Set Sort를 해야하는지 (IsDirty 판별)
    private void HandleApplySortingOrder()
    {
        if (isDirty)
        {
            ApplySortingOrder();
        }
    }

    // 실제 Sorting Order 값 적용 (실제 변동 영역을 기반으로 값 적용)
    // !!! 실제 랜더 비용 발생 가능성 존재 !!!
    private void ApplySortingOrder()
    {
        // 범위 보정
        int start = Mathf.Max(0, dirtyStartIndex);
        int end = isRangeExtended ? sortedObjs.Count - 1 : Mathf.Min(sortedObjs.Count - 1, dirtyEndIndex);

        for (int i = start; i <= end; i++)
            sortedObjs[i].depth.SetSortingOrder(order_SortingObjTop + (10 * i));
        
        isDirty = false;
        isRangeExtended = false;

        dirtyStartIndex = int.MaxValue;
        dirtyEndIndex = int.MinValue;
    }

    #endregion

    #region Add


    // 삽입 예약
    public void RequestAddSortObj(DepthController depth)
    {
        if (requestRemoveDepths.Remove(depth))
        {
            existRemoveDepth = requestRemoveDepths.Count > 0;
            return;
        }

        if (!requestAddDepths.Contains(depth))
        {
            requestAddDepths.Add(depth);
            existAddDepth = true;
        }
    }

    // 삽입 함수
    private void HandleAddSortObjs()
    {
        if (!existAddDepth)
            return;

        for (int i = 0; i < requestAddDepths.Count; i++)
            AddSortObj(requestAddDepths[i]);
        
        for (int i = minAddOrRemoveIndex; i < sortedObjs.Count; i++)
            sortedObjs[i].depth.SetSortIndex(i);

        dirtyStartIndex = Mathf.Min(dirtyStartIndex, minAddOrRemoveIndex);
        isRangeExtended = true;
        isDirty = true;

        requestAddDepths.Clear();
        minAddOrRemoveIndex = int.MaxValue;
        existAddDepth = false;
    }

    // 삽입 (개별)
    private void AddSortObj(DepthController depth)
    {
        float y = depth.GetPosY();
        int targetIndex = FindInsertIndex(y); // 이진 탐색
        if (minAddOrRemoveIndex > targetIndex) minAddOrRemoveIndex = targetIndex;
        sortedObjs.Insert(targetIndex, new DepthEntry() 
        { 
            depth = depth, 
            lastY = y 
        });

        if (depth.IsSortingMover() && !movableDepths.Contains(depth))
            movableDepths.Add(depth);
    }

    // 이진 탐색
    private int FindInsertIndex(float y)
    {
        int low = 0;
        int high = sortedObjs.Count - 1;
        int result = sortedObjs.Count;

        while (low <= high)
        {
            int mid = (low + high) / 2;

            if (y > sortedObjs[mid].lastY)
            {
                result = mid;
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }

        return result;
    }

    #endregion

    #region Remove

    // 삭제 예약
    public void RequestRemoveSortObj(DepthController depth)
    {
        if (requestAddDepths.Remove(depth))
        {
            existAddDepth = requestAddDepths.Count > 0;
            return;
        }

        if (!requestRemoveDepths.Contains(depth))
        {
            requestRemoveDepths.Add(depth);
            existRemoveDepth = true;
        }
    }

    // 삭제 함수
    private void HandleRemoveSortObjs()
    {
        if (!existRemoveDepth)
            return;

        for (int i = 0; i < requestRemoveDepths.Count; i++)
            RemoveSortObj(requestRemoveDepths[i]);

        RemoveMarkAll();

        for (int i = minAddOrRemoveIndex; i < sortedObjs.Count; i++)
            sortedObjs[i].depth.SetSortIndex(i);

        dirtyStartIndex = Mathf.Min(dirtyStartIndex, minAddOrRemoveIndex);
        isRangeExtended = true;
        isDirty = true;

        requestRemoveDepths.Clear();
        minAddOrRemoveIndex = int.MaxValue;
        existRemoveDepth = false;
    }

    // 제거 (개별)
    private void RemoveSortObj(DepthController depth)
    {
        int targetIndex = depth.sortIndex;

        if (targetIndex < 0 || targetIndex >= sortedObjs.Count)
            return;

        if (sortedObjs[targetIndex].depth != depth)
            throw new System.Exception("Remove 대상과 sortIndex가 일치하지 않음");

        if (minAddOrRemoveIndex > targetIndex) minAddOrRemoveIndex = targetIndex;
        sortedObjs[targetIndex] = new DepthEntry()
        {
            isRemoved = true
        };
        depth.SetSortIndex(-1);

        if (depth.IsSortingMover())
            movableDepths.Remove(depth);
    }

    // removed mark 시스템으로 실제 제거 (복사 및 크기 조절)
    private void RemoveMarkAll()
    {
        int size = sortedObjs.Count;
        int write = 0;

        for (int read = 0; read < size; read++)
        {
            if (sortedObjs[read].isRemoved)
                continue;

            if (read != write)
                sortedObjs[write] = sortedObjs[read];

            write++;
        }

        if (write < size)
            sortedObjs.RemoveRange(write, size - write);
    }

    #endregion

}
