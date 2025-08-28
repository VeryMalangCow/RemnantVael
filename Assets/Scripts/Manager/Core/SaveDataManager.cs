using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : PersistentSingleton<SaveDataManager>
{
    #region Value

    #region - Inspector

    [Header("=== Data")]
    [SerializeField] public List<GameObject> CharacterPrefabs;

    [Header("=== Path")]
    [SerializeField] private string JsonFilePath = "";
    [SerializeField] private string CharacterPath = "";
    [SerializeField] private string ItemPath = "";

    #endregion

    #region - Hide

    [Space(30)]
    [SerializeField] public JsonData JsonData;

    // Path
    [HideInInspector] private string DataPath = "";


    #endregion

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Load_JsonData();
    }

    #endregion

    #region Save

    private void Save_JsonData()
    {
        DataPath = Path.Combine(Application.persistentDataPath, JsonFilePath);

        TrySave_EachJsonData(this.CharacterPath, new SerializationList<EachCharacterJsonData>(JsonData.CharacterData));
        TrySave_EachJsonData(this.ItemPath, new SerializationList<EachItemJsonData>(JsonData.ItemData));

    }

    #region TrySave (Each)

    private void TrySave_EachJsonData<T>(string _EachPath, T _Data)
    {
        string eachJsonPath = Path.Combine(DataPath, $"{_EachPath}.json");

        Create_DirectoryExists(eachJsonPath);

        string jsonData = JsonUtility.ToJson(_Data, true);
        File.WriteAllText(eachJsonPath, jsonData);

        Debug.Log($"Save: {_EachPath}");
    }

    #endregion

    #endregion

    #region Load

    private void Load_JsonData()
    {
        DataPath = Path.Combine(Application.persistentDataPath, JsonFilePath);

        JsonData = new JsonData();

        JsonData.CharacterData =
            TryLoad_EachJsonData<SerializationList<EachCharacterJsonData>>(
                this.CharacterPath,
                Get_Default_CharacterData()).ListData;

        JsonData.ItemData =
            TryLoad_EachJsonData<SerializationList<EachItemJsonData>>(
                this.ItemPath,
                Get_Default_ItemData()).ListData;
    }

    #region TryLoad (Each)

    private T TryLoad_EachJsonData<T>(string _EachPath, string _DefaultData)
    {
        string eachJsonPath = Path.Combine(DataPath, $"{_EachPath}.json");

        if (!File.Exists(eachJsonPath))
        {
            Debug.Log($"Create: {_EachPath}");
            Create_DirectoryExists(eachJsonPath);
            File.WriteAllText(eachJsonPath, _DefaultData);
        }

        string jsonData = File.ReadAllText(eachJsonPath);
        Debug.Log($"Load: {_EachPath}");
        return JsonUtility.FromJson<T>(jsonData);
    }

    #endregion

    #endregion

    #region Reset

    private void Reset_JsonData()
    {
        DataPath = Path.Combine(Application.persistentDataPath, JsonFilePath);

        TryReset_EachJsonData<SerializationList<EachCharacterJsonData>>(
            this.CharacterPath,
            Get_Default_CharacterData());

        TryReset_EachJsonData<SerializationList<EachItemJsonData>>(
            this.ItemPath,
            Get_Default_ItemData());
    }

    private void TryReset_EachJsonData<T>(string _EachPath, string _DefaultData)
    {
        string eachJsonPath = Path.Combine(DataPath, $"{_EachPath}.json");

        if (!File.Exists(eachJsonPath))
        {
            Create_DirectoryExists(eachJsonPath);
        }

        File.WriteAllText(eachJsonPath, _DefaultData);
    }

    #endregion

    #region Create

    // File Create
    private void Create_DirectoryExists(string _FullPath)
    {
        string dir = Path.GetDirectoryName(_FullPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    // Default Character
    private string Get_Default_CharacterData()
    {
        return Resources.Load<TextAsset>("Json/DefaultCharacterData").text;
    }

    // Default Item
    private string Get_Default_ItemData()
    {
        return Resources.Load<TextAsset>("Json/DefaultItemData").text;
    }

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
    public List<EachCharacterJsonData> CharacterData = new List<EachCharacterJsonData>();
    public List<EachItemJsonData> ItemData = new List<EachItemJsonData>();

    public void Gain_Item(int _ID, int _Amount)
    {
        if (ItemData.Count > _ID)
        {
            ItemData[_ID].Amount += _Amount;
        }
    }
}

#endregion

#region List Convertor

[System.Serializable]
public class SerializationList<T>
{
    public SerializationList(List<T> _ListData) => ListData = _ListData;
    public List<T> ListData;
}

#endregion

#region Character

[System.Serializable]
public class EachCharacterJsonData
{
    public int ID = 0;
    public bool CanUse = false;

    public EachCharacterJsonData(int _ID, bool _CanUse)
    {
        ID = _ID;
        CanUse = _CanUse;
    }
}

#endregion

#region Item

[System.Serializable]
public class EachItemJsonData
{
    public int ID = 0;
    public string Name = "";
    public int Amount = 0;

    public EachItemJsonData(int _ID, string _Name, int _Amount = 0)
    {
        ID = _ID;
        Name = _Name;
        Amount = _Amount;
    }
}

#endregion