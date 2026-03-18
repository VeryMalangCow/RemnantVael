using UnityEngine;

public class KeycardItemController : InteractItemController
{
    #region Value

    [SerializeField] private SpriteRenderer AnimSR;

    #endregion

    #region State

    public void Set_TypeState(int _ID)
    {
        id = _ID;

        ThisSR.sprite = ResourceManager.instance.Get_KeyCardSprite(id);
        AnimSR.color = ResourceManager.instance.Get_KeycardColor(id);
        DevTool.Set_Anim(ref AOC, ThisAT, ResourceManager.instance.keycardOutlineAC);
        ThisAT.speed = 1.1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;
        return ResourceManager.instance.Get_StaticWord(117);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        PlayerManager.instance.Gain_KeyCard(id);
        PoolingManager.instance.keycardItems.Enqueue(this);
    }

    #endregion
}
