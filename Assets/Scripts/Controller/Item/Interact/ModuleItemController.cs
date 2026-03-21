using UnityEngine;
using UnityEngine.Serialization;

public class ModuleItemController : InteractItemController
{
    #region Value

    [Space(10)]
    [Header("=== State")]
    [FormerlySerializedAs("ItemDataField")][SerializeField] public ItemData_Field itemDataField;

    #endregion

    #region State

    public override void Set_State(Vector2 spawnPos)
    {
        base.Set_State(spawnPos);

        // Data
        itemDataField = new ItemData_Field(ModuleItemManager.instance.Get_RandomInteractItem());
    }

    public void Set_RankState(int rank)
    {
        itemDataField.rank = rank;
        DevTool.Set_Anim(ref aoc, at, ResourceManager.instance.Get_ModuleOutlineAC(rank - 1));
        at.speed = 1.5f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return ResourceManager.instance.Get_StaticWord(0);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        ModuleItemManager.instance.Gain_ModuleState(itemDataField);
        PoolingManager.instance.moduleItems.Enqueue(this);
    }

    #endregion
}
