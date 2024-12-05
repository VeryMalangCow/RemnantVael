using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyMinimap : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] private GameObject MinimapElement;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform MinimapElementParentRT;
    [SerializeField] private Image InnerImg;

    [Space(10)]
    [Header("=== Element")]
    [SerializeField] private List<ModifyMinimapElement> AllMMEs;
    

    #endregion

    #region Offset

    public override void Offset()
    {
        
    }

    #endregion

    #region Generate

    public void GenMinimap()
    {
        AllMMEs = new List<ModifyMinimapElement>();
        List<RoomController> allRC = StageManager.Instance.GetAllRC();

        Color mainClr = InnerImg.color;
        mainClr.a = 1f;

        for (int i = 0; i < allRC.Count; i++)
        {
            GenMinimapElement(allRC[i], mainClr);
        }
    }

    private void GenMinimapElement(RoomController _ConnetedRoom, Color _Clr)
    {
        if (Instantiate(MinimapElement, MinimapElementParentRT.transform).TryGetComponent(out ModifyMinimapElement mme))
        {
            mme.gameObject.SetActive(false);
            mme.Offset(_ConnetedRoom, _Clr);
            AllMMEs.Add(mme);
        }
    }

    #endregion

    #region Set State

    public void SetState()
    {
        RoomController CurrentRC = StageManager.Instance.CurrentRoomController;
        if (CurrentRC.ThisMME.gameObject.TryGetComponent(out RectTransform rt))
        {
            if (DOTween.IsTweening(MinimapElementParentRT))
            { DOTween.Kill(MinimapElementParentRT); }

            MinimapElementParentRT.DOAnchorPos(-rt.anchoredPosition, 0.5f);
        }


        // PC가 있는 방
        SetActiveMME(CurrentRC.ThisMME);
        if (CurrentRC.RoomRuleController.RoomType == eRoomType.Completed)
        {
            CurrentRC.ThisMME.SetState_Complete();
        }
        else
        {
            CurrentRC.ThisMME.SetState_Uncomplete();
        }

        // PC가 있는 방의 인접한 방
        List<RoomController> connectedAllRC = CurrentRC.GetConnectedRCList();
        for (int i = 0; i < connectedAllRC.Count; i++)
        {
            SetActiveMME(connectedAllRC[i].ThisMME);
            if (connectedAllRC[i].RoomRuleController.RoomType != eRoomType.Completed)
            {
                connectedAllRC[i].ThisMME.SetState_Visible();
            }
        }
    }

    private void SetActiveMME(ModifyMinimapElement _MME)
    {
        if (!_MME.gameObject.activeSelf)
        {
            _MME.SetActiveOn();
        }
    }

    public void PlayEffect()
    {
        // 효과
        if (DOTween.IsTweening(InnerImg))
        { DOTween.Kill(InnerImg); }

        InnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                InnerImg.DOFade(0.25f, 0.2f);
            });
    }

    #endregion
}
