using UnityEngine;

public class PrisonPayOperatorController : PrisonOperatorController
{
    #region Value

    [HideInInspector] private int pay = 20;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        payTxt.text = Get_NeedPay().ToString();
    }

    #endregion

    #region Get

    private int Get_NeedPay()
    {
        return pay * (targetPrison.rating + 1);
    }

    #endregion

    #region Set

    public override void Set_TargetBuild(PrisonController targetPrison)
    {
        base.Set_TargetBuild(targetPrison);

        targetPrison.payOper = this;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        //base.Get_InteractName();
        canInteract = Can_Interact();
        return ResourceManager.instance.Get_StaticWord(60);
    }

    public override void PlayInteract()
    {
        base.PlayInteract();

        if (targetPrison == null ||
            targetPrison.isOn ||
            PlayerManager.instance.playerController.currentEp <= Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.instance.playerController.AddCurrentEp(-Get_NeedPay());

        targetPrison.Set_Unlock();
    }

    #endregion
}
