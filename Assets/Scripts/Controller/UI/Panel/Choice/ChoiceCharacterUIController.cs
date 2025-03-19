using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceCharacterUIController : PanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Choice Character UI")]

    [Space(10)]
    [Header("=== Entire Component")]
    [SerializeField] private CanvasGroup BGCG;
    [SerializeField] private RectTransform PanelRT;
    [SerializeField] private OwnBtnEUIController CloseBtn;
    [HideInInspector] private float DurTime = 0.2f;
    [HideInInspector] private float TargetY = 0f;

    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private List<OwnBtnEUIController> CharacterBtnList;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        for (int i = 0; i < CharacterBtnList.Count; i++)
        {
            CharacterBtnList[i].Offset();
            CharacterBtnList[i].OwnerUIController = this;
        }

        TargetY = PanelRT.sizeDelta.y;
        PanelRT.sizeDelta = new Vector2(PanelRT.sizeDelta.x, 0f);
        BGCG.alpha = 0f;
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        if (CurrentBtn == null || !CurrentBtn.ThisBtn.interactable)
        { return; }

        if (CurrentBtn == CloseBtn)
        {
            SetOff_ThisPanel();
        }
        if (CharacterBtnList.Contains(CurrentBtn))
        {
            int index = CharacterBtnList.IndexOf(CurrentBtn);
#if UNITY_EDITOR
            Debug.Log("선택한 캐릭터의 ID: " + index);
#endif
            GameManager.Instance.DesignatedPlayerPrefab = SaveDataManager.Instance.CharacterPrefabs[index];
        }
    }

    #endregion

    #region Set Panel

    private void Reset_Data()
    {
        List<int> canUseIDList = SaveDataManager.Instance.CharacterSaveData.Get_CanUseIDList();
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


    public override void SetOn_ThisPanel()
    {
        //No Play Base

        if (DOTween.IsTweening("ChoiceCharacterUIPanel"))
        { return; }

        TitleInputManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName); 
        //InputManager.Instance.SetAim(false);

        Sequence seq = DOTween.Sequence();
        this.gameObject.SetActive(true);
        Reset_Data();

        seq.Join(PanelRT.DOSizeDelta(new Vector2(PanelRT.sizeDelta.x, TargetY), DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("ChoiceCharacterUIPanel");
    }

    public override void SetOff_ThisPanel()
    {
        //base
        if (DOTween.IsTweening("ChoiceCharacterUIPanel"))
        { return; }

        TitleInputManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
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
