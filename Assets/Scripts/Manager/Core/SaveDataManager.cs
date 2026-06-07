using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : PersistentSingleton<SaveDataManager>
{
    #region Value

    // id, amount
    public event Action<int, int> OnHighItemChanged;

    [Header("=== Data")]
    [SerializeField] public List<GameObject> characterPrefabs;

    [Header("=== Path")]
    [SerializeField] private string jsonFilePath = "Json";
    [SerializeField] private string characterPath = "CharacterData";
    [SerializeField] private string itemPath = "ItemData";
    [SerializeField] private string optionPath = "OptionData";
    [SerializeField] private string gameProgressPath = "GameProgressData";
    [SerializeField] private string infoPath = "InfoData";

    [Space(30)]
    [SerializeField] public JsonData jsonData;

    // Path
    [HideInInspector] private string dataPath = "";

    #endregion

    #region Mono

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Load_JsonData();
        Debug.Log("SaveDataManager : Offset Complete");
    }

    #endregion

    #region High Lv Item

    public void GainHighLvItem(int id, int amount)
    {
        jsonData.GainItem(id, amount);
        OnHighItemChanged?.Invoke(id, amount);
    }

    public void UseHighLvItem(int id, int amount)
    {
        jsonData.UseItem(id, amount);
        OnHighItemChanged?.Invoke(id, amount);
    }

    public int GetItemAmount(int id)
        => jsonData.GetItemAmount(id);

    #endregion


    #region Save

    public void Save_JsonData()
    {
        dataPath = Path.Combine(Application.persistentDataPath, jsonFilePath);

        TrySave_EachJsonData(dataPath, this.characterPath, new SerializationList<EachCharacterJsonData>(jsonData.characterData));
        TrySave_EachJsonData(dataPath, this.itemPath, new SerializationList<EachItemJsonData>(jsonData.itemData));
        TrySave_EachJsonData(dataPath, this.optionPath, jsonData.optionData);
        TrySave_EachJsonData(dataPath, this.gameProgressPath, jsonData.gameProgressData);
        TrySave_EachJsonData(dataPath, this.infoPath, new SerializationList<EachInfoJsonData>(jsonData.infoData));

    }

    public void Save_OptionJsonData()
    {
        dataPath = Path.Combine(Application.persistentDataPath, jsonFilePath); 
        TrySave_EachJsonData(dataPath, this.optionPath, jsonData.optionData);
    }

    #region TrySave (Each Module)

    private void TrySave_EachJsonData<T>(string dataPath, string eachPath, T data)
    {
        string eachJsonPath = Path.Combine(dataPath, $"{eachPath}.json");

        Create_DirectoryExists(eachJsonPath);

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(eachJsonPath, jsonData);

        Debug.Log($"Save: {eachPath}");
    }

    #endregion

    #endregion

    #region Load

    public void Load_JsonData()
    {
        dataPath = Path.Combine(Application.persistentDataPath, jsonFilePath);

        jsonData = new JsonData();

        jsonData.characterData =
            TryLoad_EachJsonData<SerializationList<EachCharacterJsonData>>(
                this.characterPath,
                Get_Default_CharacterData()).listData;

        jsonData.itemData =
            TryLoad_EachJsonData<SerializationList<EachItemJsonData>>(
                this.itemPath,
                Get_Default_ItemData()).listData;

        jsonData.optionData =
            TryLoad_EachJsonData<OptionJsonData>(
                this.optionPath,
                Get_Default_OptionData());

        jsonData.gameProgressData =
            TryLoad_EachJsonData<GameProgressJsonData>(
                this.gameProgressPath,
                Get_Default_GameProgressData());

        jsonData.infoData =
            TryLoad_EachJsonData<SerializationList<EachInfoJsonData>>(
                this.infoPath,
                Get_Default_InfoData()).listData;
    }

    #region TryLoad (Each Module)

    private T TryLoad_EachJsonData<T>(string eachPath, string defaultData)
    {
        string eachJsonPath = Path.Combine(dataPath, $"{eachPath}.json");

        if (!File.Exists(eachJsonPath))
        {
            Debug.Log($"Create: {eachPath}");
            Create_DirectoryExists(eachJsonPath);
            File.WriteAllText(eachJsonPath, defaultData);
        }

        string jsonData = File.ReadAllText(eachJsonPath);
        Debug.Log($"Load: {eachPath}");
        return JsonUtility.FromJson<T>(jsonData);
    }

    #endregion

    #endregion

    #region Reset

    public void Reset_JsonData()
    {
        dataPath = Path.Combine(Application.persistentDataPath, jsonFilePath);

        TryReset_EachJsonData<SerializationList<EachCharacterJsonData>>(
            this.characterPath,
            Get_Default_CharacterData());

        TryReset_EachJsonData<SerializationList<EachItemJsonData>>(
            this.itemPath,
            Get_Default_ItemData());

        TryReset_EachJsonData<OptionJsonData>(
            this.optionPath,
            Get_Default_OptionData());

        TryReset_EachJsonData<GameProgressJsonData>(
            this.gameProgressPath,
            Get_Default_GameProgressData());

        TryReset_EachJsonData<SerializationList<EachInfoJsonData>>(
            this.infoPath,
            Get_Default_InfoData());
    }

    #region TryReset (Each Module)

    private void TryReset_EachJsonData<T>(string eachPath, string defaultData)
    {
        string eachJsonPath = Path.Combine(dataPath, $"{eachPath}.json");

        if (!File.Exists(eachJsonPath))
        {
            Create_DirectoryExists(eachJsonPath);
        }

        File.WriteAllText(eachJsonPath, defaultData);
    }

    #endregion

    #endregion

    #region Create

    // File Create
    private void Create_DirectoryExists(string fullPath)
    {
        string dir = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    #endregion

    #region Get

    private string Get_Default_CharacterData() => Resources.Load<TextAsset>($"Json/Default{characterPath}").text;

    private string Get_Default_ItemData() => Resources.Load<TextAsset>($"Json/Default{itemPath}").text;

    private string Get_Default_OptionData() => Resources.Load<TextAsset>($"Json/Default{optionPath}").text;

    private string Get_Default_GameProgressData() => Resources.Load<TextAsset>($"Json/Default{gameProgressPath}").text;

    private string Get_Default_InfoData() => Resources.Load<TextAsset>($"Json/Default{infoPath}").text;

    #endregion


    #region Test

    [ContextMenu("(Test) Load Json")]
    private void Load_JsonData_Test()
    {
        Load_JsonData();
    }

    [ContextMenu("(Test) Save Data")]
    private void Save_JsonData_Test()
    {
        Save_JsonData();
    }

    [ContextMenu("(Test) Reset Data")]
    private void Reset_JsonData_Test()
    {
        Reset_JsonData();
    }
    #endregion
}

