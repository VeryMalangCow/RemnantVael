using TMPro;
using UnityEngine;

public class LanguageTxtController : MonoBehaviour
{
    #region Value

    [SerializeField] private int type = 0;

    #endregion

    #region Awake

    private void Start()
    {
        ResourceManager.instance.Add_LanguageTxt(this);
    }

    #endregion

    #region Set

    public void Set_Font(int languageID)
    {
        if (DevTool.Get_ComponentTType(gameObject, out TMP_Text txt))
            txt.font = StaticResourceManager.instance.LanguageFontReso.LanguageTxts[languageID].fontAssets[type];
    }

    #endregion
}
