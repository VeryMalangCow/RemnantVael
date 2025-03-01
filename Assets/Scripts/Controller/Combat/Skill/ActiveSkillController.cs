using UniRx;
using UnityEngine;

public class ActiveSkillController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Active Skill")]


    [Space(10)]
    [Header("=== Value")]

    [Header("-- Charge")]
    [SerializeField] protected int MaxChargeAmount = 1;
    [SerializeField] protected int CurrentChargeAmount = 0;

    [Header("-- Cooltime")]
    [SerializeField] public BUState<float> MaxCooltime;
    [SerializeField] protected float CurrentCooltime = 0f;

    [Header("-- State")]
    [SerializeField] public Sprite ThisSkillUISprite;
    [SerializeField] public ReactiveProperty<float> NeedEP = new();
    [SerializeField] public BUState<int> Tier;
    [SerializeField] public BUState<float> Power;

    [HideInInspector] public PlayerController PlayerController;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        Caculate_Cooltime();
    }

    #endregion

    #region Cooltime

    private void Caculate_Cooltime()
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

    public float Get_FillAmount()
    {
        if (MaxChargeAmount <= CurrentChargeAmount)
        {
            return 0;
        }
        else
        {
            return 1 - (CurrentCooltime / MaxCooltime.ActualState.Value);
        }
    }

    #endregion

    #region Act

    public virtual void Active_Skill()
    {
        CurrentChargeAmount--;
        PlayerController.Add_CurrentEP(
            -(NeedEP.Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value));

        Set_StartUI();
    }

    protected void Set_StartUI()
    {
        if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.Set_StartUI(); }
        else if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.Set_StartUI(); }
    }

    protected void Set_EndUI()
    {
        if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.Set_EndUI(); }
        else if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.Set_EndUI(); }
    }

    #endregion

    #region Judg Can Active

    public bool Can_Active()
    {
        if ((CurrentChargeAmount > 0) &&
            (PlayerManager.Instance.PlayerController.CurrentEP.Value > NeedEP.Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value) &&
            PlayerController.MovementState == eMovementState.IdleOrWalk)
        {
            return true;
        }
        return false;
    }

    #endregion
}
