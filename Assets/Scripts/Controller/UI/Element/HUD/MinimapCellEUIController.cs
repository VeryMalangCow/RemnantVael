using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCellEUIController : ElementUIController
{
    #region Value

    [HideInInspector] private Image ThisMMImg = null;
    [HideInInspector] private Image ThisMMOImg = null;
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

    }


    public void Offset(RoomController _RC, bool _IsNormal)
    {
        // From/To RC
        CoupleData<Sprite> thisSprites = _RC.MinimapElementIcon.Get_Base(_IsNormal);
        ref MinimapCellEUIController target = ref (_IsNormal ? ref _RC.ThisMME : ref _RC.ThisIMME);
        target = this;

        // Sprite
        ThisMMImg.sprite = thisSprites.TypeBase;
        ThisMMOImg.sprite = thisSprites.TypeSpecial;

        ThisMMImg.SetNativeSize();
        ThisMMOImg.SetNativeSize();

        // Pivot
        if (DevTool.Get_ComponentTType(gameObject, out RectTransform rt))
        {
            rt.pivot = _RC.SpritePivot;
            rt.anchoredPosition = new Vector2(
                    (float)_RC.RoomVec[0].x * IntervalMM.Get_Base(_IsNormal),
                    (float)_RC.RoomVec[0].y * IntervalMM.Get_Base(_IsNormal));
        }
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
