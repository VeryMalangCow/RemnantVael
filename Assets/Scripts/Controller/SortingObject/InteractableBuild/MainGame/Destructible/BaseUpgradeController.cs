using UnityEngine;

public class BaseUpgradeController : DestructibleBuildController, IInteract
{
    #region Value

    public static BaseUpgradeController UsingShop = null;

    #endregion

    #region Framework

    protected override void Start()
    {
        base.Start();
        Set_StateAnim();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Gen_ExplosionEffect();
    }

    #endregion

    #region Interact

    public void Play_Interact()
    {
        if (IsBroken)
        { return; }

        if (IsOn)
        {
            BaseUpgradeController.UsingShop = this;
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOn_ThisPanel();
        }
        else if (!IsOn && PlayerManager.Instance.PlayerController.CurrentEC.Value > 0)
        {
            PlayerManager.Instance.PlayerController.CurrentEC.Value--; 
            IsOn = true;
        }

        Set_StateAnim();
    }

    public override void Take_Damage(bool _SpawnItem)
    {
        base.Take_Damage(_SpawnItem);

        MainGameUIManager.Instance.BaseUpgrade_UIController.Set_Dur(ThisDurablity);
    }

    #endregion

    #region Break

    protected override void Set_Break(bool _SpawnItem)
    {
        base.Set_Break(_SpawnItem);

        if (_SpawnItem)
        { Gen_RandomBS(); }

        if (MainGameUIManager.Instance.BaseUpgrade_UIController.gameObject.activeSelf)
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_ThisPanel();
        }
    }

    private void Gen_RandomBS()
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
            Gen_BS(1);
        }
    }

    public override void Gen_Item()
    {
        Gen_BS(1);
    }

    #endregion
}