using TMPro;
using UnityEngine;

public class LanguageTxtController : MonoBehaviour
{
    #region Value

    [SerializeField] private int Type = 0;

    #endregion

    #region Offset

    private void Offset(int _LanguageID)
    {
        Set_Font(_LanguageID);
    }

    #endregion

    #region Awake

    private void Start()
    {
        Offset(GameManager.LanguageID);
        UnitManager.Instance.Add_LanguageTxt(this);
    }

    #endregion

    #region Set

    public void Set_Font(int _LanguageID)
    {
        if (DevTool.Get_ComponentTType(gameObject, out TMP_Text txt))
            txt.font = UnitManager.Instance.LanguageTxtList[_LanguageID].FontAssets[Type];
    }

    #endregion
}
