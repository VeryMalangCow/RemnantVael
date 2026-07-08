using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager : PersistentSingleton<ResourceManager>
{
    #region Language

    // 언어 변경을 위한 컴포넌트
    [HideInInspector] private HashSet<LanguageTxtController> allLanguageTxtControllers = new HashSet<LanguageTxtController>();
    [HideInInspector] public HashSet<PrisonController> allPrisons = new HashSet<PrisonController>();

    public void Add_LanguageTxt(LanguageTxtController langTxt)
    {
        allLanguageTxtControllers.Add(langTxt);
    }

    public void Clear_LanguageTxt()
    {
        allLanguageTxtControllers.Clear();
    }

    public void SetLanguageFont(int langID)
    {
        if (GameManager.languageID == langID) return;
        GameManager.languageID = langID;
        SaveDataManager.instance.jsonData.optionData.languageID = GameManager.languageID;
        SetLanguageFont();
    }


    private void SetLanguageFont()
    {
        // Change Font Asset
        foreach (LanguageTxtController ltc in allLanguageTxtControllers)
            ltc.Set_Font(GameManager.languageID);

        SetLanguage();
    }

    private void SetLanguage()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        // Change UI
        if (sceneName == "MainGame")
        {
            // UI
            MainGameUIManager.instance.SetLanguageTxt();

            // Ally
            AllyManager.instance.Set_Language();

            // Change PrisonInfo
            foreach (PrisonController prison in allPrisons)
                prison.Set_LanguageTxt();
        }
        else if (sceneName == "TitleLobby")
        {
            // UI
            TitleLobbyUIManager.instance.Set_LanguageTxt();
        }
    }


    #endregion

}