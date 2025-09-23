using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyRequestUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Request")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private TMP_Text RequestNameTxt;

    [Space(5)]
    [SerializeField] private TMP_Text CompleteDescTxt;
    [SerializeField] private Image CompleteGageImg;
    [SerializeField] private TMP_Text CompletePercentTxt;

    [Space(5)]
    [SerializeField] private TMP_Text FailDescTxt;
    [SerializeField] private Image FailGageImg;
    [SerializeField] private TMP_Text FailPercentTxt;

    [HideInInspector] public AllyHUDController AllyHUD;

    #endregion

    #region Offset

    public void Offset(AllyHUDController _EnemyHUD)
    {
        AllyHUD = _EnemyHUD;
    }

    #endregion

    #region Set

    public void Set_RequestTxt_Language(AllyRequest _Request)
    {
        if (_Request == null) return;

        RequestNameTxt.text = _Request.Get_Name();
        CompleteDescTxt.text = _Request.Get_CompleteDesc();
        FailDescTxt.text = _Request.Get_FailDesc();
    }

    public void Set_Request_CompleteProgress(float _Value)
    {
        CompleteGageImg.fillAmount = _Value;
    }

    public void Set_Request_FailProgress(float _Value)
    {
        FailGageImg.fillAmount = _Value;
    }

    public void Set_Request_CompleteTxt(float _Value, string _ProgressTxt = "")
    {
        CompletePercentTxt.text = $"{_ProgressTxt}<size=60%>({System.Math.Round(_Value, 1) * 100}%)</size>";
    }

    public void Set_Request_FailTxt(float _Value, string _ProgressTxt = "")
    {
        FailPercentTxt.text = $"{_ProgressTxt}<size=60%>({System.Math.Round(_Value, 1) * 100}%)</size>";
    }

    #endregion
}
