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

    public List<AllyCardData> Get_ChoiceAbleRandomData(int _LimitAmount)
    {
        List<AllyCardData> result = new List<AllyCardData>();
        while (true)
        {
            AllyCardData randomData = AllAllyCardData[Random.Range(0, AllAllyCardData.Count)];

            if (Can_ChoiceAble(randomData, result))
                result.Add(randomData);

            if (result.Count >= _LimitAmount)
                break;
        }
        return result;
    }

    #endregion
}
