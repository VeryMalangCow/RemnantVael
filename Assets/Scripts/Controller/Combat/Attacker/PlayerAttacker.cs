using UnityEngine;

public class PlayerAttacker : Attacker
{
    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (!this.gameObject.activeSelf)
        { return; }

        // Hit Enemy
        if (_Col.tag == "Enemy")
        {
            if (_Col.transform.parent.TryGetComponent(out EnemyController EC) &&
                !HittedObjectList.Contains(EC))
            {
                // Dir
                Vector2 dir = _Col.gameObject.transform.position - this.transform.position;

                // Critical
                bool isCritical = false;
                if (Random.Range(0f, 1f) < AttackerState.CD)
                {
                    isCritical = true;
                }

                // Effect
                Vector2 effectPos = ThisCol.ClosestPoint(EC.TargetObject.transform.position);

                PointEffect(effectPos, isCritical);

                //Damage
                EC.TakeDamage(AttackerState, isCritical, dir);
                HittedObjectList.Add(EC);
            }
        }
    }

    #endregion

    #region Effect

    public void PointEffect(Vector2 _SpanwedPos, bool _IsCritical)
    {
        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        if (!_IsCritical)
        { oota.StartAnim(PlayerManager.Instance.PlayerController.BaseHittedPointAC, _SpanwedPos, 2f, 3f); }
        else
        { oota.StartAnim(PlayerManager.Instance.PlayerController.CriticalHittedPointAC, _SpanwedPos, 2f, 3f); }
    }

    #endregion
}
