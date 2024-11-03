using DG.Tweening;
using UnityEngine;

public class EntranceSpaceUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Choice Character UI")]

    [Space(10)]
    [Header("=== Entire Component")]
    [SerializeField] private CanvasGroup BGCG;
    [SerializeField] private RectTransform PanelRT;
    [SerializeField] private ModifyOwnEachBtn CloseBtn;
    [HideInInspector] private float DurTime = 0.2f;
    [HideInInspector] private float TargetY = 0f;

    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private ModifyOwnEachBtn StartBtn;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        StartBtn.Offset();
        StartBtn.OwnerUIController = this;
    }

    protected override void Offset_UI()
    {
        TargetY = PanelRT.sizeDelta.y;
        PanelRT.sizeDelta = new Vector2(PanelRT.sizeDelta.x, 0f);
        BGCG.alpha = 0f;
    }

    #endregion

    #region Input

    public void TryInteract()
    {
        if (CurrentBtn == null || !CurrentBtn.ThisBtn.interactable)
        { return; }

        if (CurrentBtn == CloseBtn)
        {
            CloseThisPanel();
        }
        else if (CurrentBtn == StartBtn)
        {
            LoadingSceneManager.Instance.LoadScene("MainGame");
        }
    }

    #endregion

    #region Set Panel

    private void ResetData()
    {
        if (SaveDataManager.Instance.CharacterPrefabs.Contains(GameManager.Instance.DesignatedPlayerPrefab))
        {
            StartBtn.ThisBtn.interactable = true;
        }
        else
        {
            StartBtn.ThisBtn.interactable = false;
        }
    }

    public void OpenThisPanel()
    {
        if (DOTween.IsTweening("EntranceUIPanel"))
        { return; }

        InputTitleManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName);
        InputManager.Instance.SetAim(false);

        Sequence seq = DOTween.Sequence();
        this.gameObject.SetActive(true);
        ResetData();

        seq.Join(PanelRT.DOSizeDelta(new Vector2(PanelRT.sizeDelta.x, TargetY), DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("EntranceUIPanel");
    }

    public void CloseThisPanel()
    {
        if (DOTween.IsTweening("EntranceUIPanel"))
        { return; }

        InputTitleManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
        InputManager.Instance.SetAim(true);

        Sequence seq = DOTween.Sequence();
        seq.Join(PanelRT.DOSizeDelta(new Vector2(PanelRT.sizeDelta.x, 0f), DurTime));
        seq.Join(BGCG.DOFade(0f, DurTime));

        seq.SetId("EntranceUIPanel")
            .OnComplete(() =>
            { this.gameObject.SetActive(false); });
    }

    #endregion
}
