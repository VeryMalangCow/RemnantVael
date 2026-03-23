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
    [SerializeField] private SpriteRenderer shadowSr;

    #endregion

    #region - Hide


    #endregion

    #endregion

    #region Set

    private void Set_StartState(Sprite sprite, Vector2 shadowSize)
    {
        thisSr.sprite = sprite;
        shadowSr.transform.localScale = shadowSize;
        this.gameObject.SetActive(true);

        LayerOrderManager.instance.Add_NeedSortObj(this);
    }

    #endregion

    #region Reset

    private void Reset_State()
    {
        thisSr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        shadowSr.color = new Color(0, 0, 0, 0.5f);
        transform.localRotation = Quaternion.identity;
    }

    #endregion

    #region Play

    public void Play_DeadParticle(Sprite sprite, Vector2 shadowSize, Vector2 spawnPos, float startY, float throwDis, float durTime, float disappointTime)
    {
        Reset_State();
        Set_StartState(sprite, shadowSize);

        StartCoroutine(Play_DeadParticle_Cor(spawnPos, startY, throwDis, durTime, disappointTime));
    }

    private IEnumerator Play_DeadParticle_Cor(Vector2 spawnPos, float startY, float throwDis, float durTime, float disappointTime)
    {
        Play_MoveDir(spawnPos, throwDis, durTime);
        Play_YPos(startY, durTime);
        Play_Rot(durTime);

        yield return new WaitForSeconds(4f);

        Play_Disappoint(disappointTime).OnComplete(() =>
            {
                LayerOrderManager.instance.Remove_NeedSortObj(this);
                this.gameObject.SetActive(false);
                PoolingManager.instance.deadParticles.Enqueue(this);
            });
    }

    private Sequence Play_MoveDir(Vector2 spawnPos, float dis, float durTime)
    {
        transform.position = spawnPos;

        Vector2 targetDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        targetDir *= dis;

        return DOTween.Sequence(transform.DOMove(spawnPos + targetDir, durTime));
    }

    private Sequence Play_YPos(float startY, float durTime)
    {
        targetRange = startY;

        Sequence seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => targetRange, x => targetRange = x, startY * 1.2f, durTime * 0.4f).SetEase(Ease.OutQuad));
        seq.Append(DOTween.To(() => targetRange, x => targetRange = x, 0, durTime * 0.6f).SetEase(Ease.InQuad));

        return seq;
    }

    private Sequence Play_Rot(float durTime)
    {
        return DOTween.Sequence(targetObject.transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-1080, 1080)), durTime, RotateMode.FastBeyond360));
    }

    private Sequence Play_Disappoint(float durTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(shadowSr.DOFade(0, durTime));
        seq.Join(thisSr.DOFade(0, durTime));

        return seq;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Wall")
        {
            DOTween.Kill(transform);
            DOTween.Kill(targetObject.transform);
        }
    }

    #endregion
}