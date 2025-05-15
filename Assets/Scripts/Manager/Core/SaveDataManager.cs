using System.Collections.Generic;
using System.IO;
using TMPro;
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

    #endregion

    #region - Hide

    // TempData
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

        DataPath = $"{Application.dataPath}/{JsonFilePath}";
        Load_JsonData();
    }

    #endregion

    [ContextMenu("To Json Data")]
    private void Save_JsonData_FromInspector()
    {
        DataPath = $"{Application.dataPath}/{JsonFilePath}";
        Save_JsonData();
    }


    #region Reset & Save & Load

    private void Reset_JsonData()
    {
        JsonData = new JsonData();
        Save_JsonData();
    }

    private void Save_JsonData()
    {
        string jsonData = JsonUtility.ToJson(new SerializationList<EachCharacterJsonData>(JsonData.CharacterData), true);
        string path = $"{DataPath}/{CharacterPath}.json";
        File.WriteAllText(path, jsonData);
    }

    private void Load_JsonData()
    {
        JsonData = new JsonData();

        string jsonData = File.ReadAllText($"{DataPath}/{CharacterPath}.json");
        JsonData.CharacterData = JsonUtility.FromJson<SerializationList<EachCharacterJsonData>>(jsonData).ListData;
    }

    #endregion
}

#region Json

[System.Serializable]
public class JsonData
{
    public List<EachCharacterJsonData> CharacterData = new List<EachCharacterJsonData>();
}

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