using DG.Tweening;
using UnityEngine;

public abstract class InteractItemController : ItemController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Interact Item")]

    [Space(10)]
    [Header("=== Physics")]
    [SerializeField] private float spreadPower = 10f;
    [SerializeField] private float decSpreadPowerSpeed = 1f;
    [SerializeField] protected float currentSpreadPower = 0f;
    [SerializeField] protected Vector2 settedSpreadDir;
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
        currentSpreadPower = spreadPower;
        settedSpreadDir = DevTool.Get_RandomDir();
        Start_Tween();

        // Set
        this.gameObject.SetActive(true);
    }

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        OutlinerSR.sortingOrder = sortingOrder;
    }

    #endregion

    #region Framework

    protected void LateUpdate()
    {
        Play_Spread(currentSpreadPower);
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

    private void Play_Spread(float spreadPower)
    {
        if (currentSpreadPower > 0f)
        {
            currentSpreadPower -= decSpreadPowerSpeed * Time.deltaTime;
            rb.velocity = settedSpreadDir * spreadPower;
        }
        else if (currentSpreadPower != 0f)
        {
            currentSpreadPower = 0f;
            rb.velocity = Vector2.zero;
        }
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
