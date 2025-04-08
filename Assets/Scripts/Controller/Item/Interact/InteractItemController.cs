using DG.Tweening;
using UnityEngine;

public class InteractItemController : ItemController, IInteract
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Interact Item")]

    [Space(10)]
    [Header("=== Physics")]
    [SerializeField] private float SpreadPower = 10f;
    [SerializeField] private float DecSpreadPowerSpeed = 1f;
    [SerializeField] private float CurrentSpreadPower = 0f;
    [SerializeField] private Vector2 SettedSpreadDir;
    private Sequence UpDownSeq = null;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public ItemData_Field ItemDataField;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] private Animator ThisAT;

    [HideInInspector] private SpriteRenderer OutlinerSR;
    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    protected override void Offset()
    {
        base.Offset();

        OutlinerSR = DevTool.Get_ComponentTType(ThisAT.gameObject, out SpriteRenderer outlinerSr) ? outlinerSr : null;
    }

    #region State

    public override void Set_State(Vector2 _SpawnPos)
    {
        base.Set_State(_SpawnPos);

        // Anim
        CurrentSpreadPower = SpreadPower;
        SettedSpreadDir = DevTool.Get_RandomDir();
        Start_Tween();

        // Data
        ItemDataField = new ItemData_Field(ModuleItemManager.Instance.Get_RandomInteractItem());

        // Set
        this.gameObject.SetActive(true);
    }

    public void Set_RankState(int _Rank)
    {
        ItemDataField.Rank = _Rank;
        DevTool.Set_Anim(ref AOC, ThisAT, UnitManager.Instance.ModuleItemOutlinerAC[_Rank - 1]);
        ThisAT.speed = 1.5f;
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

    #region Interact

    public void Play_Interact()
    {
        PlayerManager.Instance.PlayerController.CurrentInteractable.Value = null;
        CurrentSpreadPower = 0f;
        SettedSpreadDir = Vector2.zero;

        ModuleItemManager.Instance.Gain_ModuleState(ItemDataField);
        PoolingManager.Instance.InteractItems.Queue.Enqueue(this);

        End_Tween();

        this.gameObject.SetActive(false);
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


    private void End_Tween()
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
}
