using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    #region - Current Unit

    [HideInInspector] private List<BulletController> CurrentBullets = new List<BulletController>();
    [HideInInspector] private List<DroppingBombController> CurrentBombs = new List<DroppingBombController>();
    [HideInInspector] private List<TotemeController> CurrentTotemes = new List<TotemeController>();
    [HideInInspector] private List<AttackerController> CurrentAttackers = new List<AttackerController>();

    public void Add_Unit(BulletController _Unit) => DevTool.Add_InList(CurrentBullets, _Unit);
    public void Remove_Unit(BulletController _Unit) => DevTool.Remove_InList(CurrentBullets, _Unit);


    public void Add_Unit(DroppingBombController _Unit) => DevTool.Add_InList(CurrentBombs, _Unit);
    public void Remove_Unit(DroppingBombController _Unit) => DevTool.Remove_InList(CurrentBombs, _Unit);

    public void Add_Unit(TotemeController _Unit) => DevTool.Add_InList(CurrentTotemes, _Unit);
    public void Remove_Unit(TotemeController _Unit) => DevTool.Remove_InList(CurrentTotemes, _Unit);

    public void Add_Unit(AttackerController _Unit) => DevTool.Add_InList(CurrentAttackers, _Unit);
    public void Remove_Unit(AttackerController _Unit) => DevTool.Remove_InList(CurrentAttackers, _Unit);



    public void RemoveUnits()
    {
        for (int i = 0; i < CurrentBullets.Count; i++)
            CurrentBullets[i].RemoveForce_Object();

        for (int i = 0; i < CurrentBombs.Count; i++)
            CurrentBombs[i].RemoveForce_Object();

        for (int i = 0; i < CurrentTotemes.Count; i++)
            CurrentTotemes[i].RemoveForce_Object();

        for (int i = 0; i < CurrentAttackers.Count; i++)
            CurrentAttackers[i].RemoveForce_Object();

        CurrentBullets.Clear();
        CurrentBombs.Clear();
        CurrentTotemes.Clear();
        CurrentAttackers.Clear();
    }

    #endregion

    #region - Generator

    [Space(10)]
    [Header("=== Generator")]

    [Space(5)]
    [Header("-- Explosion")]
    [SerializeField] public PlayerExplImgGenerator Player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator Build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator Enemy_ExplImgGenerator;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] public OnceTimeAnimGenerator OnceTime_AnimGenerator;

    #endregion

    #endregion

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private GameObject[] TestGO;

    public void Test_Cor()
    {
        for (int i = 0; i < TestGO.Length; i++)
            TestGO[i].gameObject.SetActive(true);
    }
}
