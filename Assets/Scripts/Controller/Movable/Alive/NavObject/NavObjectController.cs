using UnityEngine;
using UnityEngine.AI;

public class NavObjectController : AliveObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Nav")]

    [Space(10)]
    [Header("=== Temp")]

    #endregion

    #region - Hide

    [HideInInspector] protected float MoveSpeed;

    [HideInInspector] protected Vector2 MoveAtDir = Vector2.zero;

    #endregion

    #endregion

    #region Offset

    #endregion

    #region Nav

    public void Set_MoveSpeed(float _FollowSpeed)
    {
        MoveSpeed = _FollowSpeed;
    }

    public void Set_NavDir(Transform _TargetTF)
    {
        Set_NavDir(_TargetTF.position);
    }

    public void Set_NavDir(Vector2 _TargetPos)
    {
        MoveAtDir = Get_NextDir(transform.position, _TargetPos);
    }

    public Vector2 Get_NextDir(Vector3 currentPos, Vector3 targetPos)
    {
        NavMeshPath navPath = new NavMeshPath();

        if (!NavMesh.CalculatePath(currentPos, targetPos, NavMesh.AllAreas, navPath) ||
            navPath.corners.Length < 2)
        {
            return Vector2.zero;
        }

        return (navPath.corners[1] - currentPos).normalized;
    }

    public void End_Nav()
    {
        MoveAtDir = Vector2.zero;
    }

    public bool Is_ExistWall(Transform _TargetTF)
    {
        return DevTool.Is_Exist_UseLine(this.transform, _TargetTF, "Wall");
    }

    public bool Is_ExistWall(Vector2 _TargetPos)
    {
        return DevTool.Is_Exist_UseLine(this.transform.position, _TargetPos, "Wall");
    }

    protected Vector2 Get_RandomNavPos(float _radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * _radius;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, _radius, NavMesh.AllAreas);

        return hit.position;
    }

    #endregion
}
