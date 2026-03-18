using DG.Tweening;
using UnityEngine;

public abstract class InteractItemController : ItemController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Interact Item")]

    [Space(10)]
    [Header("=== Physics")]
    [SerializeField] private float SpreadPower = 10f;
    [SerializeField] private float DecSpreadPowerSpeed = 1f;
    [SerializeField] protected float CurrentSpreadPower = 0f;
    [SerializeField] protected Vector2 SettedSpreadDir;
    private Sequence UpDownSeq = null;


    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] protected Animator ThisAT;

    [HideInInspector] private SpriteRenderer OutlinerSR;
    [HideInInspector] protected AnimatorOverrideController AOC;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        OutlinerSR = DevTool.Get_ComponentTType(ThisAT.gameObject, out SpriteRenderer outlinerSr) ? outlinerSr : null;
    }

    #endregion

    #region State

    public override void Set_State(Vector2 _SpawnPos)
    {
        base.Set_State(_SpawnPos);

        // Anim
        CurrentSpreadPower = SpreadPower;
        SettedSpreadDir = DevTool.Get_RandomDir();
        Start_Tween();

        // Set
        this.gameObject.SetActive(true);
    }

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        OutlinerSR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Framework

    protected void LateUpdate()
    {
        Play_Spread(CurrentSpreadPower);
    }

    #endregion

    #region Dotween & Spread

    private void Start_Tween()
    {
        UpDownSeq = DOTween.Sequence();

        UpDownSeq.Append(TargetObject.transform.DOLocalMoveY((TargetRange + 0.2f), 1f).SetEase(Ease.InOutSine));
        UpDownSeq.Append(TargetObject.transform.DOLocalMoveY((TargetRange), 1f).SetEase(Ease.InOutSine));

        UpDownSeq
            .OnStart(() =>
            {
                TargetObject.transform.localPosition = Vector2.up * TargetRange;
            })
            .SetLoops(-1, LoopType.Restart);
    }

    protected void End_Tween()
    {
        DOTween.Kill(UpDownSeq);
        UpDownSeq = null;
    }

    private void Play_Spread(float _SpreadPower)
    {
        if (CurrentSpreadPower > 0f)
        {
            CurrentSpreadPower -= DecSpreadPowerSpeed * Time.deltaTime;
            ThisRb.velocity = SettedSpreadDir * _SpreadPower;
        }
        else if (CurrentSpreadPower != 0f)
        {
            CurrentSpreadPower = 0f;
            ThisRb.velocity = Vector2.zero;
        }
    }

    #endregion

    #region Interact

    public abstract string Get_InteractName(out bool _CanInteract);

    public virtual void Play_Interact()
    {
        PlayerManager.Instance.playerController.CurrentInteractable.Value = null;
        CurrentSpreadPower = 0f;
        SettedSpreadDir = Vector2.zero;

        End_Tween();

        SoundManager.Instance.Play_2D_SFX_Item_Random(PlayerManager.Instance.playerController.Get_AS(), "Interact", 2);

        this.gameObject.SetActive(false);
    }

    #endregion
}
