using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCellEUIController : ElementUIController
{
    #region Value

    [HideInInspector] private Image mmImg = null;
    [HideInInspector] private Image mmoImg = null;
    [HideInInspector] private Image iconImg = null;
    [HideInInspector] private CanvasGroup cg = null;

    [SerializeField] private Color unknowClr;
    [SerializeField] private CoupleData<float> intervalMm = new CoupleData<float>(36, 60);

    #endregion

    #region Offset

    public override void Offset()
    {
        mmImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;
        cg = DevTool.Get_ComponentTType(gameObject, out CanvasGroup _cg) ? _cg : null;
        cg.alpha = 0f;
        mmoImg = DevTool.Get_ComponentTType(transform.GetChild(0).gameObject, out Image outlineImg) ? outlineImg : null;
        iconImg = DevTool.Get_ComponentTType(transform.GetChild(1).gameObject, out Image _iconImg) ? _iconImg : null;
    }


    public void Offset(RoomController room, bool isNormal)
    {
        // From/To RCs
        MinimapIcon minimapReso = StaticResourceManager.instance.StageReso.minimapIcons[room.roomTypeId];

        CoupleData<Sprite> thisSprites = minimapReso.minimapElementIcon.Get_Base(isNormal);
        ref MinimapCellEUIController target = ref (isNormal ? ref room.thisMME : ref room.thisIMME);
        target = this;

        // Sprite
        mmImg.sprite = thisSprites.typeBase;
        mmoImg.sprite = thisSprites.typeSpecial;

        mmImg.SetNativeSize();
        mmoImg.SetNativeSize();

        // Pivot
        if (DevTool.Get_ComponentTType(gameObject, out RectTransform rt))
        {
            rt.pivot = minimapReso.spritePivot;
            rt.anchoredPosition = new Vector2(
                room.RoomVecWorld[0].x * intervalMm.Get_Base(isNormal),
                room.RoomVecWorld[0].y * intervalMm.Get_Base(isNormal));
        }
        CoupleData<Sprite> sprite = StaticResourceManager.instance.StageReso.GetMinimapIcon(room.roomRule);

        if (sprite != null)
        {
            iconImg.sprite = sprite.Get_Base(isNormal);
            iconImg.SetNativeSize();
        }
        iconImg.gameObject.SetActive(sprite != null ? true : false);
    }

    #endregion

    #region Set

    // 완료된 방
    public void Set_Complete(Color mainClr)
    {
        mmoImg.DOColor(mainClr, 0.5f);
    }

    // 완료되지 않은 방
    public void Set_Uncomplete()
    {
        mmoImg.DOColor(unknowClr, 0.5f);
    }

    // 보이는 방 (접근된 방)
    public void Set_Visible()
    {
        mmoImg.color = new Color(0, 0, 0, 0.5f);
    }

    // 활성화
    public void Set_ActiveOn()
    {
        this.gameObject.SetActive(true);
        cg.DOFade(1f, 0.5f);
    }

    #endregion
}
