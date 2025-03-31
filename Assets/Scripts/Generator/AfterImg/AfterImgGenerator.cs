using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AfterImgGenerator : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public List<SpriteRenderer> TargetSRList;

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
        Caculate_GenTime();
    }

    #endregion

    #region Caculate

    private void Caculate_GenTime()
    {
        if (IsOn)
        {
            CurrentGenTime += Time.deltaTime;
            if (CurrentGenTime >= DelayGenTime)
            {
                CurrentGenTime = 0;
                Gen_Img();
            }
        }
    }

    // Start Set
    public void Start_Gen(float _ImageAlpha, float _SetIntervalDelay, float _StayDur)
    {
        IsOn = true;
        ImageAlpha = _ImageAlpha;
        DelayGenTime = _SetIntervalDelay;
        StayDur = _StayDur;
    }

    // End Set
    public void End_Gen()
    {
        IsOn = false;
        DelayGenTime = 0;
    }

    // Each Gen Img
    private void Gen_Img(SpriteRenderer _TargetSR)
    {
        SpriteRenderer SR = PoolingManager.Instance.Get_OP_AfterImg();
        SR.gameObject.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);

        SR.sprite = _TargetSR.sprite;
        SR.sortingOrder = _TargetSR.sortingOrder - 1;
        Color clr = UnitManager.Instance.RandomColor;
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

    private void Gen_Img()
    {
        for (int i = 0; i < TargetSRList.Count; i++)
        {
            Gen_Img(TargetSRList[i]);
        }
    }

    #endregion
}
