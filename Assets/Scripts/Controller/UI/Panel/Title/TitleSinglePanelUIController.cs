using UnityEngine;
using UnityEngine.Serialization;

public class TitleSinglePanelUIController : UIController
{
    #region Value

    [FormerlySerializedAs("CurrentBtn")][SerializeField] protected TitleOwnBtnEUIController currentBtn = null;

    #endregion


    #region Btn

    public virtual void Set_CurrentBtn(TitleOwnBtnEUIController targetBtn)
    {
        currentBtn = targetBtn;
    }

    #endregion
}
