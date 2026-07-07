using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;



public class AllyManager : Singleton<AllyManager>, IMainGameInitializer
{
    #region Value

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [Space(20)]
    [Header("<><><><><> Ally Manager")]
    [SerializeField] private PoolSystem<ShootingAllyController> gruntAllyPool;
    [SerializeField] private PoolSystem<ShootingAllyController> ignisAllyPool;
    [SerializeField] private PoolSystem<ShootingAllyController> glaciaAllyPool;
    [SerializeField] private PoolSystem<ShootingAllyController> voltAllyPool;
    [SerializeField] private PoolSystem<ShootingAllyController> toxAllyPool;
    [SerializeField] private Transform fieldAllyParentTf;

    [SerializeField] private PoolSystem<DroppingAttackAllyController> boomaAllyPool;
    [SerializeField] private PoolSystem<DroppingTotemeAllyController> totisAllyPool;
    [SerializeField] private Transform noneAllyParentTf;

    [SerializeField] private PoolSystem<AllyTotemeController> allyTotemePool;

    private AlwaysCooltimeData totemeTimer = new AlwaysCooltimeData(1f);
    private List<TotemeController> allTotemeList = new List<TotemeController>(32);

    // Allies
    [HideInInspector] public List<AllyController> allAlly = new List<AllyController>();

