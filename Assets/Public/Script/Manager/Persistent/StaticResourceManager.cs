using System.Collections.Generic;
using UnityEngine;

public class StaticResourceManager : PersistentSingleton<StaticResourceManager>
{
    [SerializeField] private StageResoSO stageReso;
    [SerializeField] private BuildResoSO buildReso;
    [SerializeField] private EnemyResoSO enemyReso;
    [SerializeField] private AllyResoSO allyReso;
    [SerializeField] private ExplosionResoSO explosionReso;
    [SerializeField] private ItemResoSO itemReso;
    [SerializeField] private EventResoSO eventReso;
    [SerializeField] private SoundResoSO soundReso;
    [SerializeField] private LanguageResoSO languageFontReso;

    public StageResoSO StageReso => stageReso;
    public BuildResoSO BuildReso => buildReso;
    public EnemyResoSO EnemyReso => enemyReso;
    public AllyResoSO AllyReso => allyReso;
    public ExplosionResoSO ExplosionReso => explosionReso;
    public ItemResoSO ItemReso => itemReso;
    public EventResoSO EventReso => eventReso;
    public SoundResoSO SoundReso => soundReso;
    public LanguageResoSO LanguageFontReso => languageFontReso;


    // CSV

    public LanguageSet staticWords { get; private set; }
    public LanguageSet staticDescs { get; private set; }
    public LanguageSet randomNames { get; private set; }
    public LanguageColorSet properNouns { get; private set; }
    public LanguageSet enemyNames { get; private set; }

    public LanguageSet infoNames { get; private set; }
    public LanguageSet infoDescs { get; private set; }
    public LanguageSet infoKeys { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        staticWords = CSVReader.GetLanguageSet(languageFontReso.staticWordCsv);
        staticDescs = CSVReader.GetLanguageSet(languageFontReso.staticDescCsv);
        randomNames = CSVReader.GetLanguageSet(languageFontReso.randomNameCsv);
        properNouns = CSVReader.GetLanguageColorSet(languageFontReso.properNounCsv);
        enemyNames = CSVReader.GetLanguageSet(enemyReso.enemyNameCsv);

        infoNames = CSVReader.GetLanguageSet(EventReso.infoNameCsv);
        infoDescs = CSVReader.GetLanguageSet(EventReso.infoDescCsv);
        infoKeys = CSVReader.GetLanguageSet(EventReso.infoKeyCsv);
    }
}

// Language
public class LanguageSet
{
    protected Dictionary<int, string[]> allLanguage;

    public LanguageSet(Dictionary<int, string[]> allLanguage)
    { 
        this.allLanguage = allLanguage; 
    }

    public string GetLanguage(int id)
    {
        if (allLanguage.ContainsKey(id))
            return allLanguage[id][GameManager.languageID];

        return "";
    }

    public string[] GetLanguages(int id)
    {
        if (allLanguage.ContainsKey(id))
            return allLanguage[id];

        return null;
    }

    public int GetAmount()
        => allLanguage.Count;
    
}

// language + Color(Hex)
public class LanguageColorSet
{
    protected Dictionary<int, ColorStringArray> allLanguage;

    public LanguageColorSet(Dictionary<int, ColorStringArray> dict)
    {
        allLanguage = dict;
    }

    public string GetLanguage(int id)
    {
        if (allLanguage.ContainsKey(id))
        {
            ColorStringArray data = allLanguage[id];
            return $"<color=#{data.clrHex}><b>\"{data.strings[GameManager.languageID]}\"</color></b>";
        }

        return "";
    }
}

public class ColorStringArray
{
    public string clrHex;
    public string[] strings;

    public ColorStringArray(string clrHex, string[] strings)
    {
        this.clrHex = clrHex;
        this.strings = strings;
    }
}






public abstract class WordSet<T>
{
    protected Dictionary<int, T> allWord;

    public WordSet(Dictionary<int, T> allWordData)
    {
        allWord = allWordData;
    }

    public Dictionary<int, T> Get_WordData() => allWord;
    public int Get_Amount() => allWord.Count;

    public abstract string Get_Word(int _ID);
}

[System.Serializable]
public class WordSet_Just : WordSet<WordElement_Just>
{
    public WordSet_Just(Dictionary<int, WordElement_Just> dict) : base(dict)
    { }

    public override string Get_Word(int id)
    {
        if (allWord.ContainsKey(id))
            return allWord[id].words[GameManager.languageID];

        return "";
    }

    public string[] Get_Words(int id)
    {
        if (allWord.ContainsKey(id))
            return allWord[id].words;

        return null;
    }
}

[System.Serializable]
public class WordElement_Just
{
    public string[] words;

    public WordElement_Just(int id, string[] words)
    {
        this.words = words;
    }
}


[System.Serializable]
public class WordSet_WithClr : WordSet<ColorLanguageSet>
{
    public WordSet_WithClr(Dictionary<int, ColorLanguageSet> dict) : base(dict)
    { }

    public override string Get_Word(int id)
    {
        if (allWord.ContainsKey(id))
        {
            ColorLanguageSet data = allWord[id];
            return $"<color=#{data.clrHex}><b>\"{data.words[GameManager.languageID]}\"</color></b>";
        }
        else
        {
            return "";
        }
    }
}

[System.Serializable]
public class ColorLanguageSet : WordElement_Just
{
    public string clrHex;

    public ColorLanguageSet(int id, string clrHex, string[] names) : base(id, names)
    {
        this.clrHex = clrHex;
    }
}
