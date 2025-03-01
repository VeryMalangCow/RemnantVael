using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Value

    [Header("=== Value")]
    [SerializeField] public int NeedLayerObjectTopSort = 1000;

    [Header("=== Movable Object")]
    [SerializeField] public List<HaveShadowThing> NeedLayerObjects;

    #endregion

    #region Framework

    public void Update()
    {
        if(Get_NeedSort())
        {
            Set_SortAllSr();
        }
    }

    #endregion

    #region Sorting Order

    private bool Get_NeedSort()
    {
        List<HaveShadowThing> tempObjects = NeedLayerObjects.OrderBy(obj => obj.transform.position.y).ToList();
        bool needSort = !Enumerable.SequenceEqual(tempObjects, NeedLayerObjects);
        if (needSort)
        {
            NeedLayerObjects = tempObjects;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Set_SortAllSr()
    {
        for (int i = 0; i < NeedLayerObjects.Count; i++)
        {
            NeedLayerObjects[i].Set_SortingOrder(NeedLayerObjectTopSort - (10 * i));
        }
    }

    #endregion
}
