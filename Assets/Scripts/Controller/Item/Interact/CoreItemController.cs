using UnityEngine;

public class CoreItemController : InteractItemController
{
    #region Value

    [SerializeField] private SpriteRenderer AnimSR;

    #endregion

    #region State

    public void Set_TypeState(int _ID)
    {
        id = _ID;

        DevTool.Set_Anim(ref AOC, ThisAT, ResourceManager.instance.coreOutlineAC);
        ThisSR.sprite = ResourceManager.instance.Get_CoreSprite(id);
        ThisAT.speed = 1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;

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

    public override void Play_Interact()
    {
        base.Play_Interact();

        SaveDataManager.instance.jsonData.Gain_Item(id, 1);
        PoolingManager.instance.coreItems.Enqueue(this);
    }

    #endregion
}
