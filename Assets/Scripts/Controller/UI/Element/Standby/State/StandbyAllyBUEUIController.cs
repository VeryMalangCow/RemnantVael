using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyAllyBUEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Image FaceImg;
    [SerializeField] private TMP_Text NameTxt;
    // Dmg, Rof, CC, CD, Size, MSpd, KB, Dur, Spd
    [SerializeField] private TMP_Text[] StateValueArr;

    #endregion

    #region - Hide

    [HideInInspector] private RectTransform ThisRT;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = TryGetComponent(out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Set

    public void Set_Pos(Vector2 _Pos)
    {
        ThisRT.anchoredPosition = _Pos;
    }

    public void Set_Data(AllyController _Ally)
    {
        FaceImg.sprite = _Ally.Get_FrontFaceImg();
        NameTxt.text = _Ally.Get_Name();

        AllyState state = _Ally.Get_ActaulAllyState();
        StateValueArr[0].text = $"{DevTool.Get_RoundFloatString(state.Dmg.Value).Replace("+", "")}";
        StateValueArr[1].text = $"{DevTool.Get_RoundFloatString(state.Rof.Value).Replace("+", "")}<size=65%>/s</size>";
        StateValueArr[2].text = $"{DevTool.Get_RoundFloatString(state.CC.Value * 100).Replace("+", "")}<size=65%>%</size>";
        StateValueArr[3].text = $"{DevTool.Get_RoundFloatString(state.CD.Value + 1).Replace("+", "")}<size=65%>x</size>";
        StateValueArr[4].text = $"{DevTool.Get_RoundFloatString(state.AttackSize.Value).Replace("+", "")}";
        StateValueArr[5].text = $"{DevTool.Get_RoundFloatString(state.MuzzleSpeed.Value + 1).Replace("+", "")}";
        StateValueArr[6].text = $"{DevTool.Get_RoundFloatString(state.KBPower.Value).Replace("+", "")}";
        StateValueArr[7].text = $"{DevTool.Get_RoundFloatString(state.Dur.Value).Replace("+", "")}<size=65%>s</size>";
        StateValueArr[8].text = $"{DevTool.Get_RoundFloatString(state.MovementSpeed.Value).Replace("+", "")}";
    }

    #endregion
}
