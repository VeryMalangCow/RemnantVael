using UnityEngine;

public class AbsorbItemController : ItemController
{
    #region Value

    [Space(20)] [Header("<><><><><> Absorb")]

    [Header("=== Target")]
    [SerializeField] GameObject TargetGO;

    [Header("=== Absorb")]
    [SerializeField] private float AbsorbStartPower = 300f;
    [SerializeField] private float AbsorbPower = 5f;
    [SerializeField] private float RotatePower = 10f;


    #endregion

    #region Framework

    private void OnEnable()
    {
        TargetGO = PlayerManager.Instance.PlayerController.gameObject;
        ThisRb.AddForce(GetRandomDirForce());
    }

    protected override void Update()
    {
        base.Update();
        ThisRb.velocity = GetDirForce();
    }

    #endregion

    #region Absorb

    private Vector2 GetRandomDirForce()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        return new Vector2(x, y).normalized * AbsorbStartPower;
    }

    private Vector2 GetDirForce()
    {
        Vector2 resultVelocity = Vector2.Lerp(
            ThisRb.velocity.normalized, 
            (Vector2)(TargetGO.transform.position - this.transform.position).normalized, 
            RotatePower * Time.deltaTime);
        float currentVelocityPower = Vector2.Distance(Vector2.zero, ThisRb.velocity) * 0.99f;

        if (currentVelocityPower < AbsorbPower)
        { currentVelocityPower = AbsorbPower; }

        return resultVelocity * currentVelocityPower;
    }
    #endregion

}
