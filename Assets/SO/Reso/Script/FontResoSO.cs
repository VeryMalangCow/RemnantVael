using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FontResoSO", menuName = "ScriptableObject/FontResoSO")]
public class FontResoSO : ScriptableObject
{
    public LanguageTxt[] LanguageTxts;
}

[System.Serializable]
public class LanguageTxt
{
    public int id;
    public string name;

    public TMP_FontAsset[] fontAssets;
}