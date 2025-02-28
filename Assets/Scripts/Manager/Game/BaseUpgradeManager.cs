using System.Collections.Generic;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>
{
    #region Value

    [Space(10)]
    [Header("=== Data")]

    [Header("-- Attack")]
    [SerializeField] public BULevelData<float> BaseDamage_BUData;
    [SerializeField] public BULevelData<float> BaseROF_BUData;
    [SerializeField] public BULevelData<float> BaseCC_BUData;
    [SerializeField] public BULevelData<float> BaseCD_BUData;
    [SerializeField] public BULevelData<float> BaseMuzzleSpeed_BUData;
    [SerializeField] public BULevelData<float> BaseAccuracyRate_BUData;
    [SerializeField] public BULevelData<float> Knockback_BUData;

    [Header("-- EP")]
    [SerializeField] public BULevelData<float> BaseMaxEP_BUData;
    [SerializeField] public BULevelData<float> BaseSpawnESMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseNeedEP_ForSkillMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseDecEnergyPointMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseResist_BUData;

    [Header("-- Movement")]
    [SerializeField] public BULevelData<float> BaseWalkSpeed_BUData;
    [SerializeField] public BULevelData<float> BaseWalkSpeedWhenShotMultiple_BUData;
    [SerializeField] public BULevelData<float> BaseDashSpeed_BUData;
    [SerializeField] public BULevelData<float> BaseAvoidChance_BUData;

    [Header("-- Skill 0")]
    [SerializeField] public BULevelData<float> Skill0_Cooltime_BUData;
    [SerializeField] public BULevelData<float> Skill0_Power_BUData;
    [SerializeField] public BULevelData<int> Skill0_Tier_BUData;

    [Header("-- Skill 1")]
    [SerializeField] public BULevelData<float> Skill1_Cooltime_BUData;
    [SerializeField] public BULevelData<float> Skill1_Power_BUData;
    [SerializeField] public BULevelData<int> Skill1_Tier_BUData;

    #endregion

    #region Offset

    public void Offset(PlayerController _PC)
    {
        BaseDamage_BUData.Offset(_PC.BaseWeapon.BaseDamage);
        BaseROF_BUData.Offset(_PC.BaseWeapon.ROF);
        BaseCC_BUData.Offset(_PC.BaseWeapon.CC);
        BaseCD_BUData.Offset(_PC.BaseWeapon.CD);
        BaseMuzzleSpeed_BUData.Offset(_PC.BaseWeapon.MuzzleSpeed);
        BaseAccuracyRate_BUData.Offset(_PC.BaseWeapon.AccuracyRate);
        Knockback_BUData.Offset(_PC.BaseWeapon.KnockbackPower);

        BaseMaxEP_BUData.Offset(_PC.MaxEP);
        BaseSpawnESMultiple_BUData.Offset(_PC.SpawnESMultiple);
        BaseNeedEP_ForSkillMultiple_BUData.Offset(_PC.NeedEP_ForSkillMultiple);
        BaseDecEnergyPointMultiple_BUData.Offset(_PC.DecEnergyPointMultiple);
        BaseResist_BUData.Offset(_PC.TakingDmgMultiple);

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
public class BULevelData<T>
{
    [Header("=== No Input")]
    public List<BUEachLevelData<T>> BU_EachLevelDataList;

    public void Offset(BUState<T> _BaseValue)
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
public class BUEachLevelData<T>
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
