using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AllyManager : Singleton<AllyManager>
{
    #region Value

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

    // Reso
    [HideInInspector] public Dictionary<string, AllySpriteSet> allySpriteSetDict;

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

    #endregion

    #region Offset

    private void Offset()
    {
        // Card Data
        st_allAllyCardData = ResourceManager.instance.Get_StrikeTeam_AllAllyCardData();
        ut_allAllyCardData = ResourceManager.instance.Get_UplinkTeam_AllAllyCardData();
        nt_allAllyCardData = ResourceManager.instance.Get_NeoTeam_AllAllyCardData();

        // Icon
        st_cardIconArr = ResourceManager.instance.Get_AllyCardSpriteIcon(0);
        ut_cardIconArr = ResourceManager.instance.Get_AllyCardSpriteIcon(1);
        nt_cardIconArr = ResourceManager.instance.Get_AllyCardSpriteIcon(2);

        // Arr
        allAllyCardData = new AllyCardData[][] { st_allAllyCardData, ut_allAllyCardData, nt_allAllyCardData };
        allGottenAllyCards = new HashSet<int>[] { st_gottenAllyCards, ut_gottenAllyCards, nt_gottenAllyCards };
        allIconArr = new Sprite[][] { st_cardIconArr, ut_cardIconArr, nt_cardIconArr };

        // Sprite
        allySpriteSetDict = new Dictionary<string, AllySpriteSet>
        {
            { "Assult", new AllySpriteSet("Assult") },

            { "Ignis", new AllySpriteSet("Ignis") },
            { "Glacia", new AllySpriteSet("Glacia") },
            { "Volt", new AllySpriteSet("Volt") },
            { "Tox", new AllySpriteSet("Tox") }
        };

        allyState = new AllyState();

        tunerTypeIconDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < stateTypeList.Count; i++)
            tunerTypeIconDict.Add(stateTypeList[i], tunerTypeIcon[i]);

        tunerTypeMultipleValueDict = new Dictionary<string, float>();
        for (int i = 0; i < stateTypeList.Count; i++)
            tunerTypeMultipleValueDict.Add(stateTypeList[i], tunerMultipleValueByType[i]);
    }


    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    #endregion

    #region State

    public void Set_StateDmg(float _Value)
    {
        allyState.dmg.value = _Value;
        Set_AllState();
    }

    public void Set_StateRof(float _Value)
    {
        allyState.rof.value = _Value;
        Set_AllState();
    }

    public void Set_StateMovementSpeed(float _Value)
    {
        allyState.movementSpeed.value = _Value;
        Set_AllState();
    }

    public void Set_StateAttackSize(float _Value)
    {
        allyState.attackSize.value = _Value;
        Set_AllState();
    }

    public void Set_StateCC(float _Value)
    {
        allyState.criticalChacne.value = _Value;
        Set_AllState();
    }

    public void Set_StateCD(float _Value)
    {
        allyState.criticalDmg.value = _Value;
        Set_AllState();
    }

    public void Set_StateMuzzleSpeed(float _Value)
    {
        allyState.muzzleSpeed.value = _Value;
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

    private bool Is_ExistEssentialID(AllyCardData _TargetData)
    {
        if (_TargetData.essentialID != -1)
            return true;

        return false;
    }

    private bool Is_GottenEssentialCard(int _TypeID, AllyCardData _TargetData)
    {
        if (!Is_ExistEssentialID(_TargetData) || allGottenAllyCards[_TypeID].Contains(_TargetData.essentialID))
            return true;

        return false;
    }

    // 선택 가능한 ID 카드를 체크
    private bool Can_ChoiceAble(int _TypeID, AllyCardData _TargetData, List<AllyCardData> _AlreadyChoicedDataList)
    {
        if (!allGottenAllyCards[_TypeID].Contains(_TargetData.id) &&
            Is_GottenEssentialCard(_TypeID, _TargetData) &&
            !_AlreadyChoicedDataList.Contains(_TargetData))
            return true;

        return false;
    }

    #endregion

    #region Card

    public Sprite Get_CardIcon(int _TypeID, int _CardID)
    {
        return allIconArr[_TypeID][_CardID];
    }

    // 선행 카드 정보
    public AllyCardData Get_PreAllyCardData(int _TypeID, AllyCardData _TargetCard)
    {
        if (_TargetCard.essentialID == -1)
            return null;

        return allAllyCardData[_TypeID][_TargetCard.essentialID];
    }

    // 한 번에 여러개의 랜덤 카드 리턴
    public List<AllyCardData> Get_ChoiceAbleRandomData(int _TypeID, int _LimitAmount)
    {
        List<AllyCardData> result = new List<AllyCardData>();

        int i = 0;
        while (true)
        {
            i++;
            if (i > 100)
                break;

            AllyCardData randomData = allAllyCardData[_TypeID][Random.Range(0, allAllyCardData[_TypeID].Length)];

            if (Can_ChoiceAble(_TypeID, randomData, result))
                result.Add(randomData);
            else
                continue;

            if (result.Count >= _LimitAmount)
                break;
        }

        return result;
    }

    // 한 번에 한개의 랜덤 카드 리턴
    public AllyCardData Get_ChoiceAbleRandomData(int _TypeID, List<int> _AlreadyPlacedAllyCardIndexer)
    {
        List<AllyCardData> alreadyPlacedAllyCard = new List<AllyCardData>();
        for (int i = 0; i < _AlreadyPlacedAllyCardIndexer.Count; i++)
            if (_AlreadyPlacedAllyCardIndexer[i] != -1)
                alreadyPlacedAllyCard.Add(allAllyCardData[_TypeID][_AlreadyPlacedAllyCardIndexer[i]]);

        int s = 0;
        while (true)
        {
            s++;
            if (s > 100)
                break;

            AllyCardData randomData = allAllyCardData[_TypeID][Random.Range(0, allAllyCardData[_TypeID].Length)];

            if (Can_ChoiceAble(_TypeID, randomData, alreadyPlacedAllyCard))
                return randomData;
        }

        return null;
    }

    public void Add_AllyCard(int _TypeID, int _ID)
    {
        allGottenAllyCards[_TypeID].Add(_ID);
        AllyCardActivityManager.instance.Action_CorrectCardActivity(_TypeID, _ID);
    }

    #endregion

    #region BU

    public static float Get_AllyTunerStateMultiple(string _Type)
    {
        return tunerTypeMultipleValueDict[_Type];
    }

    public Sprite Get_BUIcon(string _Type)
    {
        return tunerTypeIconDict[_Type];
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

    public void Set_AllAlliesActive(bool _OnOff)
    {
        if (allAlly.Count <= 0) return;

        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].gameObject.SetActive(_OnOff);
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

    public void Set_AllAllyTargetEnemy(EnemyController _Enemy)
    {
        // Null 이여도 초기화
        for (int i = 0; i < allAlly.Count; i++)
            allAlly[i].Set_TargetEnemy(_Enemy);
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

    public string[] Get_AllyName(int _ID)
    {
        if (_ID == -1) 
            return null;

        return ResourceManager.instance.Get_AllyRandomName(_ID);
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
