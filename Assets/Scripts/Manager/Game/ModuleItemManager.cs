using System.Collections.Generic;
using UnityEngine;

public class ModuleItemManager : Singleton<ModuleItemManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Module Item")]

    [Space(10)]
    [Header("=== Resource")]
    [SerializeField] private List<Sprite> RankIconList;
    [SerializeField] private List<Sprite> MUUIDescRankIconList;

    [Space(5)]
    [SerializeField] public GameObject InventoryItemPrefab;
    [SerializeField] public GameObject InventorySlotPrefab;

    #endregion

    #region - Hide

    // Data
    [HideInInspector] public List<ItemData> ItemDataList;
    [HideInInspector] private List<MainChipData> MainChipDataList;

    // Module State
    [HideInInspector] private List<List<ModuleState>> AllModuleData = new List<List<ModuleState>>();
    
    [HideInInspector] private List<CoupleData<int>> EquippedIndex = new List<CoupleData<int>>();

    [HideInInspector] private CoupleData<int> DecompositionIndex = new CoupleData<int>(-1, -1);
    [HideInInspector] private List<CoupleData<int>> FusionIndex = new List<CoupleData<int>>();

    // Main Chip
    [HideInInspector] private Dictionary<int, int> MainChipAmalgamationDict = new Dictionary<int, int>();
    [SerializeField] private List<SynchoronyState> CurrentAllMainChipState = new List<SynchoronyState>();

    [HideInInspector] public static readonly int RowAmount = 5;
    [HideInInspector] public static readonly int ColumnAmount = 20;

    [HideInInspector] public static readonly int EquipedAmount = 6;

    [HideInInspector] public static int SynchoronyOneTierRange = 6;
    [HideInInspector] public static int SynchoronyMaxLv = 3;

    #endregion

    #region - Interface

    private List<IWhen_Hit> IWhen_HitList = new List<IWhen_Hit>();
    private List<IWhen_CriticalHit> IWhen_CriticalHitList = new List<IWhen_CriticalHit>();
    private List<IWhen_Fire> IWhen_FireList = new List<IWhen_Fire>();

    private List<IWhenAlly_Fire> IWhenAlly_FireList = new List<IWhenAlly_Fire>();

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        ItemDataList = new List<ItemData>();
        for (int i = 0; i < ResourceManager.Instance.Get_AllModuleItemAmount(); i++)
            ItemDataList.Add(ResourceManager.Instance.Get_ItemData(i));

        MainChipDataList = new List<MainChipData>();
        for (int i = 0; i < ResourceManager.Instance.Get_AllModuleSynchronyAmount(); i++)
            MainChipDataList.Add(ResourceManager.Instance.Get_MainChipData(i));
        

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
            EquippedIndex.Add(new CoupleData<int>(-1, -1));

        for (int i = 0; i < 2; i++)
            FusionIndex.Add(new CoupleData<int>(-1, -1));
    }


    #endregion

    #region Reset

    private void Clear_InterfaceMU()
    {
        IWhen_HitList.Clear();
        IWhen_FireList.Clear();
        IWhen_CriticalHitList.Clear();
    }

    private void Clear_InterfaceMC()
    {
        CurrentAllMainChipState.Clear();

        IWhenAlly_FireList.Clear();
    }


    private void Reset_Interface()
    {
        Reset_InterfaceMU();
        Reset_InterfaceMC();
    }

    private void Reset_InterfaceMU()
    {
        Clear_InterfaceMU();

        for (int i = 0; i < EquippedIndex.Count; i++)
        {
            CoupleData<int> colRow = EquippedIndex[i];
            if (colRow.TypeBase == -1 || colRow.TypeSpecial == -1)
            {
                continue; 
            }
            else
            {
                ModuleState ms = Get_EquippedModuleState(i);

                if (ms is IWhen_Hit iHit) DevTool.Add_InList(IWhen_HitList, iHit);
                if (ms is IWhen_Fire iFire) DevTool.Add_InList(IWhen_FireList, iFire);
                if (ms is IWhen_CriticalHit iCriticalHit) DevTool.Add_InList(IWhen_CriticalHitList, iCriticalHit);
            }
        }
    }

    private void Reset_InterfaceMC()
    {
        Clear_InterfaceMC();

        Set_MainChipData();

        for (int i = 0; i < CurrentAllMainChipState.Count; i++)
        {
            if (CurrentAllMainChipState[i] is IWhenAlly_Fire iFire)
                DevTool.Add_InList(IWhenAlly_FireList, iFire);
        }
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

    #region ItemData

    // 랜덤한 아이템
    public ItemData_Field Get_RandomInteractItem()
    {
        return new ItemData_Field(ItemDataList[Random.Range(0, ItemDataList.Count)]);
    }

    #endregion

    #region Module

    // 선택한 아이템의 모듈 스탯 찾기
    public ModuleState Get_ModuleState(int _Col, int _Row)
    {
        return AllModuleData[_Col][_Row];
    }

    public ModuleState Get_ModuleState(CoupleData<int> _Index)
    {
        return AllModuleData[_Index.TypeBase][_Index.TypeSpecial];
    }

    public ModuleState Get_ModuleState(InventoryItemEUIController _ItemEUI)
    {
        return AllModuleData[_ItemEUI.ThisSlot.Col][_ItemEUI.ThisSlot.Row];
    }

    #endregion

    #region Equipped

    // 비어있는 장착 ModuleState 찾기
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

    // 비어 있는 장착 슬롯 인덱스 찾기
    public int Get_EmptyEquippedIndex(CoupleData<int> _Exclude)
    {
        for (int i = 0; i < EquippedIndex.Count; i++)
        {
            if (EquippedIndex[i].TypeBase == _Exclude.TypeBase &&
                EquippedIndex[i].TypeSpecial == _Exclude.TypeSpecial)
                return -1;

            if (EquippedIndex[i].TypeBase == -1 && EquippedIndex[i].TypeSpecial == -1)
                return i;
        }
        return -1;
    }


    // 모든 장착 모듈 스탯 (빈 공간도 포함)
    public ModuleState Get_EquippedModuleState(int _Index)
    {
        CoupleData<int> colRow = EquippedIndex[_Index];
        return Get_ModuleState(colRow.TypeBase, colRow.TypeSpecial);
    }

    // 장착되어 있는 아이템의 인덱스들만
    public List<CoupleData<int>> Get_OnlyEquippedIndex()
    {
        List<CoupleData<int>> result = new List<CoupleData<int>>();
        for (int i = 0; i < EquippedIndex.Count; i++)
            if (EquippedIndex[i].TypeBase != -1 && EquippedIndex[i].TypeSpecial != -1)
                result.Add(EquippedIndex[i]);

        return result;
    }

    #endregion

    #region MainChip

    // 메인칩 데이터 찾기
    public MainChipData Get_CorrectMainChip(int _ID)
    {
        return MainChipDataList[_ID];
    }

    // 현재 메인 칩의 양 구하기
    public int Get_SynchronyAmount(int _ID)
    {
        return MainChipAmalgamationDict[_ID];
    }

    // 메인 칩의 랭크 가져오기
    // 1~6 / 7~12 / 13~18
    public int Get_SynchronyRank(int _Amalgamation)
    {
        return (_Amalgamation - 1) > 0 ? _Amalgamation / SynchoronyOneTierRange : 0;
    }

    // 모든 메인 칩 딕셔너리로 메인칩스탯 리스트 반환
    private List<SynchoronyState> Get_CurrentMainChipState()
    {
        List<SynchoronyState> result = new List<SynchoronyState>();

        foreach(KeyValuePair<int, int> mainChipAmalgamation in MainChipAmalgamationDict) // ID, Amount
        {
            int id = mainChipAmalgamation.Key;
            int rank = Get_SynchronyRank(mainChipAmalgamation.Value);
            if (rank <= 0) continue;

            SynchoronyState mcs = SynchoronyState.Get_AllSynchoronyState()[id];
            mcs.Set_State(id, rank);

            result.Add(mcs);
        }

        return result;
    }

    #endregion

    #region Sprite

    public Sprite Get_CorrectItemIcon(int _ID)
    {
        return ItemDataList[_ID].ItemIcon;
    }
    public Sprite Get_CorrectRankIcon(int _Rank)
    {
        return RankIconList[_Rank - 1];
    }
    public Sprite Get_CorrectDescRankIcon(int _Rank)
    {
        return MUUIDescRankIconList[_Rank - 1];
    }

    #endregion

    #region Forge

    // 분해 슬롯의 인덱스
    public CoupleData<int> Get_DecompositionIndex()
    {
        return new CoupleData<int>(DecompositionIndex);
    }

    // 합성 슬롯 인덱스
    public List<CoupleData<int>> Get_FusionIndex()
    {
        List<CoupleData<int>> result = new List<CoupleData<int>>();
        for (int i = 0; i < FusionIndex.Count; i++)
            result.Add(FusionIndex[i]);

        return result;
    }

    #endregion

    #region Item

    public static int Get_MS_ByDecomposition(ModuleState _ModuleState)
    {
        return (_ModuleState.ThisItemData.Rank * 2);
    }

    public static int Get_BC_ByDescomposition(ModuleState _ModuleState)
    {
        return _ModuleState.ThisItemData.Rank;
    }

    public static int Get_MS_ForFusion(ModuleState _ModuleState)
    {
        return (_ModuleState.ThisItemData.Rank + 1);
    }

    public static int Get_MS_ForMake()
    {
        return 7;
    }

    public static int Get_CB_ForMake()
    {
        return 4;
    }

    #endregion

    #endregion

    #region Set

    #region Inventory

    public void Set_ChangeInventorySlot(CoupleData<int> _Index0, CoupleData<int> _Index1)
    {
        ModuleState moduleState0 = AllModuleData[_Index0.TypeBase][_Index0.TypeSpecial];
        ModuleState moduleState1 = AllModuleData[_Index1.TypeBase][_Index1.TypeSpecial];

        // 기본 벨류
        int listIndex0 = -1;
        int listIndex1 = -1;
        CoupleData<int> newIndex0 = new CoupleData<int>(-1, -1);
        CoupleData<int> newIndex1 = new CoupleData<int>(-1, -1);

        // 모듈 스탯이 실존하고, 장착 중인 것이라면
        if (moduleState0 != null && Is_IncludeEquipped(_Index0, out int _ListIndex0))
        {
            listIndex0 = _ListIndex0;
            newIndex0 = new CoupleData<int>(_Index1);
        }
        if (moduleState1 != null && Is_IncludeEquipped(_Index1, out int _ListIndex1))
        {
            listIndex1 = _ListIndex1;
            newIndex1 = new CoupleData<int>(_Index0);
        }

        if (listIndex0 != -1) EquippedIndex[listIndex0] = newIndex0;
        if (listIndex1 != -1) EquippedIndex[listIndex1] = newIndex1;

        AllModuleData[_Index0.TypeBase][_Index0.TypeSpecial] = moduleState1;
        AllModuleData[_Index1.TypeBase][_Index1.TypeSpecial] = moduleState0;

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData);
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);
        Reset_Interface();
    }

    // 장착되어 있는 아이템의 위치값이 변경됨

    #endregion

    #region Equip

    // 아이템 장착
    public void Set_Equip(int _EquipedIndex, CoupleData<int> _InteractIndex)
    {
        EquippedIndex[_EquipedIndex] = _InteractIndex;

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);
        
        Reset_Interface(); 
    }

    public void Set_UnEquip(int _EquipedIndex)
    {
        EquippedIndex[_EquipedIndex] = new CoupleData<int>(-1, -1);

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);

        Reset_Interface(); 
    }

    public void Set_SwitchEquipment(int _ListIndex0, int _ListIndex1)
    {
        CoupleData<int> temp = new CoupleData<int>(EquippedIndex[_ListIndex0]);
        EquippedIndex[_ListIndex0] = new CoupleData<int>(EquippedIndex[_ListIndex1]);
        EquippedIndex[_ListIndex1] = new CoupleData<int>(temp);

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData);
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);
        Reset_Interface();
    }

    #endregion

    #region Decomposition

    // 분해 슬롯 장착
    public void Set_DecompositionSlot(CoupleData<int> _InteractIndex)
    {
        DecompositionIndex = _InteractIndex;
    }

    public void Set_UnDecompositionSlot()
    {
        DecompositionIndex = new CoupleData<int>(-1, -1);
    }

    #endregion

    #region Fusion

    // 합성
    public void Set_FusionSlot(int _Index, CoupleData<int> _InteractIndex)
    {
        FusionIndex[_Index] = _InteractIndex;
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_FusionUI(AllModuleData, FusionIndex);
    }

    public void Set_UnFusionSlot(int _Index)
    {
        FusionIndex[_Index] = new CoupleData<int>(-1, -1);
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_FusionUI(AllModuleData, FusionIndex);
    }
    public void Set_UnFusionSlotAll()
    {
        for (int i = 0; i < FusionIndex.Count; i++)
            FusionIndex[i] = new CoupleData<int>(-1, -1);
    }
    public void Set_SwitchFusion(int _ListIndex0, int _ListIndex1)
    {
        CoupleData<int> temp = new CoupleData<int>(FusionIndex[_ListIndex0]);
        FusionIndex[_ListIndex0] = new CoupleData<int>(FusionIndex[_ListIndex1]);
        FusionIndex[_ListIndex1] = new CoupleData<int>(temp);

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData);
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_FusionUI(AllModuleData, FusionIndex);
        Reset_Interface();
    }

    #endregion

    #region Upgrade

    #endregion

    #endregion

    #region Is

    // 분해 슬롯
    public bool Is_EmptyDecompositionSlot()
    {
        return DecompositionIndex.TypeBase == -1 && DecompositionIndex.TypeSpecial == -1;
    }

    // 퓨전 슬롯
    public bool Is_EmptyFusionSlot()
    {
        for (int i = 0; i < FusionIndex.Count; i++)
        {
            if (FusionIndex[i].TypeBase == -1 && FusionIndex[i].TypeSpecial == -1)
            {
                return true;
            }
        }
        return false;
    }

    public bool Is_EmptyFusionSlot(out int _EmptyIndex)
    {
        for (int i = 0; i < FusionIndex.Count; i++)
        {
            if (FusionIndex[i].TypeBase == -1 && FusionIndex[i].TypeSpecial == -1)
            {
                _EmptyIndex = i;
                return true;
            }
        }
        _EmptyIndex = -1;
        return false;
    }

    public bool Is_EmptyFusionSlot(int _Index)
    {
        if (FusionIndex[_Index].TypeBase == -1 && FusionIndex[_Index].TypeSpecial == -1)
        {
            return true;
        }
        return false;
    }

    // 퓨전 스롯에 이미 가지고 있는가?
    public bool Is_IncludeFusionSlots(CoupleData<int> _Index)
    {
        return Is_Include(_Index, FusionIndex);
    }

    public bool Is_IncludeFusionSlots(CoupleData<int> _Index, out int _ListIndex)
    {
        bool result = Is_Include(_Index, FusionIndex, out int listIndex);
        _ListIndex = listIndex;
        return result;
    }

    // 모두 비었는가?
    public bool Is_AllEmptyFusionSlot()
    {
        bool result = true;
        for (int i = 0; i < FusionIndex.Count; i++)
        {
            if (FusionIndex[i].TypeBase != -1 || FusionIndex[i].TypeSpecial != -1)
            {
                result = false;
            }
        }
        return result;
    }

    // 퓨전 슬롯의 랭크가 같은가?
    public bool Is_SameRankFusionSlots()
    {
        for (int i = 1; i < FusionIndex.Count; i++)
        {
            // 랭크가 다르다면
            if (Get_ModuleState(FusionIndex[0].TypeBase, FusionIndex[0].TypeSpecial).ThisItemData.Rank !=
                Get_ModuleState(FusionIndex[i].TypeBase, FusionIndex[i].TypeSpecial).ThisItemData.Rank)
            {
                return false;
            }
        }
        return true;
    }

    // 장착된 인덱스들 중에서 포함되어있는지
    public bool Is_IncludeOnlyEquipped(CoupleData<int> _Index)
    {
        return Is_Include(_Index, Get_OnlyEquippedIndex());
    }

    // 모든 장착 인덱스 중에 포함되어 있는가
    public bool Is_IncludeEquipped(CoupleData<int> _Index, out int _ListIndex)
    {
        bool result = Is_Include(_Index, EquippedIndex, out int listIndex);
        _ListIndex = listIndex;
        return result;
    }

    // 이미 가지고 있는가
    public bool Is_Include(CoupleData<int> _Index, List<CoupleData<int>> _IndexList)
    {
        for (int i = 0; i < _IndexList.Count; i++)
        {
            if (_IndexList[i].TypeBase == _Index.TypeBase &&
                _IndexList[i].TypeSpecial == _Index.TypeSpecial)
            {
                return true;
            }
        }

        return false;
    }

    public bool Is_Include(CoupleData<int> _Index, List<CoupleData<int>> _IndexList, out int _IncludeListIndex)
    {
        for (int i = 0; i < _IndexList.Count; i++)
        {
            if (_IndexList[i].TypeBase == _Index.TypeBase &&
                _IndexList[i].TypeSpecial == _Index.TypeSpecial)
            {
                _IncludeListIndex = i;
                return true;
            }
        }

        _IncludeListIndex = -1;
        return false;
    }

    #endregion

    #region Rank

    public void Set_UpRank(CoupleData<int> _Index)
    {
        AllModuleData[_Index.TypeBase][_Index.TypeSpecial].ThisItemData.Rank++;
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData);
        Reset_Interface();
    }

    #endregion

    #region Remove

    public void Remove_ModuleState(CoupleData<int> _Index)
    {
        if (Get_ModuleState(_Index.TypeBase, _Index.TypeSpecial) == null) return;

        // 장착되어 있다면 제거
        if (Is_IncludeEquipped(_Index, out int listIndex))
            EquippedIndex[listIndex] = new CoupleData<int>(-1, -1);

        AllModuleData[_Index.TypeBase][_Index.TypeSpecial] = null;

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData); 
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);
        Reset_Interface();
    }

    public void Remove_ModuleState(List<CoupleData<int>> _IndexList)
    {
        for (int i = 0; i < _IndexList.Count; i++)
        {
            if (Get_ModuleState(_IndexList[i].TypeBase, _IndexList[i].TypeSpecial) == null) return;
            
            // 장착되어 있다면 제거
            if (Is_IncludeEquipped(_IndexList[i], out int listIndex))
                EquippedIndex[listIndex] = new CoupleData<int>(-1, -1);

            AllModuleData[_IndexList[i].TypeBase][_IndexList[i].TypeSpecial] = null;
        }

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData);
        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);
        Reset_Interface();
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
        newModuleState.ThisItemData.Rank = _ItemDataField.Rank;

        ItemData_UIVisual stateUI = new ItemData_UIVisual(
            Get_CorrectItemIcon(_ItemDataField.ID),
            _ItemDataField.Rank);

        AllModuleData[index.TypeBase][index.TypeSpecial] = newModuleState;

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_InventoryUI(AllModuleData);
    }

    // 아이템을 랜덤하게 획득
    public void Gain_ModuleState()
    {
        Gain_ModuleState(Get_RandomInteractItem());
    }

    #endregion

    #region Interface (Base)

    public void Active_Hit(EnemyController _EC)
    {
        if (IWhen_HitList.Count <= 0) return;

        for (int i = 0; i < IWhen_HitList.Count; i++)
            IWhen_HitList[i].Play_When(_EC);
    }

    public void Active_CriticalHit(EnemyController _EC)
    {
        if (IWhen_CriticalHitList.Count <= 0) return;

        for (int i = 0; i < IWhen_CriticalHitList.Count; i++)
            IWhen_CriticalHitList[i].Play_When(_EC);
    }

    public void Active_Fire()
    {
        if (IWhen_FireList.Count <= 0) return;

        for (int i = 0; i < IWhen_FireList.Count; i++)
            IWhen_FireList[i].Play_When();
    }


    #endregion

    #region Interface (Syn = Ally)

    public void AllyActive_Fire(BulletController _Bullet)
    {
        if (IWhenAlly_FireList.Count <= 0) return;

        for (int i = 0; i < IWhenAlly_FireList.Count; i++)
            IWhenAlly_FireList[i].Play_When(_AC: null, _Bullet);
    }


    #endregion

    #region MainChip

    private void Add_MainChipData(ModuleState _ModuleState)
    {
        int synergyID_1 = _ModuleState.ThisItemData.R1_MainChipID;
        int synergyID_3 = _ModuleState.ThisItemData.R3_MainChipID;
        int synergyID_5 = _ModuleState.ThisItemData.R5_MainChipID;

        if (_ModuleState.ThisItemData.Rank >= 5)
        {
            DevTool.Add_AmountForDict(ref MainChipAmalgamationDict, synergyID_1, 3);
            DevTool.Add_AmountForDict(ref MainChipAmalgamationDict, synergyID_3, 2);
            DevTool.Add_AmountForDict(ref MainChipAmalgamationDict, synergyID_5, 1);
        }
        else if (_ModuleState.ThisItemData.Rank >= 3)
        {
            DevTool.Add_AmountForDict(ref MainChipAmalgamationDict, synergyID_1, 2);
            DevTool.Add_AmountForDict(ref MainChipAmalgamationDict, synergyID_3, 1);
        }
        else
        {
            DevTool.Add_AmountForDict(ref MainChipAmalgamationDict, synergyID_1, 1);
        }
    }

    public void Set_MainChipData()
    {
        MainChipAmalgamationDict = new Dictionary<int, int>();
        for (int i = 0; i < EquippedIndex.Count; i++)
        {
            int col = EquippedIndex[i].TypeBase;
            int row = EquippedIndex[i].TypeSpecial;
            if (col != -1 && row != -1)
            {
                ModuleState moduleState = Get_ModuleState(col, row);
                if (moduleState != null) Add_MainChipData(moduleState);
            }
        }

        if (MainGameUIManager.Instance.ModuleUpgrade_UIController != null)
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_SynergySlots(MainChipAmalgamationDict);

        CurrentAllMainChipState = Get_CurrentMainChipState();
    }

    #endregion

    #region Set (Language)

    public void Set_DataLanguage()
    {
        Set_ItemDataLanguage();
        Set_MainChipDataLanguage();

        MainGameUIManager.Instance.ModuleUpgrade_UIController.Set_EquipedUI(AllModuleData, EquippedIndex);
    }

    private void Set_ItemDataLanguage()
    {
        for (int i = 0; i < ItemDataList.Count; i++)
            ResourceManager.Instance.Set_DataLanguage(ItemDataList[i], i);

        for (int i = 0; i < ColumnAmount; i++)
            for (int j = 0; j < RowAmount; j++)
                if (AllModuleData[i][j] != null)
                {
                    AllModuleData[i][j].ThisItemData.Set_LanguageTxt(ItemDataList[AllModuleData[i][j].ThisItemData.ID]);
                }
    }

    private void Set_MainChipDataLanguage()
    {
        for (int i = 0; i < MainChipDataList.Count; i++)
            ResourceManager.Instance.Set_DataLanguage(MainChipDataList[i], i);
    }


    #endregion
}