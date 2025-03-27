using System.Collections;
using System.Collections.Generic;
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

    #endregion

    #region Framework

    private void Start()
    {
        Start_LayerSorting();
    }


    #endregion

    #region Set Cor

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

    #region Sorting

    // 솔팅이 필요한지를 판별해 솔트
    private void Check_Set_Sort()
    {
        if (Get_NeedSort(Get_OrderByY(NeedSortingObjects)))
        {
            Set_Sort(NeedSortingObjects);
        }
    }

    #endregion

    #region Get

    // Y값 기준으로 내림차순
    private List<DepthController> Get_OrderByY(List<DepthController> _ObjectList)
    {
        return _ObjectList.OrderBy(obj => obj.transform.position.y).ToList();
    }
    
    // 솔팅이 필요한지?
    private bool Get_NeedSort(List<DepthController> _ObjectList)
    {
        return !Enumerable.SequenceEqual(_ObjectList, NeedSortingObjects);
    }

    #endregion

    #region Set

    // 솔팅
    private void Set_Sort(List<DepthController> _ObjectList)
    {
        _ObjectList = Get_OrderByY(_ObjectList);
        for (int i = 0; i < _ObjectList.Count; i++)
        {
            _ObjectList[i].Set_SortingOrder(Order_SortingObjTop - (10 * i));
        }
    }

    #endregion
}
