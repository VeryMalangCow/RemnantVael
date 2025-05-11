using System.Collections.Generic;
using UnityEngine;

public class AllyCardActivityManager : Singleton<AllyCardActivityManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Card Activity Manager")]

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] private GameObject AssultAllyPrefab;

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform AllyParentTF;

    #endregion

    #region - Hide

    public delegate void ActivityFuncDele();
    [HideInInspector] public List<List<ActivityFuncDele>> AllActivityFuncList = new List<List<ActivityFuncDele>>();

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        List<ActivityFuncDele> ST_CardActivityList = new List<ActivityFuncDele>
        {
            ST_CardActivity_000,
            ST_CardActivity_001,
            ST_CardActivity_002,
        };

        List<ActivityFuncDele> UT_CardActivityList = new List<ActivityFuncDele>
        {
            UT_CardActivity_000,
            UT_CardActivity_001,
            UT_CardActivity_002,
        };

        List<ActivityFuncDele> NT_CardActivityList = new List<ActivityFuncDele>
        {
            NT_CardActivity_000,
            NT_CardActivity_001,
            NT_CardActivity_002,
            NT_CardActivity_003,
        };

        AllActivityFuncList = new List<List<ActivityFuncDele>>
        { ST_CardActivityList, UT_CardActivityList, NT_CardActivityList };
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    #endregion

    #region Set

    public void Action_CorrectCardActivity(int _TypeID, int _CardID)
    {
        AllActivityFuncList[_TypeID][_CardID]();
    }

    #endregion

    #region Strike Team

    #region Spawn Strike Ally (000 ~ 003)

    private void ST_CardActivity_000()
    {
        Debug.Log("ST_000 카드");
        SpawnAlly(AssultAllyPrefab);
    }
    private void ST_CardActivity_001()
    {
        Debug.Log("ST_001 카드");
        SpawnAlly(AssultAllyPrefab);
    }
    private void ST_CardActivity_002()
    {
        Debug.Log("ST_002 카드");
        SpawnAlly(AssultAllyPrefab);
    }

    private void SpawnAlly(GameObject _AllyPrefab)
    {
        AllyController ally = DevTool.Get_ComponentTType<AllyController>(
            Instantiate(_AllyPrefab, AllyParentTF));

        ally.Set_PosRandomNearPlayer();
    }

    #endregion

    #endregion

    #region Uplink Team

    private void UT_CardActivity_000()
    {
        Debug.Log("UT_000 카드");
    }
    private void UT_CardActivity_001()
    {
        Debug.Log("UT_001 카드");
    }
    private void UT_CardActivity_002()
    {
        Debug.Log("UT_002 카드");
    }

    #endregion

    #region Neo Team

    #region Booster Upgrade (000 ~ 003)

    private void NT_CardActivity_000()
    {
        Debug.Log("NT_000 카드");
        BoosterUpgrade(1);
    }

    private void NT_CardActivity_001()
    {
        Debug.Log("NT_001 카드");
        BoosterUpgrade(2);
    }

    private void NT_CardActivity_002()
    {
        Debug.Log("NT_002 카드");
        BoosterUpgrade(3);
    }

    private void NT_CardActivity_003()
    {
        Debug.Log("NT_003 카드");
        BoosterUpgrade(4);
    }


    private void BoosterUpgrade(int _BoostLv)
    {
        PlayerManager.Instance.PlayerController.CurrentBoostLv.Value = _BoostLv;
    }

    #endregion

    #endregion
}
