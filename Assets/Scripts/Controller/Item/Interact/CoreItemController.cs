using UnityEngine;

public class CoreItemController : InteractItemController
{
    #region Value

    [SerializeField] private SpriteRenderer AnimSR;

    #endregion

    #region State

    public void Set_TypeState(int _ID)
    {
        ID = _ID;

        DevTool.Set_Anim(ref AOC, ThisAT, UnitManager.Instance.CoreItemOutlinerAC);
        ThisSR.sprite = UnitManager.Instance.Get_CoreSprite(ID);
        ThisAT.speed = 1f;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = true;

        switch (ID)
        {
            case 1:
                return ResourceManager.Instance.Get_StaticWord(118);
            case 2:
                return ResourceManager.Instance.Get_StaticWord(119);
            case 3:
                return ResourceManager.Instance.Get_StaticWord(120);
            default:
                return "";
        }
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        SaveDataManager.Instance.JsonData.Gain_Item(ID, 1);
        PoolingManager.Instance.CoreItems.Enqueue(this);
    }

    #endregion
}
