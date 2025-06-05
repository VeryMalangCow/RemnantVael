using TMPro;
using UnityEngine;

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
    }

    #endregion

    #region Name

    public void Set_Name(string _Name)
    {
        NameTxt.text = _Name;
    }

    #endregion
}
