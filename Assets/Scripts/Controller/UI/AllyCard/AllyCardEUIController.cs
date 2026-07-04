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
        DevTool.SetKillTween(panelCg);
        DevTool.SetKillTween(panelRt);

        panelCg.alpha = 0;
        panelCg.DOFade(1f, durTime);

        panelRt.transform.localScale = Vector2.one * 1.2f;
        panelRt.transform.DOScale(1f, durTime);
    }

    private void Set_Sprite(int typeId, AllyCardData data)
    {
        var set = StaticResourceManager.instance.AllyReso.allyCardSpriteSets[data.rank];
        frameImg.sprite = set.frame;
        lightImg.sprite = set.light;
        bgImg.sprite = set.bg;

        iconImg.sprite = AllyManager.instance.Get_CardIcon(typeId, data.id);

        lightSeq.timeScale = data.rank + 1;
    }

    private void Set_SpriteNull()
    {
        var set = StaticResourceManager.instance.AllyReso.allyCardSpriteSets[0];
        frameImg.sprite = set.frame;
        lightImg.sprite = set.light;
        bgImg.sprite = set.bg;

        iconImg.sprite = StaticResourceManager.instance.AllyReso.allyNullIcon; 

        lightSeq.timeScale = 1;
    }

    private void Set_Txt(int typeId, AllyCardData data)
    {
        nameTxt.text = data.name.Replace("\\n", "\n");
        descTxt.text = data.desc.Replace("\\n", "\n");

        var clr = StaticResourceManager.instance.AllyReso.allyCardSpriteSets[data.rank].clr;

        rankTxt.text = ResourceManager.instance.allyCardRateArr[data.rank];
        rankTxt.color = clr;
        bgImg.color = clr;

        AllyCardData preCardData = AllyManager.instance.Get_PreAllyCardData(typeId, data);
        preNameTxt.text = preCardData != null ? $"-({preCardData.name})->" : "";
        preNameTxt.gameObject.SetActive(preCardData != null);
    }

    private void Set_TxtNull()
    {
        nameTxt.text = "NULL";
        descTxt.text = "NULL";

        rankTxt.text = "NULL";
        var clr = StaticResourceManager.instance.AllyReso.allyCardSpriteSets[0].clr;
        rankTxt.color = clr;
        bgImg.color = clr;

        preNameTxt.gameObject.SetActive(false);
    }

    private void Set_CardBGMark(int typeId)
    {
        bgMarkImg.sprite = StaticResourceManager.instance.BuildReso.prisonPrefab.builds[typeId].teamIcon.typeSpecial;
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
