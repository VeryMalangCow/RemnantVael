using UnityEngine;

public class AllyBaseUpgradeController : DestructibleBuildController, IInteract
{
    #region Value

    #region - Inspector

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

    #endregion

    #region - Hide

    public static AllyBaseUpgradeController usingShop = null;

    [HideInInspector] public static string isBrokenAnno;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_LanguageTxt();

        var prefab = StaticResourceManager.instance.BuildReso;
        var abuPrefab = StaticResourceManager.instance.BuildReso.abuPrefab;

        onOffAc = abuPrefab.onOffAniamtion;
        onOffStateAc = prefab.needChargeBetteryOnOffStateAnimation;

        brokenAc = abuPrefab.brokenAnimation;
        brokenStateAc = prefab.brokenStateAnimation;

        thisSr.material = abuPrefab.material;
        stateAnim.sr.material = abuPrefab.iconMaterial;

        base.Offset();
    }

    #endregion

    #region Interact

    public string Get_InteractName(out bool canInteract)
    {
        canInteract = !isBroken;
        return ResourceManager.instance.Get_StaticWord(99);
    }

    public void PlayInteract()
    {
        if (isBroken) return;

        TryShopInteract();
        Set_StateAnim();
    }

    private void TryShopInteract()
    {
        if (isOn)
        {
            usingShop = this;
            Set_LanguageTxt();
            MainGameUIManager.instance.abuUi.SetOnThisPanel();
        }
        else if (CanShopPowerOn())
        {
            PlayerManager.instance.playerController.UseChargedBettery(1);
            isOn = true;
            SoundManager.instance.PlayBuildSfx(transform.position, "PowerOn");
        }
    }

    private bool CanShopPowerOn()
        => !isOn && PlayerManager.instance.playerController.chargedBettery > 0;
    

    #endregion

    #region Break

    public override void Take_Damage(bool spawnItem, bool soundOn)
    {
        base.Take_Damage(spawnItem, soundOn);

        MainGameUIManager.instance.abuUi.durEui.Set_Dur(currentDur);
    }

    protected override void Play_NowBreak(bool spawnItem)
    {
        base.Play_NowBreak(spawnItem);

        if (MainGameUIManager.instance.abuUi.gameObject.activeSelf)
            MainGameUIManager.instance.abuUi.msgEui.Play_On(isBrokenAnno, 0.5f);
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
