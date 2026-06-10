using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class PuzzleUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Puzzle UI")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected float baseCountdown = 15f;

    [Space(10)]
    [Header("=== Ready Panel")]
    [SerializeField] protected PuzzleReadyPanelEUIController readyPanelEui;

    [Space(10)]
    [Header("=== Left")]
    [SerializeField] private PuzzleTimePanelEUIController timePanelEui;

    [Space(10)]
    [Header("=== Right")]
    [SerializeField] private PuzzleUnlockPanelEUIController unlockPanelEui;

    #endregion

    #region - Hide

    // Canvas Group
    [HideInInspector] protected CanvasGroup cg;



    // Static Data
    [HideInInspector] private static string secondString = "<size=50%>s</size>";

    // Success
    [HideInInspector] protected bool canSuccess = false;
    [HideInInspector] protected bool isInteractable = false;
    [HideInInspector] private bool isReady = false;
    [HideInInspector] private bool isStart = false;

    // Value
    [HideInInspector] protected float currentCountdown = 0;

    // Prison
    [HideInInspector] private PrisonController usingPrison = null;

    #endregion

    #endregion

    #region Offset

    public virtual void Offset_FirstValue(PrisonController prison)
    {
        usingPrison = prison;
    }

    public override void Offset()
    {
        base.Offset();

        // CG
        cg = DevTool.Get_ComponentTType(gameObject, out CanvasGroup _cg) ? _cg : null;

        // Left
        timePanelEui.Offset();

        // Right
        unlockPanelEui.Offset();

        // Ready
        readyPanelEui.Offset();

    }

    #endregion

    #region Framework

    private void Update()
    {
        Caculate_CountDown(Time.deltaTime);
    }

    #endregion

    #region Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

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

    private void Caculate_CountDown(float deltaTime)
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
            DevTool.SetColor(ResourceManager.instance.unlockedClr, timePanelEui.countdownTxt);
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
