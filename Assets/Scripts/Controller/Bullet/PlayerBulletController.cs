using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Framework

    protected override void Update()
    {
        base.Update();

        // Time
        currentAliveTime += Time.deltaTime;
        if (currentAliveTime >= BulletState.aliveTime)
        {
            this.gameObject.SetActive(false);
            PoolingManager.Instance.PlayerBulletQueue.Enqueue(this);
        }
    }

    #endregion

    #region State

    public override void SetState(Vector2 SpawnVec, eDamageType damageType, float _baseDamage, float _muzzleSpeed, float _aliveTime)
    {
        base.SetState(SpawnVec, damageType, _baseDamage, _muzzleSpeed, _aliveTime);

        Vector2 dir = InputManager.Instance.DirFromPlayerPos.normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, dir));

        this.transform.localRotation = targetQuat;
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit Enemy
        if (collision.tag == "Enemy")
        {
            Debug.Log("Àû Ãæµ¹");
            PoolingManager.Instance.PlayerBulletQueue.Enqueue(this);
            this.gameObject.SetActive(false);
        }
    }

    #endregion
}
