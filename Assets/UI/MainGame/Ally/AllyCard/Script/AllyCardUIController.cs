using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class AllyCardUIController : SinglePanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Card UI")]

    [SerializeField] private AllyCardEUIController cardPrefab;
    private AllyCardEUIController[] cards = new AllyCardEUIController[3];
    [SerializeField] private Transform cardParentTf;

    [SerializeField] private AllyCardRerollEUIController rerollPrefab;
    private AllyCardRerollEUIController[] rerolls = new AllyCardRerollEUIController[3];
    [SerializeField] private Transform rerollParentTf;

    [Space(5)]
    [SerializeField] private float intervalX = 668f;

    [Space(10)]
    [SerializeField] private RectTransform cardBookingFrameImgRt;
    [SerializeField] private TMP_Text cardBookingTxt;
    [SerializeField] private OwnBtnEUIController selectBtn;
    [SerializeField] private TMP_Text selectTxt;

    // Select & Booking
    private AllyCardEUIController selectingCard;
    private AllyCardEUIController bookingCard;

    // Type
    [HideInInspector] public int typeIndex = 0;

    #endregion

    #region Init

    public IEnumerator InitAsync()
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
        string s = "";
#endif
        for (int i = 0; i < 3; i++)
        {
            cards[i] = Instantiate(cardPrefab, cardParentTf);
            cards[i].gameObject.SetActive(false);
            cards[i].Offset();
            cards[i].ownerUIController = this;
            cards[i].allyOwnerUIController = this;
            cards[i].rt.anchoredPosition = new Vector2((intervalX * i) - intervalX, 0);

            rerolls[i] = Instantiate(rerollPrefab, rerollParentTf);
            rerolls[i].gameObject.SetActive(false);
            rerolls[i].Offset();
            rerolls[i].ownerUIController = this;
            rerolls[i].rt.anchoredPosition = new Vector2((intervalX * i) - intervalX, 0);

            rerolls[i].targetCardEuiController = cards[i];
            cards[i].rerollEui = rerolls[i];
#if UNITY_EDITOR
            sw.Stop();
            s += $"{sw.Elapsed.TotalMilliseconds:F2} /";
#endif
            yield return null;
#if UNITY_EDITOR
            sw.Restart();
#endif
        }
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Ally Card : <color=yellow>Gen Card & Reroll</color> : <color=red>{s}</color> ms");
#endif
        yield return null;

        selectBtn.Offset();
        selectBtn.ownerUIController = this;
        SetBaseLanguageTxt();

        gameObject.SetActive(false);

        yield return null;
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

        SoundManager.instance.PlayUiSfx("Approve");

        SetOff_ThisPanel();

        return true;
    }

    private bool Is_Interact_Reroll()
    {
        if (!DevTool.Can_CastingTType(currentBtn, out AllyCardRerollEUIController reroll))
            return false;

        if (!reroll.TryInteract())
            return false;

        int index = -1;
        for (int i = 0; i < rerolls.Length; i++)
        {
            if (rerolls[i] == reroll)
                index = i;
        }
        if (index == -1) 
            return false;

        Set_NewCard(index);
        if (bookingCard == reroll.targetCardEuiController)
        {
            bookingCard = null;
            cardBookingFrameImgRt.gameObject.SetActive(false);
        }
        SoundManager.instance.PlayUiSfx("Reroll");

        return true;
    }

    #endregion

    #region Panel

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();
        SoundManager.instance.PlayUiSfx("Approve");

        Set_NewCardDeck();
    }

    #endregion

    #region Card

    private void Set_NewCard(int index)
    {
        List<int> idList = new List<int>();
        for (int i = 0; i < cards.Length; i++)
            idList.Add(cards[i].currentId);

        AllyCardData cardData = AllyManager.instance.Get_ChoiceAbleRandomData(typeIndex, idList);

        cards[index].Set_Card(typeIndex, cardData);
    }

    private void Set_NewCardDeck()
    {
        List<AllyCardData> cardDeckData = AllyManager.instance.Get_ChoiceAbleRandomData(typeIndex, cards.Length);

        int needMoreDataAmount = cards.Length - cardDeckData.Count;
        if (needMoreDataAmount >= 0)
        {
            for (int i = 0; i < needMoreDataAmount; i++)
                cardDeckData.Add(null);
        }

        for (int i = 0; i < cards.Length; i++)
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
                DevTool.SetKillTween(selectingCard.gameObject.transform);
                selectingCard.gameObject.transform.DOScale(1f, 0.1f);
            }

            selectingCard = allyCard;

            DevTool.SetKillTween(selectingCard.gameObject.transform);
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

        SoundManager.instance.PlayUiSfx("Click01");
    }

    #endregion

    #region Play

    private void Play_BookingRT()
    {
        DevTool.SetKillTween(cardBookingFrameImgRt);

        Sequence seq = DOTween.Sequence();

        for (int i = 10; i >= 0; i--)
            if (i % 2 == 0)
                seq.Append(cardBookingFrameImgRt.DOAnchorPosY(i * 4, 0.05f));
            else
                seq.Append(cardBookingFrameImgRt.DOAnchorPosY(-(i * 4), 0.05f));
    }

    #endregion

    #region Set (Language)

    private void SetBaseLanguageTxt()
    {
        cardBookingTxt.SetText(ResourceManager.instance.Get_StaticWord(82));
        selectTxt.SetText(ResourceManager.instance.Get_StaticWord(83));

        for (int i = 0; i < rerolls.Length; i++)
            rerolls[i].Set_LanguageTxt();
    }

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        AllyManager.instance.Set_LanguageTxt();
        SetBaseLanguageTxt();
    }

    #endregion
}
