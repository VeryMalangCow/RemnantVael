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

    [HideInInspector] public readonly static int order_BuildUpper = 1;
    [HideInInspector] public readonly static int order_SortingObjTop = 5000;
    [HideInInspector] public readonly static int order_EffectImg = 9999;
    [HideInInspector] public readonly static int order_BuildLower = 10000;
    [HideInInspector] public readonly static int order_DoorIcon = 19998;
    [HideInInspector] public readonly static int order_Explosion = 19999;
    [HideInInspector] public readonly static int order_Aim = 20000;
    [HideInInspector] public readonly static int order_DmgTxt = 20001;

    [HideInInspector] private List<DepthController> lastSortedList = new();

    #endregion

    #region Framework

    private void Start()
    {
        Start_LayerSorting();
    }


    #endregion

    #region List

    public void Clear_NeedSortObj()
    {
        needSortingObjects.Clear();
    }

    public void Add_NeedSortObj(DepthController depth)
    {
        DevTool.Add_InList(needSortingObjects, depth);
    }

    public void Add_NeedSortObj<T>(List<T> depths) where T : DepthController
    {
        for (int i = 0; i < depths.Count; i++)
            Add_NeedSortObj(depths[i]);
    }

    public void Remove_NeedSortObj(DepthController depth)
    {
        DevTool.Remove_InList(needSortingObjects, depth);
    }

    #endregion

    #region Get

    // 안정적인 정렬: Y 기준 + ID 기준
    private List<DepthController> Get_StableSortedList(List<DepthController> source)
    {
        return source
            .OrderBy(obj => obj.transform.position.y)
            .ThenBy(obj => obj.GetInstanceID()) // 같은 Y 위치일 때도 항상 같은 순서 보장
            .ToList();
    }

    // 순서 비교
    private bool Is_SameList(List<DepthController> a, List<DepthController> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    #endregion

    #region Play

    // 시작
    private void Start_LayerSorting()
    {
        StartCoroutine(Play_LayerSorting_Cor());
    }

    // 매 프레임 마다 레이어 솔팅
    private IEnumerator Play_LayerSorting_Cor()
    {
        while(true)
        {
            Check_Set_Sort();
            yield return null;
        }
    }
/*
    // 종료
    private void End_LayerSorting()
    {
        StopCoroutine(LayerSortingCor);
    }
*/
    #endregion

    #region Check

    // 솔팅이 필요한지를 판별해 솔트
    private void Check_Set_Sort()
    {
        List<DepthController> sorted = Get_StableSortedList(needSortingObjects);

        if (!Is_SameList(sorted, lastSortedList))
        {
            Set_Sort(sorted);
            lastSortedList = sorted;
        }
    }

    #endregion

    #region Set

    // 솔팅
    private void Set_Sort(List<DepthController> objectList)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].Set_SortingOrder(order_SortingObjTop - (10 * i));
        }
    }

    #endregion
}
