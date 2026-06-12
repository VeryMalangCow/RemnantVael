using UnityEngine;

public class BaseUpgradeController : DestructibleBuildController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> BU")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public int spawnItem_OffMin;
    [SerializeField] public int spawnItem_OffMax;
    [SerializeField] public int spawnItem_OnMin;
    [SerializeField] public int spawnItem_OnMax;
    [SerializeField] public int spawnItem_BreakMin;
    [SerializeField] public int spawnItem_BreakMax;

    public static BaseUpgradeController usingShop = null;

    [HideInInspector] public static string isBrokenAnno;

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_LanguageTxt();

        onOffAc = ResourceManager.instance.buShop_OnOffAC;
        onOffStateAc = ResourceManager.instance.needChargeBettery_OnOffStateAC;

        brokenAc = ResourceManager.instance.buShop_BrokenAC;
        brokenStateAc = ResourceManager.instance.brokenStateAC;

        base.Offset();
    }

    #endregion

    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = !isBroken;
        return ResourceManager.instance.Get_StaticWord(97);
    }

    public void PlayInteract()
    {
        if (isBroken) return; 

        Try_ShopInteract();
        Set_StateAnim();
    }

    private void Try_ShopInteract()
    {
        if (isOn)
        {
            usingShop = this;
            Set_LanguageTxt();
            MainGameUIManager.instance.buUi.SetOnThisPanel();
        }
        else if (Can_ShopPowerOn())
        {

            PlayerManager.instance.playerController.UseChargedBettery(1);
            isOn = true;
            SoundManager.instance.Play_2D_SFX_Build("PowerOn");
        }
    }

    private bool Can_ShopPowerOn()
        => !isOn && PlayerManager.instance.playerController.chargedBettery > 0;
    

    #endregion

    #region Break

    public override void Take_Damage(bool spawnItem, bool soundOn)
    {
        base.Take_Damage(spawnItem, soundOn);

        MainGameUIManager.instance.buUi.durEui.Set_Dur(currentDur);
    }

    protected override void Play_NowBreak(bool spawnItem)
    {
        base.Play_NowBreak(spawnItem);

        if (MainGameUIManager.instance.buUi.gameObject.activeSelf)
            MainGameUIManager.instance.buUi.msgEui.Play_On(isBrokenAnno, 0.5f);
    }

    #endregion

    #region Item

    private void Gen_RandomBS(int offMin, int offMax, int onMin, int onMax)
    {
        if (!isOn)
        {
            Gen_RandomBS(offMin, offMax);
        }
        else
        {
            Gen_RandomBS(onMin, onMax);
        }
    }

    public override void Gen_ItemWhenHitted()
    {
        Gen_RandomBS(spawnItem_OffMin, spawnItem_OffMax, spawnItem_OnMin, spawnItem_OnMax);
    }

    public override void Gen_ItemWhenBreak() 
    {
        Gen_RandomBS(spawnItem_BreakMin, spawnItem_BreakMax);
    }

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        isBrokenAnno = $"<size=25&>{ResourceManager.instance.Get_StaticWord(24)}: {ResourceManager.instance.Get_StaticDesc(16)}</size>\n\n" +
            $"{ResourceManager.instance.Get_StaticDesc(17)}\n" +
            $"<size=50&>{ResourceManager.instance.Get_StaticDesc(18)}</size>";
    }

    #endregion
}