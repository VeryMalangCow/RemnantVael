using System.Collections.Generic;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>
{
    #region Value

    [Space(10)]
    [Header("=== Data")]

    [Header("-- Attack")]
    [SerializeField] public BU_OneTypeData<float> BaseDamage_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseROF_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseCC_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseCD_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseAccuracyRate_BUData;
    [SerializeField] public BU_OneTypeData<float> Knockback_BUData;

    [Header("-- EP")]
    [SerializeField] public BU_OneTypeData<float> BaseMaxEP_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseSpawnESMultiple_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseNeedEP_ForSkillMultiple_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseDecEnergyPointMultiple_BUData;

    [Header("-- Movement")]
    [SerializeField] public BU_OneTypeData<float> BaseWalkSpeed_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseWalkSpeedWhenShotMultiple_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseDashSpeed_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseAvoidChance_BUData;

    [Header("-- Skill 0")]
    [SerializeField] public BU_OneTypeData<float> Skill0_Cooltime_BUData;
    [SerializeField] public BU_OneTypeData<float> Skill0_Power_BUData;
    [SerializeField] public BU_OneTypeData<int> Skill0_Tier_BUData;

    [Header("-- Skill 1")]
    [SerializeField] public BU_OneTypeData<float> Skill1_Cooltime_BUData;
    [SerializeField] public BU_OneTypeData<float> Skill1_Power_BUData;
    [SerializeField] public BU_OneTypeData<int> Skill1_Tier_BUData;

    #endregion

    #region Offset

    public void Offset(PlayerController _PC)
    {
        BaseDamage_BUData.Offset(_PC.BaseWeapon.BaseDamage);
        BaseROF_BUData.Offset(_PC.BaseWeapon.ROF);
        BaseCC_BUData.Offset(_PC.BaseWeapon.CC);
        BaseCD_BUData.Offset(_PC.BaseWeapon.CD);
        BaseAccuracyRate_BUData.Offset(_PC.BaseWeapon.AccuracyRate);
        Knockback_BUData.Offset(_PC.BaseWeapon.KnockbackPower);

        BaseMaxEP_BUData.Offset(_PC.MaxEP);
        BaseSpawnESMultiple_BUData.Offset(_PC.SpawnESMultiple);
        BaseNeedEP_ForSkillMultiple_BUData.Offset(_PC.NeedEP_ForSkillMultiple);
        BaseDecEnergyPointMultiple_BUData.Offset(_PC.DecEnergyPointMultiple);

        BaseWalkSpeed_BUData.Offset(_PC.WalkSpeed);
        BaseWalkSpeedWhenShotMultiple_BUData.Offset(_PC.WalkSpeedWhenShotMultiple);
        BaseDashSpeed_BUData.Offset(_PC.DashController.DashSpeed);
        BaseAvoidChance_BUData.Offset(_PC.AvoidChance);

        Skill0_Cooltime_BUData.Offset(_PC.SkillWeapon.Skill_0.MaxCooltime);
        Skill0_Power_BUData.Offset(_PC.SkillWeapon.Skill_0.Power);
        Skill0_Tier_BUData.Offset(_PC.SkillWeapon.Skill_0.Tier);


        Skill1_Cooltime_BUData.Offset(_PC.SkillWeapon.Skill_1.MaxCooltime);
        Skill1_Power_BUData.Offset(_PC.SkillWeapon.Skill_1.Power);
        Skill1_Tier_BUData.Offset(_PC.SkillWeapon.Skill_1.Tier);
    }

    #endregion
}

[System.Serializable]
public class BU_OneTypeData<T>
{
    [Header("=== No Input")]
    public List<BU_EachLevelData<T>> BU_EachLevelDataList;

    public void Offset(BaseUpgradeState<T> _BaseValue)
    {
        if (_BaseValue.BaseState.GetType() == typeof(float))
        {
            // Base
            float float_BaseValue = float.Parse(_BaseValue.BaseState.ToString());

            // Upgrade
            List<float> float_UpgradeValues = new List<float>();
            for (int i = 0; i < _BaseValue.UpgradeValueByLevelRange.Count; i++)
            {
                float float_EachUpgradeValue = float.Parse(_BaseValue.UpgradeValueByLevelRange[i].ToString());
                float_UpgradeValues.Add(float_EachUpgradeValue);
            }

            Offset(float_BaseValue, float_UpgradeValues, _BaseValue.NeedPayByLevelRange);
        }
        else if (_BaseValue.BaseState.GetType() == typeof(int))
        {
            // Base
            int int_BaseValue = int.Parse(_BaseValue.BaseState.ToString());

            // Upgrade
            List<int> int_UpgradeValues = new List<int>();
            for (int i = 0; i < _BaseValue.UpgradeValueByLevelRange.Count; i++)
            {
                int int_EachUpgradeValue = int.Parse(_BaseValue.UpgradeValueByLevelRange[i].ToString());
                int_UpgradeValues.Add(int_EachUpgradeValue);
            }

            Offset(int_BaseValue, int_UpgradeValues, _BaseValue.NeedPayByLevelRange);
        }
    }

    public void Offset(float _FloatValue, List<float> _UpgradeValue, List<int> _NeedPay)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i == 0)
            {
                decimal d = (decimal)(_FloatValue + _UpgradeValue[0]);
                BU_EachLevelDataList[i].SetUpgradeValue(Mathf.RoundToInt((float)d * 100f) / 100f);
            }
            else
            {
                decimal d = (decimal)((float)BU_EachLevelDataList[i - 1].GetUpgradeValue() + _UpgradeValue[(int)(i / 3)]);
                BU_EachLevelDataList[i].SetUpgradeValue(Mathf.RoundToInt((float)d * 100f) / 100f);
            }

            BU_EachLevelDataList[i].NeedEC_ForUpgrade = _NeedPay[(int)(i/3)];
        }
    }

    public void Offset(int _IntValue, List<int> _UpgradeValue, List<int> _NeedPay)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i == 0)
            {
                int _intager = (_IntValue + _UpgradeValue[0]);
                BU_EachLevelDataList[i].SetUpgradeValue((int)_intager);
            }
            else
            {
                int _intager = ((int)BU_EachLevelDataList[i - 1].GetUpgradeValue() + _UpgradeValue[(int)(i / 3)]);
                BU_EachLevelDataList[i].SetUpgradeValue((int)_intager);
            }

            BU_EachLevelDataList[i].NeedEC_ForUpgrade = _NeedPay[(int)(i / 3)];
        }
    }
}

[System.Serializable]
public class BU_EachLevelData<T>
{
    public T UpgradeValue;
    public int NeedEC_ForUpgrade;

    public void SetUpgradeValue(object _Value)
    {
        UpgradeValue = (T)_Value;
    }

    public object GetUpgradeValue() 
    {
        return UpgradeValue;
    }
}
