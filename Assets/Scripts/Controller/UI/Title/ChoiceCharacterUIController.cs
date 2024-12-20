using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceCharacterUIController : UIController
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
    [SerializeField] private List<ModifyOwnEachBtn> CharacterBtnList;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        for (int i = 0; i < CharacterBtnList.Count; i++)
        { 
            CharacterBtnList[i].Offset();
            CharacterBtnList[i].OwnerUIController = this;
        }
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
        if (CharacterBtnList.Contains(CurrentBtn))
        {
            int index = CharacterBtnList.IndexOf(CurrentBtn);
            Debug.Log(index);
            GameManager.Instance.DesignatedPlayerPrefab = SaveDataManager.Instance.CharacterPrefabs[index];
        }
    }

    #endregion

    #region Set Panel

    private void ResetData()
    {
        List<int> canUseIDList = SaveDataManager.Instance.CharacterSaveData.GetCanUseIDList();
        for (int i = 0; i < CharacterBtnList.Count; i++)
        {
            if (canUseIDList.Contains(i))
            {
                CharacterBtnList[i].ThisBtn.interactable = true;
            }
            else
            {
                CharacterBtnList[i].ThisBtn.interactable = false;
            }
        }
    }


    public override void OpenThisPanel()
    {
        //base

        if (DOTween.IsTweening("ChoiceCharacterUIPanel"))
        { return; }

        InputTitleManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName); 
        //InputManager.Instance.SetAim(false);

        Sequence seq = DOTween.Sequence();
        this.gameObject.SetActive(true);
        ResetData();

        seq.Join(PanelRT.DOSizeDelta(new Vector2(PanelRT.sizeDelta.x, TargetY), DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("ChoiceCharacterUIPanel");
    }

    public override void CloseThisPanel()
    {
        //base
        if (DOTween.IsTweening("ChoiceCharacterUIPanel"))
        { return; }

        InputTitleManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
        //InputManager.Instance.SetAim(true);

        Sequence seq = DOTween.Sequence();
        seq.Join(PanelRT.DOSizeDelta(new Vector2(PanelRT.sizeDelta.x, 0f), DurTime));
        seq.Join(BGCG.DOFade(0f, DurTime));

        seq.SetId("ChoiceCharacterUIPanel")
            .OnComplete(() =>
            { this.gameObject.SetActive(false); });
    }

    #endregion
}
