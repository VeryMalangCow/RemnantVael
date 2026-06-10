using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseStateView : MonoBehaviour
{
    #region Value

    [SerializeField] public RectTransform panelRt;
    [SerializeField] public OwnBtnEUIController backBtn;
    [SerializeField] public OwnBtnEUIController changeTypeBtn;
    [SerializeField] public bool isBuPanelOn = true;

    [SerializeField] private TMP_Text playerStateNameTxt;
    [SerializeField] private TMP_Text allyStateNameTxt;

    #endregion

    #region Value - Player

    // DMG, ROF, CC, CD, MS, AR, KB
    // MaxEP, ESValue, SkillCost, Resist
    // WalkS, WalkSWhileS, DashP, AvoidC
    // Skill 00: Cooltime, Power, Tier
    // Skill 01: Cooltime, Power, Tier
    [Space(10)]
    [Header("=== Player")]
    [SerializeField] private PausePlayerBuStateView buStateViewPrefab;
    private PausePlayerBuStateView buStateView;

    [SerializeField] private PausePlayerMuStateView muStateViewPrefab;
    private PausePlayerMuStateView muStateView;
    [Space(5)]
    [SerializeField] private Transform stateViewParentTf;

    #endregion

    #region Value - Ally

    [Space(10)]
    [Header("=== Ally - BU")]
    [SerializeField] private ScrollPanelEUIController allyScrollEui;
    [SerializeField] private GameObject allyBuEuiPrefab;
    [HideInInspector] private List<StandbyAllyBUEUIController> allyBuEuiList = new List<StandbyAllyBUEUIController>();

    #endregion

    #region Init

    public IEnumerator InitAsync(PauseUIController uiController, Color mainClr, Color subClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        buStateView = Instantiate(buStateViewPrefab, stateViewParentTf);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=yellow>Bu State View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return buStateView.InitAsync(mainClr, subClr);

#if UNITY_EDITOR
        sw.Restart();
#endif
        muStateView = Instantiate(muStateViewPrefab, stateViewParentTf);
        muStateView.Init(); 
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=yellow>Mu State View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        backBtn.ownerUIController = uiController;
        backBtn.Offset();

        changeTypeBtn.ownerUIController = uiController;
        changeTypeBtn.Offset();

        allyScrollEui.Offset();

        SetPanel(true);

        yield return null;
    }

#endregion

    #region Panel

    public void Change_Panel()
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_01");
        SetPanel(!isBuPanelOn);
    }

    public void SetPanel(bool isBuPanelOn)
    {
        this.isBuPanelOn = isBuPanelOn;

        buStateView.gameObject.SetActive(isBuPanelOn);
        muStateView.gameObject.SetActive(!isBuPanelOn);

        for (int i = 0; i < allyBuEuiList.Count; i++)
            allyBuEuiList[i].Set_Panel(isBuPanelOn);
    }

    #endregion

    #region Set

    string Get(int index) => ResourceManager.instance.Get_StaticWord(index);

    public void SetColor(Color imgClr, Color txtClr)
    {
        playerStateNameTxt.color = imgClr;

        muStateView.SetColor(imgClr, txtClr); 

        #region Ally

        allyStateNameTxt.color = imgClr;

        for (int i = 0; i < allyBuEuiList.Count; i++)
            allyBuEuiList[i].Set_Color();

        #endregion
    }

    public void Set_LanguageTxt()
    {
        #region Player

        playerStateNameTxt.text = $"<size=70%><color=#808080>{Get(102)} - </color></size><b>[{Get(113)}]</b>";

        buStateView.SetLanguageTxt();

        #endregion

        #region Ally

        // Ally - BU
        allyStateNameTxt.text = $"<size=70%><color=#808080>{Get(102)} - </color><color=#FFFFFF></size><b>[{Get(95)}]</b></color>";

        #endregion
    }

    public void Set_State(List<AllyController> allAlly)
    {
        #region Player

        PlayerController pc = PlayerManager.instance.playerController;
        PlayerWeaponController pwc = pc.baseWeapon;
        SkillWeaponController swc = pc.skillWeapon;
        PlayerDashController pdc = pc.dash;

        buStateView.SetState();
        muStateView.SetState();

        #endregion

        #region Ally

        TryGen_AllyStateEUI(allAlly.Count);

        for (int i = 0; i < allyBuEuiList.Count; i++)
        {
            if (allAlly.Count > i)
            {
                allyBuEuiList[i].gameObject.SetActive(true);

                #region Ally BU

                allyBuEuiList[i].Set_Data(allAlly[i]);

                #endregion
            }
            else
            {
                allyBuEuiList[i].gameObject.SetActive(false);
            }
        }

        #endregion
    }

    private void TryGen_AllyStateEUI(int targetAmount)
    {
        if (allyBuEuiList.Count >= targetAmount) return;

        float baseX = -16;
        float baseY = -32;
        float intervalY = -180;

        int needAmount = targetAmount - allyBuEuiList.Count;
        for (int i = 0; i < needAmount; i++)
        {
            Instantiate(allyBuEuiPrefab, allyScrollEui.actualMovableRt).TryGetComponent(out StandbyAllyBUEUIController eui);
            eui.Offset();
            eui.Set_Pos(new Vector2(baseX, baseY + (intervalY * allyBuEuiList.Count)));
            allyBuEuiList.Add(eui);
        }

        allyScrollEui.actualMovableRt.sizeDelta = new Vector2(allyScrollEui.actualMovableRt.sizeDelta.x,
            -((baseY * 1.5f) + (intervalY * allyBuEuiList.Count)));
        allyScrollEui.Set_ScrollPanel();
    }

    #endregion
}
