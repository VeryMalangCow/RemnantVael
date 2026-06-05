using UnityEngine;

public class KeycardItemController : InteractItemController
{
    #region Value

    [SerializeField] private SpriteRenderer animSr;

    #endregion

    #region State

    public void Set_TypeState(int id)
    {
        base.id = id;

        thisSr.sprite = ResourceManager.instance.Get_KeyCardSprite(base.id);
        animSr.color = ResourceManager.instance.Get_KeycardColor(base.id);
        DevTool.Set_Anim(ref aoc, at, ResourceManager.instance.keycardOutlineAC);
        at.speed = 1.1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return ResourceManager.instance.Get_StaticWord(117);
    }

    public override void PlayInteract()
    {
        base.PlayInteract();

        PlayerManager.instance.Gain_KeyCard(id);
        DropItemManager.instance.RemoveKeycard(this);
    }

    #endregion
}
