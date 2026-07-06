using UnityEngine;

public class TabEUIController : ScrollPanelEUIController
{
    #region Value

    [Header("=== Comp")]
    [SerializeField] public TabBtnEUIController tabBtn;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        tabBtn.Offset();
    }

    #endregion
}
