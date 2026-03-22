using UnityEngine;
using UnityEngine.Serialization;

public class TabEUIController : ScrollPanelEUIController
{
    #region Value

    [Header("=== Comp")]
    [FormerlySerializedAs("ThisTabBtn")][SerializeField] public TabBtnEUIController tabBtn;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        tabBtn.Offset();
    }

    #endregion
}
