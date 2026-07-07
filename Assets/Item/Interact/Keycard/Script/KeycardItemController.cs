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

        var reso = StaticResourceManager.instance.ItemReso;
        var icons = reso.keycardIcons;

        thisSr.sprite = icons[id].sprite;
        animSr.color = icons[id].clr;
        DevTool.Set_Anim(ref aoc, at, reso.keycardOutlineAnimation);
        at.speed = 1.1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = true;
        return StaticResourceManager.instance.staticWords.GetLanguage(117);
    }

    public override void PlayInteract()
    {
        base.PlayInteract();

        PlayerManager.instance.playerController.GainKeyCard(id);
        DropItemManager.instance.RemoveKeycard(this);
    }

    #endregion
}
