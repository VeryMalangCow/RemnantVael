using UnityEngine;
using UniRx;
using System.Collections;
using System.Diagnostics;

public class HudController : UIController
{
    [Space(10)]
    [Header("=== View")]

    [SerializeField] private HudEpView epViewPrefab;
    [SerializeField] private HudTabStateView tabStateViewPrefab;
    [SerializeField] private HudTabModuleView tabModuleViewPrefab;
    [SerializeField] private HudSkillView skillViewPrefab;
    [SerializeField] private HudAllyStateView allyStateViewPrefab;
    [SerializeField] private HudBetteryShardView betteryShardViewPrefab;
    [SerializeField] private HudBetteryView betteriesViewAndLootableViewPrefab;
    [SerializeField] private HudKeyView keyViewPrefab;
    [SerializeField] private HudBoostView boostViewPrefab;
    [SerializeField] private HudTabItemView tabItemViewPrefab;
    [SerializeField] private HudInteractView interactViewPrefab;
    [SerializeField] private HudAllyPresenceView allyPresenceViewPrefab;
    [SerializeField] private HudPlayerBuffView playerBuffViewPrefab;
    [SerializeField] private HudMinimapView minimapViewPrefab;
    [SerializeField] private HudHittedView hittedViewPrefab;

    private HudEpView epView;
    private HudTabStateView tabStateView;
    private HudTabModuleView tabModuleView;
    private HudSkillView skillView;
    private HudAllyStateView allyStateView;
    private HudBetteryShardView betteryShardView;
    private HudBetteryView betteriesView;
    private HudLootableItemView lootableItemsView;
    private HudKeyView keyView; 
    private HudBoostView boostView;
    private HudTabItemView tabItemView;
    private HudInteractView interactView;
    private HudAllyPresenceView allyPresenceView; 
    private HudPlayerBuffView playerBuffView;
    private HudMinimapView minimapView;
    private HudHittedView hittedView;

    public HudEpView EpView { get { return epView; } }
    public HudTabStateView TabStateView { get { return TabStateView; } }
    public HudTabModuleView TabModuleView { get { return tabModuleView; } }
    public HudSkillView SkillView { get { return skillView; } }
    public HudAllyStateView AllyStateView { get { return allyStateView; } }
    public HudBetteryShardView BetteryShardView { get { return betteryShardView; } }
    public HudBetteryView BetteriesView { get { return betteriesView; } }
    public HudLootableItemView LootableItemsView { get { return lootableItemsView; } }
    public HudKeyView KeyView { get { return keyView; } }
    public HudBoostView BoostView { get { return boostView; } }
    public HudTabItemView TabItemView { get { return tabItemView; } }
    public HudInteractView InteractView { get { return interactView; } }
    public HudAllyPresenceView AllyPresenceView { get { return allyPresenceView; } }
    public HudPlayerBuffView PlayerBuffView { get { return playerBuffView; } }
    public HudMinimapView MinimapView { get { return minimapView; } }
    public HudHittedView HittedView { get { return hittedView; } }

