using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DeadParticleController : MovableDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Dead Particle")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SpriteRenderer ThisShadow;

    #endregion

    #region - Hide


    #endregion

    #endregion

    #region Set

    private void Set_StartState(Sprite _Sprite, Vector2 _ShadowSize)
    {
        ThisSR.sprite = _Sprite;
        ThisShadow.transform.localScale = _ShadowSize;
        this.gameObject.SetActive(true);

        LayerOrderManager.Instance.Add_NeedSortObj(this);
    }

    #endregion

    #region Reset

    private void Reset_State()
    {
        ThisSR.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        ThisShadow.color = new Color(0, 0, 0, 0.5f);
        transform.localRotation = Quaternion.identity;
    }

    #endregion

    #region Play

    public void Play_DeadParticle(
        Sprite _Sprite, Vector2 _ShadowSize, Vector2 _SpawnPos, 
        float _StartY, float _ThrowDis, float _DurTime, float _DisappointTime)
    {
        Reset_State();
        Set_StartState(_Sprite, _ShadowSize);

        StartCoroutine(Play_DeadParticle_Cor(_SpawnPos, _StartY, _ThrowDis, _DurTime, _DisappointTime));
    }

    private IEnumerator Play_DeadParticle_Cor(Vector2 _SpawnPos,
        float _StartY, float _ThrowDis, float _DurTime, float _DisappointTime)
    {
        Play_MoveDir(_SpawnPos, _ThrowDis, _DurTime);
        Play_YPos(_StartY, _DurTime);
        Play_Rot(_DurTime);

        yield return new WaitForSeconds(4f);

        Play_Disappoint(_DisappointTime).OnComplete(() =>
            {
                LayerOrderManager.Instance.Remove_NeedSortObj(this);
                this.gameObject.SetActive(false);
                PoolingManager.Instance.deadParticles.Enqueue(this);
            });
    }

    private Sequence Play_MoveDir(Vector2 _SpawnPos, float _Dis, float _DurTime)
    {
        transform.position = _SpawnPos;

        Vector2 targetDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        targetDir *= _Dis;

        return DOTween.Sequence(transform.DOMove(_SpawnPos + targetDir, _DurTime));
    }

    private Sequence Play_YPos(float _StartY, float _DurTime)
    {
        TargetRange = _StartY;

        Sequence seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => TargetRange, x => TargetRange = x, _StartY * 1.2f, _DurTime * 0.4f).SetEase(Ease.OutQuad));
        seq.Append(DOTween.To(() => TargetRange, x => TargetRange = x, 0, _DurTime * 0.6f).SetEase(Ease.InQuad));

        return seq;
    }

    private Sequence Play_Rot(float _DurTime)
    {
        return DOTween.Sequence(TargetObject.transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-1080, 1080)), _DurTime, RotateMode.FastBeyond360));
    }

    private Sequence Play_Disappoint(float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(ThisShadow.DOFade(0, _DurTime));
        seq.Join(ThisSR.DOFade(0, _DurTime));

        return seq;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (_Col.tag == "Wall")
        {
            DOTween.Kill(transform);
            DOTween.Kill(TargetObject.transform);
        }
    }

    #endregion
}