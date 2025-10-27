using UnityEngine;

public class KeycardItemController : InteractItemController
{
    #region Value

    [SerializeField] private SpriteRenderer AnimSR;

    #endregion

    #region State

    public void Set_TypeState(int _ID)
    {
        ID = _ID;

        ThisSR.sprite = ResourceManager.Instance.Get_KeyCardSprite(ID);
        AnimSR.color = ResourceManager.Instance.Get_KeycardColor(ID);
        DevTool.Set_Anim(ref AOC, ThisAT, ResourceManager.Instance.KeycardOutlineAC);
        ThisAT.speed = 1.1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;
        return ResourceManager.Instance.Get_StaticWord(117);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        PlayerManager.Instance.Gain_KeyCard(ID);
        PoolingManager.Instance.KeycardItems.Enqueue(this);
    }

    #endregion
}
