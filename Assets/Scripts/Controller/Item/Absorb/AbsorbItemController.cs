using UnityEngine;

public class AbsorbItemController : ItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Absorb")]

    [Space(10)]
    [Header("=== Target")]
    [SerializeField] GameObject TargetGO;

    [Space(10)]
    [Header("=== Absorb")]
    [SerializeField] private float AbsorbStartPower = 300f;
    [SerializeField] private float AbsorbPower = 5f;
    [SerializeField] private float RotatePower = 10f;


    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        ThisRb.AddForce(GetRandomDirForce());
    }


    protected void Update()
    {
        ThisRb.velocity = GetDirForce();
    }

    #endregion

    #region State

    protected void SetState(Vector2 _SpawnPos, GameObject _TargetObject)
    {
        this.gameObject.transform.position = _SpawnPos;
        TargetGO = _TargetObject;
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

    #region Get Item

    protected virtual void GetItem()
    {
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Collision)
    {
        if (_Collision.tag == "Player")
        {
            GetItem();
        }
    }

    #endregion

}
