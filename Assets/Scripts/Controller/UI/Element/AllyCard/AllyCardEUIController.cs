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
    [SerializeField] private Image FrameImg;
    [SerializeField] private Image LightImg;
    [SerializeField] private Image BGImg;
    [SerializeField] private Image BGMarkImg;
    [SerializeField] private Image IconImg;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text RankTxt;
    [SerializeField] private TMP_Text PreNameTxt;
    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text DescTxt;


    [Space(10)]
    [Header("=== Other Comp")]
    [SerializeField] private CanvasGroup ThisPanelCG;

    // Value
    [HideInInspector] public int CurrentID;

    // Light
    [HideInInspector] private Sequence LightSeq;

    // Comp
    [HideInInspector] private RectTransform ThisPanelRT;

    // Owner
    [HideInInspector] public AllyCardUIController AllyOwnerUIController;

    // Reroll
    [HideInInspector] public AllyCardRerollEUIController RerollEUI;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        LightSeq = Play_LightSeq();
        LightSeq.Pause();

        ThisPanelRT = DevTool.Get_ComponentTType(ThisPanelCG.gameObject, out RectTransform rt) ? rt : null;
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
        if (_Data == null)
        {
            Set_CardBGMark(_TypeID);

            CurrentID = -1;

            Set_PanelAnim(0.5f);
            Set_SpriteNull();
            Set_TxtNull();

            gameObject.SetActive(true);
            RerollEUI.gameObject.SetActive(true);
        }
        else
        {
            Set_CardBGMark(_TypeID);

            CurrentID = _Data.id;

            Set_PanelAnim(0.5f);
            Set_Sprite(_TypeID, _Data);
            Set_Txt(_TypeID, _Data);

            gameObject.SetActive(true);
            RerollEUI.gameObject.SetActive(true);
        }
    }

    private void Set_PanelAnim(float _DurTime)
    {
        DevTool.Set_KillTween(ThisPanelCG);
        DevTool.Set_KillTween(ThisPanelRT);

        ThisPanelCG.alpha = 0;
        ThisPanelCG.DOFade(1f, _DurTime);

        ThisPanelRT.transform.localScale = Vector2.one * 1.2f;
        ThisPanelRT.transform.DOScale(1f, _DurTime);
    }

    private void Set_Sprite(int _TypeID, AllyCardData _Data)
    {
        FrameImg.sprite = ResourceManager.instance.Get_AllyCardFrame(_Data.rank);
        LightImg.sprite = ResourceManager.instance.Get_AllyCardLight(_Data.rank);
        BGImg.sprite = ResourceManager.instance.Get_AllyCardBG(_Data.rank);

        IconImg.sprite = AllyManager.instance.Get_CardIcon(_TypeID, _Data.id);

        LightSeq.timeScale = _Data.rank + 1;
    }

    private void Set_SpriteNull()
    {
        FrameImg.sprite = ResourceManager.instance.Get_AllyCardFrame(0);
        LightImg.sprite = ResourceManager.instance.Get_AllyCardFrame(0);
        BGImg.sprite = ResourceManager.instance.Get_AllyCardBG(0);

        IconImg.sprite = ResourceManager.instance.allyNullIcon;

        LightSeq.timeScale = 1;
    }

    private void Set_Txt(int _TypeID, AllyCardData _Data)
    {
        NameTxt.text = _Data.name.Replace("\\n", "\n");
        DescTxt.text = _Data.desc.Replace("\\n", "\n");

        RankTxt.text = ResourceManager.instance.allyCardRateArr[_Data.rank];
        RankTxt.color = ResourceManager.instance.Get_AllyCardColor(_Data.rank);
        BGImg.color = ResourceManager.instance.Get_AllyCardColor(_Data.rank);

        AllyCardData preCardData = AllyManager.instance.Get_PreAllyCardData(_TypeID, _Data);
        PreNameTxt.text = preCardData != null ? $"-({preCardData.name})->" : "";
        PreNameTxt.gameObject.SetActive(preCardData != null);
    }

    private void Set_TxtNull()
    {
        NameTxt.text = "NULL";
        DescTxt.text = "NULL";

        RankTxt.text = "NULL";
        RankTxt.color = ResourceManager.instance.Get_AllyCardColor(0);
        BGImg.color = ResourceManager.instance.Get_AllyCardColor(0);

        PreNameTxt.gameObject.SetActive(false);
    }

    private void Set_CardBGMark(int _TypeID)
    {
        switch(_TypeID)
        {
            case 0:
                BGMarkImg.sprite = ResourceManager.instance.Get_STPrisonIcon(false);
                break;
            case 1:
                BGMarkImg.sprite = ResourceManager.instance.Get_UTPrisonIcon(false);
                break;
            case 2:
                BGMarkImg.sprite = ResourceManager.instance.Get_NTPrisonIcon(false);
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
