using UnityEngine;

public class BaseUpgradeController : DestructibleBuildingController, IInteract
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
        if (IsBroken)
        { return; }

        if (IsOn)
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.OpenThisPanel(MainGameUIManager.Instance.BaseUpgrade_UIController.TabDurTime);
        }
        else if (!IsOn && PlayerManager.Instance.PlayerController.CurrentEC.Value > 0)
        {
            PlayerManager.Instance.PlayerController.CurrentEC.Value--; 
            IsOn = true;
        }

        ApplySetStateAnim();
    }

    #endregion

    #region Break

    protected override void Break()
    {
        base.Break();

        RandomSpawnBS();
    }

    private void RandomSpawnBS()
    {
        int spawnItem = 0;
        if (!IsOn)
        {
            spawnItem = Random.Range(0, 2);
        }
        else
        {
            spawnItem = Random.Range(1, 4);
        }

        if (spawnItem <= 0)
        { return; }

        for (int i = 0; i < spawnItem; i++)
        {
            SpawnBS(1);
        }
    }

    #endregion
}