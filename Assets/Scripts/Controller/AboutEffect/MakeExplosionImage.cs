using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MakeExplosionImage : MonoBehaviour
{
    #region Value

    private Vector2 SpawnedPos;

    private Sequence TotalSeq;


    [Space(10)]
    [Header("=== Sprites")]
    [SerializeField] private List<RandomSpriteModule> RandomSpriteList;

    [System.Serializable]
    class RandomSpriteModule
    {
        public List<Sprite> SpriteList;
    }


    #endregion

    #region Explosion

    // 전체적인 360도 방향으로 분산

    public void GenExplosionImgs(Vector2 _SpawnedPos,
        int _SpawnImgAmount, float _ExplosionDis, float _DisappearDis,
        float _BiggerScale, float _BiggerMinTime, float _BiggerMaxTime,
        float _SmallerScale, float _SmallerMinTime, float _SmallerMaxTime,
        int _ModuleIndex)
    {
        GenExplosionImgs(_SpawnedPos,
       _SpawnImgAmount, _ExplosionDis, _DisappearDis,
       _BiggerScale, _BiggerMinTime, _BiggerMaxTime,
       _SmallerScale, _SmallerMinTime, _SmallerMaxTime,
       _ModuleIndex, Vector2.one);
    }

    public void GenExplosionImgs(Vector2 _SpawnedPos, 
        int _SpawnImgAmount, float _ExplosionDis, float _DisappearDis,
        float _BiggerScale, float _BiggerMinTime, float _BiggerMaxTime,
        float _SmallerScale, float _SmallerMinTime, float _SmallerMaxTime,
        int _ModuleIndex, Vector2 _OffsetSpreadVec)
    {
        if (TotalSeq != null && DOTween.IsTweening(TotalSeq))
        {
            DOTween.Complete(TotalSeq);
        }

        SpawnedPos = _SpawnedPos;
        TotalSeq = DOTween.Sequence();

        int sortOrder = 0;
        if (this.TryGetComponent(out SpriteRenderer sr))
        {
            sortOrder = sr.sortingOrder + 1;
        }

        for (int i = 0; i < _SpawnImgAmount; i++)
        {
            
            float angle = ((360f / (float)_SpawnImgAmount) * i) 
                + Random.Range(0f, (360f / (float)_SpawnImgAmount));

            Vector2 dirByAngle = new Vector2(
                    Mathf.Sin(angle), 
                    Mathf.Cos(angle));
            dirByAngle.Normalize();
            dirByAngle *= _OffsetSpreadVec;

            float biggerTime = Random.Range(_BiggerMinTime, _BiggerMaxTime);
            float smallerTime = Random.Range(_SmallerMinTime, _SmallerMaxTime);

            TotalSeq.Join(GenExplosionImg(dirByAngle, sortOrder,
                _ExplosionDis, _DisappearDis,
                _BiggerScale, biggerTime,
                _SmallerScale, smallerTime,
                _ModuleIndex));
        }

    }

    // 방향적 부채꼴 방향으로 분산
    public void GenExplosionImgs_Fan(Vector2 _SpawnedPos, Vector2 _Dir, float _AngleArea,
        int _SpawnImgAmount, float _ExplosionDis, float _DisappearDis,
        float _BiggerScale, float _BiggerMinTime, float _BiggerMaxTime,
        float _SmallerScale, float _SmallerMinTime, float _SmallerMaxTime,
        int _ModuleIndex)
    {
        if (TotalSeq != null && DOTween.IsTweening(TotalSeq))
        {
            DOTween.Complete(TotalSeq);
        }

        SpawnedPos = _SpawnedPos;
        TotalSeq = DOTween.Sequence();

        int sortOrder = 0;
        if (this.TryGetComponent(out SpriteRenderer sr))
        {
            sortOrder = sr.sortingOrder + 1;
        }

        //_Dir *= 10f;
        

        for (int i = 0; i < _SpawnImgAmount; i++)
        {
            float angle = Random.Range(0, _AngleArea) - (_AngleArea / 2);

            Vector2 dirByAngle = new Vector2(
                _Dir.x * Mathf.Cos(angle * Mathf.Deg2Rad) - _Dir.y * Mathf.Sin(angle * Mathf.Deg2Rad),
                _Dir.x * Mathf.Sin(angle * Mathf.Deg2Rad) + _Dir.y * Mathf.Cos(angle * Mathf.Deg2Rad));

            float biggerTime = Random.Range(_BiggerMinTime, _BiggerMaxTime);
            float smallerTime = Random.Range(_SmallerMinTime, _SmallerMaxTime);

            TotalSeq.Join(GenExplosionImg(dirByAngle, sortOrder,
                _ExplosionDis, _DisappearDis,
                _BiggerScale, biggerTime,
                _SmallerScale, smallerTime,
                _ModuleIndex));
        }
    }


    private Sequence GenExplosionImg(Vector2 _Dir, int _SpriteSortOrder,
        float _ExplotionDis, float _DisappearDis,
        float _BiggerScale, float _BiggerTime,
        float _SmallerScale, float _SmallerTime,
        int _ModuleIndex)
    {
        Sequence bigSeq = DOTween.Sequence();
        Sequence smallSeq = DOTween.Sequence();
        Sequence totalSeq = DOTween.Sequence();

        SpriteRenderer sr = PoolingManager.Instance.GetOP_ExplosionImg();
        sr.sortingOrder = _SpriteSortOrder;
        if (RandomSpriteList[_ModuleIndex].SpriteList != null && RandomSpriteList[_ModuleIndex].SpriteList.Count > 0)
        {
            sr.sprite = RandomSpriteList[_ModuleIndex].SpriteList[Random.Range(0, RandomSpriteList[_ModuleIndex].SpriteList.Count)];
        }

        if (DOTween.IsTweening(sr.gameObject))
        { DOTween.Complete(sr.gameObject); }

        bigSeq.Join(sr.transform.DOMove(SpawnedPos + (_Dir * _ExplotionDis), _BiggerTime).SetEase(Ease.Linear));
        bigSeq.Join(sr.transform.DOScale(_BiggerScale, _BiggerTime).SetEase(Ease.Linear));

        smallSeq.Join(sr.transform.DOMove(SpawnedPos + (_Dir * _DisappearDis), _SmallerTime).SetEase(Ease.Linear));
        smallSeq.Join(sr.transform.DOScale(_SmallerScale, _SmallerTime).SetEase(Ease.Linear));
        smallSeq.Join(sr.DOFade(0f, _SmallerTime).SetEase(Ease.Linear));

        totalSeq.Append(bigSeq);
        totalSeq.Append(smallSeq);

        sr.transform.position = SpawnedPos;
        sr.transform.localScale = Vector2.zero;
        sr.color = Color.white;
        sr.gameObject.SetActive(true);

        totalSeq 
            .OnComplete(() =>
            {
                sr.gameObject.SetActive(false);
                PoolingManager.Instance.ExplosionImgs.Queue.Enqueue(sr);
            });


        return totalSeq;
    }

    

    #endregion
}
