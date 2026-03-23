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
    [SerializeField] public TMP_Text centerNameTxt;

    [Space(10)]
    [Header("=== Left")]

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] private TMP_Text leftLvTxt;
    [SerializeField] private TMP_Text leftValueTxt;

    [Space(5)]
    [Header("-- Txt List")]
    [SerializeField] private Transform leftLvTxtParentTf;
    [SerializeField] private Transform leftValueTxtParentTf;

    [Space(5)]
    [Header("-- Value")]
    [SerializeField] private float baseSpotX;
    [SerializeField] private float intervalSpotX;

    [Space(5)]
    [Header("-- Current")]
    [SerializeField] public RectTransform currentRangeRt;


    [Space(10)]
    [Header("=== Right")]

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] public TMP_Text rightLvTxt;
    [SerializeField] public TMP_Text rightValueTxt;

    [Space(5)]
    [Header("-- Current")]
    [SerializeField] private TMP_Text currentLvTxt;
    [SerializeField] private TMP_Text currentStateTxt;

    [Space(5)]
    [Header("-- Next")]
    [SerializeField] public TMP_Text nextLvTxt;
    [SerializeField] public TMP_Text nextStateTxt;

    [Space(5)]
    [Header("-- Complete")]
    [SerializeField] private GameObject completedSignGo;

    #endregion

    #region - Hide

    // Left
    [HideInInspector] private List<TMP_Text> leftLvTxtList;
    [HideInInspector] private List<TMP_Text> leftValueTxtList;

    [HideInInspector] public Image currentRangeImg;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        currentRangeImg =
            DevTool.Get_ComponentTType(currentRangeRt.gameObject, out Image img) ? 
            img : null;

        leftLvTxtList = DevTool.Get_ChildList<TMP_Text>(leftLvTxtParentTf);
        leftValueTxtList = DevTool.Get_ChildList<TMP_Text>(leftValueTxtParentTf);

        Set_LanguageTxt();
    }

    #endregion

    #region Desc

    public void SetOn_Desc<T>(BUState<T> state, string name)
    {
        // Center
        SetOn_Center(name);

        // Left
        SetOn_Left_State(state);
        SetOn_Left_CurrentRange(state.currentLevel.Value);

        // Right
        SetOn_Right_CurrentState(state.currentLevel.Value, state.actualState.Value);
        SetOn_Right_NextState(state);
    }

    public void SetOff_Desc()
    {
        // Center
        centerNameTxt.text = "-";

        // Left
        for (int i = 0; i < leftValueTxtList.Count; i++)
        {
            leftValueTxtList[i].text = "-";
        }

        currentRangeRt.gameObject.SetActive(false);

        // Right
        nextLvTxt.text = "-";
        nextStateTxt.text = "-";

        currentLvTxt.text = "-";
        currentStateTxt.text = "-";

        completedSignGo.SetActive(false);
    }



    private void SetOn_Center(string name)
    {
        centerNameTxt.text = name;
    }

    private void SetOn_Left_State<T>(BUState<T> state)
    {
        for (int i = 0; i < leftValueTxtList.Count; i++)
        {
            string symbol = "";
            string stateString = "";

            if (state.baseState.GetType() == typeof(float))
            {
                float value = float.Parse(state.upgradeValueByLevelRange[i].ToString());
                stateString = value.ToString();
                symbol = value > 0 ? "+" : "";
            }
            else if (state.baseState.GetType() == typeof(int))
            {
                int value = int.Parse(state.upgradeValueByLevelRange[i].ToString());
                stateString = value.ToString();
                symbol = value > 0 ? "+" : "";
            }

            leftValueTxtList[i].text = symbol + stateString;
        }
    }

    private void SetOn_Left_CurrentRange(int currentLv)
    {
        currentRangeRt.gameObject.SetActive(true);

        currentRangeRt.anchoredPosition = new Vector2(
            baseSpotX + (intervalSpotX * (currentLv > 0 ? currentLv / 3 : 0)),
            currentRangeRt.anchoredPosition.y);

        currentRangeRt.gameObject.SetActive(currentLv >= 10 ? false : true);
    }

    private void SetOn_Right_CurrentState<T>(int lv, T tState)
    {
        string sizeStart = "<size=50%>";
        string sizeEnd = "</size>\n";

        currentLvTxt.text = $"{sizeStart}Level{sizeEnd}{lv}";
        currentStateTxt.text = $"{sizeStart}Value{sizeEnd}{tState}";
    }

    private void SetOn_Right_NextState<T>(BUState<T> state)
    {
        string sizeStart = "<size=50%>";
        string sizeEnd = "</size>\n";

        if (state.currentLevel.Value < DevTool.buMaxLevel)
        {
            nextLvTxt.text = $"{sizeStart}Level{sizeEnd}{state.currentLevel.Value + 1}";

            string nextValue = "";
            if (state.baseState.GetType() == typeof(float))
            {
                nextValue = 
                    (float.Parse(state.actualState.Value.ToString()) + 
                    float.Parse(state.upgradeValueByLevelRange[(int)(state.currentLevel.Value / 3)].ToString())).ToString();
            }
            else if (state.baseState.GetType() == typeof(int))
            {
                nextValue =
                    (int.Parse(state.actualState.Value.ToString()) + 
                    int.Parse(state.upgradeValueByLevelRange[(int)(state.currentLevel.Value / 3)].ToString())).ToString();
            }

            nextStateTxt.text = $"{sizeStart}Value{sizeEnd}{nextValue}";
            completedSignGo.gameObject.SetActive(false);
        }
        else
        {
            completedSignGo.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Get

    public List<Component> Get_MainColorList()
    {
        List<Component> result = new List<Component>()
        {
            centerNameTxt,
            currentRangeImg,

            nextLvTxt,
            nextStateTxt,
        };

        result.AddRange(leftValueTxtList);
        result.AddRange(leftLvTxtList);


        return result;
    }

    public List<Component> Get_SubColorList()
    {
        List<Component> result = new List<Component>()
        {
            leftLvTxt,
            leftValueTxt,

            rightLvTxt,
            rightValueTxt
        };

        return result;
    }


    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        // String
        leftLvTxt.text = ResourceManager.instance.Get_StaticWord(34);
        leftValueTxt.text = ResourceManager.instance.Get_StaticWord(35);

        rightLvTxt.text = ResourceManager.instance.Get_StaticWord(34);
        rightValueTxt.text = ResourceManager.instance.Get_StaticWord(35);

        DevTool.Get_ComponentTType<TMP_Text>(completedSignGo.transform.GetChild(DevTool.Get_TSChildIndex(completedSignGo, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(48);
    }

    #endregion
}
