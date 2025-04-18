using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllyCardUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Card UI")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform CardParentTF;
    [SerializeField] private Transform CardRerollParentTF;
    [SerializeField] private RectTransform CardBookingFrameImgRT;

    [Space(10)]
    [Header("=== Btn")]
    [SerializeField] private OwnBtnEUIController SelectBtn;

    #endregion

    #region - Hide

    // Card
    [HideInInspector] private List<AllyCardEUIController> Cards;

    //Reroll
    [HideInInspector] private List<AllyCardRerollEUIController> Rerolls;

    // Select & Booking
    [HideInInspector] private AllyCardEUIController SelectingCard;
    [HideInInspector] private AllyCardEUIController BookingCard;

    // Type
    [HideInInspector] public int TypeIndex = 0;

    #endregion

    #endregion

    #region Offset

    public override void Offset() 
    {
        Offset_Basic();
        Offset_Txt();
    }

    private void Offset_Basic()
    {
        // Cards
        Cards = DevTool.Get_ChildList<AllyCardEUIController>(CardParentTF);
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].gameObject.SetActive(false);
            Cards[i].Offset();
            Cards[i].OwnerUIController = this;
            Cards[i].AllyOwnerUIController = this;
        }

        // Reroll
        Rerolls = DevTool.Get_ChildList<AllyCardRerollEUIController>(CardRerollParentTF);
        for (int i = 0; i < Rerolls.Count; i++)
        {
            Rerolls[i].gameObject.SetActive(false);
            Rerolls[i].Offset();
            Rerolls[i].OwnerUIController = this;

            Rerolls[i].TargetCardEUIController = Cards[i];
            Cards[i].RerollEUI = Rerolls[i];
        }

        // Select Btn
        SelectBtn.Offset();
        SelectBtn.OwnerUIController = this;
    }

    private void Offset_Txt()
    {
        DevTool.Get_ComponentTType<TMP_Text>(CardBookingFrameImgRT.transform.GetChild(0).gameObject).text =
            CSVManager.Instance.Get_StaticWord(82);

        DevTool.Get_ComponentTType<TMP_Text>(SelectBtn.gameObject.transform.GetChild(0).gameObject).text =
            CSVManager.Instance.Get_StaticWord(83);
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (Is_Interact_CardBooking()) return;
        if (Is_Interact_Reroll()) return;
        if (Is_Interact_SelectBtn()) return;
    }

    private bool Is_Interact_CardBooking()
    {
        if (CurrentBtn == null ||
            SelectingCard == null ||
            CurrentBtn != SelectingCard)
            return false;

        if (SelectingCard != BookingCard)
            Set_BookingCard(SelectingCard);

        return true;
    }

    private bool Is_Interact_SelectBtn()
    {
        if (CurrentBtn != SelectBtn ||
            BookingCard == null)
            return false;

        AllyManager.Instance.Add_AllyCard(TypeIndex, BookingCard.CurrentID);
        SetOff_ThisPanel();

        return true;
    }

    private bool Is_Interact_Reroll()
    {
        if (!DevTool.Can_CastingTType(CurrentBtn, out AllyCardRerollEUIController reroll))
            return false;

        reroll.Try_Interact();
        Set_NewCard(Rerolls.IndexOf(reroll));
        if (BookingCard == reroll.TargetCardEUIController)
        {
            BookingCard = null;
            CardBookingFrameImgRT.gameObject.SetActive(false);
        }

        return true;
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Set_NewCardDeck();
    }

    #endregion

    #region Card

    private void Set_NewCard(int _Index)
    {
        List<int> idList = new List<int>();
        for (int i = 0; i < Cards.Count; i++)
            idList.Add(Cards[i].CurrentID);

        AllyCardData cardData = AllyManager.Instance.Get_ChoiceAbleRandomData(TypeIndex, idList);

        Cards[_Index].Set_Card(TypeIndex, cardData);
    }

    private void Set_NewCardDeck()
    {
        List<AllyCardData> cardDeckData = AllyManager.Instance.Get_ChoiceAbleRandomData(TypeIndex, Cards.Count);

        for (int i = 0; i < Cards.Count; i++)
            Cards[i].Set_Card(TypeIndex, cardDeckData[i]);

        BookingCard = null;
        CardBookingFrameImgRT.gameObject.SetActive(false);
        Set_SelectingCard(Cards[0]);
    }

    public void Set_SelectingCard(AllyCardEUIController _AllyCard)
    {
        if (SelectingCard != _AllyCard)
        {
            if (SelectingCard != null)
            {
                DevTool.Set_KillTween(SelectingCard.gameObject.transform);
                SelectingCard.gameObject.transform.DOScale(1f, 0.1f);
            }

            SelectingCard = _AllyCard;

            DevTool.Set_KillTween(SelectingCard.gameObject.transform);
            SelectingCard.gameObject.transform.DOScale(1.05f, 0.1f);
        }
    }

    #endregion

    #region Select & Book

    private void Set_BookingCard(AllyCardEUIController _CardEUI)
    {
        BookingCard = _CardEUI;

        CardBookingFrameImgRT.anchoredPosition = DevTool.Get_ComponentTType<RectTransform>(BookingCard.gameObject).anchoredPosition;
        CardBookingFrameImgRT.gameObject.SetActive(true);

        Play_BookingRT();
    }

    #endregion

    #region Play

    private void Play_BookingRT()
    {
        DevTool.Set_KillTween(CardBookingFrameImgRT);

        Sequence seq = DOTween.Sequence();

        for (int i = 10; i >= 0; i--)
            if (i % 2 == 0)
                seq.Append(CardBookingFrameImgRT.DOAnchorPosY(i * 4, 0.05f));
            else
                seq.Append(CardBookingFrameImgRT.DOAnchorPosY(-(i * 4), 0.05f));
    }

    #endregion
}
