using DG.Tweening;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public abstract class PuzzleUIController : SinglePanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Puzzle UI")]


    [Space(10)]
    [Header("=== Panel")]
    [SerializeField] private PuzzleReadyPanelEUIController readyPanelPrefab;
    protected PuzzleReadyPanelEUIController readyPanelEui;

    [SerializeField] private PuzzleTimePanelEUIController timePanelPrefab;
    private PuzzleTimePanelEUIController timePanelEui;

    [SerializeField] private PuzzleUnlockPanelEUIController unlockPanelPrefab;
    private PuzzleUnlockPanelEUIController unlockPanelEui;

    [Space(10)]
    [Header("=== Key")]
    [SerializeField] private Transform keyAnnoTf;

    // class
    private PrisonController usingPrison = null;
    protected CanvasGroup cg;

    // Cooldown Value
    protected static readonly float baseCountdown = 15f;
    protected float currentCountdown = 0;

    // State Value
    protected bool canSuccess = false;
    protected bool isInteractable = false;
    private bool isReady = false;
    private bool isStart = false;

    // string
    private static readonly string secondString = "<size=50%>s</size>";


#endregion

    #region Init

    public virtual IEnumerator InitAsync()
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        unlockPanelEui = Instantiate(unlockPanelPrefab, transform);
        unlockPanelEui.Offset();
        unlockPanelPrefab = null;

        timePanelEui = Instantiate(timePanelPrefab, transform);
        timePanelEui.Offset();
        timePanelPrefab = null;

        readyPanelEui = Instantiate(readyPanelPrefab, transform);
        readyPanelEui.Offset();
        readyPanelPrefab = null;

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Unlock + Time + Ready Panel</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        // CG
        cg = DevTool.Get_ComponentTType(gameObject, out CanvasGroup _cg) ? _cg : null;
        readyPanelEui.Set_KeyAnno(keyAnnoTf);
    }

    #endregion

    public virtual void SetPrison(PrisonController prison)
        => usingPrison = prison;
    
    #region Mono

    private void Update()
    {
        CaculateCountDown(Time.deltaTime);
    }

    #endregion  

    #region Panel

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        cg.alpha = 1f;

        // Value
        isReady = true;
        isStart = false;
        isInteractable = true;

        Set_AllStart();
    }

    #endregion

    #region Set (Start & Complete)

    protected virtual void Set_AllStart()
    {
        // Left Txt
        timePanelEui.Set_AllStart(currentCountdown, secondString);

        // Right Txt
        unlockPanelEui.Set_AllStart(canSuccess);

        // Ready
        readyPanelEui.Set_AllStart(currentCountdown, secondString);

        // Sound
        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");
    }

    protected virtual void Set_AllComplete()
    {
        // Value
        isInteractable = false;
    }

    protected virtual void Set_AllFailure()
    {
        // Sound
        SoundManager.instance.Play_2D_SFX_UI("Click_Reject");
    }

    #endregion

    #region Set (Panelty)

    private void Set_Panelty(float paneltyTime)
    {
        currentCountdown += paneltyTime;
        timePanelEui.Set_Panelty(paneltyTime, secondString);
        SoundManager.instance.Play_2D_SFX_Build("Damaged");
    }

    #endregion

    #region Right

    public void Check_CorrectLineSet()
    {
        bool jugeNow = Can_Success();
        if (jugeNow == canSuccess) return;
        canSuccess = jugeNow;

        unlockPanelEui.Play_LineSetChange(canSuccess);
    }

    #endregion

    #region Ready

    private void Play_ReadyToStart(float durTime)
    {
        isReady = false;
        SoundManager.instance.Play_2D_SFX_UI("Click_00");

        readyPanelEui.Play_ReadyToStart(durTime)
            .OnComplete(() =>
            {
                readyPanelEui.readyCg.gameObject.SetActive(false);
                isStart = true;
            });
    }

    #endregion

    #region Can

    public abstract bool Can_Success();

    #endregion

    #region Caculate

    private void CaculateCountDown(float deltaTime)
    {
        if (!isInteractable || !isStart) return;

        if (currentCountdown > 0f)
        {
            currentCountdown -= deltaTime;
            timePanelEui.Set_CountdownTxt(currentCountdown, secondString);
        }
        else
        {
            StartCoroutine(Play_Unlock_Failure_Cor());
            isInteractable = false;
            currentCountdown = 0f;
            timePanelEui.Set_CountdownTxt(currentCountdown, secondString);
        }
    }
    #endregion

    #region Interact

    protected bool Is_Interact_TryUnlock()
    {
        if (isReady)
        {
            Play_ReadyToStart(2f);

            return true;
        }

        if (!isInteractable || !isStart) return false;

        if (canSuccess)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_Approve");
            timePanelEui.Play_SuccessAnno(1f, 1f);
            DevTool.SetColor(PrisonBuild.unlockedClr, timePanelEui.countdownTxt);
            StartCoroutine(Play_Unlock_Complete_Cor());

            return true;
        }
        else
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_00");
            timePanelEui.Play_FailureAnno(1f, 1f);
            Set_Panelty(-0.5f);

            return false;
        }
    }

    private IEnumerator Play_Unlock_Complete_Cor()
    {
        Set_AllComplete();

        cg.DOFade(0f, 1.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        usingPrison.Set_Unlock();
        usingPrison = null;

        yield return new WaitForSeconds(1f);

        SetOff_ThisPanel();
    }


    private IEnumerator Play_Unlock_Failure_Cor()
    {
        Set_AllFailure();

        cg.DOFade(0f, 1.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        PlayerManager.instance.playerController.Set_PrisonPanelty();

        yield return new WaitForSeconds(1f);

        SetOff_ThisPanel();
    }

    #endregion
}
