using UnityEngine;

public class CoreItemController : InteractItemController
{
    #region Value

    [SerializeField] private SpriteRenderer animSr;

    #endregion

    #region State

    public void Set_TypeState(int id)
    {
        base.id = id;

        DevTool.Set_Anim(ref aoc, at, ResourceManager.instance.coreOutlineAC);
        thisSr.sprite = ResourceManager.instance.Get_CoreSprite(base.id);
        at.speed = 1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = true;

        switch (id)
        {
            case 1:
                return ResourceManager.instance.Get_StaticWord(118);
            case 2:
                return ResourceManager.instance.Get_StaticWord(119);
            case 3:
                return ResourceManager.instance.Get_StaticWord(120);
            default:
                return "";
        }
    }

    public override void PlayInteract()
    {
        base.PlayInteract();

        SaveDataManager.instance.jsonData.Gain_Item(id, 1);
        DropItemManager.instance.RemoveCore(this);
    }

    #endregion
}
