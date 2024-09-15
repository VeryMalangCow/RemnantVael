using UnityEngine;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;

public class OneOffShopUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> One-Off Shop")]

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private OneOffShopEachData<float> DamageShop;
    [SerializeField] private OneOffShopEachData<float> ROFShop;
    [SerializeField] private OneOffShopEachData<float> MaxEPShop;
    [SerializeField] private OneOffShopEachData<float> WalkSpeedShop;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Button CloseBtn;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        DamageShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage, BaseUpgradeManager.Instance.BaseDamage_BUData);
        ROFShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.ROF, BaseUpgradeManager.Instance.BaseROF_BUData);
        MaxEPShop.Offset(PlayerManager.Instance.PlayerController.MaxEP, BaseUpgradeManager.Instance.BaseMaxEP_BUData);
        WalkSpeedShop.Offset(PlayerManager.Instance.PlayerController.WalkSpeed, BaseUpgradeManager.Instance.BaseWalkSpeed_BUData);
    }

    protected override void Offset_UI()
    {
        // Close Btn
        CloseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                UIManager.Instance.OneOffShopUIController.CloseThisPanel(TabDurTime);
            });

        // Tab Btn List
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            int index = i;
            ThisPanelTabList[index].ThisTabBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    ChangeThisPanel(TabDurTime, index);
                });
        }

        // BG Offset
        if (TryGetComponent(out Image img))
        {
            Color BGColor = img.color;
            BGColor.a = 0f;
            img.color = BGColor;
        }
    }

    #endregion

    #region Set Panel

    public override void OpenThisPanel(float _DurTime)
    {
        base.OpenThisPanel(_DurTime);

        if(TryGetComponent(out Image img))
        {
            img.DOFade(0.5f, _DurTime);
        }
    }

    public override void CloseThisPanel(float _DurTime)
    {
        if (IsTweening)
        { return; }

        base.CloseThisPanel(_DurTime);

        if (TryGetComponent(out Image img))
        {
            img.DOFade(0f, _DurTime);
        }
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
               int currentLv = _CurrentLevel;
               if(currentLv < _Upgrade_BUOTD.BU_EachLevelDataList.Count)
               {
                   Upgrade_MTAFB.Set(currentLv, _Upgrade_BUOTD.BU_EachLevelDataList[currentLv].NeedEC_ForUpgrade);
               }
               else if (currentLv == _Upgrade_BUOTD.BU_EachLevelDataList.Count)
               {
                   Upgrade_MTAFB.Set(currentLv, 0);
               }
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
