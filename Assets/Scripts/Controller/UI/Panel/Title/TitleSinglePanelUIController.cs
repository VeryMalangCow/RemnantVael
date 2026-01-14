using UnityEngine;

public class TitleSinglePanelUIController : UIController
{
    #region Value

    [SerializeField] protected TitleOwnBtnEUIController CurrentBtn = null;

    #endregion


    #region Btn

    public virtual void Set_CurrentBtn(TitleOwnBtnEUIController _TargetBtn)
    {
        CurrentBtn = _TargetBtn;
    }

    #endregion
}