#region Json

[System.Serializable]
public class JsonData
{
    public List<EachCharacterJsonData> characterData = new List<EachCharacterJsonData>();
    public List<EachItemJsonData> itemData = new List<EachItemJsonData>();
    public OptionJsonData optionData = new OptionJsonData();
    public GameProgressJsonData gameProgressData = new GameProgressJsonData();
    public List<EachInfoJsonData> infoData = new List<EachInfoJsonData>();

    public void GainItem(int id, int amount)
    {
        itemData[id].amount += amount;
    }
    public void UseItem(int id, int amount)
    {
        itemData[id].amount -= amount;
    }

    public int GetItemAmount(int id)
        => itemData[id].amount;
}

#endregion

#region Character

[System.Serializable]
public class EachCharacterJsonData
{
    public int id = 0;
    public bool canUse = false;

    public EachCharacterJsonData(int id, bool canUse)
    {
        this.id = id;
        this.canUse = canUse;
    }
}

#endregion

#region Item

[System.Serializable]
public class EachItemJsonData
{
    public int id = 0;
    public string name = "";
    public int amount = 0;

    public EachItemJsonData(int id, string name, int amount = 0)
    {
        this.id = id;
        this.name = name;
        this.amount = amount;
    }
}

#endregion

#region Option

[System.Serializable]
public class OptionJsonData
{
    public int languageID = 0;
    public eScreenMode screenMode = eScreenMode.fullScreen;
    public eResolution resolutionMode = eResolution.w1920h1080;
    public eFPS fps = eFPS.f144;
    public float bgmVolume = 0.2f;
    public float sfxVolume = 0.2f;
}

public enum eScreenMode
{
    fullScreen, borderless, window
}

public enum eResolution
{
    w1280h720, w1600h900, w1920h1080, w2560h1440
}

public enum eFPS
{
    f30, f60, f120, f144, f200
}

#endregion

#region Game Progress

[System.Serializable]
public class GameProgressJsonData
{
    public int currentProgressing = 0;

    public bool usableVault = false;

    public bool usableBU = false;
    public bool usableMU = false;

    public bool usableABU = false;
    public bool usableAMU = false;

    public bool usableSTPrison = false;
    public bool usableUTPrison = false;
    public bool usableNTPrison = false;
}

#endregion

#region Info

[System.Serializable]
public class EachInfoJsonData
{
    public int id = 0;
    public bool canVisible = false;

    public EachInfoJsonData(int id, bool canVisible)
    {
        this.id = id;
        this.canVisible = canVisible;
    }
}


#endregion

#region List Convertor

[System.Serializable]
public class SerializationList<T>
{
    public SerializationList(List<T> listData) => this.listData = listData;
    public List<T> listData;
}

#endregion
