using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyManager : Singleton<AllyManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Manager")]

    [Space(10)]
    [Header("=== Ally Controller")]
    [SerializeField] public List<AllyController> AllAllies = new List<AllyController>();

    #endregion
}
