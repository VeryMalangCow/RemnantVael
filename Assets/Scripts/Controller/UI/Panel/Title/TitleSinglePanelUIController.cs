using UnityEngine;

public class TitleSinglePanelUIController : UIController
{
    #region Value

    [SerializeField] protected TitleOwnBtnEUIController currentBtn = null;

    #endregion


    #region Btn

    public virtual void Set_CurrentBtn(TitleOwnBtnEUIController targetBtn)
    {
        currentBtn = targetBtn;
    }

    #endregion
}
