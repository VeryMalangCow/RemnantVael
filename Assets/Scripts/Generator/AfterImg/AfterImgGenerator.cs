using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AfterImgGenerator : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public List<SpriteRenderer> targetSRList;
    [SerializeField] private Color thisColor;

    [Space(10)]
    [Header("=== Caculate")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private float currentGenTime = 0;
    [SerializeField] private float imageAlpha = 1f;
    [SerializeField] private float delayGenTime = 1f;
    [SerializeField] private float stayDur = 1f;

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
        if (isOn)
        {
            currentGenTime += Time.deltaTime;
            if (currentGenTime >= delayGenTime)
            {
                currentGenTime = 0;
                Gen_Img(thisColor);
            }
        }
    }

    // Start Set
    public void Start_Gen(float imgAlpha, float intervalDelay, float stayDur)
    {
        isOn = true;
        imageAlpha = imgAlpha;
        delayGenTime = intervalDelay;
        this.stayDur = stayDur;
    }

    // End Set
    public void End_Gen()
    {
        isOn = false;
        delayGenTime = 0;
    }

    // Each Gen Img
    private void Gen_Img(SpriteRenderer sr, Color clr)
    {
        SpriteRenderer SR = PoolingManager.instance.Get_OP_AfterImg();
        SR.gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);

        SR.sprite = sr.sprite;
        SR.sortingOrder = sr.sortingOrder - 1;
        Color _clr = clr;
        _clr.a = Mathf.Clamp(imageAlpha, 0f, 1f);
        SR.color = _clr;
        SR.gameObject.transform.position = sr.transform.position;
        SR.gameObject.transform.localScale = sr.transform.lossyScale;
        SR.gameObject.SetActive(true);
        SR.DOFade(0f, stayDur)
            .OnComplete(() => 
            {
                SR.gameObject.SetActive(false);
                PoolingManager.instance.afterImgs.Enqueue(SR);
            });
        
    }

    private void Gen_Img(Color clr)
    {
        for (int i = 0; i < targetSRList.Count; i++)
        {
            Gen_Img(targetSRList[i], clr);
        }
    }

    #endregion
}
