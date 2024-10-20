using UnityEngine;

public class ActiveSkillController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Active Skill")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] public PlayerController PlayerController;

    [Header("-- Charge")]
    [SerializeField] protected int MaxChargeAmount = 1;
    [SerializeField] protected int CurrentChargeAmount = 0;

    [Header("-- Cooltime")]
    [SerializeField] protected BaseUpgradeState<float> MaxCooltime;
    [SerializeField] protected float CurrentCooltime = 0f;

    [Header("-- State")]
    [SerializeField] protected float NeedEP = 10f;
    [SerializeField] protected BaseUpgradeState<int> Tier;
    [SerializeField] protected BaseUpgradeState<float> Power;


    #endregion

    #region Framework

    protected virtual void Update()
    {
        CaculateCooltime();
    }

    #endregion

    #region Cooltime

    private void CaculateCooltime()
    {
        if (MaxCooltime.ActualState.Value > CurrentCooltime &&
            MaxChargeAmount > CurrentChargeAmount)
        {
            CurrentCooltime += Time.deltaTime;

            if (MaxCooltime.ActualState.Value <= CurrentCooltime)
            {
                CurrentChargeAmount++;
                if (CurrentChargeAmount >= MaxChargeAmount) 
                {
                    CurrentCooltime = 0;
                }
                else
                {
                    CurrentCooltime = CurrentCooltime - MaxCooltime.ActualState.Value;
                }
            }
        }
    }

    #endregion

    #region Act

    public virtual void ActiveSkill()
    {
        CurrentChargeAmount--;
        PlayerController.AddCurrentEP(-NeedEP);
    }

    #endregion

    #region Judg Can Active

    protected bool CanActive()
    {
        if ((CurrentChargeAmount > 0) &&
            (PlayerManager.Instance.PlayerController.CurrentEP.Value > NeedEP * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value) &&
            PlayerController.MovementState == eMovementState.IdleOrWalk)
        {
            return true;
        }
        return false;
    }

    #endregion
}
