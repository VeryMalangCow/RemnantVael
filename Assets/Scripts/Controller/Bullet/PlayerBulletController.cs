using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Framework

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Time
        currentAliveTime += Time.deltaTime;
        if (currentAliveTime >= aliveTime)
        {
            this.gameObject.SetActive(false);
            ObjectPoolingManager.Instance.SetOP_PlayerBulletController(this);
        }
    }

    #endregion

    #region State

    public override void SetState(Vector2 SpawnVec, float _baseDamage, float _muzzleSpeed, float _aliveTime)
    {
        base.SetState(SpawnVec, _baseDamage, _muzzleSpeed, _aliveTime);

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
            ObjectPoolingManager.Instance.SetOP_PlayerBulletController(this);
            this.gameObject.SetActive(false);
        }
    }

    #endregion
}
