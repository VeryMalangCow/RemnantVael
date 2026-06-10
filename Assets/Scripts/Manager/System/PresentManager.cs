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
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        // 오너들의 데이터 Init
        ownerDataset = new PresenterOwnerData();
        uiDataset = new PresenterUIData();

        // Owner
        ownerDataset.player = PlayerManager.instance.playerController;
        ownerDataset.saveData = SaveDataManager.instance;


        // UI
        MainGameUIManager uiSet = MainGameUIManager.instance;

        uiDataset.hud = uiSet.hud;
        uiDataset.interactAnno = uiSet.interactAnnoUi;

        uiDataset.baseUpgrade = uiSet.buUi;
        uiDataset.moduleUpgrade = uiSet.muUi;

        uiDataset.allyBaseUpgrade = uiSet.abuUi;
        uiDataset.allyModuleUpgrade = uiSet.amuUi;

        uiDataset.premiumCreditCvt = uiSet.premiumCreditCvtUi;
        uiDataset.protoCoreCvt = uiSet.protoCoreCvtUi;
        uiDataset.etherCoreCvt = uiSet.etherCoreCvtUi;
        uiDataset.originCoreCvt = uiSet.originCoreCvtUi;

        uiDataset.aimRound = InputManager.instance.aimRoundController;

        CollectPresenters();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"PresentManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif

        for (int i = 0; i < presenters.Count; i++)
        {
#if UNITY_EDITOR
            sw.Restart();
#endif
            presenters[i].Init(ownerDataset, uiDataset);
            presenters[i].SubscribeOn();
#if UNITY_EDITOR
            sw.Stop();
            UnityEngine.Debug.Log($"Presenter : <color=orange>{transform.GetChild(i)}</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
            yield return null;
        }
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
    public SaveDataManager saveData;
}                            

public class PresenterUIData
{
    public HudController hud;
    public InteractAnnoUIController interactAnno;
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
    public void SubscribeOn();
    public void SubscribeOff();
}
