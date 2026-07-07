using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FontResoSO", menuName = "ScriptableObject/FontResoSO")]
public class LanguageResoSO : ScriptableObject
{
    public static int languageAmount = 2;
    public TextAsset staticWordCsv;
    public TextAsset staticDescCsv;

    public LanguageTxt[] LanguageTxts;
}

[System.Serializable]
public class LanguageTxt
{
    public int id;
    public string name;

    public TMP_FontAsset[] fontAssets;
}