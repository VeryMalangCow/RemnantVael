using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PuzzleUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Puzzle UI")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected float BaseCountdown = 15f;

    [Space(10)]
    [Header("=== Ready Panel")]
    [SerializeField] protected PuzzleReadyPanelEUIController ReadyPanelEUI;

    [Space(10)]
    [Header("=== Left")]
    [SerializeField] private PuzzleTimePanelEUIController TimePanelEUI;

    [Space(10)]
    [Header("=== Right")]
    [SerializeField] private PuzzleUnlockPanelEUIController UnlockPanelEUI;

    #endregion

    #region - Hide

    // Canvas Group
    [HideInInspector] protected CanvasGroup ThisCG;



    // Static Data
    [HideInInspector] private static string SecondString = "<size=50%>s</size>";

    // Success
    [HideInInspector] protected bool CanSuccess = false;
    [HideInInspector] protected bool IsInteractable = false;
    [HideInInspector] private bool IsReady = false;
    [HideInInspector] private bool IsStart = false;

    // Value
    [HideInInspector] protected float CurrentCountdown = 0;

    // Prison
    [HideInInspector] private PrisonController UsingPrison = null;

    #endregion

    #endregion

    #region Offset

    public virtual void Offset_FirstValue(PrisonController _Prison)
    {
        UsingPrison = _Prison;
    }

    public override void Offset()
    {
        base.Offset();
        
        // CG
        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;

        // Left
        TimePanelEUI.Offset();

        // Right
        UnlockPanelEUI.Offset();

        // Ready
        ReadyPanelEUI.Offset();

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

        ThisCG.alpha = 1f;

        // Value
        IsReady = true;
        IsStart = false;
        IsInteractable = true;

        Set_AllStart();
    }

    #endregion

    #region Set (Start & Complete)

    protected virtual void Set_AllStart()
    {
        // Left Txt
        TimePanelEUI.Set_AllStart(CurrentCountdown, SecondString);

        // Right Txt
        UnlockPanelEUI.Set_AllStart(CanSuccess);

        // Ready
        ReadyPanelEUI.Set_AllStart(CurrentCountdown, SecondString);
    }

    protected virtual void Set_AllComplete()
    {
        // Value
        IsInteractable = false;

        // Sound
        SoundManager.Instance.Play_2D_SFX_Build("PrisonUnlock");
    }

    protected virtual void Set_AllFailure()
    {

    }

    #endregion

    #region Set (Panelty)

    private void Set_Panelty(float _PaneltyTime)
    {
        CurrentCountdown += _PaneltyTime;
        TimePanelEUI.Set_Panelty(_PaneltyTime, SecondString);
    }

    #endregion

    #region Right

    public void Check_CorrectLineSet()
    {
        bool jugeNow = Can_Success();
        if (jugeNow == CanSuccess) return;
        CanSuccess = jugeNow;

        UnlockPanelEUI.Play_LineSetChange(CanSuccess);
    }

    #endregion

    #region Ready

    private void Play_ReadyToStart(float _DurTime)
    {
        IsReady = false;

        ReadyPanelEUI.Play_ReadyToStart(_DurTime)
            .OnComplete(() =>
            {
                ReadyPanelEUI.ReadyCG.gameObject.SetActive(false);
                IsStart = true;
            });
    }

    #endregion

    #region Can

    public abstract bool Can_Success();

    #endregion

    #region Caculate

    private void Caculate_CountDown(float _DeltaTime)
    {
        if (!IsInteractable || !IsStart) return;

        if (CurrentCountdown > 0f)
        {
            CurrentCountdown -= _DeltaTime;
            TimePanelEUI.Set_CountdownTxt(CurrentCountdown, SecondString);
        }
        else
        {
            StartCoroutine(Play_Unlock_Failure_Cor());
            IsInteractable = false;
            CurrentCountdown = 0f;
            TimePanelEUI.Set_CountdownTxt(CurrentCountdown, SecondString);
        }
    }
    #endregion

    #region Interact

    protected bool Is_Interact_TryUnlock()
    {
        if (IsReady)
        {
            Play_ReadyToStart(2f);

            return true;
        }

        if (!IsInteractable || !IsStart)
            return false;

        if (CanSuccess)
        {
            TimePanelEUI.Play_SuccessAnno(1f, 1f);
            DevTool.Set_Color(UnitManager.Instance.UnlockedClr, TimePanelEUI.CountdownTxt);
            StartCoroutine(Play_Unlock_Complete_Cor());

            return true;
        }
        else
        {
            TimePanelEUI.Play_FailureAnno(1f, 1f);
            Set_Panelty(-0.5f);
            return false;
        }
    }

    private IEnumerator Play_Unlock_Complete_Cor()
    {
        Set_AllComplete();

        ThisCG.DOFade(0f, 1.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        UsingPrison.Set_Unlock();
        UsingPrison = null;

        yield return new WaitForSeconds(1f);

        SetOff_ThisPanel();
    }


    private IEnumerator Play_Unlock_Failure_Cor()
    {
        Set_AllFailure();

        ThisCG.DOFade(0f, 1.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        PlayerManager.Instance.PlayerController.Set_PrisonPanelty();

        yield return new WaitForSeconds(1f);

        SetOff_ThisPanel();
    }

    #endregion
}
