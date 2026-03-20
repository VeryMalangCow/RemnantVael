using UniRx;
using UnityEngine;

public class BuffController : IDController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]
    [SerializeField] public string buffName = "";

    [Space(10)]
    [Header("=== Charge")]
    [SerializeField] public int maxBuffCharge = 1;
    [SerializeField] public int gainCharge = 1;
    [SerializeField] public int reductionCharge = 1;
    [SerializeField] public bool isIncreaseByTime = false;


    [Space(10)]
    [Header("=== Timer")]
    [SerializeField] private bool condition_DurTimer = false;
    [SerializeField] protected float maxDurTime = 1;
    [SerializeField] private bool initializationWhenGain = true;
    [SerializeField] private bool initializationWhenLoss = false;

    [Space(10)]
    [Header("=== Other Condition")]
    [SerializeField] public bool condition_PlayerSyncSet = false;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Sprite thisIconSprite;


    [HideInInspector] protected ReactiveProperty<int> currentBuffCharge = new();
    [HideInInspector] private ReactiveProperty<float> currentDurTime = new();
    [HideInInspector] protected BuffIconEUIController thisBuffEUI = null;

    #endregion

    #region Framework

    protected virtual void Start()
    {
        currentDurTime.Value = 0;
        currentBuffCharge.Value = 0;

        this.enabled = false;
    }

    private void Update()
    {
        Caculate_Timer();
    }

    #endregion

    #region Buff Time Dur

    private void Caculate_Timer()
    {
        if (!condition_DurTimer) return;

        if (isIncreaseByTime)
        {
            Caculate_IncreaseTimer();
        }
        else
        {
            Caculate_ReductionTimer();
        }
    }

    private void Caculate_ReductionTimer()
    {
        if (currentBuffCharge.Value <= 0) return;

        Caculate_Cooltime();
        Caculate_BuffUI(isReductionTimer: true);
        Caculate_CoolTimeCharge(new Dele(Reduct_Buff));

    }

    private void Caculate_IncreaseTimer()
    {
        if (currentBuffCharge.Value >= maxBuffCharge) return;

        Caculate_Cooltime();
        Caculate_BuffUI(isReductionTimer: false);
        Caculate_CoolTimeCharge(new Dele(Gain_Buff));
    }


    private void Caculate_Cooltime()
    {
        if (currentDurTime.Value < maxDurTime)
        {
            currentDurTime.Value += Time.deltaTime;
        }
    }

    private void Caculate_CoolTimeCharge(Dele dele)
    {
        if (currentDurTime.Value >= maxDurTime)
        {
            currentDurTime.Value -= maxDurTime;
            dele();
        }
    }

    private void Caculate_BuffUI(bool isReductionTimer)
    {
        if (thisBuffEUI != null)
        {
            float percent = currentDurTime.Value / maxDurTime;
            thisBuffEUI.ThisShadowImg.fillAmount = isReductionTimer ?
                percent : (1f - percent);
        }
    }

    #endregion

    #region Buff Effect

    private void Add_BuffEffect()
    {
        DevTool.Add_InList(PlayerManager.instance.playerController.CurrentBuffs, this);

        switch (this)
        {
            case IWhen_GetElectricity elec:
                DevTool.Add_InList(BuffManager.instance.iWhen_GetElectricityList, elec);
                break;

            default:
                break;
        }
    }

    private void Remove_BuffEffect()
    {
        DevTool.Remove_InList(PlayerManager.instance.playerController.CurrentBuffs, this);

        switch (this)
        {
            case IWhen_GetElectricity elec:
                DevTool.Remove_InList(BuffManager.instance.iWhen_GetElectricityList, elec);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Is

    private void Is_MaxBuff()
    {
        if (currentBuffCharge.Value >= maxBuffCharge)
        {
            currentDurTime.Value = 0;
        }
    }

    private void Is_EndBuff()
    {
        if (currentBuffCharge.Value <= 0 && !isIncreaseByTime)
        {
            End_Buff();
            currentDurTime.Value = 0;
        }
    }

    #endregion

    #region UI

    private void Gain_BuffUI()
    {
        thisBuffEUI = PoolingManager.instance.Get_OP_BuffUI();
        thisBuffEUI.Offset();
        thisBuffEUI.ThisShadowImg.fillAmount = 0;

        DevTool.Add_InList(MainGameUIManager.instance.playerHUD_UIController.AllBuffIconUI, thisBuffEUI);

        thisBuffEUI.gameObject.SetActive(true);
    }

    private void Remove_BuffUI()
    {
        DevTool.Remove_InList(MainGameUIManager.instance.playerHUD_UIController.AllBuffIconUI, thisBuffEUI);

        thisBuffEUI.gameObject.SetActive(false);
        PoolingManager.instance.buffIcons.Enqueue(thisBuffEUI);
        thisBuffEUI = null;
    }

    #endregion

    #region Actual

    public virtual void Max_Buff()
    {
        // Value
        currentBuffCharge.Value = maxBuffCharge;

        Add_BuffEffect();
        Is_MaxBuff();

        // UI
        if (thisBuffEUI == null)
        {
            Gain_BuffUI();
        }

        MainGameUIManager.instance.playerHUD_UIController.Set_BuffPosUI();
        thisBuffEUI.Set_Icon(thisIconSprite, currentBuffCharge.Value, maxBuffCharge);

        enabled = true;
    }

    public virtual void Gain_Buff()
    {
        // Value
        currentBuffCharge.Value = Mathf.Min(currentBuffCharge.Value + gainCharge, maxBuffCharge);
        
        // Initialization
        if (initializationWhenGain)
        { currentDurTime.Value = 0; }

        //
        Add_BuffEffect();
        Is_MaxBuff();

        // UI
        if (thisBuffEUI == null)
        {
            Gain_BuffUI();
        }

        MainGameUIManager.instance.playerHUD_UIController.Set_BuffPosUI();
        thisBuffEUI.Set_Icon(thisIconSprite, currentBuffCharge.Value, maxBuffCharge);

        enabled = true;
    }

    public virtual void Reduct_Buff()
    {
        // Value
        currentBuffCharge.Value = Mathf.Max(currentBuffCharge.Value - reductionCharge, 0);

        // Initialization
        if (initializationWhenLoss)
        { currentDurTime.Value = 0; }

        if (thisBuffEUI == null)
        {
            Gain_BuffUI();
        }

        // UI
        MainGameUIManager.instance.playerHUD_UIController.Set_BuffPosUI();
        thisBuffEUI.Set_Icon(thisIconSprite, currentBuffCharge.Value, maxBuffCharge);

        //
        Is_EndBuff();
    }

    public virtual void End_Buff()
    {
        // Value
        currentBuffCharge.Value = 0;
        currentDurTime.Value = 0;

        Remove_BuffEffect();

        if (thisBuffEUI != null)
        {
            Remove_BuffUI();
        }

        MainGameUIManager.instance.playerHUD_UIController.Set_BuffPosUI();

        enabled = false; 
    }

    #endregion
}
