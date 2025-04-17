using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] private List<AllyCardData> AllAllyCardData = new List<AllyCardData>();
    [HideInInspector] private HashSet<int> GottenAllyCards = new HashSet<int>();

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        AllAllyCardData = CSVManager.Instance.Get_AllAllyCardData();
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

    private bool Is_GottenEssentialCard(AllyCardData _TargetData)
    {
        if (!Is_ExistEssentialID(_TargetData) || GottenAllyCards.Contains(_TargetData.EssentialID))
            return true;

        return false;
    }

    // 선택 가능한 ID 카드를 체크
    private bool Can_ChoiceAble(AllyCardData _TargetData, List<AllyCardData> _AlreadyChoicedDataList)
    {
        if (!GottenAllyCards.Contains(_TargetData.ID) &&
            Is_GottenEssentialCard(_TargetData) &&
            !_AlreadyChoicedDataList.Contains(_TargetData))
            return true;

        return false;
    }

    #endregion

    #region Get

    public AllyCardData Get_PreAllyCardData(AllyCardData _TargetCard)
    {
        if (_TargetCard.EssentialID == -1)
            return null;

        return AllAllyCardData[_TargetCard.EssentialID];
    }

    // 한 번에 여러개의 랜덤 카드 리턴
    public List<AllyCardData> Get_ChoiceAbleRandomData(int _LimitAmount)
    {
        List<AllyCardData> result = new List<AllyCardData>();

        int i = 0;
        while (true)
        {
            AllyCardData randomData = AllAllyCardData[Random.Range(0, AllAllyCardData.Count)];

            if (Can_ChoiceAble(randomData, result))
                result.Add(randomData);
            else
                continue;

            if (result.Count >= _LimitAmount)
                break;

            i++;
            if (i > 30)
                break;
        }
        for (int j = 0; j < result.Count; j++) Debug.Log(result[j].ID);
        return result;
    }

    // 한 번에 한개의 랜덤 카드 리턴


    #endregion

    #region Add

    public void Add_AllyCard(int _ID)
    {
        GottenAllyCards.Add(_ID);
    }

    #endregion
}
