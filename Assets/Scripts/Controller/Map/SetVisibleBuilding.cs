using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SetVisibleBuilding : MonoBehaviour
{

    #region Value

    [Header("<><><><><> Visible State")]
    [SerializeField] private bool IsCompletlyVisible = true;
    [SerializeField] private float HalfVisibleValue = 0.7f;
    [SerializeField] private float DurTime = 0.3f;
    [SerializeField] private List<SpriteRenderer> SetSRList;

    [HideInInspector] private Sequence ThisSeq;
    [SerializeField] private HashSet<Collider2D> currentCollisions = new HashSet<Collider2D>();

    #endregion

    #region Framework

    private void LateUpdate()
    {
        if (IsColliding() && IsCompletlyVisible == true)
        {
            IsCompletlyVisible = false;
            SetVisible(HalfVisibleValue);
        }
        else if (!IsColliding() && IsCompletlyVisible == false)
        {
            IsCompletlyVisible = true;
            SetVisible(1f);
        }
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        currentCollisions.Add(_Col);
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        currentCollisions.Remove(_Col);
    }

    #endregion

    #region Set State

    public bool IsColliding()
    {
        return currentCollisions.Count > 0;
    }

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
