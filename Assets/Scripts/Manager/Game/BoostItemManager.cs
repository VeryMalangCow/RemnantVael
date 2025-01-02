using System.Collections.Generic;
using UnityEngine;

public class BoostItemManager : Singleton<BoostItemManager>
{
    #region Value

    [Header("=== All Type")]
    [SerializeField] public List<ItemData> ItemDataList;

    [Header("=== Gotten Item")]
    [HideInInspector] private List<PassiveSkill> Gotten_PSList = new List<PassiveSkill>();
    [HideInInspector] public List<PassiveSkill> Equiped_PSList = new List<PassiveSkill>();


    [Header("=== Icon Data")]
    [SerializeField] private List<Sprite> RankIconList;
    [SerializeField] private List<Sprite> MUUIDescRankIconList;

    // Interface
    private List<IWhen_Always> iWhen_AlwaysList = new List<IWhen_Always>();
    public List<IWhen_Fire> iWhen_FireList = new List<IWhen_Fire>();

    #endregion

    #region Field

    public ItemData GetRandomInteractItem()
    {
        return ItemDataList[Random.Range(0, ItemDataList.Count)];
    }

    public void GetItemSkill(ItemData _ItemData)
    {
        foreach (PassiveSkill PIS in PassiveSkill.AllPassiveItemSkill())
        {
            if (PIS.ThisItemData.ID == _ItemData.ID) 
            {
                PIS.ThisItemData = new ItemData(_ItemData);

                PIS.ThisMEII = MainGameUIManager.Instance.ModuleUpgrade_UIController.SpawnMEIIList(PIS.ThisItemData.ItemIcon, GetCorrectRankIcon(PIS), PIS.ThisItemData.BoostLv);
                
                foreach(ModifyEachInventoryItem MEII in PIS.ThisMEII)
                {
                    MEII.gameObject.name = $"{PIS.ThisItemData.ID}_{PIS.ThisItemData.Rank}_{PIS.ThisItemData.BoostLv}";
                }

                Gotten_PSList.Add(PIS);
            }
        }
    }

    public void ResetInterface()
    {
        foreach(PassiveSkill PS in Equiped_PSList)
        {
            if (PS is IWhen_Always iGet)
            { iWhen_AlwaysList.Add(iGet); }
            if (PS is IWhen_Fire iFire)
            { iWhen_FireList.Add(iFire); }
        }
    }

    #endregion

    #region Interface

    public void ActiveSkill_Always()
    {
        foreach (IWhen_Always fire in iWhen_AlwaysList)
        {
            fire.When_Always();
        }
    }

    public void ActiveSkill_Fire()
    {
        foreach (IWhen_Fire fire in iWhen_FireList)
        {
            fire.When_Fire();
        }
    }

    #endregion

    #region Find

    public PassiveSkill GetPassiveSkill_Equiped(ModifyEachInventoryItem _MEII)
    {
        foreach (PassiveSkill PS in Gotten_PSList)
        {
            if (PS.ThisExtraMEII.Contains(_MEII))
            {
                return PS;
            }
        }

        return null;
    }

    public PassiveSkill GetPassiveSkill_Inventory(ModifyEachInventoryItem _MEII)
    {
        foreach(PassiveSkill PS in Gotten_PSList)
        {
            if (PS.ThisMEII.Contains(_MEII))
            {
                return PS;
            }
        }

        return null;
    }


    public Sprite GetCorrectRankIcon(PassiveSkill _PS)
    {
        return RankIconList[_PS.ThisItemData.Rank - 1];
    }
    public Sprite GetCorrectMUUIDescRankIcon(PassiveSkill _PS)
    {
        return MUUIDescRankIconList[_PS.ThisItemData.Rank - 1];
    }

    #endregion

    #region Decomposition

    public int NeedEC_AbleUpgrade(ModifyEachInventoryItem _MEII)
    {
        if (_MEII == null)
        { return 0; }

        PassiveSkill ps = GetPassiveSkill_Inventory(_MEII);
        if (ps != null)
        {
            return (ps.ThisItemData.BoostLv + 1);
        }
        return 0;
    }

    public int NeedMC_AbleFusion(ModifyEachInventoryItem _MEII)
    {
        if (_MEII == null)
        { return 0; }

        PassiveSkill ps = GetPassiveSkill_Inventory(_MEII);
        if (ps != null)
        {
            return (ps.ThisItemData.Rank + 1);
        }
        return 0;
    }


    #endregion

    #region Delete

    public void DeletePassiveSkill(ModifyEachInventoryItem _MEII)
    {
        PassiveSkill foundPS = FindPassiveSkill(Gotten_PSList, _MEII);

        if (Gotten_PSList.Contains(foundPS))
        { Gotten_PSList.Remove(foundPS); }

        if (Equiped_PSList.Contains(foundPS))
        { Equiped_PSList.Remove(foundPS); }

        DestroyMEIIList(foundPS);
        foundPS = null;
    }

    private PassiveSkill FindPassiveSkill(List<PassiveSkill> TargetPsList, ModifyEachInventoryItem _MEII)
    {
        foreach(PassiveSkill ps in TargetPsList)
        {
            if (ps.ThisMEII.Contains(_MEII))
            {
                return ps;
            }
        }
        return null;
    }

    private void DestroyMEIIList(PassiveSkill _PS)
    {
        for (int i = _PS.ThisMEII.Count - 1; i >= 0; i--)
        { Destroy(_PS.ThisMEII[i].gameObject); }

        for (int i = _PS.ThisExtraMEII.Count - 1; i >= 0; i--)
        { Destroy(_PS.ThisExtraMEII[i].gameObject); }
    }

    #endregion

    #region ModuleItem

    public void Spawn_MI_000(int _Rank, int _BoostLv)
    {
        // 편의성
        PlayerController PC = PlayerManager.Instance.PlayerController;
        PlayerWeaponController PCWeapon = PC.BaseWeapon;

        // 확률
        if ((_Rank * _BoostLv) > UnityEngine.Random.Range(0, 100))
        {
            Debug.Log("스폰");
            // 데미지 계산
            float dmg = _Rank * PCWeapon.BaseDamage.ActualState.Value;
            PlayerBulletController pbc = PoolingManager.Instance.GetOP_MI_000_Bullets();
            Vector2 dir = PCWeapon.GetDir(PC.transform.position);
            // 스폰 탄 스탯
            BulletState bulletState = new BulletState(
                eDamageType.Energy,
                dmg, PCWeapon.MuzzleSpeed.ActualState.Value * 0.7f, 2,
                false, 1,
                false, 0, 0);
            pbc.SetState(PC.transform.position, 10, bulletState, dir, 0.35f);
            // Sorting Layer
            if (PC.TargetObject.gameObject.TryGetComponent(out HaveShadowThing hst))
            { pbc.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }
        }

    }

    #endregion
}

[System.Serializable]
public class ItemData
{
    public int ID;
    public string Name;
    public string Description;
    public string EquipDescription;
    public Sprite ItemIcon;

    [Space(10)]

    public int BoostLv = 1;
    public int Rank = 1;

    public ItemData() { }
    public ItemData(ItemData _ItemData)
    {
        ID = _ItemData.ID;
        Name = _ItemData.Name;
        Description = _ItemData.Description;
        EquipDescription = _ItemData.EquipDescription;
        ItemIcon = _ItemData.ItemIcon;
        BoostLv = _ItemData.BoostLv;
        Rank = _ItemData.Rank;
    }
}
