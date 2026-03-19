using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    #region - Current Unit

    [HideInInspector] private List<BulletController> currentBullets = new List<BulletController>();
    [HideInInspector] private List<DroppingBombController> currentBombs = new List<DroppingBombController>();
    [HideInInspector] private List<TotemeController> currentTotemes = new List<TotemeController>();
    [HideInInspector] private List<AttackerController> currentAttackers = new List<AttackerController>();

    public void Add_Unit(BulletController _Unit) => DevTool.Add_InList(currentBullets, _Unit);
    public void Remove_Unit(BulletController _Unit) => DevTool.Remove_InList(currentBullets, _Unit);


    public void Add_Unit(DroppingBombController _Unit) => DevTool.Add_InList(currentBombs, _Unit);
    public void Remove_Unit(DroppingBombController _Unit) => DevTool.Remove_InList(currentBombs, _Unit);

    public void Add_Unit(TotemeController _Unit) => DevTool.Add_InList(currentTotemes, _Unit);
    public void Remove_Unit(TotemeController _Unit) => DevTool.Remove_InList(currentTotemes, _Unit);

    public void Add_Unit(AttackerController _Unit) => DevTool.Add_InList(currentAttackers, _Unit);
    public void Remove_Unit(AttackerController _Unit) => DevTool.Remove_InList(currentAttackers, _Unit);



    public void RemoveUnits()
    {
        for (int i = 0; i < currentBullets.Count; i++)
            currentBullets[i].RemoveForce_Object();

        for (int i = 0; i < currentBombs.Count; i++)
            currentBombs[i].RemoveForce_Object();

        for (int i = 0; i < currentTotemes.Count; i++)
            currentTotemes[i].RemoveForce_Object();

        for (int i = 0; i < currentAttackers.Count; i++)
            currentAttackers[i].RemoveForce_Object();

        currentBullets.Clear();
        currentBombs.Clear();
        currentTotemes.Clear();
        currentAttackers.Clear();
    }

    #endregion

    #region - Generator

    [Space(10)]
    [Header("=== Generator")]

    [Space(5)]
    [Header("-- Explosion")]
    [SerializeField] public PlayerExplImgGenerator player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator enemy_ExplImgGenerator;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] public OnceTimeAnimGenerator onceTime_AnimGenerator;

    #endregion

    #endregion

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private GameObject[] testGO;
    [SerializeField] private GameObject ultraModeGO;
    public void Test_Cor()
    {
        for (int i = 0; i < testGO.Length; i++)
            testGO[i].gameObject.SetActive(true);
    }

    public void Set_UltraModeGO(bool onOff)
    {
        ultraModeGO.SetActive(onOff);
    }

}
