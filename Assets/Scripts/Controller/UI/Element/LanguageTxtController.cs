using TMPro;
using UnityEngine;

public class LanguageTxtController : MonoBehaviour
{
    #region Value

    [SerializeField] private int type = 0;

    #endregion

    #region Offset

    private void Offset(int languageID)
    {
        Set_Font(languageID);
    }

    #endregion

    #region Awake

    private void Start()
    {
        Offset(GameManager.languageID);
        ResourceManager.instance.Add_LanguageTxt(this);
    }

    #endregion

    #region Set

    public void Set_Font(int languageID)
    {
        if (DevTool.Get_ComponentTType(gameObject, out TMP_Text txt))
            txt.font = ResourceManager.instance.languageTxtArr[languageID].fontAssets[type];
    }

    #endregion
}
