using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class DirectionalController<T, U> : MonoBehaviour where U : Component
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Directional")]

    [Space(10)]
    [Header("=== TF")]
    [FormerlySerializedAs("RotationTargetTF")][SerializeField] protected Transform rotTargetTf;

    [Space(10)]
    [Header("=== TType, UType")]
    [FormerlySerializedAs("ThisComp")][SerializeField] public U comp;
    [FormerlySerializedAs("ThisDirectionalList")][SerializeField] protected List<T> dirList;

    [Space(10)]
    [Header("=== Value")]
    [HideInInspector] protected ReactiveProperty<int> currentIndex = new();


    #endregion

    #region Framework

    protected virtual void Start()
    {
        currentIndex.Value = 5;
    }

    protected virtual void LateUpdate()
    {
        Check_CorrectIndex();
    }

    #endregion

    #region Index

    private void Check_CorrectIndex()
    {
        int cacualatedIndex = DevTool.Get_Index(rotTargetTf.localRotation.eulerAngles.y);
        if (cacualatedIndex != currentIndex.Value)
        {
            currentIndex.Value = cacualatedIndex;
        }
    }

    #endregion
}
