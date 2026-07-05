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
        PoolableSpriteRenderer poolableSr = VFXManager.instance.SpawnAfterImg();
        if (poolableSr == null)
        {
            Debug.Log("<color=red>poolableSr is NULL</color>");
            return;
        }
        SpriteRenderer afterSr = poolableSr.spriteRenderer;
        afterSr.gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);

        afterSr.sprite = sr.sprite;
        afterSr.material = sr.material;
        afterSr.sortingOrder = sr.sortingOrder - 1;
        afterSr.color = clr;
        afterSr.gameObject.transform.position = sr.transform.position;
        afterSr.gameObject.transform.localScale = sr.transform.lossyScale;
        afterSr.DOFade(0f, stayDur)
            .OnComplete(() => 
            {
                VFXManager.instance.RemoveAfterImg(poolableSr);
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
