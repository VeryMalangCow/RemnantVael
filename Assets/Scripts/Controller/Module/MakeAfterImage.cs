using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MakeAfterImage : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private SpriteRenderer ThisSR;
    [SerializeField] private float StayDur = 1f;

    [Header("=== Caculate")]
    [SerializeField] private bool IsOn = false;
    [SerializeField] private float CurrentGenTime = 0;
    [SerializeField] private float DelayGenTime = 0;

    #endregion

    #region Framework

    private void Update()
    {
        CaculateGenTime();
    }

    #endregion

    #region Caculate

    public void StartGen(float _SetIntervalDelay)
    {
        IsOn = true;
        DelayGenTime = _SetIntervalDelay;
    }

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

    private void GenImg()
    {
        SpriteRenderer SR = PoolingManager.Instance.GetOP_AfterImg();
        SR.sprite = ThisSR.sprite;
        SR.color = GameManager.Instance.RandomColor;
        SR.gameObject.transform.position = this.transform.position;
        SR.gameObject.transform.localScale = this.transform.lossyScale;
        SR.gameObject.SetActive(true);
        SR.DOFade(0f, StayDur)
            .OnComplete(() => 
            {
                PoolingManager.Instance.PlayerAfterImgs.Queue.Enqueue(SR);
            });
        
    }

    public void EndGen()
    {
        IsOn = false;
        DelayGenTime = 0;
    }


    #endregion
}
