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

                //PointEffect(effectPos, AttackerState.DamageType, isCritical);

                //Damage
                EC.TakeDamage(AttackerState, isCritical, dir);
                HittedObjectList.Add(EC);
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildingController DBC))
            {
                DBC.TakeDamage(true);
            }
        }
    }

    #endregion
}
