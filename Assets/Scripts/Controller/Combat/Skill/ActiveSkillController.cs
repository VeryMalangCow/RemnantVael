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
    [SerializeField] public BaseUpgradeState<float> MaxCooltime;
    [SerializeField] protected float CurrentCooltime = 0f;

    [Header("-- State")]
    [SerializeField] public Sprite ThisSkillUISprite;
    [SerializeField] public ReactiveProperty<float> NeedEP = new();
    [SerializeField] public BaseUpgradeState<int> Tier;
    [SerializeField] public BaseUpgradeState<float> Power;

    [HideInInspector] public PlayerController PlayerController;

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

    public float GetFillAmount()
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

    public virtual void ActiveSkill()
    {
        CurrentChargeAmount--;
        PlayerController.AddCurrentEP(
            -(NeedEP.Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value));

        SetStartUI();
    }

    protected void SetStartUI()
    {
        if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.SetStartUI(); }
        else if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.SetStartUI(); }
    }

    protected void SetEndUI()
    {
        if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.SetEndUI(); }
        else if (PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1 == this)
        { MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.SetEndUI(); }
    }

    #endregion

    #region Judg Can Active

    public bool CanActive()
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
