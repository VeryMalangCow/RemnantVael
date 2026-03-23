using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllyCardEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Image frameImg;
    [SerializeField] private Image lightImg;
    [SerializeField] private Image bgImg;
    [SerializeField] private Image bgMarkImg;
    [SerializeField] private Image iconImg;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text rankTxt;
    [SerializeField] private TMP_Text preNameTxt;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descTxt;


    [Space(10)]
    [Header("=== Other Comp")]
    [SerializeField] private CanvasGroup panelCg;

    // Value
    [HideInInspector] public int currentId;

    // Light
    [HideInInspector] private Sequence lightSeq;

    // Comp
    [HideInInspector] private RectTransform panelRt;

    // Owner
    [HideInInspector] public AllyCardUIController allyOwnerUIController;

    // Reroll
    [HideInInspector] public AllyCardRerollEUIController rerollEui;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        lightSeq = Play_LightSeq();
        lightSeq.Pause();

        panelRt = DevTool.Get_ComponentTType(panelCg.gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        lightSeq.Play();
    }

    private void OnDisable()
    {
        lightSeq.Pause();
    }

    #endregion

    #region Set

    public void Set_Card(int typeId, AllyCardData data)
    {
        if (data == null)
        {
            Set_CardBGMark(typeId);

            currentId = -1;

            Set_PanelAnim(0.5f);
            Set_SpriteNull();
            Set_TxtNull();

            gameObject.SetActive(true);
            rerollEui.gameObject.SetActive(true);
        }
        else
        {
            Set_CardBGMark(typeId);

            currentId = data.id;

            Set_PanelAnim(0.5f);
            Set_Sprite(typeId, data);
            Set_Txt(typeId, data);

            gameObject.SetActive(true);
            rerollEui.gameObject.SetActive(true);
        }
    }

    private void Set_PanelAnim(float durTime)
    {
        DevTool.Set_KillTween(panelCg);
        DevTool.Set_KillTween(panelRt);

        panelCg.alpha = 0;
        panelCg.DOFade(1f, durTime);

        panelRt.transform.localScale = Vector2.one * 1.2f;
        panelRt.transform.DOScale(1f, durTime);
    }

    private void Set_Sprite(int typeId, AllyCardData data)
    {
        frameImg.sprite = ResourceManager.instance.Get_AllyCardFrame(data.rank);
        lightImg.sprite = ResourceManager.instance.Get_AllyCardLight(data.rank);
        bgImg.sprite = ResourceManager.instance.Get_AllyCardBG(data.rank);

        iconImg.sprite = AllyManager.instance.Get_CardIcon(typeId, data.id);

        lightSeq.timeScale = data.rank + 1;
    }

    private void Set_SpriteNull()
    {
        frameImg.sprite = ResourceManager.instance.Get_AllyCardFrame(0);
        lightImg.sprite = ResourceManager.instance.Get_AllyCardFrame(0);
        bgImg.sprite = ResourceManager.instance.Get_AllyCardBG(0);

        iconImg.sprite = ResourceManager.instance.allyNullIcon;

        lightSeq.timeScale = 1;
    }

    private void Set_Txt(int typeId, AllyCardData data)
    {
        nameTxt.text = data.name.Replace("\\n", "\n");
        descTxt.text = data.desc.Replace("\\n", "\n");

        rankTxt.text = ResourceManager.instance.allyCardRateArr[data.rank];
        rankTxt.color = ResourceManager.instance.Get_AllyCardColor(data.rank);
        bgImg.color = ResourceManager.instance.Get_AllyCardColor(data.rank);

        AllyCardData preCardData = AllyManager.instance.Get_PreAllyCardData(typeId, data);
        preNameTxt.text = preCardData != null ? $"-({preCardData.name})->" : "";
        preNameTxt.gameObject.SetActive(preCardData != null);
    }

    private void Set_TxtNull()
    {
        nameTxt.text = "NULL";
        descTxt.text = "NULL";

        rankTxt.text = "NULL";
        rankTxt.color = ResourceManager.instance.Get_AllyCardColor(0);
        bgImg.color = ResourceManager.instance.Get_AllyCardColor(0);

        preNameTxt.gameObject.SetActive(false);
    }

    private void Set_CardBGMark(int typeId)
    {
        switch(typeId)
        {
            case 0:
                bgMarkImg.sprite = ResourceManager.instance.Get_STPrisonIcon(false);
                break;
            case 1:
                bgMarkImg.sprite = ResourceManager.instance.Get_UTPrisonIcon(false);
                break;
            case 2:
                bgMarkImg.sprite = ResourceManager.instance.Get_NTPrisonIcon(false);
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

        DevTool.Set_AlphaColor(lightImg, 1);

        seq.Append(lightImg.DOFade(0.2f, 5f).SetEase(Ease.Linear));
        seq.Join(rankTxt.DOFade(0.2f, 5f).SetEase(Ease.Linear));
        seq.Append(lightImg.DOFade(1f, 0.5f).SetEase(Ease.Linear));
        seq.Join(rankTxt.DOFade(1f, 0.5f).SetEase(Ease.Linear));

        seq.SetLoops(-1, LoopType.Restart);

        return seq;
    }


    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!isCanSelect || btn == null || !btn.interactable) return;

        if (ownerUIController != null) allyOwnerUIController.Set_SelectingCard(this);
    }

    #endregion
}
