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
    [SerializeField] List<TitleElement> AllTitleElementUI;
    [SerializeField] List<TitleTSElement> AllTitleTSElementUI;
    [SerializeField] List<TitleTSTFElement> AllTitleTSTFElementUI;
    
    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private TitleOwnBtnEUIController StartBtn;
    [SerializeField] private TitleOwnBtnEUIController OptionBtn;
    [SerializeField] private TitleOwnBtnEUIController QuitBtn;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    #endregion

    #region - Hide

    // Value
    [HideInInspector] private bool IsStarting = false;
    [HideInInspector] private float UIElementMovingPowerMultiple = 0.002f;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        StartBtn.Offset();
        StartBtn.OwnerUIController = this;

        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;

        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;

        BGCG.alpha = 1f;

        IsStarting = false;
    }

    #endregion

    #region Framework

    private void Start()
    {
        Set_UIElementTS(true);
        Set_UIElementTSTF(false);
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

    private void Set_UIElementTSTF(bool _Loop)
    {
        for (int i = 0; i < AllTitleTSTFElementUI.Count; i++)
        {
            int index = i;
            TitleTSTFElement trueShadowTFElementSet = AllTitleTSTFElementUI[index];
            List<TrueShadow> targetTsList = trueShadowTFElementSet.Get_TargetTSList();
            Get_EachUIElementTS(targetTsList, trueShadowTFElementSet.Max, trueShadowTFElementSet.Min, trueShadowTFElementSet.DurTime, _Loop);
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
        if (CurrentBtn == null || IsStarting)
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

    #region Play

    private IEnumerator Play_Starting_Cor(float _DelayTime)
    {
        TitleLobbyUIManager.Instance.Get_JustFadeIn(_DelayTime);

        yield return new WaitForSeconds(_DelayTime + 0.2f);

        LoadingSceneManager.Instance.Play_LoadScene("MainGame");
    }

    #endregion
}
