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
    [Header("=== Contdition")]
    [SerializeField] private bool ConditionWillDone = false;

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
        CaculateTimer();
    }

    #endregion

    #region Buff Time Dur

    private void CaculateTimer()
    {
        if (!IsReductionOrIncrease)
        {
            ReductionCaculateTimer();
        }
        else
        {
            IncreaseCaculateTimer();
        }
    }

    private void ReductionCaculateTimer()
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
                ReductBuff();

                if (CurrentBuffCharge.Value <= 0)
                {
                    EndBuff();
                    CurrentDurTime.Value = 0;
                }
            }
        }
    }

    private void IncreaseCaculateTimer()
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
                GainBuff();
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

    public virtual void GainBuff()
    {
        Debug.Log("실행?");
        CurrentBuffCharge.Value = Mathf.Min(CurrentBuffCharge.Value + GainCharge, MaxBuffCharge);
        
        if (InitializationWhenGain)
        { CurrentDurTime.Value = 0; }

        PlayerManager.Instance.PlayerController.Set_GainBuff(this);

        if (ThisMBI == null)
        {
            // UI
            ThisMBI = PoolingManager.Instance.GetOP_BuffUI();
            ThisMBI.Offset();
            ThisMBI.SetIcon(ThisIconSprite, CurrentBuffCharge.Value); 
            ThisMBI.ThisShadowImg.fillAmount = 0;
            ThisMBI.gameObject.SetActive(true);

            // UI Pos
            MainGameUIManager.Instance.PlayerHUD_UIController.SetUI_GainBuff(ThisMBI);
        }
        

        ThisMBI.SetIcon(CurrentBuffCharge.Value);
    }

    public virtual void ReductBuff()
    {
        CurrentBuffCharge.Value = Mathf.Max(CurrentBuffCharge.Value - ReductionCharge, 0);

        if (InitializationWhenLoss)
        { CurrentDurTime.Value = 0; }

        if (ThisMBI != null)
        {
            ThisMBI.SetIcon(CurrentBuffCharge.Value);

            // UI Pos
            MainGameUIManager.Instance.PlayerHUD_UIController.SetUI_ReductBuff(ThisMBI);
        }
    }

    public virtual void EndBuff()
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
            MainGameUIManager.Instance.PlayerHUD_UIController.SetUI_EndBuff(ThisMBI);
        }

        enabled = false; 
    }

    #endregion
}
