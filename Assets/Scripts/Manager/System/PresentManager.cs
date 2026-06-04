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
    [SerializeField] private PlayerEpPresenter epPresenter;

    private PresenterOwnerData ownerDataset;
    private PresenterUIData uiDataset;

    #region Init
    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // 오너들의 데이터 Init
        ownerDataset = new PresenterOwnerData();
        ownerDataset.player = PlayerManager.instance.playerController;

        uiDataset = new PresenterUIData();
        uiDataset.hud = MainGameUIManager.instance.playerHud;


        epPresenter.Init(ownerDataset, uiDataset);



        ownerDataset = null;
        uiDataset = null;

        sw.Stop();
        UnityEngine.Debug.Log($"PresentManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

        yield return null;
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
}

public interface IPresentable
{
    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData);
}
