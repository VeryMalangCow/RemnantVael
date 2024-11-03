using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MakeAfterImage : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private List<SpriteRenderer> TargetSRList;

    [Space(10)]
    [Header("=== Caculate")]
    [SerializeField] private bool IsOn = false;
    [SerializeField] private float CurrentGenTime = 0;
    [SerializeField] private float ImageAlpha = 1f;
    [SerializeField] private float DelayGenTime = 1f;
    [SerializeField] private float StayDur = 1f;

    #endregion

    #region Framework

    private void Update()
    {
        CaculateGenTime();
    }

    #endregion

    #region Caculate

    private void CaculateGenTime()
    {
        if (IsOn)
        {
            CurrentGenTime += Time.deltaTime;
            if (CurrentGenTime >= DelayGenTime)
            {
                CurrentGenTime = 0;
                GenImg();
            }
        }
    }

    // Start Set
    public void StartGen(float _ImageAlpha, float _SetIntervalDelay, float _StayDur)
    {
        IsOn = true;
        ImageAlpha = _ImageAlpha;
        DelayGenTime = _SetIntervalDelay;
        StayDur = _StayDur;
    }

    // End Set
    public void EndGen()
    {
        IsOn = false;
        DelayGenTime = 0;
    }

    // Each Gen Img
    private void GenImg(SpriteRenderer _TargetSR)
    {
        SpriteRenderer SR = PoolingManager.Instance.GetOP_AfterImg();
        SR.sprite = _TargetSR.sprite;
        SR.sortingOrder = _TargetSR.sortingOrder - 1;
        Color clr = GameManager.Instance.RandomColor;
        clr.a = Mathf.Clamp(ImageAlpha, 0f, 1f);
        SR.color = clr;
        SR.gameObject.transform.position = _TargetSR.transform.position;
        SR.gameObject.transform.localScale = _TargetSR.transform.lossyScale;
        SR.gameObject.SetActive(true);
        SR.DOFade(0f, StayDur)
            .OnComplete(() => 
            {
                PoolingManager.Instance.AfterImgs.Queue.Enqueue(SR);
            });
        
    }

    private void GenImg()
    {
        for (int i = 0; i < TargetSRList.Count; i++)
        {
            GenImg(TargetSRList[i]);
        }
    }

    #endregion
}
