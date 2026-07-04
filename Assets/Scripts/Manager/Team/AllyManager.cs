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

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Manager")]

    [Space(10)]
    [Header("=== Reso")]

    [Space(5)]
    [Header("-- Tuner")]
    [SerializeField] private List<Sprite> tunerTypeIcon;

    #endregion

    #region - Hide

    // Allies
    [HideInInspector] public List<AllyController> allAlly = new List<AllyController>();

    // Ally Card Data
    [HideInInspector] private AllyCardData[] st_allAllyCardData;
    [HideInInspector] private HashSet<int> st_gottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] st_cardIconArr;

    [HideInInspector] private AllyCardData[] ut_allAllyCardData;
    [HideInInspector] private HashSet<int> ut_gottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] ut_cardIconArr;

    [HideInInspector] private AllyCardData[] nt_allAllyCardData;
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

    #endregion


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

    #endregion

    // Init
    public IEnumerator Initialize()
    {
        yield return gruntAllyPool.InitAsync(fieldAllyParentTf, 8, 8f);
        yield return ignisAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);
        yield return glaciaAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);
        yield return voltAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);
        yield return toxAllyPool.InitAsync(fieldAllyParentTf, 4, 8f);

        yield return boomaAllyPool.InitAsync(noneAllyParentTf, 4, 8f);
        yield return totisAllyPool.InitAsync(noneAllyParentTf, 4, 8f);

        yield return allyTotemePool.InitAsync(16, 8f);

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        var prefab = StaticResourceManager.instance.AllyReso;
        // Card Data
        st_allAllyCardData = ResourceManager.instance.Get_StrikeTeam_AllAllyCardData();
        ut_allAllyCardData = ResourceManager.instance.Get_UplinkTeam_AllAllyCardData();
        nt_allAllyCardData = ResourceManager.instance.Get_NeoTeam_AllAllyCardData();

        // Icon
        st_cardIconArr = prefab.allyCardIcons[0].array;
        ut_cardIconArr = prefab.allyCardIcons[1].array;
        nt_cardIconArr = prefab.allyCardIcons[2].array;

        // Arr
        allAllyCardData = new AllyCardData[][] { st_allAllyCardData, ut_allAllyCardData, nt_allAllyCardData };
        allGottenAllyCards = new HashSet<int>[] { st_gottenAllyCards, ut_gottenAllyCards, nt_gottenAllyCards };
        allIconArr = new Sprite[][] { st_cardIconArr, ut_cardIconArr, nt_cardIconArr };

        allyState = new AllyState();

        tunerTypeIconDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < stateTypeList.Count; i++)
            tunerTypeIconDict.Add(stateTypeList[i], tunerTypeIcon[i]);

        tunerTypeMultipleValueDict = new Dictionary<string, float>();
        for (int i = 0; i < stateTypeList.Count; i++)
            tunerTypeMultipleValueDict.Add(stateTypeList[i], tunerMultipleValueByType[i]);

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"AllyManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

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
    public void SpawnGruntAlly() => gruntAllyPool.Dequeue();
    public void SpawnIgnisAlly() => ignisAllyPool.Dequeue();
    public void SpawnGlaciaAlly() => glaciaAllyPool.Dequeue();
    public void SpawnVoltAlly() => voltAllyPool.Dequeue();
    public void SpawnToxAlly() => toxAllyPool.Dequeue();


    public void SpawnBoomaAlly() => boomaAllyPool.Dequeue();
    public void SpawnTotisAlly() => totisAllyPool.Dequeue();



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

            randomIndex = Random.Range(0, ResourceManager.instance.Get_AllAllyRandomNameAmount());
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

        return ResourceManager.instance.Get_AllyRandomName(id);
    }

    public void Set_Language()
    {
        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].Set_Language();
    }

    #endregion

    #region Set (Lang)

    public void Set_LanguageTxt()
    {
        AllyCardData[] stData = ResourceManager.instance.Get_StrikeTeam_AllAllyCardData();
        for (int i = 0; i < st_allAllyCardData.Length; i++)
            st_allAllyCardData[i].Set_LanguageTxt(stData[i].name, stData[i].desc);

        AllyCardData[] utData = ResourceManager.instance.Get_UplinkTeam_AllAllyCardData();
        for (int i = 0; i < ut_allAllyCardData.Length; i++)
            ut_allAllyCardData[i].Set_LanguageTxt(utData[i].name, utData[i].desc);

        AllyCardData[] ntData = ResourceManager.instance.Get_NeoTeam_AllAllyCardData();
        for (int i = 0; i < nt_allAllyCardData.Length; i++)
            nt_allAllyCardData[i].Set_LanguageTxt(ntData[i].name, ntData[i].desc);
    }

    #endregion
}
