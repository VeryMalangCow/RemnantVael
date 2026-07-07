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

    protected override void Awake()
    {
        base.Awake();

        staticWords = new LanguageSet(CSVReader.GetLanguageSet(languageFontReso.staticWordCsv));
        staticDescs = new LanguageSet(CSVReader.GetLanguageSet(languageFontReso.staticDescCsv));

    }
}

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
