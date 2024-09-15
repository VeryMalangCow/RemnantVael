using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BaseUpgradeManager : Singleton<BaseUpgradeManager>
{
    #region Value

    [Space(10)]
    [Header("=== Data")]

    [Header("-- Attack")]
    [SerializeField] public BU_OneTypeData<float> BaseDamage_BUData;

    [SerializeField] public BU_OneTypeData<float> BaseROF_BUData; 

    [SerializeField] public BU_OneTypeData<float> BaseAccuracyRate_BUData; 


    [Header("-- EP")]
    [SerializeField] public BU_OneTypeData<float> BaseMaxEP_BUData;


    [Header("-- Movement")]
    [SerializeField] public BU_OneTypeData<float> BaseWalkSpeed_BUData; 

    #endregion

    #region Offset

    public void Offset(PlayerController _PC)
    {
        BaseDamage_BUData.Offset(_PC.BaseWeapon.BaseDamage.ActualState.Value);
        BaseROF_BUData.Offset(_PC.BaseWeapon.ROF.ActualState.Value);
        BaseAccuracyRate_BUData.Offset(_PC.BaseWeapon.AccuracyRate.ActualState.Value);

        BaseMaxEP_BUData.Offset(_PC.MaxEP.ActualState.Value);

        BaseWalkSpeed_BUData.Offset(_PC.WalkSpeed.ActualState.Value);
    }

    #endregion
}

[System.Serializable]
public class BU_OneTypeData<T>
{
    [Header("=== Need Input")]
    public float UpgradeValue;
    public int UpgradeType;

    [Header("=== No Input")]
    public T BaseValue;
    public List<BU_EachLevelData<T>> BU_EachLevelDataList;

    public void Offset(T _BaseValue)
    {
        BaseValue = _BaseValue;

        if (BaseValue.GetType() == typeof(float))
        {
            float floatValue = float.Parse(BaseValue.ToString());
            switch(UpgradeType)
            {
                case 0:
                    Offset_Type_00(floatValue, UpgradeValue);
                    break;

                case 1:
                    Offset_Type_01(floatValue, UpgradeValue);
                    break;

                default:
                    break;
            }
        }
    }

    private void Offset_Type_00(float _FloatValue, float _UpgradeValue)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i == 0)
            {
                BU_EachLevelDataList[i].SetUpgradeValue(_FloatValue + (1f * _UpgradeValue));
            }
            else
            {
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[i - 1].GetUpgradeValue() + (((int)(i / 3) + 1) * _UpgradeValue));
            }

            BU_EachLevelDataList[i].NeedEC_ForUpgrade = ((int)(i / 3) + 1);
        }
    }

    public void Offset_Type_01(float _FloatValue, float _UpgradeValue)
    {
        for (int i = 0; i < BU_EachLevelDataList.Count; i++)
        {
            if (i >= 0 && i <= 2)
            {
                BU_EachLevelDataList[i].SetUpgradeValue(_FloatValue + (_FloatValue * (i + 1) * 0.1f * _UpgradeValue));
            }  
            else if (i >= 3 && i <= 5)
            {
                int _i = i - 2;
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[2].GetUpgradeValue() + (_FloatValue * _i * 0.25f * _UpgradeValue));
            }
            else if (i >= 6 && i <= 8)
            {
                int _i = i - 5;
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[5].GetUpgradeValue() + (_FloatValue * _i * 0.45f * _UpgradeValue));
            }
            else
            {
                int _i = i - 8;
                BU_EachLevelDataList[i].SetUpgradeValue((float)BU_EachLevelDataList[8].GetUpgradeValue() + (_FloatValue * _i * 0.7f * _UpgradeValue));
            }

            BU_EachLevelDataList[i].NeedEC_ForUpgrade = ((int)(i / 3) + 1);
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
