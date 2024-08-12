using UniRx;
using UnityEngine;

public class EnemyController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Header("=== Data")]
    [SerializeField] private string EnemyID;

    [Header("=== State")]
    [SerializeField] private ReactiveProperty<float> MaxHP = new();
    [SerializeField] private ReactiveProperty<float> CurrentHP = new();

    [Header("=== UI")]
    [SerializeField] private ModifyReductionFocusProgressBar HP_ProgressBar;

    #endregion

    #region Fremework

    private void Offset()
    {
        CurrentHP.Value = MaxHP.Value;
    }

    private void Start()
    {
        Offset();

        CurrentHP
            .Subscribe(_CurrentHP =>
            {
                HP_ProgressBar.SetFillImgSmooth(CurrentHP.Value, MaxHP.Value);
            });
    }

    #endregion

    #region Damaged

    public void TakeDamage(eDamageType _DamageType, float _Damage)
    {
        if (base.IsDead) 
        { return; }

        if(_DamageType == eDamageType.Physics)
        {
            SetIsDead(CurrentHP.Value, _Damage);
            CurrentHP.Value -= _Damage;
            if (CurrentHP.Value <= 0f)
            {
                base.IsDead = true;
                Die();
            }
        }
        else
        {
            SpawnES(_Damage);
        }


    }

    private void Die()
    {
        SpawnBS(4);
        SpawnII(1);
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Spawn Item

    private void SpawnII(int _SpawnRank)
    {
        InteractItemController IIC = PoolingManager.Instance.GetOP_InteractableItem();
        IIC.SetState(this.transform.position, _SpawnRank);
    }

    private void SpawnES(float _Value)
    {
        EnergyShrapnelController ESC = PoolingManager.Instance.GetOP_EnergyShrapnel();
        ESC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        ESC.gameObject.SetActive(true);
    }

    private void SpawnBS(int _Value)
    {
        BetteryShrapnelController BSC = PoolingManager.Instance.GetOP_BetteryShrapnel();
        BSC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        BSC.gameObject.SetActive(true);
    }

    #endregion

}


