using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescBUEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Desc BaseUpgrade")]

    [Space(10)]
    [Header("=== Center")]
    [SerializeField] public TMP_Text CenterNameTxt;

    [Space(10)]
    [Header("=== Left")]

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] private TMP_Text LeftLVTxt;
    [SerializeField] private TMP_Text LeftValueTxt;

    [Space(5)]
    [Header("-- Txt List")]
    [SerializeField] private Transform LeftLVTxtParentTF;
    [SerializeField] private Transform LeftValueTxtParentTF;

    [Space(5)]
    [Header("-- Value")]
    [SerializeField] private float BaseSpotX;
    [SerializeField] private float IntervalSpotX;

    [Space(5)]
    [Header("-- Current")]
    [SerializeField] public RectTransform CurrentRangeRT;


    [Space(10)]
    [Header("=== Right")]

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] public TMP_Text RightLVTxt;
    [SerializeField] public TMP_Text RightValueTxt;

    [Space(5)]
    [Header("-- Current")]
    [SerializeField] private TMP_Text CurrentLvTxt;
    [SerializeField] private TMP_Text CurrentStateTxt;

    [Space(5)]
    [Header("-- Next")]
    [SerializeField] public TMP_Text NextLvTxt;
    [SerializeField] public TMP_Text NextStateTxt;

    [Space(5)]
    [Header("-- Complete")]
    [SerializeField] private GameObject CompletedSignGO;

    #endregion

    #region - Hide

    // Left
    [HideInInspector] private List<TMP_Text> LeftLVTxtList;
    [HideInInspector] private List<TMP_Text> LeftValueTxtList;

    [HideInInspector] public Image CurrentRangeImg;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        // String
        LeftLVTxt.text = CSVManager.Instance.Get_StaticWord(34);
        LeftValueTxt.text = CSVManager.Instance.Get_StaticWord(35);

        RightLVTxt.text = CSVManager.Instance.Get_StaticWord(34);
        RightValueTxt.text = CSVManager.Instance.Get_StaticWord(35);

        CurrentRangeImg =
            DevTool.Get_ComponentTType(CurrentRangeRT.gameObject, out Image img) ? 
            img : null;

        LeftLVTxtList = DevTool.Get_ChildList<TMP_Text>(LeftLVTxtParentTF);
        LeftValueTxtList = DevTool.Get_ChildList<TMP_Text>(LeftValueTxtParentTF);

        DevTool.Get_ComponentTType<TMP_Text>(CompletedSignGO.transform.GetChild(0).gameObject).text = CSVManager.Instance.Get_StaticWord(48);
    }

    #endregion

    #region Desc

    public void SetOn_Desc<T>(BUState<T> _State, string _Name)
    {
        // Center
        SetOn_Center(_Name);

        // Left
        SetOn_Left_State(_State);
        SetOn_Left_CurrentRange(_State.CurrentLevel.Value);

        // Right
        SetOn_Right_CurrentState(_State.CurrentLevel.Value, _State.ActualState.Value);
        SetOn_Right_NextState(_State);
    }

    public void SetOff_Desc()
    {
        // Center
        CenterNameTxt.text = "-";

        // Left
        for (int i = 0; i < LeftValueTxtList.Count; i++)
        {
            LeftValueTxtList[i].text = "-";
        }

        CurrentRangeRT.gameObject.SetActive(false);

        // Right
        NextLvTxt.text = "-";
        NextStateTxt.text = "-";

        CurrentLvTxt.text = "-";
        CurrentStateTxt.text = "-";

        CompletedSignGO.SetActive(false);
    }



    private void SetOn_Center(string _Name)
    {
        CenterNameTxt.text = _Name;
    }

    private void SetOn_Left_State<T>(BUState<T> _State)
    {
        for (int i = 0; i < LeftValueTxtList.Count; i++)
        {
            string symbol = "";
            string stateString = "";

            if (_State.BaseState.GetType() == typeof(float))
            {
                float value = float.Parse(_State.UpgradeValueByLevelRange[i].ToString());
                stateString = value.ToString();
                symbol = value > 0 ? "+" : "";
            }
            else if (_State.BaseState.GetType() == typeof(int))
            {
                int value = int.Parse(_State.UpgradeValueByLevelRange[i].ToString());
                stateString = value.ToString();
                symbol = value > 0 ? "+" : "";
            }

            LeftValueTxtList[i].text = symbol + stateString;
        }
    }

    private void SetOn_Left_CurrentRange(int _CurrentLv)
    {
        CurrentRangeRT.gameObject.SetActive(true);

        CurrentRangeRT.anchoredPosition = new Vector2(
            BaseSpotX + (IntervalSpotX * (_CurrentLv > 0 ? _CurrentLv / 3 : 0)),
            CurrentRangeRT.anchoredPosition.y);

        CurrentRangeRT.gameObject.SetActive(_CurrentLv >= 10 ? false : true);
    }

    private void SetOn_Right_CurrentState<T>(int _Lv, T _State)
    {
        string sizeStart = "<size=50%>";
        string sizeEnd = "</size>\n";

        CurrentLvTxt.text = $"{sizeStart}Level{sizeEnd}{_Lv}";
        CurrentStateTxt.text = $"{sizeStart}Value{sizeEnd}{_State}";
    }

    private void SetOn_Right_NextState<T>(BUState<T> _State)
    {
        string sizeStart = "<size=50%>";
        string sizeEnd = "</size>\n";

        if (_State.CurrentLevel.Value < DevTool.BU_MaxLevel)
        {
            NextLvTxt.text = $"{sizeStart}Level{sizeEnd}{_State.CurrentLevel.Value + 1}";

            string nextValue = "";
            if (_State.BaseState.GetType() == typeof(float))
            {
                nextValue = 
                    (float.Parse(_State.ActualState.Value.ToString()) + 
                    float.Parse(_State.UpgradeValueByLevelRange[(int)(_State.CurrentLevel.Value / 3)].ToString())).ToString();
            }
            else if (_State.BaseState.GetType() == typeof(int))
            {
                nextValue =
                    (int.Parse(_State.ActualState.Value.ToString()) + 
                    int.Parse(_State.UpgradeValueByLevelRange[(int)(_State.CurrentLevel.Value / 3)].ToString())).ToString();
            }

            NextStateTxt.text = $"{sizeStart}Value{sizeEnd}{nextValue}";
            CompletedSignGO.gameObject.SetActive(false);
        }
        else
        {
            CompletedSignGO.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Get

    public List<Component> Get_MainColorList()
    {
        List<Component> result = new List<Component>()
        {
            CenterNameTxt,
            CurrentRangeImg,

            NextLvTxt,
            NextStateTxt,
        };

        result.AddRange(LeftValueTxtList);
        result.AddRange(LeftLVTxtList);


        return result;
    }

    public List<Component> Get_SubColorList()
    {
        List<Component> result = new List<Component>()
        {
            LeftLVTxt,
            LeftValueTxt,

            RightLVTxt,
            RightValueTxt
        };

        return result;
    }


    #endregion
}
