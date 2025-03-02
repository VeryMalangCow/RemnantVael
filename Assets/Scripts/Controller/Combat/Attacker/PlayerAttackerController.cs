using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackerController : AttackerController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Object")]
    [SerializeField] protected List<MovableObjectController> HittedObjectList = new List<MovableObjectController>();

    #endregion

    #region Framework

    private void OnEnable()
    {
        HittedObjectList.Clear();
    }

    #endregion

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
                EC.Take_Damaged(AttackerState, isCritical, dir);
                HittedObjectList.Add(EC);
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildController DBC))
            {
                DBC.Take_Damage(true);
            }
        }
    }

    #endregion
}