    // Ally Card Data
    [HideInInspector] private AllyCardData[] stAllyCardDatas;
    [HideInInspector] private HashSet<int> st_gottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] st_cardIconArr;

    [HideInInspector] private AllyCardData[] utAllyCardDatas;
    [HideInInspector] private HashSet<int> ut_gottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] ut_cardIconArr;

    [HideInInspector] private AllyCardData[] ntAllyCardDatas;
    [HideInInspector] private HashSet<int> nt_gottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] nt_cardIconArr;

    [HideInInspector] private AllyCardData[][] allAllyCardData = null;
    [HideInInspector] private HashSet<int>[] allGottenAllyCards = null;
    [HideInInspector] private Sprite[][] allIconArr = null;

    // Base State
    [HideInInspector] private AllyState allyState = new AllyState();
    [HideInInspector] public AllyState getAllyState { get { return allyState; } }

    // Name
    [HideInInspector] private HashSet<int> usedAllyName = new HashSet<int>();

    // String
    [HideInInspector]
    public static readonly List<string> stateTypeList = new List<string>
        { "Dmg", "Rof", "MovementSpeed", "AttackSize", "CC", "CD", "MuzzleSpeed", "KBPower", "Dur" };
    [HideInInspector]
    public static List<float> tunerMultipleValueByType = new List<float>
        { 0.05f, 0.05f, 0.05f, 0.1f, 0.05f, 0.1f, 0.1f, 0.1f, 0.05f };

    // Base Upgrade Data
    [HideInInspector] public static List<int> tunerTypePercent = new List<int> { 8, 5, 3, 2, 1 };
    [HideInInspector] private Dictionary<string, Sprite> tunerTypeIconDict;

    [HideInInspector] private static Dictionary<string, float> tunerTypeMultipleValueDict;

    // Card
    public AllyCardBaseData[] stCardBaseDatas { get; private set; }
    public AllyCardBaseData[] utCardBaseDatas { get; private set; }
    public AllyCardBaseData[] ntCardBaseDatas { get; private set; }

    public AllyLanguageSet stLanguage { get; private set; }
    public AllyLanguageSet utLanguage { get; private set; }
    public AllyLanguageSet ntLanguage { get; private set; }

    // Request
    public LanguageSet requestName { get; private set; }
    public LanguageSet requestFailure { get; private set; }
    public LanguageSet requestSuccess { get; private set; }

    // Tuner
    public LanguageSet tunerStateName { get; private set; }

    #endregion          

    // Init
    public IEnumerator Initialize()
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        var reso = StaticResourceManager.instance.AllyReso;

        // Card Data
        var st = reso.stCsv;
        stLanguage = new AllyLanguageSet(st.nameCsv, st.descCsv);
        stCardBaseDatas = CSVReader.GetAllyCardBaseDatas(st.csv);

        var ut = reso.utCsv;
        utLanguage = new AllyLanguageSet(ut.nameCsv, ut.descCsv);
        utCardBaseDatas = CSVReader.GetAllyCardBaseDatas(ut.csv);

        var nt = reso.ntCsv;
        ntLanguage = new AllyLanguageSet(nt.nameCsv, nt.descCsv);
        ntCardBaseDatas = CSVReader.GetAllyCardBaseDatas(nt.csv);

        // Request
        requestName = CSVReader.GetLanguageSet(reso.requestNameCsv);
        requestFailure = CSVReader.GetLanguageSet(reso.requestFailureCsv);
        requestSuccess = CSVReader.GetLanguageSet(reso.requestSuccessCsv);

        // Tuner
        tunerStateName = CSVReader.GetLanguageSet(reso.tunerStateCsv);

        // Icon
        st_cardIconArr = reso.allyCardIcons[0].array;
        ut_cardIconArr = reso.allyCardIcons[1].array;
        nt_cardIconArr = reso.allyCardIcons[2].array;

        // Arr
        allAllyCardData = new AllyCardData[][] { stAllyCardDatas, utAllyCardDatas, ntAllyCardDatas };
        allGottenAllyCards = new HashSet<int>[] { st_gottenAllyCards, ut_gottenAllyCards, nt_gottenAllyCards };
        allIconArr = new Sprite[][] { st_cardIconArr, ut_cardIconArr, nt_cardIconArr };

        allyState = new AllyState();

        var tunerIcons = reso.tunerTypeIcons;
        tunerTypeIconDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < stateTypeList.Count; i++)
            tunerTypeIconDict.Add(stateTypeList[i], tunerIcons[i]);

        tunerTypeMultipleValueDict = new Dictionary<string, float>();
        for (int i = 0; i < stateTypeList.Count; i++)
            tunerTypeMultipleValueDict.Add(stateTypeList[i], tunerMultipleValueByType[i]);

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"AllyManager: <color=orange>CSV</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return gruntAllyPool.InitAsync(fieldAllyParentTf, 8, 8f);
        yield return ignisAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);
        yield return glaciaAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);
        yield return voltAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);
        yield return toxAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);

        yield return boomaAllyPool.InitAsync(noneAllyParentTf, 4, 8f);
        yield return totisAllyPool.InitAsync(noneAllyParentTf, 4, 8f);

        yield return allyTotemePool.InitAsync(16, 8f);

        enabled = true;
    }

    // Centralized Update
    private void Update()
    {
        float dt = Time.deltaTime;

        HandleAlly_Shooting(gruntAllyPool, dt);
        HandleAlly_Shooting(ignisAllyPool, dt);
        HandleAlly_Shooting(glaciaAllyPool, dt);
        HandleAlly_Shooting(voltAllyPool, dt);
        HandleAlly_Shooting(toxAllyPool, dt);

        HandleAlly_AttackCharging(boomaAllyPool, dt);
        HandleAlly_AttackCharging(totisAllyPool, dt);

        HandleToteme_TotemeTimer(dt);
    }

    private void HandleAlly_Shooting<T>(PoolSystem<T> pool, float dt) where T : ShootingAllyController
    {
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].HandleState(dt);
    }

    private void HandleAlly_AttackCharging<T>(PoolSystem<T> pool, float dt) where T : DroppingAllyController
    {
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].HandleAttackCharge(dt);
    }

    private void HandleToteme_TotemeTimer(float dt)
    {
        if (totemeTimer.Is_Full(dt))
            for (int i = 0; i < allTotemeList.Count; i++)
                allTotemeList[i].Active_Buff();
    }


    // Centralized FixedUpdate
    private void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        HandleFieldAlly_Movement(gruntAllyPool, fdt);
        HandleFieldAlly_Movement(ignisAllyPool, fdt);
        HandleFieldAlly_Movement(glaciaAllyPool, fdt);
        HandleFieldAlly_Movement(voltAllyPool, fdt);
        HandleFieldAlly_Movement(toxAllyPool, fdt);
    }

    private void HandleFieldAlly_Movement<T>(PoolSystem<T> pool, float fdt) where T : FieldUnitAllyController
    {
        var objs = pool.objs;
        var activeIndices = pool.activeIndices;

        for (int i = activeIndices.Count - 1; i >= 0; i--)
            objs[activeIndices[i]].HandleMovement(fdt);
    }


    #region Pool

    // Ally
    public void SpawnGruntAlly() 
    {
        var ally = gruntAllyPool.Dequeue();
        ally.StartNew_Request();
    }
    public void SpawnIgnisAlly() 
    {
        var ally = ignisAllyPool.Dequeue(); 
        ally.StartNew_Request();
    }
    public void SpawnGlaciaAlly() 
    {
        var ally = glaciaAllyPool.Dequeue(); 
        ally.StartNew_Request();
    }
    public void SpawnVoltAlly()
    {
        var ally = voltAllyPool.Dequeue(); 
        ally.StartNew_Request();
    }
    public void SpawnToxAlly() 
    {
        var ally = toxAllyPool.Dequeue(); 
        ally.StartNew_Request();
    }


    public void SpawnBoomaAlly() 
    {
        var ally = boomaAllyPool.Dequeue(); 
        ally.StartNew_Request();
    }
    public void SpawnTotisAlly() 
    {
        var ally = totisAllyPool.Dequeue(); 
        ally.StartNew_Request();
    }



    // Toteme
    public void Add_Toteme(TotemeController toteme) => DevTool.Add_InList(allTotemeList, toteme);
    public void Remove_Toteme(TotemeController toteme) => DevTool.Remove_InList(allTotemeList, toteme);
    

    public AllyTotemeController SpawnAllyToteme() => allyTotemePool.Dequeue();
    public void RemoveAllyToteme(AllyTotemeController item) => allyTotemePool.Enqueue(item);

    #endregion

    #region State

    public void Set_StateDmg(float value)
    {
        allyState.dmg.value = value;
        Set_AllState();
    }

    public void Set_StateRof(float value)
    {
        allyState.rof.value = value;
        Set_AllState();
    }

    public void Set_StateMovementSpeed(float value)
    {
        allyState.movementSpeed.value = value;
        Set_AllState();
    }

    public void Set_StateAttackSize(float value)
    {
        allyState.attackSize.value = value;
        Set_AllState();
    }

    public void Set_StateCC(float value)
    {
        allyState.criticalChacne.value = value;
        Set_AllState();
    }

    public void Set_StateCD(float value)
    {
        allyState.criticalDmg.value = value;
        Set_AllState();
    }

    public void Set_StateMuzzleSpeed(float value)
    {
        allyState.muzzleSpeed.value = value;
        Set_AllState();
    }

    private void Set_AllState()
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].Set_AllState();
    }

    #endregion

    #region Is

    private bool Is_ExistEssentialID(AllyCardData targetData)
    {
        if (targetData.essentialID != -1)
            return true;

        return false;
    }

    private bool Is_GottenEssentialCard(int typeId, AllyCardData targetData)
    {
        if (!Is_ExistEssentialID(targetData) || allGottenAllyCards[typeId].Contains(targetData.essentialID))
            return true;

        return false;
    }

    // 선택 가능한 ID 카드를 체크
    private bool Can_ChoiceAble(int typeID, AllyCardData targetData, List<AllyCardData> alreadyChoicedDataList)
    {
        if (!allGottenAllyCards[typeID].Contains(targetData.id) &&
            Is_GottenEssentialCard(typeID, targetData) &&
            !alreadyChoicedDataList.Contains(targetData))
            return true;

        return false;
    }

    #endregion

    #region Card

    public Sprite Get_CardIcon(int typeID, int cardID)
    {
        return allIconArr[typeID][cardID];
    }

    // 선행 카드 정보
    public AllyCardData Get_PreAllyCardData(int typeID, AllyCardData targetCard)
    {
        if (targetCard.essentialID == -1)
            return null;

        return allAllyCardData[typeID][targetCard.essentialID];
    }

    // 한 번에 여러개의 랜덤 카드 리턴
    public List<AllyCardData> Get_ChoiceAbleRandomData(int typeID, int limitAmount)
    {
        List<AllyCardData> result = new List<AllyCardData>();

        int i = 0;
        while (true)
        {
            i++;
            if (i > 100)
                break;

            AllyCardData randomData = allAllyCardData[typeID][Random.Range(0, allAllyCardData[typeID].Length)];

            if (Can_ChoiceAble(typeID, randomData, result))
                result.Add(randomData);
            else
                continue;

            if (result.Count >= limitAmount)
                break;
        }

        return result;
    }

    // 한 번에 한개의 랜덤 카드 리턴
    public AllyCardData Get_ChoiceAbleRandomData(int typeId, List<int> alreadyPlacedAllyCardIndexer)
    {
        List<AllyCardData> alreadyPlacedAllyCard = new List<AllyCardData>();
        for (int i = 0; i < alreadyPlacedAllyCardIndexer.Count; i++)
            if (alreadyPlacedAllyCardIndexer[i] != -1)
                alreadyPlacedAllyCard.Add(allAllyCardData[typeId][alreadyPlacedAllyCardIndexer[i]]);

        int s = 0;
        while (true)
        {
            s++;
            if (s > 100)
                break;

            AllyCardData randomData = allAllyCardData[typeId][Random.Range(0, allAllyCardData[typeId].Length)];

            if (Can_ChoiceAble(typeId, randomData, alreadyPlacedAllyCard))
                return randomData;
        }

        return null;
    }

    public void Add_AllyCard(int typeId, int id)
    {
        allGottenAllyCards[typeId].Add(id);
        AllyCardActivityManager.instance.Action_CorrectCardActivity(typeId, id);
    }

    #endregion

    #region BU

    public static float Get_AllyTunerStateMultiple(string type)
    {
        return tunerTypeMultipleValueDict[type];
    }

    public Sprite Get_BUIcon(string type)
    {
        return tunerTypeIconDict[type];
    }

    #endregion

    #region MU

    public void Set_AllAlliesSync()
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
        {
            allAlly[i].Set_ActingSync();
        }
    }

    #endregion

    #region Set (Ally Set)

    public void Set_AllAlliesActive(bool onOff)
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].gameObject.SetActive(onOff);
    }

    public void Start_AllAllies_Combat()
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
        {
            allAlly[i].Start_MainCor();
        }
    }

    public void Stop_AllAllies_Combat()
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].Stop_MainCor();
    }

    public void Set_AllAllyPlayerNearPos()
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
        {
            if (allAlly[i] is FieldUnitAllyController fuAlly)
                fuAlly.Set_PosRandomNearPlayer();

        }
    }

    public void Set_AllAllyTargetEnemy(EnemyController enemy)
    {
        // Null 이여도 초기화
        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].Set_TargetEnemy(enemy);
    }

    #endregion

    #region Name

    public int Get_AllyNameID()
    {
        int randomIndex = -1;

        int safeInt = 0;
        while (true)
        {
            safeInt++;
            if (safeInt > 100) break; 

            randomIndex = Random.Range(0, StaticResourceManager.instance.randomNames.GetAmount());
            if (!usedAllyName.Contains(randomIndex))
            {
                usedAllyName.Add(randomIndex);
                break;
            }
        }

        return randomIndex;
    }

    public string[] Get_AllyName(int id)
    {
        if (id == -1) 
            return null;

        return StaticResourceManager.instance.randomNames.GetLanguages(id);
    }

    public void Set_Language()
    {
        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].Set_Language();
    }

    #endregion

    #region Set (Lang)

    public void SetLanguageTxt()
    {
        if (stAllyCardDatas == null) stAllyCardDatas = GetNewAllyCardData(stCardBaseDatas, stLanguage);
        else SetLanguageAllyCardData(stAllyCardDatas, stLanguage);


        if (utAllyCardDatas == null) utAllyCardDatas = GetNewAllyCardData(utCardBaseDatas, utLanguage);
        else SetLanguageAllyCardData(utAllyCardDatas, utLanguage);


        if (ntAllyCardDatas == null) ntAllyCardDatas = GetNewAllyCardData(ntCardBaseDatas, ntLanguage);
        else SetLanguageAllyCardData(ntAllyCardDatas, ntLanguage);
    }

    public AllyCardData[] GetNewAllyCardData(AllyCardBaseData[] data, AllyLanguageSet languageSet)
    {
        AllyCardData[] result = new AllyCardData[data.Length];
        for (int i = 0; i < data.Length; i++)
            result[i] = new AllyCardData(data[i], languageSet.name.GetLanguage(i), languageSet.desc.GetLanguage(i));

        return result;
    }

    public void SetLanguageAllyCardData(AllyCardData[] data, AllyLanguageSet languageSet)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i].name = languageSet.name.GetLanguage(i);
            data[i].desc = languageSet.desc.GetLanguage(i);
        }
    }

    #endregion
}
public class AllyLanguageSet
{
    public LanguageSet name;
    public LanguageSet desc;

    public AllyLanguageSet(TextAsset nameTextAsset, TextAsset descTextAsset)
    {
        name = CSVReader.GetLanguageSet(nameTextAsset);
        desc = CSVReader.GetLanguageSet(descTextAsset);
    }
}


[System.Serializable]
public class AllyCardBaseData
{
    public int id;
    public int rank;
    public int essentialId;

    public AllyCardBaseData(int id, int rank, int essentialID)
    {
        this.id = id;
        this.rank = rank;
        essentialId = essentialID;
    }
}

[System.Serializable]
public class AllyCardData
{
    public int id;
    public int rank;
    public int essentialID;

    public string name;
    public string desc;

    public AllyCardData(AllyCardBaseData baseData, string name, string desc)
    {
        id = baseData.id;
        rank = baseData.rank;
        essentialID = baseData.essentialId;
        this.name = name;
        this.desc = desc;
    }

}
