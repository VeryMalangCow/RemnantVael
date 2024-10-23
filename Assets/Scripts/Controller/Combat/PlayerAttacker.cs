using UnityEngine;

public class PlayerAttacker : Attacker
{
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
                Debug.Log("Ãæµ¹");
                Vector2 dir = _Col.gameObject.transform.position - this.transform.position;
                EC.TakeDamage(this.AttackerState, dir);
                HittedObjectList.Add(EC);
            }
        }
    }
}
