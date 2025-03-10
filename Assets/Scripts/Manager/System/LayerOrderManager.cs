using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Value

    [Header("=== Value")]
    [SerializeField] public readonly static int NeedLayerObjectTopSort = 1000;
    [SerializeField] public readonly static int EffectImgSort = 3000;

    [Header("=== Movable Object")]
    [SerializeField] public List<DepthController> NeedLayerObjects;


    [HideInInspector] private Coroutine LayerSortingCor = null;

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
        if (Get_NeedSort(Get_OrderByY(NeedLayerObjects)))
        {
            Set_Sort(NeedLayerObjects);
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
        return !Enumerable.SequenceEqual(_ObjectList, NeedLayerObjects);
    }

    #endregion

    #region Set

    // 솔팅
    private void Set_Sort(List<DepthController> _ObjectList)
    {
        _ObjectList = Get_OrderByY(_ObjectList);
        for (int i = 0; i < _ObjectList.Count; i++)
        {
            _ObjectList[i].Set_SortingOrder(NeedLayerObjectTopSort - (10 * i));
        }
    }

    #endregion
}
