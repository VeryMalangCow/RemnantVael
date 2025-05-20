using System;
using System.Collections.Generic;
using System.Reflection;
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
        List<ActivityFuncDele> ST_CardActivityList = Init_DelegateList("ST_CardActivity_");
        List<ActivityFuncDele> UT_CardActivityList = Init_DelegateList("UT_CardActivity_");
        List<ActivityFuncDele> NT_CardActivityList = Init_DelegateList("NT_CardActivity_");

        AllActivityFuncList = new List<List<ActivityFuncDele>>
        { ST_CardActivityList, UT_CardActivityList, NT_CardActivityList };
    }

    public List<ActivityFuncDele> Init_DelegateList(string _MethodPrefix)
    {
        List<ActivityFuncDele> delegateList = new List<ActivityFuncDele>();

        // 현재 클래스 타입 정보 가져오기
        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        foreach (MethodInfo method in methods)
        {
            // 이름, 반환형, 파라미터 체크
            if (method.Name.StartsWith(_MethodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 0)
            {
                try
                {
                    ActivityFuncDele del = method.IsStatic
                        ? (ActivityFuncDele)Delegate.CreateDelegate(typeof(ActivityFuncDele), method)
                        : (ActivityFuncDele)Delegate.CreateDelegate(typeof(ActivityFuncDele), this, method);

                    delegateList.Add(del);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"델리게이트 생성 실패: {method.Name} - {ex.Message}");
                }
            }
        }

        return delegateList;
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
        if (_CardID == -1) return;

        AllActivityFuncList[_TypeID][_CardID]();

        if (_TypeID == 0) Debug.Log($"ST_{_CardID} 카드");
        else if (_TypeID == 0) Debug.Log($"UT_{_CardID} 카드");
        else Debug.Log($"NT_{_CardID} 카드");
    }

    #endregion

    #region Strike Team

    #region Spawn Strike Ally (000 ~ 003)

    private void ST_CardActivity_000()
    {
        SpawnAlly(AssultAllyPrefab);
    }
    private void ST_CardActivity_001()
    {
        SpawnAlly(AssultAllyPrefab);
    }
    private void ST_CardActivity_002()
    {
        SpawnAlly(AssultAllyPrefab);
    }

    private void SpawnAlly(GameObject _AllyPrefab)
    {
        AllyController ally = DevTool.Get_ComponentTType<AllyController>(
            Instantiate(_AllyPrefab, AllyParentTF));

        ally.Set_PosRandomNearPlayer();
    }

    #endregion

    #region Dmg Upgrade

    private void ST_CardActivity_003()
    {
        Upgrade_Dmg(1.1f);
    }
    private void ST_CardActivity_004()
    {
        Upgrade_Dmg(1.3f);
    }
    private void ST_CardActivity_005()
    {
        Upgrade_Dmg(1.6f);
    }
    private void ST_CardActivity_006()
    {
        Upgrade_Dmg(2f);
    }
    private void ST_CardActivity_007()
    {
        Upgrade_Dmg(2.5f);
    }


    private void Upgrade_Dmg(float _DmgMultiple)
    {
        AllyManager.Instance.Set_StateDmg(_DmgMultiple);
    }

    #endregion

    #region Rof Upgrade

    private void ST_CardActivity_008()
    {
        Upgrade_Rof(1.1f);
    }
    private void ST_CardActivity_009()
    {
        Upgrade_Rof(1.3f);
    }
    private void ST_CardActivity_010()
    {
        Upgrade_Rof(1.6f);
    }
    private void ST_CardActivity_011()
    {
        Upgrade_Rof(2f);
    }
    private void ST_CardActivity_012()
    {
        Upgrade_Rof(2.5f);
    }


    private void Upgrade_Rof(float _DmgMultiple)
    {
        AllyManager.Instance.Set_StateRof(_DmgMultiple);
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
        BoosterUpgrade(1);
    }

    private void NT_CardActivity_001()
    {
        BoosterUpgrade(2);
    }

    private void NT_CardActivity_002()
    {
        BoosterUpgrade(3);
    }

    private void NT_CardActivity_003()
    {
        BoosterUpgrade(4);
    }


    private void BoosterUpgrade(int _BoostLv)
    {
        PlayerManager.Instance.PlayerController.CurrentBoostLv.Value = _BoostLv;
    }

    #endregion

    #endregion
}
