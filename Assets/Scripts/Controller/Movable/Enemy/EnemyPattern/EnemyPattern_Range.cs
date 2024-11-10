using System.Collections;

public class EnemyPattern_Range : EnemyPattern
{
    #region Pattern

    public override bool CanPlayPattern()
    {
        return true;
    }

    public override void StartPattern()
    {
        StartCoroutine(ThisPattern());
    }

    public override void EndPattern()
    {
        
    }

    #endregion

    #region Actual

    private IEnumerator ThisPattern()
    {
        yield return null;
    }

    #endregion
}

