using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudBoostView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private RectTransform boostRt;
    [SerializeField] private TMP_Text boostLv;
    [SerializeField] private GameObject[] boostLightArr;
    [SerializeField] private GameObject[] boostLightWheelArr;
    [SerializeField] List<Image> boostInnerList;
    private float defaultBoostRectY;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] mainClrImgs;
    [SerializeField] private TMP_Text[] mainClrTmps;
    [SerializeField] private Image[] subClrImgs;

    private Tween tabTween;

    // Init
    public void Init(Color mainClr, Color subClr)
    {
        defaultBoostRectY = boostRt.anchoredPosition.y;
        ColorInit(mainClr, subClr);

        gameObject.SetActive(true);

    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(mainClr, mainClrImgs);
        mainClrImgs = null;
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }


    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        tabTween = boostRt.DOAnchorPosY(0, durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        tabTween = boostRt.DOAnchorPosY(defaultBoostRectY, durTime);
    }

    public void SetBoostLvUI(int currentLv)
    {
        SetTextOfBoost(currentLv);
        Play_ActiveBoost(boostLightArr, currentLv);
        Play_ActiveBoost(boostLightWheelArr, currentLv);
        PlayRollBoost(currentLv);
    }

    private void SetTextOfBoost(int currentLv)
    {
        boostLv.text = currentLv.ToString();
        DevTool.Set_AlphaColor(boostLv, currentLv == 0 ? 0.1f : 0.25f * (currentLv));
    }

    private void Play_ActiveBoost(GameObject[] arr, int currentLv)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (DevTool.Get_ComponentTType(arr[i].gameObject, out Image img))
            {
                DevTool.SetKillTween(img);

                if (i < currentLv) img.DOFade(1f, 0.2f);
                else img.DOFade(0f, 0.2f);
            }
        }
    }

    // 부스트 돌리기
    private void PlayRollBoost(int currentLv)
    {
        for (int i = 0; i < boostLightWheelArr.Length; i++)
        {
            if (DevTool.Get_ComponentTType(boostLightWheelArr[i].gameObject, out RectTransform rt))
            {
                if (i < currentLv) Play_EachRollBoost(rt, i);
                else DevTool.SetKillTween(rt);
            }
        }
    }

    // 부스트 하나씩 돌리기
    private void Play_EachRollBoost(RectTransform rt, int index)
    {
        rt.DOLocalRotate(new Vector3(0, 0, 360), 0.2f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        rt.DOLocalRotate(new Vector3(0, 0, 360), 3f / (index + 1f), RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Restart);
                    });
    }
}
