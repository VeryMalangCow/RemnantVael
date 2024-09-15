using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Populer")]
    [SerializeField] protected List<ModifyEachTab> ThisPanelTabList;
    [SerializeField] private ModifyEachTab CurrentThisPanelTab;
    [SerializeField] public float TabDurTime = 0.4f;
    [SerializeField] protected bool IsTweening = false;

    #endregion

    #region Abstract Function

    protected abstract void Offset_Module();
    protected abstract void Offset_UI();

    #endregion

    #region Framework

    protected virtual void Start()
    {
        Offset_Module();
        Offset_UI();
    }

    #endregion

    #region Set Panel

    public virtual void OpenThisPanel(float _DurTime)
    {
        // Other
        UIManager.Instance.CurrentOpeningUIController = this;
        InputManager.Instance.InputMoveDir = Vector2.zero;
        InputManager.Instance.enabled = false;

        this.gameObject.SetActive(true);

        CurrentThisPanelTab = ThisPanelTabList[0];

        // Seq
        IsTweening = true;
        Sequence Seq = DOTween.Sequence();
        Sequence OpenSeq = OpenWindow(ThisPanelTabList[0], _DurTime);
        List <Sequence> CloseSeq = CloseWindowList(ThisPanelTabList, 0);

        foreach (Sequence seq in CloseSeq) 
        {
            Seq.Join(seq);
        }

        Seq.Append(OpenSeq);

        Seq.OnComplete(() =>
        {
            IsTweening = false;
        });
    }

    public virtual void ChangeThisPanel(float _DurTime, int _indexWindow)
    {
        //Other
        if (CurrentThisPanelTab == ThisPanelTabList[_indexWindow] || IsTweening)
        { return; }

        CurrentThisPanelTab = ThisPanelTabList[_indexWindow];

        // Seq
        IsTweening = true;
        Sequence Seq = DOTween.Sequence();
        Sequence OpenSeq = OpenWindow(ThisPanelTabList[_indexWindow], _DurTime);
        List<Sequence> CloseSeq = CloseWindowList(ThisPanelTabList, _DurTime);

        foreach (Sequence seq in CloseSeq)
        {
            Seq.Join(seq);
        }

        Seq.Append(OpenSeq);

        Seq.OnComplete(() =>
        {
            IsTweening = false;
        });
    }

    public virtual void CloseThisPanel(float _DurTime)
    {
        //Other
        if (IsTweening)
        { return; }

        UIManager.Instance.CurrentOpeningUIController = null;
        InputManager.Instance.enabled = true;

        // Seq
        IsTweening = true;
        Sequence Seq = DOTween.Sequence();
        List<Sequence> CloseSeq = CloseWindowList(ThisPanelTabList, _DurTime);

        foreach (Sequence seq in CloseSeq)
        {
            Seq.Join(seq);
        }

        Seq.OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                IsTweening = false;
            });
    }

    #endregion

    #region Set Window

    private Sequence OpenWindow(ModifyEachTab _TargetTab, float _DurTime)
    {
        Sequence Seq = DOTween.Sequence();

        Seq.Append(_TargetTab.ThisPanelRT.DOSizeDelta(_TargetTab.ThisOriginalPanelSize, _DurTime)
            .OnStart(() =>
            {
                _TargetTab.ThisPanelRT.gameObject.SetActive(true);
                _TargetTab.ThisPanelRT.sizeDelta = new Vector2(_TargetTab.ThisOriginalPanelSize.x, 0);
            }));

        return Seq;
    }

    private Sequence CloseWindow(ModifyEachTab _TargetTab, float _DurTime)
    {
        Sequence Seq = DOTween.Sequence();

        Seq.Append(_TargetTab.ThisPanelRT.DOSizeDelta(new Vector2(_TargetTab.ThisOriginalPanelSize.x, 0), _DurTime)
            .OnStart(() =>
            {
                _TargetTab.ThisPanelRT.sizeDelta = _TargetTab.ThisOriginalPanelSize;
            })
            .OnComplete(() =>
            {
                _TargetTab.ThisPanelRT.gameObject.SetActive(false);
            }));

        return Seq;
    }

    private List<Sequence> CloseWindowList(List<ModifyEachTab> _TargetTabList, float _DurTime)
    {
        List<Sequence> Seq = new List<Sequence>();
        for (int i = 0; i < _TargetTabList.Count; i++)
        {
            Seq.Add(CloseWindow(_TargetTabList[i], _DurTime));
        }
        return Seq;
    }

    #endregion
}