    public IEnumerator InitAsync(Color mainClr, Color subClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        isTabInteracted.Value = false;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Offset</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        PlayerController player = PlayerManager.instance.playerController;


#if UNITY_EDITOR
        sw.Restart();
#endif
        epView = Instantiate(epViewPrefab, transform);
        epView.Init(mainClr, subClr);
        epViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Ep View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        tabStateView = Instantiate(tabStateViewPrefab, transform);
        tabStateView.Init(mainClr);
        tabStateViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>TabState View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        tabModuleView = Instantiate(tabModuleViewPrefab, transform);
        tabModuleView.Init();
        tabModuleViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>TabModule View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        skillView = Instantiate(skillViewPrefab, transform);
        skillView.Init(player.skillWeapon, mainClr, subClr);
        skillViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Skill View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        allyStateView = Instantiate(allyStateViewPrefab, transform);
        allyStateView.Init();
        allyStateViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>AllyState View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        betteryShardView = Instantiate(betteryShardViewPrefab, transform);
        betteryShardView.Init(subClr);
        betteryShardViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>BetteryShard View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        betteriesView = Instantiate(betteriesViewAndLootableViewPrefab, transform);
        betteriesView.Init(player, mainClr, subClr);
        if (betteriesView.gameObject.TryGetComponent(out HudLootableItemView _lootableItemsView))
        {
            lootableItemsView = _lootableItemsView;
            lootableItemsView.Init();
        }
        betteriesViewAndLootableViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Betteries & LootableItems View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        keyView = Instantiate(keyViewPrefab, transform);
        keyView.Init();
        keyViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Key View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        boostView = Instantiate(boostViewPrefab, transform);
        boostView.Init(mainClr, subClr);
        boostViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Boost View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        tabItemView = Instantiate(tabItemViewPrefab, transform);
        tabItemView.Init();
        tabItemViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Tab Item View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        interactView = Instantiate(interactViewPrefab, transform);
        interactView.Init(mainClr, subClr);
        interactViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Interact View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        allyPresenceView = Instantiate(allyPresenceViewPrefab, transform);
        allyPresenceView.Init(mainClr, subClr);
        allyPresenceViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Ally Presence View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        playerBuffView = Instantiate(playerBuffViewPrefab, transform);
        playerBuffView.Init();
        playerBuffViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Player Buff View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        minimapView = Instantiate(minimapViewPrefab, transform);
        minimapView.Init(mainClr, subClr);
        minimapViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Minimap View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        hittedView = Instantiate(hittedViewPrefab, transform);
        hittedView.Init(mainClr, subClr);
        hittedViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=yellow>Minimap View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
        SetLanguageTxt();
        yield return null;
    }


    #region Value

    #region - Inspector


    [Space(10)]
    [SerializeField] public CanvasGroup cg;

    [Space(10)]
    [SerializeField] public ReactiveProperty<bool> isTabInteracted = new ReactiveProperty<bool>();
    [SerializeField] public bool isTabInputed = false;

    private float tabInputedCurrentTime = 0f;
    private float tabInputedMaxTime = 0.25f;
    private readonly float tabInteractDurTime = 0.25f;

    #endregion

    #endregion

    #region Reset

    private void Reset_Tab()
    {
        PlayerController player = PlayerManager.instance.playerController;

        tabStateView.ResetTab(player);
        skillView.ResetTab(player.skillWeapon);
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        Caculate_TabInput(Time.deltaTime);
    }

    #endregion

    #region Caculate

    private void Caculate_TabInput(float deltaTime)
    {
        if (isTabInputed)  
            Caculate_WhenTabInputOn(deltaTime);
        else  
            Caculate_WhenTabInputOff(deltaTime);
    }

    private void Caculate_WhenTabInputOn(float deltaTime)
    {
        if (tabInputedMaxTime > tabInputedCurrentTime) // 인풋 시간 계산
        {
            tabInputedCurrentTime += deltaTime;
        }
        else // 인풋 시간 충분 상태
        {
            if (!isTabInteracted.Value)
            {
                SetOn_TabInteract();
            }
        }
    }

    private void Caculate_WhenTabInputOff(float deltaTime)
    {
        if (tabInputedCurrentTime != 0)
        {
            tabInputedCurrentTime = 0f;
        }
        if (isTabInteracted.Value)
        {
            SetOff_TabInteract();
        }
    }

    #endregion

    #region Tab

    public void SetOn_TabInteract()
    {
        if (isTabInteracted.Value) return;
        isTabInteracted.Value = true;

        Reset_Tab();

        tabStateView.TabOn(tabInteractDurTime);
        tabModuleView.TabOn(tabInteractDurTime);
        boostView.TabOn(tabInteractDurTime);
        tabItemView.TabOn(tabInteractDurTime);
        minimapView.TabOn(tabInteractDurTime);
    }

    public void SetOff_TabInteract()
    {
        if (!isTabInteracted.Value) return; 
        isTabInteracted.Value = false;

        tabStateView.TabOff(tabInteractDurTime);
        tabModuleView.TabOff(tabInteractDurTime);
        boostView.TabOff(tabInteractDurTime);
        tabItemView.TabOff(tabInteractDurTime);
        minimapView.TabOff(tabInteractDurTime);
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        tabStateView.SetLanguage();
        skillView.SetLanguage();
        interactView.SetLanguage();
        allyPresenceView.SetLanguage();
        minimapView.SetStageDescription();
    }

    #endregion
}

