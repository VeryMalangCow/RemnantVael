using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BuildOpacityController : MonoBehaviour
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
        if (Is_Colliding() && IsCompletlyVisible == true)
        {
            IsCompletlyVisible = false;
            Set_Visible(HalfVisibleValue);
        }
        else if (!Is_Colliding() && IsCompletlyVisible == false)
        {
            IsCompletlyVisible = true;
            Set_Visible(1f);
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

    public bool Is_Colliding()
    {
        return currentCollisions.Count > 0;
    }

    private void Set_Visible(float _Alpha)
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
