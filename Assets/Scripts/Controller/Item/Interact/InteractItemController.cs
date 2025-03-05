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
    [SerializeField] public ItemData ThisItemData;
    
    #endregion

    #region State

    public void Set_State(Vector2 _SpawnPos, int _BoostLv, int _ItemRank)
    {
        base.SetState(_SpawnPos);

        ThisItemData.BoostLv = _BoostLv;
        ThisItemData.Rank = _ItemRank;
        CurrentSpreadPower = SpreadPower;
        SettedSpreadDir = Get_RandomDir();

        

        UpDownSeq = DOTween.Sequence();

        UpDownSeq.Append(TargetObject.transform.DOLocalMoveY((TargetRange + 0.2f), 1f).SetEase(Ease.InOutSine));
        UpDownSeq.Append(TargetObject.transform.DOLocalMoveY((TargetRange), 1f).SetEase(Ease.InOutSine));
        // Test
        UpDownSeq
            .OnStart(() =>
            {
                TargetObject.transform.localPosition = Vector2.up * TargetRange;
            })
            .SetLoops(-1, LoopType.Restart);

        ThisItemData = new ItemData(ModuleItemManager.Instance.Get_RandomInteractItem());

        this.gameObject.SetActive(true);
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
    }

    protected void OnDisable()
    {
        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
    }

    protected void Update()
    {
        Play_Spread(CurrentSpreadPower);
    }

    #endregion

    #region Spread

    private Vector2 Get_RandomDir()
    {
        float _X = Random.Range(-1.0f, 1.0f);
        float _Y = Random.Range(-1.0f, 1.0f);
        return new Vector2(_X, _Y).normalized;
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

    public void Play_Interact()
    {
        CurrentSpreadPower = 0f;
        SettedSpreadDir = Vector2.zero;

        ModuleItemManager.Instance.Get_ModuleState(ThisItemData);
        PoolingManager.Instance.InteractItems.Queue.Enqueue(this);

        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
        DOTween.Kill(UpDownSeq);
        UpDownSeq = null;

        this.gameObject.SetActive(false);
    }

    #endregion
}
