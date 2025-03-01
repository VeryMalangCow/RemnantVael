using UniRx;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]
    [SerializeField] public int BuffID = 0;
    [SerializeField] public string BuffName = "";

    [Space(10)]
    [SerializeField] private int MaxBuffCharge = 1;
    [SerializeField] protected ReactiveProperty<int> CurrentBuffCharge = new();
    [SerializeField] private int GainCharge = 1;
    [SerializeField] private int ReductionCharge = 1;
    [SerializeField] [Tooltip(" False = Reduction / True = Increase")] 
    protected bool IsReductionOrIncrease = false;

    [Space(10)]
    [Header("=== Timer")]
    [SerializeField] private bool DurTimerWillDone = false;
    [SerializeField] private float MaxDurTime = 1;
    [SerializeField] private ReactiveProperty<float> CurrentDurTime = new();
    [SerializeField] private bool InitializationWhenGain = true;
    [SerializeField] private bool InitializationWhenLoss = false;

    [Space(10)]
    [Header("=== Hitted")]
    //[SerializeField] private bool HittedWillDone = false;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Sprite ThisIconSprite;
    [SerializeField] protected ModifyBuffIcon ThisMBI = null;

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
        if (!IsReductionOrIncrease)
        {
            Caculate_ReductionTimer();
        }
        else
        {
            Caculate_IncreaseTimer();
        }
    }

    private void Caculate_ReductionTimer()
    {
        // 지속시간이 존재 + 현재 버프가 진행중이라면
        if (DurTimerWillDone && CurrentBuffCharge.Value > 0)
        {
            // 지속시간이 흐름
            if (CurrentDurTime.Value < MaxDurTime)
            {
                CurrentDurTime.Value += Time.deltaTime;
            }
            if (ThisMBI != null)
            {
                ThisMBI.ThisShadowImg.fillAmount = CurrentDurTime.Value / MaxDurTime;
            }

            // 지속 시간이 다 되었다면
            if (CurrentDurTime.Value >= MaxDurTime)
            {
                CurrentDurTime.Value -= MaxDurTime;
                Reduct_Buff();

                if (CurrentBuffCharge.Value <= 0)
                {
                    End_Buff();
                    CurrentDurTime.Value = 0;
                }
            }
        }
    }

    private void Caculate_IncreaseTimer()
    {
        // 지속시간이 존재 + 현재 버프가 진행중이라면
        if (DurTimerWillDone && CurrentBuffCharge.Value < MaxBuffCharge)
        {
            // 지속시간이 흐름
            if (CurrentDurTime.Value < MaxDurTime)
            {
                CurrentDurTime.Value += Time.deltaTime;
            }
            if (ThisMBI != null)
            {
                ThisMBI.ThisShadowImg.fillAmount = 1.0f - (CurrentDurTime.Value / MaxDurTime);
            }

            // 지속 시간이 다 되었다면
            if (CurrentDurTime.Value >= MaxDurTime)
            {
                Gain_Buff();
                CurrentDurTime.Value -= MaxDurTime;

                if (CurrentBuffCharge.Value >= MaxBuffCharge)
                {
                    CurrentDurTime.Value = 0;
                }
            }
        }
    }

    #endregion

    #region Buff

    public virtual void Gain_Buff()
    {
        CurrentBuffCharge.Value = Mathf.Min(CurrentBuffCharge.Value + GainCharge, MaxBuffCharge);
        
        if (InitializationWhenGain)
        { CurrentDurTime.Value = 0; }

        PlayerManager.Instance.PlayerController.Set_GainBuff(this);

        if (ThisMBI == null)
        {
            // UI
            ThisMBI = PoolingManager.Instance.Get_OP_BuffUI();
            ThisMBI.Offset();
            ThisMBI.Set_Icon(ThisIconSprite, CurrentBuffCharge.Value); 
            ThisMBI.ThisShadowImg.fillAmount = 0;
            ThisMBI.gameObject.SetActive(true);

            // UI Pos
            MainGameUIManager.Instance.PlayerHUD_UIController.Set_GainBuffUI(ThisMBI);
        }
        

        ThisMBI.Set_Icon(CurrentBuffCharge.Value);
    }

    public virtual void Reduct_Buff()
    {
        CurrentBuffCharge.Value = Mathf.Max(CurrentBuffCharge.Value - ReductionCharge, 0);

        if (InitializationWhenLoss)
        { CurrentDurTime.Value = 0; }

        if (ThisMBI != null)
        {
            ThisMBI.Set_Icon(CurrentBuffCharge.Value);

            // UI Pos
            MainGameUIManager.Instance.PlayerHUD_UIController.Set_ReductBuffUI(ThisMBI);
        }
    }

    public virtual void End_Buff()
    {
        CurrentBuffCharge.Value = 0;
        CurrentDurTime.Value = 0;

        PlayerManager.Instance.PlayerController.Set_EndBuff(this);

        if (ThisMBI != null)
        {
            // UI
            ThisMBI.gameObject.SetActive(false);
            PoolingManager.Instance.BuffIcons.Queue.Enqueue(ThisMBI);
            ThisMBI = null;

            // UI Pos
            MainGameUIManager.Instance.PlayerHUD_UIController.Set_EndBuffUI(ThisMBI);
        }

        enabled = false; 
    }

    #endregion
}
