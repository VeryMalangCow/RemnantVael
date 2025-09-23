using TMPro;
using UnityEngine;
using UniRx;

public class AllyHUDController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> HUD")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public Canvas ThisCanvas;
    [SerializeField] public AllyStateUIController StateUI;
    [SerializeField] public AllyBuffUIController TemporaryBuffUI;
    [SerializeField] public AllyBuffUIController PermanentBuffUI;
    [SerializeField] public AllyRequestUIController RequestUI;

    [SerializeField] private CanvasGroup BaseCG;
    [SerializeField] private CanvasGroup RequestCG;

    [Space(10)]
    [Header("=== Name")]
    [SerializeField] private TMP_Text NameTxt;

    #endregion

    #region - Hide


    #endregion

    #endregion

    #region Offset

    public void Offset()
    {
        StateUI.Offset(this);
        TemporaryBuffUI.Offset(this);
        PermanentBuffUI.Offset(this);
        RequestUI.Offset(this);

        Offset_Subscribe();
    }

    public void Offset_Subscribe()
    {
        MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInteracted.Subscribe(_Value =>
            {
                BaseCG.alpha = _Value ? 0f : 1f;
                RequestCG.alpha = _Value ? 1f : 0f;
            });
    }

    #endregion

    #region Name

    public void Set_Name(string _Name)
    {
        NameTxt.text = _Name;
    }

    #endregion
}
