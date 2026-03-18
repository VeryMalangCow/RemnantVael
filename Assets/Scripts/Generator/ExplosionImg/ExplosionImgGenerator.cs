using DG.Tweening;
using UnityEngine;

public class ExplosionImgGenerator : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Explosion")]

    [Space(10)]
    [Header("=== Sprites")]
    [HideInInspector] protected Sequence TotalSeq;


    #endregion

    #region Gen

    // 원형: Circle
    protected void Gen_ExplImg_Circle(ExplState _State)
    {
        Gen_ExplImg_Ellipse(_State, 1f, 1f);
    }

    // 타원형: Ellipse
    protected void Gen_ExplImg_Ellipse(ExplState _State, float _X, float _Y)
    {
        DevTool.Set_CompleteTween(TotalSeq);
        TotalSeq = DOTween.Sequence();

        float singleAngle = 360 / _State.BaseState.SpawnAmount; // 한칸 앵글
        for (int i = 0; i < _State.BaseState.SpawnAmount; i++)
        {
            _State.Set_AllDir(DevTool.Get_DirFromAngle((i * singleAngle)));
            _State.Set_RandomAngleValue_JustAdd(singleAngle);
            _State.Set_MultipleAllDir(new Vector2(_X, _Y));
            _State.Set_RandomValue();

            TotalSeq.Join(
                Gen_EachExplImg(
                    _State.BaseState.SpawnPos,
                    _State.SpriteState,
                    _State.FirstState,
                    _State.SecondState));
        }
    }


    // 부채꼴: Sector
    protected void Gen_ExplImg_Sector(ExplState _State, Vector2 _Dir, float _AngleExtent)
    {
        DevTool.Set_CompleteTween(TotalSeq);
        TotalSeq = DOTween.Sequence();

        for (int i = 0; i < _State.BaseState.SpawnAmount; i++)
        {
            _State.Set_RandomValue();
            _State.Set_RandomAngleValue_PivotZero(_AngleExtent);

            TotalSeq.Join(
                Gen_EachExplImg(
                    _State.BaseState.SpawnPos,
                    _State.SpriteState,
                    _State.FirstState,
                    _State.SecondState));
        }
    }

    #endregion

    #region Each One

    // 하나의 이펙트 이미지를 생성
    private Sequence Gen_EachExplImg(Vector2 _SpawnPos, ExplState_Sprite _SpriteState, ExplState_MoveAndScale _FirstState, ExplState_MoveAndScale _SecondState)
    {
        SpriteRenderer sr = PoolingManager.instance.Get_OP_ExplosionImg();
        sr.gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        return Play_ExplImg(sr, _SpawnPos, _SpriteState, _FirstState, _SecondState);
    }

    #endregion

    #region Tween

    // 전체적인 움직임을 표현하는 
    private Sequence Play_ExplImg(SpriteRenderer _SR, Vector2 _SpawnPos, ExplState_Sprite _SpriteState, ExplState_MoveAndScale _FirstState, ExplState_MoveAndScale _SecondState)
    {
        DevTool.Set_CompleteTween(_SR.gameObject);

        Sequence Seq = DOTween.Sequence();

        Seq.Append(Play_ExplImg_MoveScale(_SR, _SpawnPos, _FirstState));
        Seq.Append(Play_ExplImg_MoveScaleFadeOut(_SR, _SpawnPos, _SecondState));
        Seq.OnStart(() => { SetOn_SR(_SR, _SpawnPos, _SpriteState); });
        Seq.OnComplete(() => { SetOff_SR(_SR); });
        return Seq;
    }

    // 커지는
    private Sequence Play_ExplImg_MoveScale(SpriteRenderer _SR, Vector2 _SpawnPos, ExplState_MoveAndScale _State)
    {
        Sequence Seq = DOTween.Sequence();

        Seq.Join(_SR.transform.DOMove(_SpawnPos + (_State.Dir * _State.Dis), _State.Time).SetEase(Ease.Linear));
        Seq.Join(_SR.transform.DOScale(_State.Scale, _State.Time).SetEase(Ease.Linear));

        return Seq;
    }

    // 작아지고 꺼지는 
    private Sequence Play_ExplImg_MoveScaleFadeOut(SpriteRenderer _SR, Vector2 _SpawnPos, ExplState_MoveAndScale _State)
    {
        Sequence Seq = Play_ExplImg_MoveScale(_SR, _SpawnPos, _State);
        Seq.Join(_SR.DOFade(0f, _State.Time).SetEase(Ease.Linear));

        return Seq;
    }

    #endregion

    #region Set

    protected virtual void SetOn_SR(SpriteRenderer _SR, Vector2 _SpawnPos, ExplState_Sprite _State)
    {
        _SR.transform.position = _SpawnPos;
        _SR.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0, 90));
        _SR.transform.localScale = Vector2.zero;

        _SR.color = Color.white;
        _SR.sortingOrder = LayerOrderManager.order_EffectImg;

        _SR.sprite = DevTool.Get_Random(_State.Sprite);
        _SR.material = _State.Material;

        _SR.gameObject.SetActive(true);
    }

    protected virtual void SetOff_SR(SpriteRenderer _SR)
    {
        _SR.gameObject.SetActive(false);

        PoolingManager.instance.explosionImgs.Enqueue(_SR);
    }

    #endregion
}
