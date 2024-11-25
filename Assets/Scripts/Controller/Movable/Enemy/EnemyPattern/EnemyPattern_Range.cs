using UnityEngine;
using System.Collections;

public class EnemyPattern_Range : EnemyPattern
{
    #region Framework

    private void Update()
    {
        if (!IsPlayingThisPattern)
        {
            StopCoroutine(ThisPattern());
        }
    }

    #endregion

    #region Can Check

    public override bool CanPlayPattern()
    {
        return true;
    }

    #endregion

    #region Start End

    public override void StartPattern()
    {

        base.StartPattern();
    }

    public override void EndPattern()
    {

        base.EndPattern();
    }

    #endregion

    #region Actual

    protected override IEnumerator ThisPattern()
    {
        yield return new WaitForSeconds(StartDelay);

        Vector2 targetDir =
            ((Vector2)PlayerManager.Instance.PlayerController.transform.position
            - (Vector2)ThisEnemy.transform.position).normalized;



        yield return new WaitForSeconds(EndDelay);

        EndPattern();
        ThisEnemy.TryGetAnyPattern();
    }

    #endregion
}

