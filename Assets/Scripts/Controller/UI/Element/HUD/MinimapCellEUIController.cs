using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCellEUIController : ElementUIController
{
    #region Value

    [HideInInspector] private Image ThisMMImg = null;
    [HideInInspector] private Image ThisMMOImg = null;
    [HideInInspector] private Image ThisIconImg = null;
    [HideInInspector] private CanvasGroup ThisCG = null;

    [SerializeField] private Color UnknowColor;
    [SerializeField] private CoupleData<float> IntervalMM = new CoupleData<float>(36, 60);

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisMMImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;
        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;
        ThisCG.alpha = 0f;
        ThisMMOImg = DevTool.Get_ComponentTType(transform.GetChild(0).gameObject, out Image outlineImg) ? outlineImg : null;
        ThisIconImg = DevTool.Get_ComponentTType(transform.GetChild(1).gameObject, out Image iconImg) ? iconImg : null;
    }


    public void Offset(RoomController _Room, bool _IsNormal)
    {
        // From/To RC
        MinimapIcon minimapReso = ResourceManager.instance.Get_MinimapIcon(_Room.roomStaticId);

        CoupleData<Sprite> thisSprites = minimapReso.minimapElementIcon.Get_Base(_IsNormal);
        ref MinimapCellEUIController target = ref (_IsNormal ? ref _Room.thisMME : ref _Room.thisIMME);
        target = this;

        // Sprite
        ThisMMImg.sprite = thisSprites.typeBase;
        ThisMMOImg.sprite = thisSprites.typeSpecial;

        ThisMMImg.SetNativeSize();
        ThisMMOImg.SetNativeSize();

        // Pivot
        if (DevTool.Get_ComponentTType(gameObject, out RectTransform rt))
        {
            rt.pivot = minimapReso.spritePivot;
            rt.anchoredPosition = new Vector2(
                    (float)_Room.roomVec[0].x * IntervalMM.Get_Base(_IsNormal),
                    (float)_Room.roomVec[0].y * IntervalMM.Get_Base(_IsNormal));
        }
        CoupleData<Sprite> sprite = StageManager.instance.Get_CorrectMinimapIcon(_Room.roomRule);

        if (sprite != null)
        {
            ThisIconImg.sprite = sprite.Get_Base(_IsNormal);
            ThisIconImg.SetNativeSize();
        }
        ThisIconImg.gameObject.SetActive(sprite != null ? true : false);
    }

    #endregion

    #region Set

    // 완료된 방
    public void Set_Complete(Color _MainColor)
    {
        ThisMMOImg.DOColor(_MainColor, 0.5f);
    }

    // 완료되지 않은 방
    public void Set_Uncomplete()
    {
        ThisMMOImg.DOColor(UnknowColor, 0.5f);
    }

    // 보이는 방 (접근된 방)
    public void Set_Visible()
    {
        ThisMMOImg.color = new Color(0, 0, 0, 0.5f);
    }

    // 활성화
    public void Set_ActiveOn()
    {
        this.gameObject.SetActive(true);
        ThisCG.DOFade(1f, 0.5f);
    }

    #endregion
}
