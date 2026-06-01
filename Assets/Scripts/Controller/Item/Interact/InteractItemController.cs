using DG.Tweening;
using UnityEngine;

public abstract class InteractItemController : ItemController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Interact Item")]

    [Space(10)]
    [Header("=== Physics")]
    private Sequence upDownSeq = null;


    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] protected Animator at;

    [HideInInspector] private SpriteRenderer OutlinerSR;
    [HideInInspector] protected AnimatorOverrideController aoc;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        OutlinerSR = DevTool.Get_ComponentTType(at.gameObject, out SpriteRenderer outlinerSr) ? outlinerSr : null;
    }

    #endregion

    #region State

    public override void Set_State(Vector2 spawnPos)
    {
        base.Set_State(spawnPos);

        // Anim
        Start_Tween();
    }

    #endregion

    #region Sorting Order

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        OutlinerSR.sortingOrder = sortingOrder;
    }

    #endregion

    #region Dotween & Spread

    private void Start_Tween()
    {
        upDownSeq = DOTween.Sequence();

        upDownSeq.Append(targetObject.transform.DOLocalMoveY((targetRange + 0.2f), 1f).SetEase(Ease.InOutSine));
        upDownSeq.Append(targetObject.transform.DOLocalMoveY((targetRange), 1f).SetEase(Ease.InOutSine));

        upDownSeq
            .OnStart(() =>
            {
                targetObject.transform.localPosition = Vector2.up * targetRange;
            })
            .SetLoops(-1, LoopType.Restart);
    }

    protected void End_Tween()
    {
        DOTween.Kill(upDownSeq);
        upDownSeq = null;
    }

    #endregion

    #region Interact

    public abstract string Get_InteractName(out bool canInteract);

    public virtual void Play_Interact()
    {
        PlayerManager.instance.playerController.currentInteractable.Value = null;
        currentSpreadPower = 0f;
        settedSpreadDir = Vector2.zero;

        End_Tween();

        SoundManager.instance.Play_2D_SFX_Item_Random(PlayerManager.instance.playerController.Get_AS(), "Interact", 2);

        this.gameObject.SetActive(false);
    }

    #endregion
}
