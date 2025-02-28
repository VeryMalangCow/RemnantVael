using UnityEngine;

public class ModuleUpgradeController : DestructibleBuildController, IInteract
{
    public static ModuleUpgradeController UsingShop = null;

    #region Framework

    protected override void Start()
    {
        base.Start();
        ApplySetStateAnim();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ExplosionEffect();
    }

    #endregion

    #region Interact

    public void Interact()
    {
        if (IsBroken)
        { return; }

        if (IsOn)
        {
            ModuleUpgradeController.UsingShop = this;
            MainGameUIManager.Instance.ModuleUpgrade_UIController.OpenThisPanel();
        }
        else if (!IsOn && PlayerManager.Instance.PlayerController.CurrentEC.Value > 0)
        {
            PlayerManager.Instance.PlayerController.CurrentEC.Value--;
            IsOn = true;
        }

        ApplySetStateAnim();
    }
    public override void TakeDamage(bool _SpawnItem)
    {
        base.TakeDamage(_SpawnItem);

        MainGameUIManager.Instance.ModuleUpgrade_UIController.SetDur(ThisDurablity);
    }

    #endregion

    #region Break

    protected override void Break(bool _SpawnItem)
    {
        base.Break(_SpawnItem);

        if (_SpawnItem)
        { RandomSpawnMS(); }

        if (MainGameUIManager.Instance.ModuleUpgrade_UIController.gameObject.activeSelf)
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.CloseThisPanel();
        }
    }

    private void RandomSpawnMS()
    {
        int spawnItem = 0;
        if (!IsOn)
        {
            spawnItem = Random.Range(0, 2);
        }
        else
        {
            spawnItem = Random.Range(2, 4);
        }

        if (spawnItem <= 0)
        { return; }

        for (int i = 0; i < spawnItem; i++)
        {
            SpawnMS(1);
        }
    }

    public override void SpawnItem()
    {
        SpawnMS(1);
    }

    #endregion
}