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

    // 내부 캐시 버퍼
    private readonly List<DepthController> sortedBuffer = new();

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

    /*
    // 종료
    private void End_LayerSorting()
    {
        StopCoroutine(LayerSortingCor);
    }
    */


    #endregion

    #region Sorting

    // 솔팅이 필요한지를 판별해 솔트
    private void Check_Set_Sort()
    {
        // NeedSortingObjects를 기준으로 정렬 버퍼 생성
        sortedBuffer.Clear();
        sortedBuffer.AddRange(NeedSortingObjects);

        // Y값 기준 정렬
        sortedBuffer.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));

        // 순서가 바뀌었는지 확인
        bool changed = false;
        for (int i = 0; i < sortedBuffer.Count; i++)
        {
            if (sortedBuffer[i] != NeedSortingObjects[i])
            {
                changed = true;
                break;
            }
        }

        if (changed)
        {
            Set_Sort(sortedBuffer);

            // NeedSortingObjects에 정렬된 순서를 반영
            for (int i = 0; i < sortedBuffer.Count; i++)
            {
                NeedSortingObjects[i] = sortedBuffer[i];
            }
        }
    }

    #endregion

    #region Set

    // 솔팅
    private void Set_Sort(List<DepthController> _ObjectList)
    {
        for (int i = 0; i < _ObjectList.Count; i++)
        {
            int desiredOrder = Order_SortingObjTop - (10 * i);
            _ObjectList[i].Set_SortingOrder(desiredOrder);
        }
    }

    #endregion
}
