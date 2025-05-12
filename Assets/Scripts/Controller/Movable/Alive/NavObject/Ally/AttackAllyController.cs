using System.Collections;
using UnityEngine;

public class AttackAllyController : AllyController
{

    #region Play

    protected override IEnumerator Play_Main_Cor()
    {
        yield return StartCoroutine(base.Play_Main_Cor());

        while (true)
        {
            if (Enemy != null)
            {
                Set_PingedState();
                yield return new WaitForSeconds(FollowInitDelay);
            }
            else
            {
                Set_NoPingedState();
                yield return new WaitForSeconds(FollowInitDelay);
            }
        }
    }

    #endregion

    #region Set (Ping)

    private void Set_PingedState() // 적에게
    {
        // 따라가기
        if (Is_FollowState(Enemy.transform, ForEnemyDis, true))
        {
            Set_NavDir(Enemy.transform);
        }
        // 공격
        else
        {
            Stop_Follow();
        }
    }


    private void Set_NoPingedState() // 플레이어에게
    {
        // 따라가기
        if (Is_FollowState(Player.transform, ForPlayerDis, false))
        {
            Debug.Log("Player Follow");
            Set_NavDir(Player.transform);
        }
        // 정지
        else
        {
            Stop_Follow();
        }
    }

    #endregion
}
