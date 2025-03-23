using System.Collections.Generic;
using UnityEngine;

public class ModuleItemManager : Singleton<ModuleItemManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Module Item")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public List<ItemData> ItemDataList;
    [SerializeField] private List<MainChipData> MainChipDataList;

    [Space(10)]
    [Header("=== Resource")]
    [SerializeField] private List<Sprite> RankIconList;
    [SerializeField] private List<Sprite> MUUIDescRankIconList;
    [Space(5)]
    [SerializeField] public GameObject InventoryItemPrefab;
    [SerializeField] public GameObject InventorySlotPrefab;

    #region - Hide

    // Module State
    [HideInInspector] private List<List<ModuleState>> AllModuleData = new List<List<ModuleState>>();
    
    [HideInInspector] private List<CoupleData<int>> EquipedIndex = new List<CoupleData<int>>();

    [HideInInspector] private CoupleData<int> DescompositionIndex = new CoupleData<int>(-1, -1);
    [HideInInspector] private List<CoupleData<int>> FusionIndex = new List<CoupleData<int>>();
    [HideInInspector] private CoupleData<int> UpgradeIndex = new CoupleData<int>(-1, -1);


    // Main Chip
    [HideInInspector] private Dictionary<int, int> MainChopAmalgamationDict = new Dictionary<int, int>();


    [HideInInspector] public static readonly int RowAmount = 5;
    [HideInInspector] public static readonly int ColumnAmount = 20;

    [HideInInspector] public static readonly int EquipedAmount = 6;

    #endregion

    #endregion

    #region - Interface

    private List<IWhen_Hit> IWhen_HitList = new List<IWhen_Hit>();
    private List<IWhen_CriticalHit> IWhen_CriticalHitList = new List<IWhen_CriticalHit>();
    private List<IWhen_Fire> IWhen_FireList = new List<IWhen_Fire>();

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        // 모든 MS List를 Null 값을 사용해 빈 공간을 지정
        for (int i = 0; i < ColumnAmount; i++)
        {
            List<ModuleState> eachColumnMSList = new List<ModuleState>();
            for (int j = 0; j < RowAmount; j++)
            {
                eachColumnMSList.Add(null);
            }
            AllModuleData.Add(eachColumnMSList);
        }

        // 인덱스도 초기화
        for (int i = 0; i < EquipedAmount; i++)
            EquipedIndex.Add(new CoupleData<int>(-1, -1));

        for (int i = 0; i < 2; i++)
            FusionIndex.Add(new CoupleData<int>(-1, -1));
    }


    #endregion

    #region Reset

    private void Reset_ClearInterface()
    {
        IWhen_HitList.Clear();
        IWhen_FireList.Clear();
        IWhen_CriticalHitList.Clear();
    }

    public void Reset_Interface()
    {
        Reset_ClearInterface();

        /*foreach (ModuleState MS in Equiped_MSList)
        {
            if (MS is IWhen_Hit iHit && !iWhen_HitList.Contains(iHit))
            { iWhen_HitList.Add(iHit); }
            if (MS is IWhen_Fire iFire && !iWhen_FireList.Contains(iFire))
            { iWhen_FireList.Add(iFire); }
            if (MS is IWhen_CriticalHit iCriticalHit && !iWhen_CriticalHitList.Contains(iCriticalHit))
            { iWhen_CriticalHitList.Add(iCriticalHit); }
        }*/
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        Offset();
    }

    #endregion

    #region Get

    // 랜덤한 아이템
    public ItemData_Field Get_RandomInteractItem()
    {
        return new ItemData_Field(ItemDataList[Random.Range(0, ItemDataList.Count)]);
    }

    // 비어있는 ModuleState (Gotten) 찾기
    public CoupleData<int> Get_EmptyModuleState()
    {
        for (int i = 0; i < ColumnAmount; i++)
        {
            for (int j = 0; j < RowAmount; j++)
            {
                if (AllModuleData[i][j] == null)
                {
                    return new CoupleData<int>(i, j);
                }
            }
        }

        return new CoupleData<int>(-1, -1);
    }

    #endregion

    #region Gain

    // 아이템 상호작용해 획득
    public void Gain_ModuleState(ItemData_Field _ItemDataField)
    {
        // 빈 공간이 있어야 획득 가능
        CoupleData<int> index = Get_EmptyModuleState();
        if (index.TypeBase == -1 || index.TypeSpecial == -1) return;
       
        // 아이템 데이터 초기화
        ModuleState newModuleState = ModuleState.Get_AllModuleState()[_ItemDataField.ID];
        newModuleState.Set_State(ItemDataList[_ItemDataField.ID]);

        ItemData_UIVisual stateUI = new ItemData_UIVisual(
            Get_CorrectItemIcon(_ItemDataField.ID),
            _ItemDataField.Rank,
            _BoostLv: 0);

        AllModuleData[index.TypeBase][index.TypeSpecial] = newModuleState;

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Inventory_InEquip.Set_InventoryUI(AllModuleData);
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Inventory_InForge.Set_InventoryUI(AllModuleData);
    }

    #endregion



    #region Interface

    public void Active_Hit(EnemyController _EC)
    {
        if (IWhen_HitList.Count > 0)
        {
            for (int i = 0; i < IWhen_HitList.Count; i++)
            {
                IWhen_HitList[i].Play_When(_EC);
            }
        }
    }

    public void Active_CriticalHit(EnemyController _EC)
    {
        if (IWhen_CriticalHitList.Count > 0)
        {
            for (int i = 0; i < IWhen_CriticalHitList.Count; i++)
            {
                IWhen_CriticalHitList[i].Play_When(_EC);
            }
        }
    }

    public void Active_Fire()
    {
        if (IWhen_FireList.Count > 0)
        {
            for (int i = 0; i < IWhen_FireList.Count; i++)
            {
                IWhen_FireList[i].Play_When();
            }
        }
    }

    #endregion

    #region Find

    public Sprite Get_CorrectItemIcon(int _ID)
    {
        return ItemDataList[_ID].ItemIcon;
    }    
    public Sprite Get_CorrectRankIcon(int _Rank)
    {
        return RankIconList[_Rank - 1];
    }

    public Sprite Get_CorrectMUUIDescRankIcon(ModuleState _MS)
    {
        return MUUIDescRankIconList[_MS.ThisItemData.Rank - 1];
    }

    #endregion

    #region Forge

    // 분해
    public static int Get_MS_ByDescomposition(ModuleState _ModuleState)
    {
        return (_ModuleState.ThisItemData.Rank * 2);
    }

    public static int Get_BC_ByDescomposition(ModuleState _ModuleState)
    {
        return _ModuleState.ThisItemData.BoostLv;
    }

    // 합성
    public static int Get_MC_ForFusion(ModuleState _ModuleState)
    {
        return (_ModuleState.ThisItemData.Rank + 1);
    }

    // 업글
    public static int Get_EC_ForUpgrade(ModuleState _ModuleState)
    {
        return (_ModuleState.ThisItemData.BoostLv + 1);
    }

    


    #endregion

    #region MainChip

    public MainChipData Get_CorrectMainChip(int _ID)
    {
        for (int i = 0; i < MainChipDataList.Count; i++)
        {
            if (MainChipDataList[i].ID == _ID)
            {
                return MainChipDataList[i];
            }
        }
        return null;
    }

    public void Set_MainChipData()
    {
        /*
        MainChopAmalgamationDict = new Dictionary<int, int>();
        if (Equiped_MSList.Count > 0)
        {
            for (int i = 0; i < Equiped_MSList.Count; i++)
            {
                if (Equiped_MSList == null) continue;

                int synergyID_1 = Equiped_MSList[i].ThisItemData.R1_MainChipID;
                int synergyID_3 = Equiped_MSList[i].ThisItemData.R3_MainChipID;
                int synergyID_5 = Equiped_MSList[i].ThisItemData.R5_MainChipID;

                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 0.5f;
                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 0.5f;
                MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[2].alpha = 0.5f;

                if (Equiped_MSList[i].ThisItemData.Rank >= 5)
                {
                    Add_MainChipData(synergyID_1, 3);
                    Add_MainChipData(synergyID_3, 2);
                    Add_MainChipData(synergyID_5, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[2].alpha = 1;
                }
                else if (Equiped_MSList[i].ThisItemData.Rank >= 3)
                {
                    Add_MainChipData(synergyID_1, 2);
                    Add_MainChipData(synergyID_3, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[1].alpha = 1;

                }
                else
                {
                    Add_MainChipData(synergyID_1, 1);

                    MainGameUIManager.Instance.ModuleUpgrade_UIController.AmalgamationDescTxtList[0].alpha = 1;
                }
            }
        }
        if (MainGameUIManager.Instance.ModuleUpgrade_UIController != null)
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_SynergySlots(MainChopAmalgamationDict);
        }
*/
    }

    public void Add_MainChipData(int _SynergyID, int _Amount)
    {
        if (MainChopAmalgamationDict.ContainsKey(_SynergyID))
        { MainChopAmalgamationDict[_SynergyID] += _Amount; }
        else
        { MainChopAmalgamationDict.Add(_SynergyID, _Amount); }
    }

    #endregion
}