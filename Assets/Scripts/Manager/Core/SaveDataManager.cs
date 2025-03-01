using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : PersistentSingleton<SaveDataManager>
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public List<GameObject> CharacterPrefabs;
    [HideInInspector] public CharacterSaveData CharacterSaveData = new CharacterSaveData();


    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        // Load Data
        CharacterSaveData.Load_Data();
    }

    #endregion
}

#region Character

public class CharacterSaveData
{
    Dictionary<int, bool> SaveData;

    public void Load_Data()
    {
        SaveData = new Dictionary<int, bool>()
        {
            {0, true},
            {1, true},
            {2, false}
        };
    }

    // ID값에 맞는 사용 여부
    public bool Can_UseCharacter(int _ID)
    {
        return SaveData[_ID];
    }

    // 사용 가능한 리스트 가져오기
    public List<int> Get_CanUseIDList()
    {
        List<int> IDs = new List<int>();
        foreach (KeyValuePair<int, bool> keyValue in SaveData)
        {
            if (keyValue.Value == true)
            {
                IDs.Add(keyValue.Key);
            }
        }
        return IDs;
    }
}

#endregion