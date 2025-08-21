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

        ThisSR.sprite = MainGameUIManager.Instance.Get_KeyCardSprite(ID);
        AnimSR.color = UnitManager.Instance.KeycardItemOutlinerColorList[ID];
        DevTool.Set_Anim(ref AOC, ThisAT, UnitManager.Instance.KeycardItemOutlinerAC);
        ThisAT.speed = 1.1f;
    }

    #endregion

    #region Interact

    public override void Play_Interact()
    {
        base.Play_Interact();

        PlayerManager.Instance.Gain_KeyCard(ID);
        PoolingManager.Instance.KeycardItems.Queue.Enqueue(this);
    }

    #endregion
}
