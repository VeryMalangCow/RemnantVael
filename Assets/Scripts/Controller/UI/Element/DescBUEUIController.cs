using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescBUEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Desc _ For BaseUpgrade")]

    [Space(10)]
    [Header("=== Center")]
    [SerializeField] public TMP_Text CenterName;

    [Space(10)]
    [Header("=== Upgrade Graph")]
    [SerializeField] public TMP_Text UpgradeGraphLVTxt;
    [SerializeField] public List<TMP_Text> UpgradeGraphLV_TxtList;
    [SerializeField] public TMP_Text UpgradeGraphValueTxt;
    [SerializeField] public List<TMP_Text> UpgradeGraphDetailState_TxtList;
    [SerializeField] public RectTransform CurrentUpgradeGraphSpot;
    [SerializeField] private float BaseSpotX;
    [SerializeField] private float IntervalSpotX;

    [Space(10)]
    [Header("=== Current & Next Upgrade")]
    [SerializeField] public TMP_Text UpgradeNextLVTxt;
    [SerializeField] public TMP_Text UpgradeNextValueTxt;
    [SerializeField] private TMP_Text CurrentLvTxt;
    [SerializeField] private TMP_Text CurrentStateTxt;
    [SerializeField] public TMP_Text NextLvTxt;
    [SerializeField] public TMP_Text NextStateTxt;
    [SerializeField] private GameObject CompletedSignGO;

    // Other
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
        }
    }

    #endregion

    #region Desc

    public void SetOn_Desc<T>(BUState<T> _MTAFB)
    {
        
        // Graph
        CenterName.text = _MTAFB.Name;

        for (int i = 0; i < UpgradeGraphDetailState_TxtList.Count; i++) 
        {
            if (_MTAFB.BaseState.GetType() == typeof(float))
            {
                float value = float.Parse(_MTAFB.UpgradeValueByLevelRange[i].ToString());
                string valueText = value > 0 ? "+" + value.ToString() : value.ToString();
                UpgradeGraphDetailState_TxtList[i].text = valueText;
            }
            else if (_MTAFB.BaseState.GetType() == typeof(int))
            {
                int value = int.Parse(_MTAFB.UpgradeValueByLevelRange[i].ToString());
                string valueText = value > 0 ? "+" + value.ToString() : value.ToString();
                UpgradeGraphDetailState_TxtList[i].text = valueText;
            }
            
        }

        int lv = _MTAFB.CurrentLevel.Value;
        CurrentUpgradeGraphSpot.gameObject.SetActive(true);

        if (0 <= lv && lv <= 2)
        {
            CurrentUpgradeGraphSpot.anchoredPosition 
                = new Vector2(BaseSpotX, CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else if (3 <= lv && lv <= 5)
        {
            CurrentUpgradeGraphSpot.anchoredPosition
                = new Vector2(BaseSpotX + (IntervalSpotX * 1), CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else if (6 <= lv && lv <= 8)
        {
            CurrentUpgradeGraphSpot.anchoredPosition
                = new Vector2(BaseSpotX + (IntervalSpotX * 2), CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else if (9 == lv)
        {
            CurrentUpgradeGraphSpot.anchoredPosition
                = new Vector2(BaseSpotX + (IntervalSpotX * 3), CurrentUpgradeGraphSpot.anchoredPosition.y);
        }
        else
        {
            CurrentUpgradeGraphSpot.gameObject.SetActive(false);
        }

        // Current & Next
        CurrentLvTxt.text = "<size=50%>Level</size>\n" + _MTAFB.CurrentLevel.Value.ToString();
        CurrentStateTxt.text = "<size=50%>Value</size>\n" + _MTAFB.ActualState.Value.ToString();

        if (_MTAFB.CurrentLevel.Value < 10)
        {
            NextLvTxt.text = "<size=50%>Level</size>\n" + (_MTAFB.CurrentLevel.Value + 1).ToString();
            if (_MTAFB.BaseState.GetType() == typeof(float))
            {
                float value = float.Parse(_MTAFB.ActualState.Value.ToString());
                float plusValue = float.Parse(_MTAFB.UpgradeValueByLevelRange[(int)(_MTAFB.CurrentLevel.Value / 3)].ToString());

                NextStateTxt.text = "<size=50%>Value</size>\n" + (value + plusValue).ToString();
            }
            else if (_MTAFB.BaseState.GetType() == typeof(int))
            {
                int value = int.Parse(_MTAFB.ActualState.Value.ToString());
                int plusValue = int.Parse(_MTAFB.UpgradeValueByLevelRange[(int)(_MTAFB.CurrentLevel.Value / 3)].ToString());

                NextStateTxt.text = "<size=50%>Value</size>\n" + (value + plusValue).ToString();
            }
            
            CompletedSignGO.gameObject.SetActive(false);
        }
        else
        {
            CompletedSignGO.gameObject.SetActive(true);
        }

    }

    public void SetOff_Desc()
    {
        CenterName.text = "-";

        for (int i = 0; i < UpgradeGraphDetailState_TxtList.Count; i++)
        {
            UpgradeGraphDetailState_TxtList[i].text = "-";
        }

        NextLvTxt.text = "-";
        NextStateTxt.text = "-";

        CurrentLvTxt.text = "-";
        CurrentStateTxt.text = "-";
    }


    #endregion
}
