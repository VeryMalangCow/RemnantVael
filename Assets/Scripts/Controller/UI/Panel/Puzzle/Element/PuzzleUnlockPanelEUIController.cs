using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUnlockPanelEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Puzzle Unlock")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private CanvasGroup SuccessCG;
    [SerializeField] private CanvasGroup FailureCG;
    [SerializeField] private TMP_Text TryUnlockTxt;
    [SerializeField] private TMP_Text InputTxt;
    [SerializeField] private Image InputImg;
    [SerializeField] private RectTransform RollingRT;

    #endregion

    #region - Hide

    // Txt
    [HideInInspector] private TMP_Text SuccessTxt;
    [HideInInspector] private TMP_Text FailureTxt;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        FailureTxt = DevTool.Get_ComponentTType(FailureCG.transform.GetChild(0).gameObject, out TMP_Text fTxt) ? fTxt : null;
        SuccessTxt = DevTool.Get_ComponentTType(SuccessCG.transform.GetChild(0).gameObject, out TMP_Text sTxt) ? sTxt : null;

        // Key Img
        InputImg.sprite = UnitManager.Instance.SpaceBarSprite;
        InputImg.SetNativeSize();
    }

    #endregion

    #region Set

    public void Set_AllStart(bool _CanSuccess)
    {
        DevTool.Set_Color(UnitManager.Instance.LockedClr, FailureTxt);
        DevTool.Set_Color(UnitManager.Instance.UnlockedClr, SuccessTxt);

        TryUnlockTxt.text = CSVManager.Instance.Get_StaticWord(85);
        InputTxt.text = CSVManager.Instance.Get_StaticWord(88);
        SuccessTxt.text = CSVManager.Instance.Get_StaticWord(86);
        FailureTxt.text = CSVManager.Instance.Get_StaticWord(87);

        Play_LineSetChange(_CanSuccess);
    }


    public void Play_LineSetChange(bool _CanSuccess)
    {
        Set_SuccessPanel(_CanSuccess, 0.5f);
        Set_FailurePanel(_CanSuccess, 0.5f);

        Set_Roller(_CanSuccess, 1f);
    }

    private void Set_SuccessPanel(bool _CanSuccess, float _DurTime)
    {
        DevTool.Set_KillTween(SuccessCG);

        SuccessCG.DOFade(_CanSuccess ? 1f : 0.3f, _DurTime);
    }

    private void Set_FailurePanel(bool _CanSuccess, float _DurTime)
    {
        DevTool.Set_KillTween(FailureCG);

        FailureCG.DOFade(_CanSuccess ? 0.3f : 1f, _DurTime);
    }

    private void Set_Roller(bool _CanSuccess, float _DurTime)
    {
        DevTool.Set_KillTween(RollingRT);

        float targetAngle = _CanSuccess ? 0 : 180;
        Quaternion endQuatValue = Quaternion.Euler(0f, 0f, targetAngle);
        RollingRT.DORotateQuaternion(endQuatValue, _DurTime).SetEase(Ease.OutElastic);
    }

    #endregion
}
