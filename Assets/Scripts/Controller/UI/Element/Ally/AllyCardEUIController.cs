using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllyCardEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Image FrameImg;
    [SerializeField] private Image LightImg;
    [SerializeField] private Image BGMarkImg;
    [SerializeField] private TMP_Text RankTxt;
    [SerializeField] private TMP_Text PreNameTxt;
    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text DescTxt;

    // Value
    [HideInInspector] public int CurrentID;

    // Light
    [HideInInspector] private Sequence LightSeq;

    // Owner
    [HideInInspector] public AllyCardUIController AllyOwnerUIController;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        LightSeq = Play_LightSeq();
        LightSeq.Pause();
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        LightSeq.Play();
    }

    private void OnDisable()
    {
        LightSeq.Pause();
    }

    #endregion

    #region Set

    public void Set_Card(int _TypeID, AllyCardData _Data)
    {
        Set_CardBGMark(_TypeID);

        FrameImg.sprite = UnitManager.Instance.AllyCardFrameList[_Data.Rank];
        LightImg.sprite = UnitManager.Instance.AllyCardLightList[_Data.Rank];
        RankTxt.text = UnitManager.Instance.AllyCardRateList[_Data.Rank];
        RankTxt.color = UnitManager.Instance.AllyCardColorList[_Data.Rank];

        CurrentID = _Data.ID;
        NameTxt.text = _Data.Name;
        DescTxt.text = _Data.Desc;

        AllyCardData preCardData = AllyManager.Instance.Get_PreAllyCardData(_Data);
        PreNameTxt.text = preCardData != null ? $"-({preCardData.Name})->" : "";
        PreNameTxt.gameObject.SetActive(preCardData != null);

        LightSeq.timeScale = _Data.Rank + 1;

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

    #region Play

    private Sequence Play_LightSeq()
    {
        Sequence seq = DOTween.Sequence();

        DevTool.Set_AlphaColor(LightImg, 1);

        seq.Append(LightImg.DOFade(0.2f, 5f).SetEase(Ease.Linear));
        seq.Join(RankTxt.DOFade(0.2f, 5f).SetEase(Ease.Linear));
        seq.Append(LightImg.DOFade(1f, 0.5f).SetEase(Ease.Linear));
        seq.Join(RankTxt.DOFade(1f, 0.5f).SetEase(Ease.Linear));

        seq.SetLoops(-1, LoopType.Restart);

        return seq;
    }


    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        if (OwnerUIController != null) AllyOwnerUIController.Set_SelectingCard(this);
    }

    #endregion
}
