using UnityEngine;

public class ModuleUpgradeController : DestructibleBuildController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> BU")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public int SpawnItem_OffMin;
    [SerializeField] public int SpawnItem_OffMax;
    [SerializeField] public int SpawnItem_OnMin;
    [SerializeField] public int SpawnItem_OnMax;
    [SerializeField] public int SpawnItem_BreakMin;
    [SerializeField] public int SpawnItem_BreakMax;

    public static ModuleUpgradeController UsingShop = null;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        Gen_ExplosionEffect();
    }

    #endregion

    #region Interact

    public void Play_Interact()
    {
        if (IsBroken)
        { return; }

        Try_ShopInteract();
        Set_StateAnim();
    }

    private void Try_ShopInteract()
    {
        if (IsOn)
        {
            UsingShop = this;
            MainGameUIManager.Instance.ModuleUpgrade_UIController.SetOn_ThisPanel();
        }
        else if (Can_ShopPowerOn())
        {
            PlayerManager.Instance.PlayerController.CurrentEC.Value--;
            IsOn = true;
        }
    }

    private bool Can_ShopPowerOn()
    {
        return !IsOn && PlayerManager.Instance.PlayerController.CurrentEC.Value > 0;
    }

    #endregion

    #region Break

    public override void Take_Damage(bool _SpawnItem)
    {
        base.Take_Damage(_SpawnItem);

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_Dur(CurrentDur);
    }

    protected override void Play_NowBreak(bool _SpawnItem)
    {
        base.Play_NowBreak(_SpawnItem);

        if (MainGameUIManager.Instance.ModuleUpgrade_UIController.gameObject.activeSelf)
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.SetOff_ThisPanel();
        }
    }
    #endregion

    #region Item

    private void Gen_RandomMS(int _OffMin, int _OffMax, int _OnMin, int _OnMax)
    {
        if (!IsOn)
        {
            Gen_RandomMS(_OffMin, _OffMax);
        }
        else
        {
            Gen_RandomMS(_OnMin, _OnMax);
        }
    }

    public override void Gen_ItemWhenHitted()
    {
        Gen_RandomMS(SpawnItem_OffMin, SpawnItem_OffMax, SpawnItem_OnMin, SpawnItem_OnMax);
    }

    public override void Gen_ItemWhenBreak()
    {
        Gen_RandomMS(SpawnItem_BreakMin, SpawnItem_BreakMax);
    }

    #endregion
}