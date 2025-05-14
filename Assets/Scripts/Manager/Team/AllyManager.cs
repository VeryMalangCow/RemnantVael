using System.Collections.Generic;
using UnityEngine;

public class AllyManager : Singleton<AllyManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Manager")]

    [Space(10)]
    [Header("=== Ally Controller")]
    [SerializeField] public List<AllyController> AllAllies = new List<AllyController>();

    #endregion

    #region - Hide

    // Ally Card Data
    [SerializeField] private List<AllyCardData> ST_AllAllyCardData = new List<AllyCardData>();
    [HideInInspector] private HashSet<int> ST_GottenAllyCards = new HashSet<int>();
    [HideInInspector] private List<Sprite> ST_CardIconList = new List<Sprite>();

    [SerializeField] private List<AllyCardData> UT_AllAllyCardData = new List<AllyCardData>();
    [HideInInspector] private HashSet<int> UT_GottenAllyCards = new HashSet<int>();
    [HideInInspector] private List<Sprite> UT_CardIconList = new List<Sprite>();

    [SerializeField] private List<AllyCardData> NT_AllAllyCardData = new List<AllyCardData>();
    [HideInInspector] private HashSet<int> NT_GottenAllyCards = new HashSet<int>();
    [HideInInspector] private List<Sprite> NT_CardIconList = new List<Sprite>();

    [HideInInspector] private List<List<AllyCardData>> AllAllyCardData = null;
    [HideInInspector] private List<HashSet<int>> AllGottenAllyCards = null;
    [HideInInspector] private List<List<Sprite>> AllIconList = null;

    // Base State
    [HideInInspector] private AllyState AllyState;
    [HideInInspector] public AllyState GetAllyState { get { return AllyState; } }

    // Reso
    [SerializeField] public AllySpriteSet AssultAllySpriteSet = new AllySpriteSet();

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        // Card Data
        ST_AllAllyCardData = CSVManager.Instance.Get_StrikeTeam_AllAllyCardData();
        UT_AllAllyCardData = CSVManager.Instance.Get_UplinkTeam_AllAllyCardData();
        NT_AllAllyCardData = CSVManager.Instance.Get_NeoTeam_AllAllyCardData();

        // Icon
        ST_CardIconList = CSVManager.Instance.Get_AllyCardSpriteIcon(0);
        UT_CardIconList = CSVManager.Instance.Get_AllyCardSpriteIcon(1);
        NT_CardIconList = CSVManager.Instance.Get_AllyCardSpriteIcon(2);

        AllAllyCardData = new List<List<AllyCardData>>
        { ST_AllAllyCardData, UT_AllAllyCardData, NT_AllAllyCardData };
        AllGottenAllyCards = new List<HashSet<int>>
        { ST_GottenAllyCards, UT_GottenAllyCards, NT_GottenAllyCards };
        AllIconList = new List<List<Sprite>>
        { ST_CardIconList, UT_CardIconList, NT_CardIconList };

        // Sprite
        AssultAllySpriteSet.Offset("Assult");

        AllyState = new AllyState();
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
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

    #region Get
    public Sprite Get_CardIcon(int _TypeID, int _CardID)
    {
        return AllIconList[_TypeID][_CardID];
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
            AllyCardData randomData = AllAllyCardData[_TypeID][Random.Range(0, AllAllyCardData[_TypeID].Count)];

            if (Can_ChoiceAble(_TypeID, randomData, result))
                result.Add(randomData);
            else
                continue;

            if (result.Count >= _LimitAmount)
                break;

            i++;
            if (i > 30)
                break;
        }

        return result;
    }

    // 한 번에 한개의 랜덤 카드 리턴
    public AllyCardData Get_ChoiceAbleRandomData(int _TypeID, List<int> _AlreadyPlacedAllyCardIndexer)
    {
        List<AllyCardData> alreadyPlacedAllyCard = new List<AllyCardData>();
        for (int i = 0; i < _AlreadyPlacedAllyCardIndexer.Count; i++)
            alreadyPlacedAllyCard.Add(AllAllyCardData[_TypeID][_AlreadyPlacedAllyCardIndexer[i]]);

        int s = 0;
        while (true)
        {
            AllyCardData randomData = AllAllyCardData[_TypeID][Random.Range(0, AllAllyCardData[_TypeID].Count)];

            if (Can_ChoiceAble(_TypeID, randomData, alreadyPlacedAllyCard))
                return randomData;

            s++;
            if (s > 30)
                break;
        }
        return null;
    }

    #endregion

    #region Set (Ally Set)

    public void Start_AllAllies_Combat()
    {
        if (AllAllies.Count <= 0) return;

        for (int i = 0; i < AllAllies.Count; i++)
            AllAllies[i].Start_MainCor();
    }

    public void Stop_AllAllies_Combat()
    {
        if (AllAllies.Count <= 0) return;

        for (int i = 0; i < AllAllies.Count; i++)
            AllAllies[i].Stop_MainCor();
    }

    public void Set_AllAllyPlayerNearPos()
    {
        if (AllAllies.Count <= 0) return;

        for (int i = 0; i < AllAllies.Count; i++)
            AllAllies[i].Set_PosRandomNearPlayer();
    }

    public void Set_AllAllyTargetEnemy(EnemyController _Enemy)
    {
        // Null 이여도 초기화
        for (int i = 0; i < AllAllies.Count; i++)
            AllAllies[i].Set_TargetEnemy(_Enemy);
    }

    #endregion

    #region Set (State)

    public void Set_AllState(AllyState _StateValue)
    {
        AllyState = new AllyState(_StateValue);

        if (AllAllies.Count <= 0) return;

        for (int i = 0; i < AllAllies.Count; i++)
            AllAllies[i].Set_AllState(AllyState);
    }

    #endregion

    #region Set (Lang)

    public void Set_LanguageTxt()
    {
        List<AllyCardData> stData = CSVManager.Instance.Get_StrikeTeam_AllAllyCardData();
        for (int i = 0; i < ST_AllAllyCardData.Count; i++)
            ST_AllAllyCardData[i].Set_LanguageTxt(stData[i].Name, stData[i].Desc);

        List<AllyCardData> utData = CSVManager.Instance.Get_UplinkTeam_AllAllyCardData();
        for (int i = 0; i < UT_AllAllyCardData.Count; i++)
            UT_AllAllyCardData[i].Set_LanguageTxt(utData[i].Name, utData[i].Desc);

        List<AllyCardData> ntData = CSVManager.Instance.Get_NeoTeam_AllAllyCardData();
        for (int i = 0; i < NT_AllAllyCardData.Count; i++)
            NT_AllAllyCardData[i].Set_LanguageTxt(ntData[i].Name, ntData[i].Desc);
    }

    #endregion

    #region Add

    public void Add_AllyCard(int _TypeID, int _ID)
    {
        AllGottenAllyCards[_TypeID].Add(_ID);
        AllyCardActivityManager.Instance.Action_CorrectCardActivity(_TypeID, _ID);
    }

    #endregion
}
