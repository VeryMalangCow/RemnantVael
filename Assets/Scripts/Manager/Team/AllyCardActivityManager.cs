using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AllyCardActivityManager : Singleton<AllyCardActivityManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Card Activity Manager")]

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

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 대상 메서드 필터링 및 정렬
        var filteredMethods = methods
            .Where(method =>
                method.Name.StartsWith(_MethodPrefix) &&
                method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 0)
            .OrderBy(method =>
            {
                string numberPart = method.Name.Substring(_MethodPrefix.Length);
                return int.TryParse(numberPart, out int result) ? result : int.MaxValue;
            });

        foreach (MethodInfo method in filteredMethods)
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

    #region (000 ~ 002) Spawn < Assult >

    private void ST_CardActivity_000()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Grunt"));
    }
    private void ST_CardActivity_001()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Grunt"));
    }
    private void ST_CardActivity_002()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Grunt"));
    }

    #endregion

    #region (003 ~ 007) Dmg Upgrade

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


    private void Upgrade_Dmg(float _Value)
    {
        AllyManager.Instance.Set_StateDmg(_Value);
    }

    #endregion

    #region (008 ~ 012) Rof Upgrade

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


    private void Upgrade_Rof(float _Value)
    {
        AllyManager.Instance.Set_StateRof(_Value);
    }

    #endregion

    #region (013 ~ 017) Movement Upgrade

    private void ST_CardActivity_013()
    {
        Upgrade_Movement(1.1f);
    }
    private void ST_CardActivity_014()
    {
        Upgrade_Movement(1.3f);
    }
    private void ST_CardActivity_015()
    {
        Upgrade_Movement(1.6f);
    }
    private void ST_CardActivity_016()
    {
        Upgrade_Movement(2f);
    }
    private void ST_CardActivity_017()
    {
        Upgrade_Movement(2.5f);
    }


    private void Upgrade_Movement(float _Value)
    {
        AllyManager.Instance.Set_StateMovementSpeed(_Value);
    }

    #endregion

    #region (018 ~ 022) AttackSize Upgrade

    private void ST_CardActivity_018()
    {
        Upgrade_AttackSize(1.5f);
    }
    private void ST_CardActivity_019()
    {
        Upgrade_AttackSize(2.1f);
    }
    private void ST_CardActivity_020()
    {
        Upgrade_AttackSize(2.8f);
    }
    private void ST_CardActivity_021()
    {
        Upgrade_AttackSize(3.6f);
    }
    private void ST_CardActivity_022()
    {
        Upgrade_AttackSize(4.5f);
    }


    private void Upgrade_AttackSize(float _Value)
    {
        AllyManager.Instance.Set_StateAttackSize(_Value);
    }

    #endregion

    #region (023 ~ 027) CC Upgrade

    private void ST_CardActivity_023()
    {
        Upgrade_CC(1.15f);
    }
    private void ST_CardActivity_024()
    {
        Upgrade_CC(1.3f);
    }
    private void ST_CardActivity_025()
    {
        Upgrade_CC(1.55f);
    }
    private void ST_CardActivity_026()
    {
        Upgrade_CC(1.8f);
    }
    private void ST_CardActivity_027()
    {
        Upgrade_CC(2.2f);
    }


    private void Upgrade_CC(float _Value)
    {
        AllyManager.Instance.Set_StateCC(_Value);
    }

    #endregion

    #region (028 ~ 032) CD Upgrade

    private void ST_CardActivity_028()
    {
        Upgrade_CD(1.2f);
    }
    private void ST_CardActivity_029()
    {
        Upgrade_CD(1.5f);
    }
    private void ST_CardActivity_030()
    {
        Upgrade_CD(1.9f);
    }
    private void ST_CardActivity_031()
    {
        Upgrade_CD(2.4f);
    }
    private void ST_CardActivity_032()
    {
        Upgrade_CD(3f);
    }


    private void Upgrade_CD(float _Value)
    {
        AllyManager.Instance.Set_StateCD(_Value);
    }

    #endregion

    #region (033 ~ 037) Muzzle Upgrade

    private void ST_CardActivity_033()
    {
        Upgrade_Muzzle(1.3f);
    }
    private void ST_CardActivity_034()
    {
        Upgrade_Muzzle(1.7f);
    }
    private void ST_CardActivity_035()
    {
        Upgrade_Muzzle(2.2f);
    }
    private void ST_CardActivity_036()
    {
        Upgrade_Muzzle(2.8f);
    }
    private void ST_CardActivity_037()
    {
        Upgrade_Muzzle(3.5f);
    }


    private void Upgrade_Muzzle(float _Value)
    {
        AllyManager.Instance.Set_StateMuzzleSpeed(_Value);
    }

    #endregion

    #region (038 ~ 040) Spawn < Booma >

    private void ST_CardActivity_038()
    {
        SpawnAlly(ResourceManager.Instance.Get_NoneUnitAlly("Booma"));
    }
    private void ST_CardActivity_039()
    {
        SpawnAlly(ResourceManager.Instance.Get_NoneUnitAlly("Booma"));
    }
    private void ST_CardActivity_040()
    {
        SpawnAlly(ResourceManager.Instance.Get_NoneUnitAlly("Booma"));
    }

    #endregion

    #endregion

    #region Uplink Team 

    #region (000 ~ 001) Spawn < Ignis >

    private void UT_CardActivity_000()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Ignis"));
    }
    private void UT_CardActivity_001()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Ignis"));
    }

    #endregion

    #region (002 ~ 003) Spawn < Glacia >

    private void UT_CardActivity_002()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Glacia"));
    }

    private void UT_CardActivity_003()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Glacia"));
    }

    #endregion

    #region (004 ~ 005) Spawn < Volt >

    private void UT_CardActivity_004()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Volt"));
    }

    private void UT_CardActivity_005()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Volt"));
    }

    #endregion

    #region (006 ~ 007) Spawn < Tox >

    private void UT_CardActivity_006()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Tox"));
    }

    private void UT_CardActivity_007()
    {
        SpawnAlly(ResourceManager.Instance.Get_FieldUnitAlly("Tox"));
    }

    #endregion

    #region (008~009) Spawn < >

    private void UT_CardActivity_008()
    {
        SpawnAlly(ResourceManager.Instance.Get_NoneUnitAlly("Totis"));
    }

    private void UT_CardActivity_009()
    {
        SpawnAlly(ResourceManager.Instance.Get_NoneUnitAlly("Totis"));
    }

    #endregion

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
        PlayerManager.Instance.playerController.CurrentBoostLv.Value = _BoostLv;
    }

    #endregion

    #endregion


    #region Unique

    // Ally 생성
    private void SpawnAlly(GameObject _AllyPrefab)
    {
        AllyController ally = DevTool.Get_ComponentTType<AllyController>(
            Instantiate(_AllyPrefab, AllyParentTF));

        ally.Set_SpawnFirst();
    }


    #endregion
}
