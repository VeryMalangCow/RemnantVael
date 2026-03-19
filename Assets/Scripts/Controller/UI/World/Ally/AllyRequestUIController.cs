using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyRequestUIController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Request")]

    [Space(10)]
    [Header("=== Public")]
    [SerializeField] private TMP_Text RequestNameTxt;

    [Space(10)]
    [Header("=== Complete")]
    [SerializeField] private TMP_Text CompleteDescTxt;
    [SerializeField] private Image CompleteGageImg;
    [SerializeField] private TMP_Text CompletePercentTxt;

    [Space(10)]
    [Header("=== Fail")]
    [SerializeField] private TMP_Text FailDescTxt;
    [SerializeField] private Image FailGageImg;
    [SerializeField] private TMP_Text FailPercentTxt;

    [Space(10)]
    [Header("=== Difficulty")]
    [SerializeField] private Image DiffcultyImg;
    [SerializeField] private TMP_Text DiffcultyExtraTxt;

    [Space(10)]
    [Header("=== Reward")]
    [SerializeField] private Image RewardImg;
    [SerializeField] private TMP_Text RewardExtraTxt;

    #endregion

    #region -Hide

    [HideInInspector] public AllyHUDController AllyHUD;

    #endregion

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

        DiffcultyImg.sprite = ResourceManager.instance.Get_AllyRequestRank(_Request.Get_Rank());
        DiffcultyExtraTxt.text = $"{(_Request.Get_Rank() + 1)}";

        RewardImg.sprite = ResourceManager.instance.Get_AllyRequestReward(_Request.Get_RewardType());
        int extraAmount = AllyRequest.rewardCaculateDict[_Request.Get_RewardType()](_Request.Get_Rank());
        if (extraAmount != -1)
        { RewardExtraTxt.text = $"+{extraAmount}"; }
        else
        {
            Debug.Log("타입이 다른 보상");
        }
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
