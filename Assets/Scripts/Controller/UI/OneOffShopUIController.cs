using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class OneOffShopUIController : UIController
{
    
    #region Value

    [Space(20)]
    [Header("<><><><><> One-Off Shop")]

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private OneOffShopEachData<float> DamageShop;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Button CloseBtn;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        DamageShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage, BaseUpgradeManager.Instance.BaseDamage_BUData);
    }

    protected override void Offset_UI()
    {
        CloseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                UIManager.Instance.OneOffShopUIController.CloseThisPanel();
            });
    }

    #endregion
}

[System.Serializable]
public class OneOffShopEachData<T>
{
    [SerializeField] private ModifyTextAmountForBuy Upgrade_MTAFB;
    [SerializeField] private Button Upgrade_BuyBtn;

    public void Offset(BaseUpgradeState<T> _Upgrade_BUS, BU_OneTypeData<T> _Upgrade_BUOTD)
    {
        Upgrade_MTAFB.Offset();

        Upgrade_BuyBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                TryBuy(_Upgrade_BUS, _Upgrade_BUOTD);
            });

        _Upgrade_BUS.CurrentLevel
           .Subscribe(_CurrentLevel =>
           {
               int currentLV = _CurrentLevel;
               Upgrade_MTAFB.Set(currentLV);
           });
    }

    private void TryBuy(BaseUpgradeState<T> _Upgrade_BUS, BU_OneTypeData<T> _Upgrade_BUOTD)
    {
        int index = _Upgrade_BUS.CurrentLevel.Value;
        int needEC = _Upgrade_BUOTD.BU_EachLevelDataList[index].NeedEC_ForUpgrade;
        int hadEC = PlayerManager.Instance.PlayerController.CurrentEC.Value;
        Debug.Log(needEC + " / " + hadEC);
        if (needEC <= hadEC)
        {
            Buy(_Upgrade_BUS, needEC, _Upgrade_BUOTD.BU_EachLevelDataList.Count, _Upgrade_BUOTD.BU_EachLevelDataList[index].UpgradeValue);
        }
        else
        {
            NotEnoughEC();
        }
    }

    private void Buy(BaseUpgradeState<T> _Upgrade_BUS, int _UseEC, int _MaxUpgradeLevel, T _SetValue)
    {
        _Upgrade_BUS.CurrentLevel.Value++;
        _Upgrade_BUS.ActualState.Value = _SetValue;
        PlayerManager.Instance.PlayerController.CurrentEC.Value -= _UseEC;
        if (_MaxUpgradeLevel <= _Upgrade_BUS.CurrentLevel.Value)
        {
            Upgrade_BuyBtn.interactable = false;
        }
    }

    private void NotEnoughEC()
    {
        Debug.Log("ºÎÁ·!");
    }
}
