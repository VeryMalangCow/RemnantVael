using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PresentManager : MonoBehaviour, IMainGameInitializer
{
    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [Space(10)]
    [Header("=== Player")]
    private List<IPresentable> presenters = new List<IPresentable>();

    private PresenterOwnerData ownerDataset;
    private PresenterUIData uiDataset;

    #region Init

    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // 오너들의 데이터 Init
        ownerDataset = new PresenterOwnerData();
        uiDataset = new PresenterUIData();

        // Owner
        ownerDataset.player = PlayerManager.instance.playerController;


        // UI
        MainGameUIManager uiSet = MainGameUIManager.instance;

        uiDataset.hud = uiSet.playerHud;

        uiDataset.baseUpgrade = uiSet.baseUpgradeUi;
        uiDataset.moduleUpgrade = uiSet.moduleUpgradeUi;

        uiDataset.allyBaseUpgrade = uiSet.allyBaseUpgradeUi;
        uiDataset.allyModuleUpgrade = uiSet.allyModuleUpgradeUi;

        uiDataset.premiumCreditCvt = uiSet.premiumCreditCvtUi;
        uiDataset.protoCoreCvt = uiSet.protoCoreCvtUi;
        uiDataset.etherCoreCvt = uiSet.etherCoreCvtUi;
        uiDataset.originCoreCvt = uiSet.originCoreCvtUi;

        uiDataset.aimRound = InputManager.instance.aimRoundController;

        CollectPresenters();

        sw.Stop();
        UnityEngine.Debug.Log($"PresentManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

        for (int i = 0; i < presenters.Count; i++)
        {
            sw.Restart();
            presenters[i].Init(ownerDataset, uiDataset);

            sw.Stop();
            UnityEngine.Debug.Log($"Presenter : <color=orange>{transform.GetChild(i)}</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

            yield return null;
        }
        
       
        yield return null;
    }

    private void CollectPresenters()
    {
        presenters.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out IPresentable presenter))
                presenters.Add(presenter);
        }
    }

    #endregion
}

public class PresenterOwnerData
{
    public PlayerController player;
}                            

public class PresenterUIData
{
    public PlayerHUDController hud;
    public AimRoundController aimRound;

    public BaseUpgradeUIController baseUpgrade;
    public ModuleUpgradeUIController moduleUpgrade;

    public AllyBaseUpgradeUIController allyBaseUpgrade;
    public AllyModuleUpgradeUIController allyModuleUpgrade;

    public PremiumCreditCvtUIController premiumCreditCvt;
    public ProtoCoreCvtUIController protoCoreCvt;
    public EtherCoreCvtUIController etherCoreCvt;
    public OriginCoreCvtUIController originCoreCvt;

}

public interface IPresentable
{
    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData);
}
