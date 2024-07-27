using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class LayerOrderManager : Singleton<LayerOrderManager>
{
    #region Value

    [Header("=== Value")]
    [SerializeField] public int MovableObjectTopSort = 1000;

    [Header("=== Movable Object")]
    [SerializeField] public List<MovableObject> MovableObjects;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
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
        List<MovableObject> tempMovableObjects = MovableObjects.OrderBy(obj => obj.transform.position.y).ToList();
        bool needSort = !Enumerable.SequenceEqual(tempMovableObjects, MovableObjects);
        if (needSort)
        {
            MovableObjects = tempMovableObjects;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SortAllSr()
    {
        for (int i = 0; i < MovableObjects.Count; i++)
        {
            MovableObjects[i].SetSortingOrder(MovableObjectTopSort - (2 * i));
        }
    }

    #endregion
}
