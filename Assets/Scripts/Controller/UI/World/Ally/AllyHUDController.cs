using TMPro;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.Serialization;

public class AllyHUDController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> HUD")]

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("ThisCanvas")][SerializeField] public Canvas canvas;
    [FormerlySerializedAs("StateUI")][SerializeField] public AllyStateUIController stateUi;
    [FormerlySerializedAs("TemporaryBuffUI")][SerializeField] public AllyBuffUIController temporaryBuffUi;
    [FormerlySerializedAs("PermanentBuffUI")][SerializeField] public AllyBuffUIController permanentBuffUi;
    [FormerlySerializedAs("RequestUI")][SerializeField] public AllyRequestUIController requestUi;
    [FormerlySerializedAs("IconRT")][SerializeField] private RectTransform iconRt;

    [FormerlySerializedAs("BaseCG")][SerializeField] private CanvasGroup baseCg;
    [FormerlySerializedAs("RequestCG")][SerializeField] private CanvasGroup requestCg;

    [Space(10)]
    [Header("=== Name")]
    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;

    #endregion

    #region - Hide


    #endregion

    #endregion

    #region Offset

    public void Offset()
    {
        stateUi.Offset(this);
        temporaryBuffUi.Offset(this);
        permanentBuffUi.Offset(this);
        requestUi.Offset(this);

        Offset_Subscribe();
    }

    public void Offset_Subscribe()
    {
        MainGameUIManager.instance.playerHUD_UIController.isTabInteracted.Subscribe(value =>
            {
                baseCg.alpha = value ? 0f : 1f;
                requestCg.alpha = value ? 1f : 0f;
            });
    }

    #endregion

    #region Name

    public void Set_Name(string name)
    {
        nameTxt.text = name;
    }

    #endregion

    #region ActiveFX

    public void Play_IconRT()
    {
        DevTool.Set_KillTween(iconRt);

        Sequence seq = DOTween.Sequence();
        seq.Append(iconRt.DOScale(1.25f, 0.1f));
        seq.Append(iconRt.DOScale(1f, 0.1f));
    }

    #endregion
}
