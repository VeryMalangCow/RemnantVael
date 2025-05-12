using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UniRx;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Value

    [Header("=== Movable Object")]
    [SerializeField] public List<DepthController> NeedSortingObjects;


    [HideInInspector] private Coroutine LayerSortingCor = null;

    [HideInInspector] public readonly static int Order_BuildUpper = 1;
    [HideInInspector] public readonly static int Order_SortingObjTop = 5000;
    [HideInInspector] public readonly static int Order_EffectImg = 9999;
    [HideInInspector] public readonly static int Order_BuildLower = 10000;
    [HideInInspector] public readonly static int Order_Aim = 20000;

    [HideInInspector] private List<DepthController> LastSortedList = new();

    #endregion

    #region Framework

    private void Start()
    {
        Start_LayerSorting();
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
    private bool IsSameList(List<DepthController> a, List<DepthController> b)
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
        LayerSortingCor = StartCoroutine(Play_LayerSorting_Cor());
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

    // 종료
    private void End_LayerSorting()
    {
        StopCoroutine(LayerSortingCor);
    }

    #endregion

    #region Check

    // 솔팅이 필요한지를 판별해 솔트
    private void Check_Set_Sort()
    {
        List<DepthController> sorted = Get_StableSortedList(NeedSortingObjects);

        if (!IsSameList(sorted, LastSortedList))
        {
            Set_Sort(sorted);
            LastSortedList = sorted;
        }
    }

    #endregion

    #region Set

    // 솔팅
    private void Set_Sort(List<DepthController> _ObjectList)
    {
        for (int i = 0; i < _ObjectList.Count; i++)
        {
            _ObjectList[i].Set_SortingOrder(Order_SortingObjTop - (10 * i));
        }
    }

    #endregion
}
