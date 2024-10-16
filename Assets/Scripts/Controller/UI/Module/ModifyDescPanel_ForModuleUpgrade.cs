using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ModifyDescPanel_ForModuleUpgrade : UIModule
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Desc _ For ModuleUpgrade")]

    [Space(10)]
    [Header("=== Rank")]
    [SerializeField] private Image CurrentRankImg;


    [HideInInspector] private RectTransform ThisRT;
    [HideInInspector] private float OriginalHeight;
    [HideInInspector] private CanvasGroup ThisCG;

    #endregion

    #region Offset

    public override void Offset()
    {
        if (ThisRT == null && this.TryGetComponent(out RectTransform rt))
        {
            ThisRT = rt;
            OriginalHeight = ThisRT.rect.height;

            ThisRT.sizeDelta = new Vector2(ThisRT.sizeDelta.x, 0f);
        }
        if (ThisCG == null && this.TryGetComponent(out CanvasGroup cg))
        {
            ThisCG = cg;

            ThisCG.alpha = 0f;
        }
    }

    #endregion

    #region Open / Close

    public void OpenThisPanel(float _DurTime)
    {
        ThisRT.DOSizeDelta(new Vector2(ThisRT.sizeDelta.x, OriginalHeight), _DurTime);
        ThisCG.DOFade(1f, _DurTime);
    }

    public void CloseThisPanel(float _DurTime)
    {
        ThisRT.DOSizeDelta(new Vector2(ThisRT.sizeDelta.x, 0f), _DurTime);
        ThisCG.DOFade(0f, _DurTime);
    }

    #endregion

    #region Desc

    public void SetDesc()
    {

    }

    #endregion
}
