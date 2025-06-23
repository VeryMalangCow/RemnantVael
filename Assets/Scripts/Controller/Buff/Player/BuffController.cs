using UniRx;
using UnityEngine;

public class BuffController : IDController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]
    [SerializeField] public string BuffName = "";

    [Space(10)]
    [Header("=== Charge")]
    [SerializeField] protected int MaxBuffCharge = 1;
    [SerializeField] private int GainCharge = 1;
    [SerializeField] private int ReductionCharge = 1;
    // False = Reduction / True = Increase
    [SerializeField] protected bool IsIncreaseByTime = false;


    [Space(10)]
    [Header("=== Timer")]
    [SerializeField] private bool Condition_DurTimer = false;
    [SerializeField] protected float MaxDurTime = 1;
    [SerializeField] private bool InitializationWhenGain = true;
    [SerializeField] private bool InitializationWhenLoss = false;

    [Space(10)]
    [Header("=== Other Condition")]
    [SerializeField] public bool Condition_PlayerSyncSet = false;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Sprite ThisIconSprite;


    [HideInInspector] protected ReactiveProperty<int> CurrentBuffCharge = new();
    [HideInInspector] private ReactiveProperty<float> CurrentDurTime = new();
    [HideInInspector] protected BuffIconEUIController ThisBuffEUI = null;

    #endregion

    #region Framework

    protected virtual void Start()
    {
        CurrentDurTime.Value = 0;
        CurrentBuffCharge.Value = 0;

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
        if (!Condition_DurTimer) return;

        if (IsIncreaseByTime)
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
        if (CurrentBuffCharge.Value <= 0) return;

        Caculate_Cooltime();
        Caculate_BuffUI(_IsReductionTimer: true);
        Caculate_CoolTimeCharge(new Dele(Reduct_Buff));

    }

    private void Caculate_IncreaseTimer()
    {
        if (CurrentBuffCharge.Value >= MaxBuffCharge) return;

        Caculate_Cooltime();
        Caculate_BuffUI(_IsReductionTimer: false);
        Caculate_CoolTimeCharge(new Dele(Gain_Buff));
    }


    private void Caculate_Cooltime()
    {
        if (CurrentDurTime.Value < MaxDurTime)
        {
            CurrentDurTime.Value += Time.deltaTime;
        }
    }

    private void Caculate_CoolTimeCharge(Dele _Dele)
    {
        if (CurrentDurTime.Value >= MaxDurTime)
        {
            CurrentDurTime.Value -= MaxDurTime;
            _Dele();
        }
    }

    private void Caculate_BuffUI(bool _IsReductionTimer)
    {
        if (ThisBuffEUI != null)
        {
            float percent = CurrentDurTime.Value / MaxDurTime;
            ThisBuffEUI.ThisShadowImg.fillAmount = _IsReductionTimer ?
                percent : (1f - percent);
        }
    }

    #endregion

    #region Buff Effect

    private void Add_BuffEffect()
    {
        DevTool.Add_InList(PlayerManager.Instance.PlayerController.CurrentBuffs, this);

        switch (this)
        {
            case IWhen_GetElectricity elec:
                DevTool.Add_InList(BuffManager.Instance.IWhen_GetElectricityList, elec);
                break;

            default:
                break;
        }
    }

    private void Remove_BuffEffect()
    {
        DevTool.Remove_InList(PlayerManager.Instance.PlayerController.CurrentBuffs, this);

        switch (this)
        {
            case IWhen_GetElectricity elec:
                DevTool.Remove_InList(BuffManager.Instance.IWhen_GetElectricityList, elec);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Is

    private void Is_MaxBuff()
    {
        if (CurrentBuffCharge.Value >= MaxBuffCharge)
        {
            CurrentDurTime.Value = 0;
        }
    }

    private void Is_EndBuff()
    {
        if (CurrentBuffCharge.Value <= 0 && !IsIncreaseByTime)
        {
            End_Buff();
            CurrentDurTime.Value = 0;
        }
    }

    #endregion

    #region UI

    private void Gain_BuffUI()
    {
        ThisBuffEUI = PoolingManager.Instance.Get_OP_BuffUI();
        ThisBuffEUI.Offset();
        ThisBuffEUI.ThisShadowImg.fillAmount = 0;

        DevTool.Add_InList(MainGameUIManager.Instance.PlayerHUD_UIController.AllBuffIconUI, ThisBuffEUI);

        ThisBuffEUI.gameObject.SetActive(true);
    }

    private void Remove_BuffUI()
    {
        DevTool.Remove_InList(MainGameUIManager.Instance.PlayerHUD_UIController.AllBuffIconUI, ThisBuffEUI);

        ThisBuffEUI.gameObject.SetActive(false);
        PoolingManager.Instance.BuffIcons.Queue.Enqueue(ThisBuffEUI);
        ThisBuffEUI = null;
    }

    #endregion

    #region Actual

    public virtual void Max_Buff()
    {
        // Value
        CurrentBuffCharge.Value = MaxBuffCharge;

        Add_BuffEffect();
        Is_MaxBuff();

        // UI
        if (ThisBuffEUI == null)
        {
            Gain_BuffUI();
        }

        MainGameUIManager.Instance.PlayerHUD_UIController.Set_BuffPosUI();
        ThisBuffEUI.Set_Icon(ThisIconSprite, CurrentBuffCharge.Value, MaxBuffCharge);

        enabled = true;
    }

    public virtual void Gain_Buff()
    {
        // Value
        CurrentBuffCharge.Value = Mathf.Min(CurrentBuffCharge.Value + GainCharge, MaxBuffCharge);
        
        // Initialization
        if (InitializationWhenGain)
        { CurrentDurTime.Value = 0; }

        //
        Add_BuffEffect();
        Is_MaxBuff();

        // UI
        if (ThisBuffEUI == null)
        {
            Gain_BuffUI();
        }

        MainGameUIManager.Instance.PlayerHUD_UIController.Set_BuffPosUI();
        ThisBuffEUI.Set_Icon(ThisIconSprite, CurrentBuffCharge.Value, MaxBuffCharge);

        enabled = true;
    }

    public virtual void Reduct_Buff()
    {
        // Value
        CurrentBuffCharge.Value = Mathf.Max(CurrentBuffCharge.Value - ReductionCharge, 0);

        // Initialization
        if (InitializationWhenLoss)
        { CurrentDurTime.Value = 0; }

        if (ThisBuffEUI == null)
        {
            Gain_BuffUI();
        }

        // UI
        MainGameUIManager.Instance.PlayerHUD_UIController.Set_BuffPosUI();
        ThisBuffEUI.Set_Icon(ThisIconSprite, CurrentBuffCharge.Value, MaxBuffCharge);

        //
        Is_EndBuff();
    }

    public virtual void End_Buff()
    {
        // Value
        CurrentBuffCharge.Value = 0;
        CurrentDurTime.Value = 0;

        Remove_BuffEffect();

        if (ThisBuffEUI != null)
        {
            Remove_BuffUI();
        }

        MainGameUIManager.Instance.PlayerHUD_UIController.Set_BuffPosUI();

        enabled = false; 
    }

    #endregion
}
