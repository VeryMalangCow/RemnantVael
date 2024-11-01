using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SetVisibleBuilding : MonoBehaviour
{

    #region Value

    [Header("<><><><><> Visible State")]
    [SerializeField] private float HalfVisibleValue = 0.7f;
    [SerializeField] private float DurTime = 0.3f;
    [SerializeField] private List<SpriteRenderer> SetSRList;

    [HideInInspector] private Sequence ThisSeq;
    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (_Col.tag == "Player")
        {
            SetVisible(HalfVisibleValue);
        }
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        if (_Col.tag == "Player")
        {
            SetVisible(1f);
        }
    }

    #endregion

    #region Set State

    private void SetVisible(float _Alpha)
    {
        if (ThisSeq != null && DOTween.IsTweening(ThisSeq))
        { DOTween.Kill(ThisSeq); }

        ThisSeq = DOTween.Sequence();
        for (int i = 0; i < SetSRList.Count; i++)
        {
            ThisSeq.Join(SetSRList[i].DOFade(_Alpha, DurTime));
        }
    }

    #endregion
}
