using UnityEngine;

public class AllyModuleUpgradeController : DestructibleBuildController, IInteract
{
    #region Value

    #region - Inspector

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

    #endregion

    #region - Hide

    public static AllyModuleUpgradeController UsingShop = null;

    [HideInInspector] public static string IsBrokenAnno;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_LanguageTxt();

        OnOffAC = UnitManager.Instance.AllyMUShop_OnOffAC;
        OnOffStateAC = UnitManager.Instance.NeedChargeBettery_OnOffStateAC;

        BrokenAC = UnitManager.Instance.AllyMUShop_BrokenAC;
        BrokenStateAC = UnitManager.Instance.BrokenStateAC;

        base.Offset();
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
            Set_LanguageTxt();
            MainGameUIManager.Instance.AllyModuleUpgrade_UIController.SetOn_ThisPanel();
        }
        else if (Can_ShopPowerOn())
        {
            PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value--;
            IsOn = true;
        }
    }

    private bool Can_ShopPowerOn()
    {
        return !IsOn && PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value > 0;
    }

    #endregion

    #region Break

    public override void Take_Damage(bool _SpawnItem)
    {
        base.Take_Damage(_SpawnItem);

        // MainGameUIManager.Instance.AllyModuleUpgrade_UIController.ThisDurEUI.Set_Dur(CurrentDur);
    }

    protected override void Play_NowBreak(bool _SpawnItem)
    {
        base.Play_NowBreak(_SpawnItem);

        // if (MainGameUIManager.Instance.AllyModuleUpgrade_UIController.gameObject.activeSelf)
        //     MainGameUIManager.Instance.AllyModuleUpgrade_UIController.ThisMsgEUI.Play_On(IsBrokenAnno, 0.5f);
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

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        //IsBrokenAnno = $"<size=25&>{ResourceManager.Instance.Get_StaticWord(24)}: {ResourceManager.Instance.Get_StaticDesc(16)}</size>\n\n" +
        //    $"{ResourceManager.Instance.Get_StaticDesc(17)}\n" +
        //    $"<size=50&>{ResourceManager.Instance.Get_StaticDesc(19)}</size>";
    }

    #endregion
}
