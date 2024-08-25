using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>
{
    #region 

    [Header("=== Data")]
    [SerializeField] public BU_OneTypeData<float> BaseDamage_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseROF_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseMaxEP_BUData;
    [SerializeField] public BU_OneTypeData<float> BaseWalkSpeed_BUData;

    #endregion

    #region Offset

    public void Offset(PlayerController _PC)
    {
        BaseDamage_BUData.Offset(_PC.BaseWeapon.BaseDamage.ActualState.Value);
        BaseROF_BUData.Offset(_PC.BaseWeapon.ROF.ActualState.Value);
        BaseMaxEP_BUData.Offset(_PC.MaxEP.ActualState.Value);
        BaseWalkSpeed_BUData.Offset(_PC.WalkSpeed.ActualState.Value);
    }

    #endregion
}

[System.Serializable]
public class BU_OneTypeData<T>
{
    public T BaseValue;
    public List<BU_EachLevelData<T>> BU_EachLevelDataList;

    public void Offset(T _BaseValue)
    {
        BaseValue = _BaseValue;

        if (BaseValue.GetType() == typeof(float))
        {
            float floatValue = float.Parse(BaseValue.ToString());
            Offset(floatValue);
        }
    }

    private void Offset(float _FloatValue)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i >= 0 && i <= 2)
            {
                BU_EachLevelDataList[i].SetUpgradeValue(_FloatValue + (_FloatValue * (i + 1) * 0.1f));
                BU_EachLevelDataList[i].NeedEC_ForUpgrade = 1;
            }
            else if (i >= 3 && i <= 5)
            {
                int _i = i - 2;
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[2].GetUpgradeValue() + (_FloatValue * _i * 0.25f));
                BU_EachLevelDataList[i].NeedEC_ForUpgrade = 2;
            }
            else if (i >= 6 && i <= 8)
            {
                int _i = i - 5;
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[5].GetUpgradeValue() + (_FloatValue * _i * 0.45f));
                BU_EachLevelDataList[i].NeedEC_ForUpgrade = 3;
            }
            else
            {
                int _i = i - 8;
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[8].GetUpgradeValue() + (_FloatValue * _i * 0.7f));
                BU_EachLevelDataList[i].NeedEC_ForUpgrade = 5;
            }
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
