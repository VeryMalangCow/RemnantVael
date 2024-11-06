using UnityEngine;

public class BaseUpgradeController : DestructibleBuildingController, IInteract
{
    public static BaseUpgradeController UsingShop = null;

    #region Framework

    private void Start()
    {
        ApplySetStateAnim();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        MEI.GenExplosionImgs(
                    MEI.gameObject.transform.position,
                    16, 0.15f, 0.75f,
                    2.0f, 0.05f, 0.1f,
                    1.0f, 0.5f, 1.0f,
                    0, StageManager.Instance.GetCurrentStageMaterial());
    }

    #endregion

    #region Interact

    public void Interact()
    {
        if (IsBroken)
        { return; }

        if (IsOn)
        {
            BaseUpgradeController.UsingShop = this;
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

    protected override void Break(bool _SpawnItem)
    {
        base.Break(_SpawnItem);

        if (_SpawnItem)
        { RandomSpawnBS(); }

        if (MainGameUIManager.Instance.BaseUpgrade_UIController.gameObject.activeSelf)
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.CloseThisPanel(MainGameUIManager.Instance.BaseUpgrade_UIController.TabDurTime);
        }
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
            spawnItem = Random.Range(2, 4);
        }

        if (spawnItem <= 0)
        { return; }

        for (int i = 0; i < spawnItem; i++)
        {
            SpawnBS(1);
        }
    }

    public override void SpawnItem()
    {
        SpawnBS(1);
    }

    #endregion
}