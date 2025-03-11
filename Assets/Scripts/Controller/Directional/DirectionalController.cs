using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DirectionalController<T, U> : MonoBehaviour where U : Component
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Directional")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] protected Transform RotationTargetTF;

    [Space(10)]
    [Header("=== TType, UType")]
    [SerializeField] public U ThisComp;
    [SerializeField] protected List<T> ThisDirectionalList;

    [Space(10)]
    [Header("=== Value")]
    [HideInInspector] protected ReactiveProperty<int> CurrentIndex = new();


    #endregion

    #region Framework

    protected virtual void Start()
    {
        CurrentIndex.Value = 5;
    }

    protected virtual void LateUpdate()
    {
        Check_CorrectIndex();
    }

    #endregion

    #region Index

    private void Check_CorrectIndex()
    {
        int cacualatedIndex = Get_Index(RotationTargetTF.localRotation.eulerAngles.y);
        if (cacualatedIndex != CurrentIndex.Value)
        {
            CurrentIndex.Value = cacualatedIndex;
        }
    }

    private int Get_Index(float _EulerAngleY)
    {
        return (int)((_EulerAngleY + 67.5f) % 360 * 0.0222222f);
    }

    #endregion
}
