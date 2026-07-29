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

    [HideInInspector] protected float moveSpeed;

    [HideInInspector] protected Vector2 moveAtDir = Vector2.zero;

    private readonly NavMeshPath navPath = new();

    #endregion

    #endregion

    #region Offset

    #endregion

    #region Nav

    public void Set_MoveSpeed(float followSpeed)
    {
        moveSpeed = followSpeed;
    }

    public void SetNavDir(Transform targetTf)
    {
        Set_NavDir(targetTf.position);
    }

    public void Set_NavDir(Vector2 targetPos)
    {
        moveAtDir = Get_NextDir(transform.position, targetPos);
    }

    public Vector2 Get_NavDir()
    {
        return moveAtDir;
    }

    public Vector2 Get_NextDir(Vector3 currentPos, Vector3 targetPos)
    {
        if (!NavMesh.CalculatePath(currentPos, targetPos, NavMesh.AllAreas, navPath))
            return Vector2.zero;

        Vector3[] corners = navPath.corners;

        if (corners.Length < 2)
            return Vector2.zero;

        return (corners[1] - currentPos).normalized;
    }

    public void EndNav()
    {
        moveAtDir = Vector2.zero;
    }

    public bool IsExistWall(Transform targetTf)
    {
        return DevTool.Is_Exist_UseLine(this.transform, targetTf, "Wall");
    }

    public bool Is_ExistWall(Vector2 targetPos)
    {
        return DevTool.Is_Exist_UseLine(this.transform.position, targetPos, "Wall");
    }

    protected Vector2 Get_RandomNavPos(Vector3 centerPos, float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos += centerPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }

    protected Vector2 Get_RandomNavPos(float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }

    #endregion
}
