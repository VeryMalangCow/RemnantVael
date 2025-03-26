using UnityEngine;

public class BaseUpgradeController : DestructibleBuildController, IInteract
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

    public static BaseUpgradeController UsingShop = null;

    [HideInInspector] public static string IsBrokenAnno = 
        "<size=25&>Broken: Interaction is Limited</size>\n\n" +
        "!!! If you close this window now, you will not be able to interact with this shop. !!!\n" +
        "<size=50&>You can only View the Information.</size>";

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        // VFX
        UnitManager.Instance.Build_ExplImgGenerator.Expl_Build(TargetObject.gameObject.transform.position);
    }

    #endregion

    #region Interact

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
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOn_ThisPanel();
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

        MainGameUIManager.Instance.BaseUpgrade_UIController.ThisDurEUI.Set_Dur(CurrentDur);
    }

    protected override void Play_NowBreak(bool _SpawnItem)
    {
        base.Play_NowBreak(_SpawnItem);

        if (MainGameUIManager.Instance.BaseUpgrade_UIController.gameObject.activeSelf)
            MainGameUIManager.Instance.BaseUpgrade_UIController.ThisMsgEUI.Play_On(IsBrokenAnno, 0.5f);
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
}