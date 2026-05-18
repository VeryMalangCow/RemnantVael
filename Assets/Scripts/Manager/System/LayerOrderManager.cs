using System.Collections.Generic;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Struct

    private struct DepthEntry : System.IEquatable<DepthEntry>
    {
        public float y;
        public DepthController depth;

        public bool isEmpty() { return depth == null; }

        public bool Equals(DepthEntry other)
        {
            return depth == other.depth;
        }

    }

    #endregion

    #region Value

    [Header("=== Movable Object")]
    [HideInInspector] private List<DepthEntry> needSortingObjects = new List<DepthEntry>();
    [HideInInspector] private List<DepthEntry> dirtySortingObjects = new List<DepthEntry>();

    [HideInInspector] public readonly static int order_BuildUpper = 1;
    [HideInInspector] public readonly static int order_SortingObjTop = 5000;
    [HideInInspector] public readonly static int order_EffectImg = 9999;
    [HideInInspector] public readonly static int order_BuildLower = 10000;
    [HideInInspector] public readonly static int order_DoorIcon = 19998;
    [HideInInspector] public readonly static int order_Explosion = 19999;
    [HideInInspector] public readonly static int order_Aim = 20000;
    [HideInInspector] public readonly static int order_DmgTxt = 20001;
   

    private bool isDirty = false;

    #endregion

    #region Framework

    private void LateUpdate()
    {
        HandleSort();
    }

    #endregion

    #region Sort

    private void HandleSort()
    {
        if (isDirty)
        {
            SetSortDirty();
        }
    }

    // 신호가 들어오면 실제로 해당 Depth가 Dirty인지 판별
    public void CheckIsDirty(int index, float newY)
    {
        if (index < 0 || index >= needSortingObjects.Count || needSortingObjects[index].depth == null)
            return;

        // 해당 인덱스 요소의 Y값 변경
        DepthEntry element = needSortingObjects[index];
        element.y = newY;
        needSortingObjects[index] = element;

        // 상단과 비교
        if (index > 0)
        {
            // 상단 Depth보다 Y값이 크다면
            if (needSortingObjects[index].y > needSortingObjects[index - 1].y)
            {
                AddDirty(index);
                return;
            }
        }
        // 하단과 비교
        if (index < needSortingObjects.Count - 1)
        {
            // 하단 Depth보다 Y값이 작으면
            if (needSortingObjects[index].y < needSortingObjects[index + 1].y)
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
        needSortingObjects[index] = new DepthEntry { depth = null };
        isDirty = true;
    }

    // Dirty List값들을 솔팅
    private void SetSortDirty()
    {
        needSortingObjects.RemoveAll(e => e.isEmpty());

        bool isPutIn;
        for (int i = 0; i < dirtySortingObjects.Count; i++)
        {
            isPutIn = false;
            for (int j = 0; j < needSortingObjects.Count; j++)
            {
                if (dirtySortingObjects[i].y > needSortingObjects[j].y)
                {
                    needSortingObjects.Insert(j, dirtySortingObjects[i]);
                    isPutIn = true;
                    break;
                }
            }

            if (!isPutIn)
            {
                needSortingObjects.Add(dirtySortingObjects[i]);
            }
        }

        SetSort(needSortingObjects);
    }

    private void SetSort(List<DepthEntry> objectList)
    {
        dirtySortingObjects.Clear();

        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].depth.SetSortingOrder(order_SortingObjTop + (10 * i));
            objectList[i].depth.SetSortIndex(i);
        }

        isDirty = false;
    }

    #endregion

    #region List

    public void ClearNeedSortObj()
    {
        needSortingObjects.Clear();
    }

    // 솔팅이 필요한 Depth를 List에 추가
    public void AddNeedSortObj(DepthController depth)
    {
        DevTool.Add_InList(dirtySortingObjects, new DepthEntry { y = depth.GetPosY(), depth = depth });
        isDirty = true;
    }

    public void AddNeedSortObj<T>(List<T> depths) where T : DepthController
    {
        for (int i = 0; i < depths.Count; i++)
            depths[i].AddSortingLayer();
    }

    // 솔팅에 필요하지않은 Depth를 List에서 제거
    public void RemoveNeedSortObj(DepthController depth)
    {
        for (int i = 0; i < needSortingObjects.Count; i++)
        {
            if (needSortingObjects[i].depth == depth)
            {
                needSortingObjects[i] = new DepthEntry { depth = null };
                isDirty = true;
                break;
            }
        }

        dirtySortingObjects.RemoveAll(e => e.depth == depth);
        depth.SetSortIndex(-1);
    }

    #endregion

}
