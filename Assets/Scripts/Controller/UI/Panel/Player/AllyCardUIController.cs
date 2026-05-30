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
    [SerializeField] private Transform cardParentTf;
    [SerializeField] private Transform cardRerollParentTf;
    [SerializeField] private RectTransform cardBookingFrameImgRt;

    [Space(10)]
    [Header("=== Btn")]
    [SerializeField] private OwnBtnEUIController selectBtn;

    #endregion

    #region - Hide

    // Card
    [HideInInspector] private List<AllyCardEUIController> cards;

    //Reroll
    [HideInInspector] private List<AllyCardRerollEUIController> rerolls;

    // Select & Booking
    [HideInInspector] private AllyCardEUIController selectingCard;
    [HideInInspector] private AllyCardEUIController bookingCard;

    // Type
    [HideInInspector] public int typeIndex = 0;

    #endregion

    #endregion

    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);
        Offset_Basic(); 
        Set_BaseLanguageTxt();
    }

    private void Offset_Basic()
    {
        // Cards
        cards = DevTool.Get_ChildList<AllyCardEUIController>(cardParentTf);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.SetActive(false);
            cards[i].Offset();
            cards[i].ownerUIController = this;
            cards[i].allyOwnerUIController = this;
        }

        // Reroll
        rerolls = DevTool.Get_ChildList<AllyCardRerollEUIController>(cardRerollParentTf);
        for (int i = 0; i < rerolls.Count; i++)
        {
            rerolls[i].gameObject.SetActive(false);
            rerolls[i].Offset();
            rerolls[i].ownerUIController = this;

            rerolls[i].targetCardEuiController = cards[i];
            cards[i].rerollEui = rerolls[i];
        }

        // Select Btn
        selectBtn.Offset();
        selectBtn.ownerUIController = this;
    }


    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_CardBooking()) return;
        if (Is_Interact_Reroll()) return;
        if (Is_Interact_SelectBtn()) return;
    }

    private bool Is_Interact_CardBooking()
    {
        if (currentBtn == null ||
            selectingCard == null ||
            currentBtn != selectingCard)
            return false;

        if (selectingCard != bookingCard)
            Set_BookingCard(selectingCard);

        return true;
    }

    private bool Is_Interact_SelectBtn()
    {
        if (currentBtn != selectBtn ||
            bookingCard == null)
            return false;

        AllyManager.instance.Add_AllyCard(typeIndex, bookingCard.currentId);

        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

        SetOff_ThisPanel();

        return true;
    }

    private bool Is_Interact_Reroll()
    {
        if (!DevTool.Can_CastingTType(currentBtn, out AllyCardRerollEUIController reroll))
            return false;

        reroll.Try_Interact();
        Set_NewCard(rerolls.IndexOf(reroll));
        if (bookingCard == reroll.targetCardEuiController)
        {
            bookingCard = null;
            cardBookingFrameImgRt.gameObject.SetActive(false);
        }
        SoundManager.instance.Play_2D_SFX_UI("Reroll");

        return true;
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();
        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

        Set_NewCardDeck();
    }

    #endregion

    #region Card

    private void Set_NewCard(int index)
    {
        List<int> idList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
            idList.Add(cards[i].currentId);

        AllyCardData cardData = AllyManager.instance.Get_ChoiceAbleRandomData(typeIndex, idList);

        cards[index].Set_Card(typeIndex, cardData);
    }

    private void Set_NewCardDeck()
    {
        List<AllyCardData> cardDeckData = AllyManager.instance.Get_ChoiceAbleRandomData(typeIndex, cards.Count);

        int needMoreDataAmount = cards.Count - cardDeckData.Count;
        if (needMoreDataAmount >= 0)
        {
            for (int i = 0; i < needMoreDataAmount; i++)
                cardDeckData.Add(null);
        }

        for (int i = 0; i < cards.Count; i++)
            cards[i].Set_Card(typeIndex, cardDeckData[i]);

        bookingCard = null;
        cardBookingFrameImgRt.gameObject.SetActive(false);
        Set_SelectingCard(cards[0]);
    }

    public void Set_SelectingCard(AllyCardEUIController allyCard)
    {
        if (selectingCard != allyCard)
        {
            if (selectingCard != null)
            {
                DevTool.Set_KillTween(selectingCard.gameObject.transform);
                selectingCard.gameObject.transform.DOScale(1f, 0.1f);
            }

            selectingCard = allyCard;

            DevTool.Set_KillTween(selectingCard.gameObject.transform);
            selectingCard.gameObject.transform.DOScale(1.05f, 0.1f);
        }
    }

    #endregion

    #region Select & Book

    private void Set_BookingCard(AllyCardEUIController cardEui)
    {
        bookingCard = cardEui;

        cardBookingFrameImgRt.anchoredPosition = DevTool.Get_ComponentTType<RectTransform>(bookingCard.gameObject).anchoredPosition;
        cardBookingFrameImgRt.gameObject.SetActive(true);

        Play_BookingRT();

        SoundManager.instance.Play_2D_SFX_UI("Click_01");
    }

    #endregion

    #region Play

    private void Play_BookingRT()
    {
        DevTool.Set_KillTween(cardBookingFrameImgRt);

        Sequence seq = DOTween.Sequence();

        for (int i = 10; i >= 0; i--)
            if (i % 2 == 0)
                seq.Append(cardBookingFrameImgRt.DOAnchorPosY(i * 4, 0.05f));
            else
                seq.Append(cardBookingFrameImgRt.DOAnchorPosY(-(i * 4), 0.05f));
    }

    #endregion

    #region Set (Language)

    private void Set_BaseLanguageTxt()
    {
        DevTool.Get_ComponentTType<TMP_Text>(cardBookingFrameImgRt.transform.GetChild(DevTool.Get_TSChildIndex(cardBookingFrameImgRt, 0)).gameObject).text =
            ResourceManager.instance.Get_StaticWord(82);

        DevTool.Get_ComponentTType<TMP_Text>(selectBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(selectBtn, 0)).gameObject).text =
            ResourceManager.instance.Get_StaticWord(83);

        for (int i = 0; i < rerolls.Count; i++)
            rerolls[i].Set_LanguageTxt();
    }

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        AllyManager.instance.Set_LanguageTxt();
        Set_BaseLanguageTxt();
    }

    #endregion
}
