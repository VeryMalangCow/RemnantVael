using DG.Tweening;
using LeTai.TrueShadow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLobbyUIController : TitleSinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Title Lobby UI Controller")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform BtnsRT;
    [SerializeField] private CanvasGroup BGCG;

    [Space(10)]
    [Header("=== Element")]
    [SerializeField] private List<TitleElement> AllTitleElementUI;
    [SerializeField] private List<TitleTSElement> AllTitleTSElementUI;

    [Space(5)]
    [Header("-- Smoke")]
    [SerializeField] private List<TitleSmokeEUIController> AllTitleSmokeEUI;

    [Space(5)]
    [Header("-- Cloud")]
    [SerializeField] private List<RectTransform> CloudRTList_BackMoon;
    [SerializeField] private List<RectTransform> CloudRTList_FrontMoon;
    [SerializeField] private float CloudMovingLimit = 3000f;
    [SerializeField] private float CloudMovingTime = 1f;

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] public GameObject SmokeCellEUIPrefab;
    [SerializeField] public GameObject CloudCellEUIPrefab;
    
    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private TitleOwnBtnEUIController StartBtn;
    [SerializeField] private TitleOwnBtnEUIController OptionBtn;
    [SerializeField] private TitleOwnBtnEUIController QuitBtn;
    [SerializeField] private RectTransform SelectedRT;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime;

    [Space(10)]
    [Header("=== Interact")]    
    [SerializeField] private IInteract CurrentInteractable;

    #endregion

    #region - Hide

    // Value
    [HideInInspector] public bool IsInIntro = true;
    [HideInInspector] private bool IsStarting = false;
    [HideInInspector] private float UIElementMovingPowerMultiple = 0.002f;

    // Btn
    [HideInInspector] private List<TitleOwnBtnEUIController> TitleAllBtns;
    [HideInInspector] private TitleOwnBtnEUIController CurrentMouseBtn = null;

    [HideInInspector] private List<Tween> CouldTween = null;

    #endregion

    #endregion

    #region Offset
    public override void Offset()
    {
        base.Offset();

        DevTool.Get_ComponentTType<Canvas>(gameObject).planeDistance = 10;

        StartBtn.Offset();
        StartBtn.OwnerUIController = this;

        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;

        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;

        TitleAllBtns = new List<TitleOwnBtnEUIController>
        { StartBtn, OptionBtn, QuitBtn };

        BGCG.alpha = 1f;

        IsStarting = false;

        for (int i = 0; i < AllTitleSmokeEUI.Count; i++)
        {
            AllTitleSmokeEUI[i].OwnerUIController = this;
            AllTitleSmokeEUI[i].Offset();
        }

        CouldTween = new List<Tween>
        {
            Play_CloudMoving_FromLeft(CloudRTList_BackMoon[0], CloudMovingTime),
            Play_CloudMoving_FromLeft(CloudRTList_FrontMoon[0], CloudMovingTime),
            Play_CloudMoving_FromCenter(CloudRTList_BackMoon[1], CloudMovingTime),
            Play_CloudMoving_FromCenter(CloudRTList_FrontMoon[1], CloudMovingTime)
        };
    }

    #endregion

    #region Play

    private void SetOff_Play()
    {
        for (int i = 0; i < AllTitleSmokeEUI.Count; i++)
            AllTitleSmokeEUI[i].Stop_VFX();

        for (int i = 0; i < CouldTween.Count; i++)
            DevTool.Set_KillTween(CouldTween[i]);

        CouldTween = null;
    }

    #endregion

    #region Cloud

    private Tween Play_CloudMoving_FromLeft(RectTransform _CloudRT, float _DurTime)
    {
        Tween tween = _CloudRT.DOAnchorPosX(CloudMovingLimit, _DurTime)
            .OnComplete(() =>
            {
                _CloudRT.anchoredPosition = new Vector2(-CloudMovingLimit, 0);
            })
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);


        return tween;
    }

    private Tween Play_CloudMoving_FromCenter(RectTransform _CloudRT, float _DurTime)
    {
        Tween tween = _CloudRT.DOAnchorPosX(CloudMovingLimit, _DurTime * 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _CloudRT.anchoredPosition = new Vector2(-CloudMovingLimit, 0);
                _CloudRT.DOAnchorPosX(CloudMovingLimit, _DurTime).OnComplete(() =>
                {
                    _CloudRT.anchoredPosition = new Vector2(-CloudMovingLimit, 0);
                })
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
            });

        return tween;
    }

    #endregion

    #region Framework

    private void Start()
    {
        Set_UIElementTS(true);

        Set_CurrentBtn(StartBtn);
    }

    private void LateUpdate()
    {
        Vector2 movingPower = Get_MovingPowerVec() * -1;
        Set_UIElementTF(movingPower);
    }


    #endregion

    #region UI Element (TS)

    private void Set_UIElementTS(bool _Loop)
    {
        for (int i = 0; i < AllTitleTSElementUI.Count; i++)
        {
            int index = i;
            TitleTSElement trueShadowElementSet = AllTitleTSElementUI[index];
            Get_EachUIElementTS(trueShadowElementSet.ThisTSList, trueShadowElementSet.Max, trueShadowElementSet.Min, trueShadowElementSet.DurTime, _Loop);
        }
    }


    private void Get_EachUIElementTS(List<TrueShadow> _TSList, float _Max, float _Min, float _DurTime, bool _Loop = true)
    {
        for (int i = 0; i < _TSList.Count; i++)
        {
            int index = i;

            _TSList[index].Size = _Min;

            Tween tween = DOTween.To(
                () => _TSList[index].Size,
                x => _TSList[index].Size = x,
                _Max, _DurTime * 0.5f)
                .SetEase(Ease.Linear);

            if (_Loop)
                tween.SetLoops(-1, LoopType.Yoyo);
        }
    }

    #endregion

    #region UI Element (Pos)

    private void Set_UIElementTF(Vector2 _MovingPower)
    {
        for (int i = 0; i < AllTitleElementUI.Count; i++)
        {
            Vector2 thisVec = new Vector2(AllTitleElementUI[i].MovingPowerX, AllTitleElementUI[i].MovingPowerY);
            Vector2 targetVec = thisVec * _MovingPower;
            Set_EachUIElementPos(AllTitleElementUI[i].MovingRT, targetVec);
        }
    }

    private void Set_EachUIElementPos(RectTransform _RT, Vector2 _TargetVec)
    {
        _RT.anchoredPosition = _TargetVec;
    }

    private Vector2 Get_MovingPowerVec()
    {
        return UIElementMovingPowerMultiple * TitleInputManager.Instance.Get_AnchorMousePos();
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        if (CurrentBtn == null || CurrentMouseBtn == null || IsStarting || IsInIntro)
        { return; }

        TitleInputManager.Instance.Play_MousePointerClick();

        if (CurrentBtn == StartBtn)
        {
            IsStarting = true;
            StartCoroutine(Play_Starting_Cor(2f));
        }
        else if (CurrentBtn == OptionBtn)
        { 
            Debug.Log("옵션 창 키기");
        }
        else if (CurrentBtn == QuitBtn)
        { 
            Application.Quit(); 
        }
    }

    #endregion

    #region Btn

    public override void Set_CurrentBtn(TitleOwnBtnEUIController _TargetBtn)
    {
        if (CurrentBtn == _TargetBtn) return;

        base.Set_CurrentBtn(_TargetBtn);

        DevTool.Set_KillTween(SelectedRT);
        SelectedRT.DOAnchorPosY(CurrentBtn.ThisRT.anchoredPosition.y, 0.2f);

        for (int i = 0; i < TitleAllBtns.Count; i++)
        {
            if (CurrentBtn == TitleAllBtns[i])
                TitleAllBtns[i].Set_SelectOnThis(0.2f);
            else
                TitleAllBtns[i].Set_SelectOffThis(0.2f);
        }
    }

    public void Set_CurrentMouseBtn(TitleOwnBtnEUIController _TargetBtn)
    {
        if (CurrentMouseBtn == _TargetBtn) return;

        CurrentMouseBtn = _TargetBtn;
    }

    #endregion

    #region Play

    private IEnumerator Play_Starting_Cor(float _DelayTime)
    {
        TitleLobbyUIManager.Instance.Get_JustFadeIn(_DelayTime);

        yield return new WaitForSeconds(_DelayTime + 0.2f);

        SetOff_Play();

        LoadingSceneManager.Instance.Play_LoadScene("MainGame");
    }

    #endregion
}
