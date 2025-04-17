using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyCardEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Image FrameImg;
    [SerializeField] private Image LightImg;
    [SerializeField] private Image BGMarkImg;
    [SerializeField] private TMP_Text RankTxt;
    [SerializeField] private TMP_Text NameTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        
    }

    #endregion

    #region Set

    public void Set_Card(int _TypeID, AllyCardData _Data)
    {
        Set_CardBGMark(_TypeID);

        FrameImg.sprite = UnitManager.Instance.AllyCardFrameList[_Data.Rank];
        LightImg.sprite = UnitManager.Instance.AllyCardLightList[_Data.Rank];
        RankTxt.text = UnitManager.Instance.AllyCardRateList[_Data.Rank];

        NameTxt.text = _Data.Name;

        gameObject.SetActive(true);
    }

    private void Set_CardBGMark(int _TypeID)
    {
        switch(_TypeID)
        {
            case 0:
                BGMarkImg.sprite = UnitManager.Instance.StrikeTeamIcon.TypeSpecial;
                break;
            case 1:
                BGMarkImg.sprite = UnitManager.Instance.UplinkTeamIcon.TypeSpecial;
                break;
            case 2:
                BGMarkImg.sprite = UnitManager.Instance.NeoTeamIcon.TypeSpecial;
                break;

            default:
                break;
        }
    }

    #endregion
}
