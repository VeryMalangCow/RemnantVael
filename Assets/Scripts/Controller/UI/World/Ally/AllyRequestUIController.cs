using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AllyRequestUIController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Request")]

    [Space(10)]
    [Header("=== Public")]
    [FormerlySerializedAs("ThisCanvas")][SerializeField] private TMP_Text RequestNameTxt;

    [Space(10)]
    [Header("=== Complete")]
    [FormerlySerializedAs("CompleteDescTxt")][SerializeField] private TMP_Text completeDescTxt;
    [FormerlySerializedAs("CompleteGageImg")][SerializeField] private Image completeGaugeImg;
    [FormerlySerializedAs("CompletePercentTxt")][SerializeField] private TMP_Text completePercentTxt;

    [Space(10)]
    [Header("=== Fail")]
    [FormerlySerializedAs("FailDescTxt")][SerializeField] private TMP_Text failDescTxt;
    [FormerlySerializedAs("FailGageImg")][SerializeField] private Image failGaugeImg;
    [FormerlySerializedAs("FailPercentTxt")][SerializeField] private TMP_Text failPercentTxt;

    [Space(10)]
    [Header("=== Difficulty")]
    [FormerlySerializedAs("DiffcultyImg")][SerializeField] private Image diffcultyImg;
    [FormerlySerializedAs("DiffcultyExtraTxt")][SerializeField] private TMP_Text diffcultyExtraTxt;

    [Space(10)]
    [Header("=== Reward")]
    [FormerlySerializedAs("RewardImg")][SerializeField] private Image rewardImg;
    [FormerlySerializedAs("RewardExtraTxt")][SerializeField] private TMP_Text rewardExtraTxt;

    #endregion

    #region -Hide

    [HideInInspector] public AllyHUDController allyHud;

    #endregion

    #endregion

    #region Offset

    public void Offset(AllyHUDController enemyHud)
    {
        allyHud = enemyHud;
    }

    #endregion

    #region Set

    public void Set_RequestTxt_Language(AllyRequest request)
    {
        if (request == null) return;

        RequestNameTxt.text = request.Get_Name();
        completeDescTxt.text = request.Get_CompleteDesc();
        failDescTxt.text = request.Get_FailDesc();

        diffcultyImg.sprite = ResourceManager.instance.Get_AllyRequestRank(request.Get_Rank());
        diffcultyExtraTxt.text = $"{(request.Get_Rank() + 1)}";

        rewardImg.sprite = ResourceManager.instance.Get_AllyRequestReward(request.Get_RewardType());
        int extraAmount = AllyRequest.rewardCaculateDict[request.Get_RewardType()](request.Get_Rank());
        if (extraAmount != -1)
        { rewardExtraTxt.text = $"+{extraAmount}"; }
        else
        {
            Debug.Log("타입이 다른 보상");
        }
    }

    public void Set_Request_CompleteProgress(float _Value)
    {
        completeGaugeImg.fillAmount = _Value;
    }

    public void Set_Request_FailProgress(float value)
    {
        failGaugeImg.fillAmount = value;
    }

    public void Set_Request_CompleteTxt(float value, string progressTxt = "")
    {
        completePercentTxt.text = $"{progressTxt}<size=60%>({System.Math.Round(value, 1) * 100}%)</size>";
    }

    public void Set_Request_FailTxt(float value, string progressTxt = "")
    {
        failPercentTxt.text = $"{progressTxt}<size=60%>({System.Math.Round(value, 1) * 100}%)</size>";
    }

    #endregion
}
