using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudTabItemView : MonoBehaviour
{
    [SerializeField] private RectTransform highLvItemRt;
    [SerializeField] private List<TMP_Text> highLvItemAmountTxtList;

    private float defaultHighLvItemRectX;

    private Tween tabTween;

    public void Init()
    {
        defaultHighLvItemRectX = highLvItemRt.anchoredPosition.x;
        InitHighLvItemUI();

        gameObject.SetActive(true);
    }

    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        tabTween = highLvItemRt.DOAnchorPosX(0, durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        tabTween = highLvItemRt.DOAnchorPosX(defaultHighLvItemRectX, durTime);
    }

    public void InitHighLvItemUI()
    {
        var itemData = SaveDataManager.instance.jsonData.itemData;
        for (int i = 0; i < itemData.Count; i++)
            highLvItemAmountTxtList[i].text = itemData[i].amount.ToString();
    }

    public void SetHighLvItemUI(int id, int amount)
    {
        highLvItemAmountTxtList[id].text = amount.ToString();
    }
}
