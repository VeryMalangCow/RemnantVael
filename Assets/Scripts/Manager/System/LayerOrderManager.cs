using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Struct

    [System.Serializable]
    private struct DepthEntry : System.IEquatable<DepthEntry>
    {
        public DepthController depth;
        [HideInInspector] public float lastY;
        [HideInInspector] public float newY;
        [HideInInspector] public bool isRemoved;
        [HideInInspector] public bool isMover;

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
    [SerializeField] private List<DepthEntry> needSortingObjects = new List<DepthEntry>(256);
    [SerializeField] private List<DepthEntry> dirtySortingObjects = new List<DepthEntry>(128);

    private bool isDirty = false;
    private bool isRangeExtended = false;

    private int dirtyStartIndex = int.MaxValue;
    private int dirtyEndIndex = int.MinValue;

    #endregion

    #region MonoBehaviour

    private void Update()
    {
        CheckIsDirtyAll();
    }

    private void LateUpdate()
    {
        HandleSort();
    }

    private void HandleSort()
    {
        if (isDirty)
        {
            SortDirty();
        }
    }

    #endregion

    #region Dirty

    private void CheckIsDirtyAll()
    {
        for (int i = 0; i < needSortingObjects.Count; i++)
        {
            DepthEntry entry = needSortingObjects[i];

            if (entry.depth == null) continue;

            if (entry.isMover)
            {
                float currentY = entry.depth.GetPosY();

                if (Mathf.Abs(currentY - entry.lastY) > 0.01f) // 미세한 오차 방지
                {
                    entry.lastY = currentY;
                    entry.newY = currentY;
                    needSortingObjects[i] = entry;
                    CheckIsDirty(i, currentY);
                }
            }
        }
    }

    // 신호가 들어오면 실제로 해당 Depth가 Dirty인지 판별
    public void CheckIsDirty(int index, float newY)
    {
        // 해당 인덱스 요소의 Y값 변경
        DepthEntry element = needSortingObjects[index];
        element.newY = newY;
        needSortingObjects[index] = element;

        // 상단과 비교
        if (index > 0)
        {
            // 상단 Depth보다 Y값이 크다면
            if (needSortingObjects[index].newY > needSortingObjects[index - 1].newY)
            {
                AddDirty(index);
                return;
            }
        }
        // 하단과 비교
        if (index < needSortingObjects.Count - 1)
        {
            // 하단 Depth보다 Y값이 작으면
            if (needSortingObjects[index].newY < needSortingObjects[index + 1].newY)
            {
                AddDirty(index);
                return;
            }
        }
    }

    // Dirty List에 추가
    private void AddDirty(int index)
    {
        DevTool.Add_InList(dirtySortingObjects, needSortingObjects[index]);
        needSortingObjects[index] = new DepthEntry { isRemoved = true };
        isDirty = true;

        UpdateRange(index);
    }

    // Dirty List값들을 솔팅
    private void SortDirty()
    {
        RemoveAll();

        for (int i = 0; i < dirtySortingObjects.Count; i++)
        {
            DepthEntry targetDepthEntry = dirtySortingObjects[i];
            targetDepthEntry.newY = targetDepthEntry.depth.GetPosY();
            
            int low = 0;
            int high = needSortingObjects.Count - 1;
            int j = needSortingObjects.Count;
            int mid;

            while (low <= high)
            {
                mid = (low + high) / 2;
                if (targetDepthEntry.newY > needSortingObjects[mid].newY)
                {
                    j = mid;
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }

            needSortingObjects.Insert(j, targetDepthEntry);

            if (j < dirtyStartIndex) dirtyStartIndex = j;
            if (j <= dirtyEndIndex) dirtyEndIndex++;
            else dirtyEndIndex = j;
        }

        if (isRangeExtended) 
        {
            dirtyEndIndex = needSortingObjects.Count - 1; 
        }
        
        ApplySortingOrder(needSortingObjects);
    }

    // 실제 Sorting Order 값 적용
    private void ApplySortingOrder(List<DepthEntry> objectList)
    {
        dirtySortingObjects.Clear();

        // 혹시 모르는 보정
        int start = Mathf.Max(0, dirtyStartIndex);
        int end = Mathf.Min(objectList.Count - 1, dirtyEndIndex);

        for (int i = start; i <= end; i++)
        {
            objectList[i].depth.SetSortingOrder(order_SortingObjTop + (10 * i));
            objectList[i].depth.SetSortIndex(i);
        }

        isDirty = false;
        isRangeExtended = false;

        dirtyStartIndex = int.MaxValue;
        dirtyEndIndex = int.MinValue;
    }

    // 시작점, 종료점 판별
    private void UpdateRange(int index)
    {
        if (dirtyStartIndex > index) dirtyStartIndex = index;
        if (dirtyEndIndex < index) dirtyEndIndex = index;
    }


    #endregion

    #region List

    // 솔팅 오브젝트과 세팅 값 초기화
    public void ClearNeedSortObj()
    {
        for (int i = 0; i < needSortingObjects.Count; i++)
        {
            if (needSortingObjects[i].depth != null)
                    needSortingObjects[i].depth.SetSortIndex(-1); // 모든 객체의 인덱스 초기화
        }
        needSortingObjects.Clear(); 
        dirtySortingObjects.Clear();

        isDirty = false;
        isRangeExtended = false;

        dirtyStartIndex = int.MaxValue;
        dirtyEndIndex = int.MinValue;
    }

    // 솔팅이 필요한 Depth를 List에 추가 (Dirty 리스트에 추가)
    private void RegisterDepth(DepthController depth, bool isMover = true)
    {
        float y = depth.GetPosY();
        DevTool.Add_InList(dirtySortingObjects,
            new DepthEntry
            {
                lastY = y,
                newY = y,
                depth = depth,
                isMover = isMover
            }); 
    }

    // (개별)
    public void AddNeedSortObj(DepthController depth, bool isMover = true)
    {
        RegisterDepth(depth, isMover);

        isDirty = true;
        isRangeExtended = true;
    }

    // (다중)
    public void AddNeedSortObj<T>(List<T> depths, bool isMover = true) where T : DepthController
    {
        for (int i = 0; i < depths.Count; i++)
            RegisterDepth(depths[i], isMover);

        isDirty = true;
        isRangeExtended = true;
    }

    // 솔팅에 필요하지않은 Depth를 List에서 제거
    public void RemoveNeedSortObj(DepthController depth)
    {
        int index = depth.sortingElementIndex;

        // 1. 인덱스가 -1인 경우 (방금 추가됐거나, 이미 삭제됐거나, Clear 된 경우)
        if (index == -1)
        {
            // "방금 추가된 상태"일 수 있으니 Dirty 리스트만 빠르게 확인
            for (int i = 0; i < dirtySortingObjects.Count; i++)
            {
                if (dirtySortingObjects[i].depth == depth)
                {
                    dirtySortingObjects.RemoveAt(i);
                    isDirty = true;
                    break;
                }
            }
            return; // 메인 리스트(needSortingObjects)에는 어차피 없거나 이미 마크됨
        }

        // 2. 인덱스가 있는 경우: 안전하게 순회 삭제
        // (O(1) 접근 후 mismatch 시 순회하는 방식이 베스트지만,
        // 사용자님 말씀대로 그냥 순회하는 게 속 편하고 안전하다면 이 로직만 써도 무방합니다.)
        for (int i = 0; i < needSortingObjects.Count; i++)
        {
            if (needSortingObjects[i].depth == depth)
            {
                needSortingObjects[i] = new DepthEntry { isRemoved = true };
                UpdateRange(i);
                isDirty = true;
                isRangeExtended = true;
                break;
            }
        }

        depth.SetSortIndex(-1);
    }

    // 실제 필요없는 값 List에서 제거
    private void RemoveAll()
    {
        int size = needSortingObjects.Count;
        int write = 0;
        int removedInRange = 0;
        bool needsIndexFix = !isRangeExtended;

        for (int read = 0; read < size; read++)
        {
            if (needSortingObjects[read].isRemoved)
            {
                // EndIndex보다 앞에서 지워진 것만 카운트 (그 뒤는 어차피 범위 밖이라 상관없음)
                if (needsIndexFix && read <= dirtyEndIndex)
                    removedInRange++;
                continue;
            }

            if (read != write)
                needSortingObjects[write] = needSortingObjects[read];

            write++;
        }

        if (size != write)
        {
            if (needsIndexFix)
                dirtyEndIndex -= removedInRange; // End만 정확하게 당겨줌

            needSortingObjects.RemoveRange(write, size - write);
        }
    }

    #endregion

}
