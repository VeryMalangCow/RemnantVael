using UnityEngine;

public class PrisonPayOperatorController : PrisonOperatorController
{
    #region Value

    [HideInInspector] private int Pay = 10;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        PayTxt.text = Get_NeedPay().ToString();
    }

    #endregion

    #region Get

    private int Get_NeedPay()
    {
        return Pay * (TargetPrisonController.DangerRating + 1);
    }

    #endregion

    #region Set

    public override void Set_TargetBuild(PrisonController _TargetPrison)
    {
        base.Set_TargetBuild(_TargetPrison);

        _TargetPrison.PayOper = this;
    }

    #endregion

    #region Interact

    public override void Play_Interact()
    {

    }

    #endregion
}
