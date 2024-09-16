using System.Collections;
using UniRx;
using UnityEngine;

public class EnemyController : MovableObject, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private string EnemyID;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float MaxHP;
    [HideInInspector] private ReactiveProperty<float> CurrentHP = new();

    [SerializeField] private float MaxEP;
    [HideInInspector] private ReactiveProperty<float> CurrentEP = new();
    [HideInInspector] public bool IsLethargy = false;
    [SerializeField] private float RecoverLethargyTime = 4f;

    [SerializeField] private float ItemDropPercent = 0.0f;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private ModifyReductionFocusProgressBar HP_ProgressBar;
    [SerializeField] private ModifyReductionFocusProgressBar EP_ProgressBar;

    #endregion

    #region Fremework

    private void Offset()
    {
        HP_ProgressBar.Offset();
        EP_ProgressBar.Offset();

        CurrentHP.Value = MaxHP;
        CurrentEP.Value = MaxEP;
    }

    private void Start()
    {
        Offset();

        CurrentHP
            .Subscribe(_CurrentHP =>
            {
                HP_ProgressBar.SetFillImgSmooth(CurrentHP.Value, MaxHP);
            });

        CurrentEP
            .Subscribe(_CurrentEP =>
            {
                EP_ProgressBar.SetFillImgSmooth(CurrentEP.Value, MaxEP);
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
            if(!IsLethargy)
            {
                SpawnES(_Damage);
            }
        }


    }

    private void Die()
    {
        // Drop Bettery
        SpawnBS(1);

        // Drop Item
        SpawnII(1);

        StopCoroutine(RecoverLethargy());
        PlayerManager.Instance.CameraController.PlayKillShake(PlayerManager.Instance.PlayerController.ExecutionTime);

        this.gameObject.SetActive(false);
    }

    #endregion

    #region Spawn Item

    // Interactable Item
    private void SpawnII(int _SpawnRank)
    {
        if (ItemDropPercent < UnityEngine.Random.Range(0f, 1f))
        { return; }

        InteractItemController IIC = PoolingManager.Instance.GetOP_InteractableItem();
        IIC.SetState(this.transform.position, _SpawnRank);
    }

    // Energy Shrapnel
    private void SpawnES(float _Value)
    {
        float targetValue = 0;
        if (CurrentEP.Value > _Value)
        {
            targetValue = _Value;
            CurrentEP.Value -= _Value;
        }
        else if (CurrentEP.Value > 0)
        {
            targetValue = CurrentEP.Value;
            CurrentEP.Value = 0;
            IsLethargy = true;
            StartCoroutine(RecoverLethargy());
            Debug.Log(this.gameObject.name + " / Lethargy!!!");
        }

        EnergyShrapnelController ESC = PoolingManager.Instance.GetOP_EnergyShrapnel();
        ESC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            targetValue);
        ESC.gameObject.SetActive(true);
    }

    // Bettery Shrapnel
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

    #region State

    private IEnumerator RecoverLethargy()
    {
        yield return new WaitForSeconds(0.5f);

        EP_ProgressBar.SetFillFullImgSmooth(RecoverLethargyTime);

        yield return new WaitForSeconds(RecoverLethargyTime);

        CurrentEP.Value = MaxEP;
        IsLethargy = false;
        Debug.Log(this.gameObject.name + " / Recover Lethargy!!!");
    }


    #endregion

    #region Interact

    public void Interact()
    {
        Die();
    }

    #endregion
}


