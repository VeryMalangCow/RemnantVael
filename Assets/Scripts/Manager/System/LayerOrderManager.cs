using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Value

    [Header("=== Movable Object")]
    [SerializeField] private List<DepthController> needSortingObjects;
    [HideInInspector] private List<DepthController> dirtySortingObjects = new List<DepthController>();

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
    public void CheckIsDirty(int index)
    {
        if (index < 0 || index >= needSortingObjects.Count || needSortingObjects[index] == null)
            return;

        // 상단과 비교
        if (index > 0)
        {
            // 상단 Depth보다 Y값이 크다면
            if (needSortingObjects[index].GetPosY() > needSortingObjects[index - 1].GetPosY())
            {
                AddDirty(index);
                return;
            }
        }
        // 하단과 비교
        if (index < needSortingObjects.Count - 1)
        {
            // 하단 Depth보다 Y값이 작으면
            if (needSortingObjects[index].GetPosY() < needSortingObjects[index + 1].GetPosY())
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
        needSortingObjects[index] = null;
        isDirty = true;
    }

    // Dirty List값들을 솔팅
    private void SetSortDirty()
    {
        needSortingObjects.RemoveAll(obj => obj == null);

        bool isPutIn;
        for (int i = 0; i < dirtySortingObjects.Count; i++)
        {
            isPutIn = false;
            for (int j = 0; j < needSortingObjects.Count; j++)
            {
                if (dirtySortingObjects[i].GetPosY() > needSortingObjects[j].GetPosY())
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

    private void SetSort(List<DepthController> objectList)
    {
        dirtySortingObjects.Clear();

        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].SetSortingOrder(order_SortingObjTop + (10 * i));
            objectList[i].SetSortIndex(i);
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
        DevTool.Add_InList(dirtySortingObjects, depth);
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

        DevTool.Remove_InList(needSortingObjects, depth);

        DevTool.Remove_InList(dirtySortingObjects, depth);
        depth.SetSortIndex(-1);
    }

    #endregion

}
