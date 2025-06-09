using UnityEngine;

public class TabEUIController : ScrollPanelEUIController
{
    #region Value

    [Header("=== Comp")]
    [SerializeField] public TabBtnEUIController ThisTabBtn;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisTabBtn.Offset();
    }

    #endregion
}
