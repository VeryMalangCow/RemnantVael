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
    [SerializeField] private CanvasGroup successCg;
    [SerializeField] private CanvasGroup failureCg;
    [SerializeField] private TMP_Text tryUnlockTxt;
    [SerializeField] private TMP_Text inputTxt;
    [SerializeField] private Image inputImg;
    [SerializeField] private RectTransform rollingRt;

    #endregion

    #region - Hide

    // Txt
    [HideInInspector] private TMP_Text successTxt;
    [HideInInspector] private TMP_Text failureTxt;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        failureTxt = DevTool.Get_ComponentTType(failureCg.transform.GetChild(0).gameObject, out TMP_Text fTxt) ? fTxt : null;
        successTxt = DevTool.Get_ComponentTType(successCg.transform.GetChild(0).gameObject, out TMP_Text sTxt) ? sTxt : null;

        // Key Img
        inputImg.sprite = StaticResourceManager.instance.BuildReso.prisonPrefab.spaceBarSprite;
        inputImg.SetNativeSize();
    }

    #endregion

    #region Set

    public void Set_AllStart(bool canSuccess)
    {
        var prefab = StaticResourceManager.instance.BuildReso.prisonPrefab;
        var words = StaticResourceManager.instance.staticWords;

        DevTool.SetColor(prefab.lockedClr, failureTxt);
        DevTool.SetColor(PrisonBuild.unlockedClr, successTxt);

        tryUnlockTxt.text = words.GetLanguage(85);
        inputTxt.text = words.GetLanguage(88);
        successTxt.text = words.GetLanguage(86);
        failureTxt.text = words.GetLanguage(87);

        Play_LineSetChange(canSuccess);
    }


    public void Play_LineSetChange(bool canSuccess)
    {
        Set_SuccessPanel(canSuccess, 0.5f);
        Set_FailurePanel(canSuccess, 0.5f);

        Set_Roller(canSuccess, 1f);
    }

    private void Set_SuccessPanel(bool canSuccess, float durTime)
    {
        DevTool.SetKillTween(successCg);

        successCg.DOFade(canSuccess ? 1f : 0.3f, durTime);
    }

    private void Set_FailurePanel(bool canSuccess, float durTime)
    {
        DevTool.SetKillTween(failureCg);

        failureCg.DOFade(canSuccess ? 0.3f : 1f, durTime);
    }

    private void Set_Roller(bool canSuccess, float durTime)
    {
        DevTool.SetKillTween(rollingRt);

        float targetAngle = canSuccess ? 0 : 180;
        Quaternion endQuatValue = Quaternion.Euler(0f, 0f, targetAngle);
        rollingRt.DORotateQuaternion(endQuatValue, durTime).SetEase(Ease.OutElastic);
    }

    #endregion
}
