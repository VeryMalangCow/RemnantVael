using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModifyDescPanel_ForBaseUpgrade : UIModule
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Desc _ For BaseUpgrade")]

    [Space(10)]
    [Header("=== Upgrade Graph")]
    [SerializeField] private List<TMP_Text> UpgradeGraphLV_TxtList;
    [SerializeField] private List<TMP_Text> UpgradeGraphDetailState_TxtList;
    [SerializeField] private RectTransform CurrentUpgradeGraphSpot;

    [Space(10)]
    [Header("=== Current & Next Upgrade")]
    [SerializeField] private TMP_Text CurrentLvTxt;
    [SerializeField] private TMP_Text CurrentStateTxt;
    [SerializeField] private TMP_Text NextLvTxt;
    [SerializeField] private TMP_Text NextStateTxt;
    [SerializeField] private GameObject CompletedSignGO;

    [Space(10)]
    [Header("=== Description")]
    [SerializeField] private TMP_Text DescriptionTxt;

    [HideInInspector] private RectTransform ThisRT;
    [HideInInspector] private float OriginalHeight;
    [HideInInspector] private CanvasGroup ThisCG;

    #endregion

    #region Offset

    public override void Offset()
    {
        if (ThisRT == null && this.TryGetComponent(out RectTransform rt))
        {
            ThisRT = rt; 
            OriginalHeight = ThisRT.rect.height;

            ThisRT.sizeDelta = new Vector2(ThisRT.sizeDelta.x, 0f);
        }
        if (ThisCG == null && this.TryGetComponent(out CanvasGroup cg))
        { 
            ThisCG = cg;

            ThisCG.alpha = 0f;
        }
    }

    #endregion

    #region Open / Close

    public void OpenThisPanel(float _DurTime)
    {
        ThisRT.DOSizeDelta(new Vector2(ThisRT.sizeDelta.x, OriginalHeight), _DurTime);
        ThisCG.DOFade(1f, _DurTime);
    }

    public void CloseThisPanel(float _DurTime)
    {
        ThisRT.DOSizeDelta(new Vector2(ThisRT.sizeDelta.x, 0f), _DurTime);
        ThisCG.DOFade(0f, _DurTime);
    }

    #endregion

    #region Desc

    public void SetDesc(BaseUpgradeState<float> _MTAFB)
    {
        // Graph
        for (int i = 0; i < UpgradeGraphDetailState_TxtList.Count; i++) 
        {
            float value = _MTAFB.UpgradeValueByLevelRange[i];
            string valueText = value > 0 ? "+" + value.ToString() : value.ToString();
            UpgradeGraphDetailState_TxtList[i].text = valueText;
        }

        int lv = _MTAFB.CurrentLevel.Value;
        CurrentUpgradeGraphSpot.gameObject.SetActive(true);

        if (0 <= lv && lv <= 2)
        {
            CurrentUpgradeGraphSpot.anchoredPosition 
                = new Vector2(0, CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else if (3 <= lv && lv <= 5)
        {
            CurrentUpgradeGraphSpot.anchoredPosition
                = new Vector2(125, CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else if (6 <= lv && lv <= 8)
        {
            CurrentUpgradeGraphSpot.anchoredPosition
                = new Vector2(250, CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else if (9 == lv)
        {
            CurrentUpgradeGraphSpot.anchoredPosition
                = new Vector2(375, CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else
        {
            CurrentUpgradeGraphSpot.gameObject.SetActive(false);
        }

        // Current & Next
        CurrentLvTxt.text = _MTAFB.CurrentLevel.Value.ToString();
        CurrentStateTxt.text = _MTAFB.ActualState.Value.ToString();

        if (_MTAFB.CurrentLevel.Value < 10)
        {
            NextLvTxt.text = (_MTAFB.CurrentLevel.Value + 1).ToString();
            NextStateTxt.text = (_MTAFB.ActualState.Value + _MTAFB.UpgradeValueByLevelRange[(int)(_MTAFB.CurrentLevel.Value / 3)]).ToString();
            CompletedSignGO.gameObject.SetActive(false);
        }
        else
        {
            CompletedSignGO.gameObject.SetActive(true);
        }

        DescriptionTxt.text = _MTAFB.Desc.ToString();
    }

    #endregion
}
