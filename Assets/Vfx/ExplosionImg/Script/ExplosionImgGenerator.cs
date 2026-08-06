using DG.Tweening;
using UnityEngine;

public class ExplosionImgGenerator : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Explosion")]

    [Space(10)]
    [Header("=== Sprites")]
    [HideInInspector] protected Sequence totalSeq;


    #endregion

    #region Gen

    // 원형: Circle
    protected void Gen_ExplImg_Circle(ExplState state)
    {
        Gen_ExplImg_Ellipse(state, 1f, 1f);
    }

    // 타원형: Ellipse
    protected void Gen_ExplImg_Ellipse(ExplState state, float x, float y)
    {
        DevTool.Set_CompleteTween(totalSeq);
        totalSeq = DOTween.Sequence();

        float singleAngle = 360 / state.baseState.spawnAmount; // 한칸 앵글
        for (int i = 0; i < state.baseState.spawnAmount; i++)
        {
            state.Set_AllDir(DevTool.GetDirFromAngle((i * singleAngle)));
            state.Set_RandomAngleValue_JustAdd(singleAngle);
            state.Set_MultipleAllDir(new Vector2(x, y));
            state.Set_RandomValue();

            totalSeq.Join(
                Gen_EachExplImg(
                    state.baseState.spawnPos,
                    state.spriteState,
                    state.firstState,
                    state.secondState));
        }
    }


    // 부채꼴: Sector
    protected void Gen_ExplImg_Sector(ExplState state, Vector2 dir, float angleExtent)
    {
        DevTool.Set_CompleteTween(totalSeq);
        totalSeq = DOTween.Sequence();

        for (int i = 0; i < state.baseState.spawnAmount; i++)
        {
            state.Set_RandomValue();
            state.Set_RandomAngleValue_PivotZero(angleExtent);

            totalSeq.Join(
                Gen_EachExplImg(
                    state.baseState.spawnPos,
                    state.spriteState,
                    state.firstState,
                    state.secondState));
        }
    }

    #endregion

    #region Each One

    // 하나의 이펙트 이미지를 생성
    private Sequence Gen_EachExplImg(Vector2 spawnPos, ExplState_Sprite spriteState, ExplState_MoveAndScale firstState, ExplState_MoveAndScale secondState)
    {
        PoolableSpriteRenderer poolableSr = VfxManager.instance.SpawnExplosionImg();
        SpriteRenderer sr = poolableSr.spriteRenderer;
        sr.gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        return Play_ExplImg(poolableSr, spawnPos, spriteState, firstState, secondState);
    }

    #endregion

    #region Tween

    // 전체적인 움직임을 표현하는 
    private Sequence Play_ExplImg(PoolableSpriteRenderer poolableSr, Vector2 spawnPos, ExplState_Sprite spriteState, ExplState_MoveAndScale firstState, ExplState_MoveAndScale secondState)
    {
        SpriteRenderer sr = poolableSr.spriteRenderer;
        DevTool.Set_CompleteTween(sr.gameObject);

        Sequence Seq = DOTween.Sequence();

        Seq.Append(Play_ExplImg_MoveScale(sr, spawnPos, firstState));
        Seq.Append(Play_ExplImg_MoveScaleFadeOut(sr, spawnPos, secondState));
        Seq.OnStart(() => { SetOn_SR(sr, spawnPos, spriteState); });
        Seq.OnComplete(() => { SetOff_SR(poolableSr); });
        return Seq;
    }

    // 커지는
    private Sequence Play_ExplImg_MoveScale(SpriteRenderer sr, Vector2 spawnPos, ExplState_MoveAndScale state)
    {
        Sequence Seq = DOTween.Sequence();

        Seq.Join(sr.transform.DOMove(spawnPos + (state.dir * state.dis), state.time).SetEase(Ease.Linear));
        Seq.Join(sr.transform.DOScale(state.scale, state.time).SetEase(Ease.Linear));

        return Seq;
    }

    // 작아지고 꺼지는 
    private Sequence Play_ExplImg_MoveScaleFadeOut(SpriteRenderer sr, Vector2 spawnPos, ExplState_MoveAndScale state)
    {
        Sequence Seq = Play_ExplImg_MoveScale(sr, spawnPos, state);
        Seq.Join(sr.DOFade(0f, state.time).SetEase(Ease.Linear));

        return Seq;
    }

    #endregion

    #region Set

    protected virtual void SetOn_SR(SpriteRenderer sr, Vector2 spawnPos, ExplState_Sprite state)
    {
        sr.transform.position = spawnPos;
        sr.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0, 90));
        sr.transform.localScale = Vector2.zero;

        sr.color = Color.white;
        sr.sortingOrder = SortingOrderManager.order_EffectImg;

        sr.sprite = DevTool.Get_Random(state.sprite);
        sr.material = state.material;

        sr.gameObject.SetActive(true);
    }

    protected virtual void SetOff_SR(PoolableSpriteRenderer sr)
    {
        sr.gameObject.SetActive(false);

        VfxManager.instance.RemoveExplosionImg(sr);
    }

    #endregion
}
