using UnityEngine;

public class ModuleUpgradeController : BuildingController_OnlyPlayerLayer, IInteract
{

    #region Framework

    protected override void Start()
    {
        base.Start();
        ApplySetStateAnim();
    }

    #endregion

    #region Interact

    public void Interact()
    {
        if (IsOn)
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.OpenThisPanel(MainGameUIManager.Instance.ModuleUpgrade_UIController.TabDurTime);
        }
        else if (!IsOn && PlayerManager.Instance.PlayerController.CurrentEC.Value > 0)
        {
            PlayerManager.Instance.PlayerController.CurrentEC.Value--;
            IsOn = true;
        }

        ApplySetStateAnim();
    }

    #endregion
}
