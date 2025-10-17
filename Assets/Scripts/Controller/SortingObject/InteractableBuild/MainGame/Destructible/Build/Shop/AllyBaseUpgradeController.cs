using UnityEngine;

public class AllyBaseUpgradeController : DestructibleBuildController, IInteract
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

    public static AllyBaseUpgradeController UsingShop = null;

    [HideInInspector] public static string IsBrokenAnno;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_LanguageTxt();

        OnOffAC = UnitManager.Instance.AllyBUShop_OnOffAC;
        OnOffStateAC = UnitManager.Instance.NeedChargeBettery_OnOffStateAC;
        
        BrokenAC = UnitManager.Instance.AllyBUShop_BrokenAC;
        BrokenStateAC = UnitManager.Instance.BrokenStateAC;

        base.Offset();
    }

    #endregion

    #region Interact

    public string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = !IsBroken;
        return ResourceManager.Instance.Get_StaticWord(99);
    }

    public void Play_Interact()
    {
        if (IsBroken) return;

        Try_ShopInteract();
        Set_StateAnim();
    }

    private void Try_ShopInteract()
    {
        if (IsOn)
        {
            UsingShop = this;
            Set_LanguageTxt();
            MainGameUIManager.Instance.AllyBaseUpgrade_UIController.SetOn_ThisPanel();
        }
        else if (Can_ShopPowerOn())
        {
            PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value--;
            IsOn = true;
            SoundManager.Instance.Play_2D_SFX_Build("PowerOn");
        }
    }

    private bool Can_ShopPowerOn()
    {
        return !IsOn && PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value > 0;
    }

    #endregion

    #region Break

    public override void Take_Damage(bool _SpawnItem, bool _SoundOn)
    {
        base.Take_Damage(_SpawnItem, _SoundOn);

        MainGameUIManager.Instance.AllyBaseUpgrade_UIController.ThisDurEUI.Set_Dur(CurrentDur);
    }

    protected override void Play_NowBreak(bool _SpawnItem)
    {
        base.Play_NowBreak(_SpawnItem);

        if (MainGameUIManager.Instance.AllyBaseUpgrade_UIController.gameObject.activeSelf)
            MainGameUIManager.Instance.AllyBaseUpgrade_UIController.ThisMsgEUI.Play_On(IsBrokenAnno, 0.5f);
    }

    #endregion

    #region Item

    private void Gen_RandomBS(int _OffMin, int _OffMax, int _OnMin, int _OnMax)
    {
        if (!IsOn)
        {
            Gen_RandomBS(_OffMin, _OffMax);
        }
        else
        {
            Gen_RandomBS(_OnMin, _OnMax);
        }
    }

    public override void Gen_ItemWhenHitted()
    {
        Gen_RandomBS(SpawnItem_OffMin, SpawnItem_OffMax, SpawnItem_OnMin, SpawnItem_OnMax);
    }

    public override void Gen_ItemWhenBreak()
    {
        Gen_RandomBS(SpawnItem_BreakMin, SpawnItem_BreakMax);
    }

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        IsBrokenAnno = $"<size=25&>{ResourceManager.Instance.Get_StaticWord(24)}: {ResourceManager.Instance.Get_StaticDesc(16)}</size>\n\n" +
            $"{ResourceManager.Instance.Get_StaticDesc(17)}\n" +
            $"<size=50&>{ResourceManager.Instance.Get_StaticDesc(18)}</size>";
    }

    #endregion
}
