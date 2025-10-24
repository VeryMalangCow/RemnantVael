using System.Collections.Generic;
using UniRx;
using UnityEngine;

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
    [SerializeField] private List<Sprite> TunerTypeIcon;

    #endregion

    #region - Hide

    // Allies
    [HideInInspector] public List<AllyController> AllAlly = new List<AllyController>();

    // Ally Card Data
    [HideInInspector] private AllyCardData[] ST_AllAllyCardData;
    [HideInInspector] private HashSet<int> ST_GottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] ST_CardIconArr;

    [HideInInspector] private AllyCardData[] UT_AllAllyCardData;
    [HideInInspector] private HashSet<int> UT_GottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] UT_CardIconArr;

    [HideInInspector] private AllyCardData[] NT_AllAllyCardData;
    [HideInInspector] private HashSet<int> NT_GottenAllyCards = new HashSet<int>();
    [HideInInspector] private Sprite[] NT_CardIconArr;

    [HideInInspector] private AllyCardData[][] AllAllyCardData = null;
    [HideInInspector] private HashSet<int>[] AllGottenAllyCards = null;
    [HideInInspector] private Sprite[][] AllIconArr = null;

    // Base State
    [HideInInspector] private AllyState AllyState = new AllyState();
    [HideInInspector] public AllyState GetAllyState { get { return AllyState; } }

    // Reso
    [HideInInspector] public Dictionary<string, AllySpriteSet> AllySpriteSetDict;

    // Name
    [HideInInspector] private string[][] AllyAllNameArr;
    [HideInInspector] private HashSet<int> UsedAllyName = new HashSet<int>();

    // String
    [HideInInspector]
    public static readonly List<string> StateTypeList = new List<string> 
        { "Dmg", "Rof", "MovementSpeed", "AttackSize", "CC", "CD", "MuzzleSpeed", "KBPower", "Dur" };
    [HideInInspector]
    public static List<float> TunerMultipleValueByType = new List<float>
        { 0.05f, 0.05f, 0.05f, 0.1f, 0.05f, 0.1f, 0.1f, 0.1f, 0.05f };

    // Base Upgrade Data
    [HideInInspector] public static List<int> TunerTypePercent = new List<int> { 8, 5, 3, 2, 1 };
    [HideInInspector] private Dictionary<string, Sprite> TunerTypeIconDict;

    [HideInInspector] private static Dictionary<string, float> TunerTypeMultipleValueDict;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        // Card Data
        ST_AllAllyCardData = ResourceManager.Instance.Get_StrikeTeam_AllAllyCardData();
        UT_AllAllyCardData = ResourceManager.Instance.Get_UplinkTeam_AllAllyCardData();
        NT_AllAllyCardData = ResourceManager.Instance.Get_NeoTeam_AllAllyCardData();

        // Icon
        ST_CardIconArr = ResourceManager.Instance.Get_AllyCardSpriteIcon(0);
        UT_CardIconArr = ResourceManager.Instance.Get_AllyCardSpriteIcon(1);
        NT_CardIconArr = ResourceManager.Instance.Get_AllyCardSpriteIcon(2);

        // Arr
        AllAllyCardData = new AllyCardData[][] { ST_AllAllyCardData, UT_AllAllyCardData, NT_AllAllyCardData };
        AllGottenAllyCards = new HashSet<int>[] { ST_GottenAllyCards, UT_GottenAllyCards, NT_GottenAllyCards };
        AllIconArr = new Sprite[][] { ST_CardIconArr, UT_CardIconArr, NT_CardIconArr };

        // Sprite
        AllySpriteSetDict = new Dictionary<string, AllySpriteSet>
        {
            { "Assult", new AllySpriteSet("Assult") },

            { "Ignis", new AllySpriteSet("Ignis") },
            { "Glacia", new AllySpriteSet("Glacia") },
            { "Volt", new AllySpriteSet("Volt") },
            { "Tox", new AllySpriteSet("Tox") }
        };

        AllyState = new AllyState();

        // Random Name
        AllyAllNameArr = ResourceManager.Instance.Get_AllAllyRandomName();

        TunerTypeIconDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < StateTypeList.Count; i++)
            TunerTypeIconDict.Add(StateTypeList[i], TunerTypeIcon[i]);

        TunerTypeMultipleValueDict = new Dictionary<string, float>();
        for (int i = 0; i < StateTypeList.Count; i++)
            TunerTypeMultipleValueDict.Add(StateTypeList[i], TunerMultipleValueByType[i]);
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
        AllyState.Dmg.Value = _Value;
        Set_AllState();
    }

    public void Set_StateRof(float _Value)
    {
        AllyState.Rof.Value = _Value;
        Set_AllState();
    }

    public void Set_StateMovementSpeed(float _Value)
    {
        AllyState.MovementSpeed.Value = _Value;
        Set_AllState();
    }

    public void Set_StateAttackSize(float _Value)
    {
        AllyState.AttackSize.Value = _Value;
        Set_AllState();
    }

    public void Set_StateCC(float _Value)
    {
        AllyState.CC.Value = _Value;
        Set_AllState();
    }

    public void Set_StateCD(float _Value)
    {
        AllyState.CD.Value = _Value;
        Set_AllState();
    }

    public void Set_StateMuzzleSpeed(float _Value)
    {
        AllyState.MuzzleSpeed.Value = _Value;
        Set_AllState();
    }

    private void Set_AllState()
    {
        if (AllAlly.Count <= 0) return;

        for (int i = 0; i < AllAlly.Count; i++)
            AllAlly[i].Set_AllState();
    }

    #endregion

    #region Is

    private bool Is_ExistEssentialID(AllyCardData _TargetData)
    {
        if (_TargetData.EssentialID != -1)
            return true;

        return false;
    }

    private bool Is_GottenEssentialCard(int _TypeID, AllyCardData _TargetData)
    {
        if (!Is_ExistEssentialID(_TargetData) || AllGottenAllyCards[_TypeID].Contains(_TargetData.EssentialID))
            return true;

        return false;
    }

    // 선택 가능한 ID 카드를 체크
    private bool Can_ChoiceAble(int _TypeID, AllyCardData _TargetData, List<AllyCardData> _AlreadyChoicedDataList)
    {
        if (!AllGottenAllyCards[_TypeID].Contains(_TargetData.ID) &&
            Is_GottenEssentialCard(_TypeID, _TargetData) &&
            !_AlreadyChoicedDataList.Contains(_TargetData))
            return true;

        return false;
    }

    #endregion

    #region Card

    public Sprite Get_CardIcon(int _TypeID, int _CardID)
    {
        return AllIconArr[_TypeID][_CardID];
    }

    // 선행 카드 정보
    public AllyCardData Get_PreAllyCardData(int _TypeID, AllyCardData _TargetCard)
    {
        if (_TargetCard.EssentialID == -1)
            return null;

        return AllAllyCardData[_TypeID][_TargetCard.EssentialID];
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

            AllyCardData randomData = AllAllyCardData[_TypeID][Random.Range(0, AllAllyCardData[_TypeID].Length)];

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
                alreadyPlacedAllyCard.Add(AllAllyCardData[_TypeID][_AlreadyPlacedAllyCardIndexer[i]]);

        int s = 0;
        while (true)
        {
            s++;
            if (s > 100)
                break;

            AllyCardData randomData = AllAllyCardData[_TypeID][Random.Range(0, AllAllyCardData[_TypeID].Length)];

            if (Can_ChoiceAble(_TypeID, randomData, alreadyPlacedAllyCard))
                return randomData;
        }

        return null;
    }

    public void Add_AllyCard(int _TypeID, int _ID)
    {
        AllGottenAllyCards[_TypeID].Add(_ID);
        AllyCardActivityManager.Instance.Action_CorrectCardActivity(_TypeID, _ID);
    }

    #endregion

    #region BU

    public static float Get_AllyTunerStateMultiple(string _Type)
    {
        return TunerTypeMultipleValueDict[_Type];
    }

    public Sprite Get_BUIcon(string _Type)
    {
        return TunerTypeIconDict[_Type];
    }

    #endregion

    #region MU

    public void Set_AllAlliesSync()
    {
        if (AllAlly.Count <= 0) return;

        for (int i = 0; i < AllAlly.Count; i++)
        {
            AllAlly[i].Set_ActingSync();
        }
    }

    #endregion

    #region Set (Ally Set)

    public void Set_AllAlliesActive(bool _OnOff)
    {
        if (AllAlly.Count <= 0) return;

        for (int i = 0; i < AllAlly.Count; i++)
            AllAlly[i].gameObject.SetActive(_OnOff);
    }

    public void Start_AllAllies_Combat()
    {
        if (AllAlly.Count <= 0) return;

        for (int i = 0; i < AllAlly.Count; i++)
        {
            AllAlly[i].Start_MainCor();
        }
    }

    public void Stop_AllAllies_Combat()
    {
        if (AllAlly.Count <= 0) return;

        for (int i = 0; i < AllAlly.Count; i++)
            AllAlly[i].Stop_MainCor();
    }

    public void Set_AllAllyPlayerNearPos()
    {
        if (AllAlly.Count <= 0) return;

        for (int i = 0; i < AllAlly.Count; i++)
        {
            if (AllAlly[i] is FieldUnitAllyController fuAlly)
                fuAlly.Set_PosRandomNearPlayer();

        }
    }

    public void Set_AllAllyTargetEnemy(EnemyController _Enemy)
    {
        // Null 이여도 초기화
        for (int i = 0; i < AllAlly.Count; i++)
            AllAlly[i].Set_TargetEnemy(_Enemy);
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
            if (safeInt > 100)
            { break; }

            randomIndex = Random.Range(0, AllyAllNameArr.Length);
            if (!UsedAllyName.Contains(randomIndex))
            {
                UsedAllyName.Add(randomIndex);
                break;
            }
        }

        return randomIndex;
    }

    public string[] Get_AllyName(int _ID)
    {
        if (_ID == -1) 
            return null;

        return AllyAllNameArr[_ID];
    }

    public void Set_Language()
    {
        for (int i = 0; i < AllAlly.Count; i++)
            AllAlly[i].Set_Language();
    }

    #endregion

    #region Set (Lang)

    public void Set_LanguageTxt()
    {
        AllyCardData[] stData = ResourceManager.Instance.Get_StrikeTeam_AllAllyCardData();
        for (int i = 0; i < ST_AllAllyCardData.Length; i++)
            ST_AllAllyCardData[i].Set_LanguageTxt(stData[i].Name, stData[i].Desc);

        AllyCardData[] utData = ResourceManager.Instance.Get_UplinkTeam_AllAllyCardData();
        for (int i = 0; i < UT_AllAllyCardData.Length; i++)
            UT_AllAllyCardData[i].Set_LanguageTxt(utData[i].Name, utData[i].Desc);

        AllyCardData[] ntData = ResourceManager.Instance.Get_NeoTeam_AllAllyCardData();
        for (int i = 0; i < NT_AllAllyCardData.Length; i++)
            NT_AllAllyCardData[i].Set_LanguageTxt(ntData[i].Name, ntData[i].Desc);
    }

    #endregion
}
