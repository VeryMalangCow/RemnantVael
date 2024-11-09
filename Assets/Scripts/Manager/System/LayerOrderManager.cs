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
        if(NeedSort())
        {
            SortAllSr();
        }
    }

    #endregion

    #region Sorting Order

    private bool NeedSort()
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

    public void SortAllSr()
    {
        for (int i = 0; i < NeedLayerObjects.Count; i++)
        {
            NeedLayerObjects[i].SetSortingOrder(NeedLayerObjectTopSort - (10 * i));
        }
    }

    #endregion
}
