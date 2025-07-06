using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class NoneUnitAllyController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> None Unit Ally")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected AllyState MultipleAllyState;
    [SerializeField] private float MaxHP = 150f;
    [SerializeField] private float MaxEP = 100f;
    [SerializeField] private ReactiveProperty<eAllyNoneUnitStateMode> AllyStateMode = new();

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public AllyBuffController BuffController;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Sprite FrontFaceSprite;


    #endregion

    #region - Hide

    #endregion

    #endregion
}
