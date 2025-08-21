using UnityEngine;

public class ModuleItemController : InteractItemController
{
    #region Value

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public ItemData_Field ItemDataField;

    #endregion

    #region State

    public override void Set_State(Vector2 _SpawnPos)
    {
        base.Set_State(_SpawnPos);

        // Data
        ItemDataField = new ItemData_Field(ModuleItemManager.Instance.Get_RandomInteractItem());
    }

    public void Set_RankState(int _Rank)
    {
        ItemDataField.Rank = _Rank;
        DevTool.Set_Anim(ref AOC, ThisAT, UnitManager.Instance.ModuleItemOutlinerAC[_Rank - 1]);
        ThisAT.speed = 1.5f;
    }

    #endregion

    #region Interact

    public override void Play_Interact()
    {
        base.Play_Interact();

        ModuleItemManager.Instance.Gain_ModuleState(ItemDataField);
        PoolingManager.Instance.ModuleItems.Queue.Enqueue(this);
    }

    #endregion
}
