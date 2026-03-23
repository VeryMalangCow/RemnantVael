using System.Collections.Generic;
using UnityEngine;

public class FieldObjectSpawnController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Field Object Spawn")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private int amount = 24;
    [SerializeField] private Vector2 spreadDir;
    [SerializeField] private float angle = 90;
    [SerializeField] private float radius = 10;
    [SerializeField] private float radialBias = 4f;

    [HideInInspector] private static readonly float intervalX = 0.5f;
    [HideInInspector] private static readonly float intervalY = 0.25f;

    #endregion

    #region Get

    #region Static

    private static Vector2Int RandomPointInSectorInt(
        Vector2 spreadDir, float angleDeg, float radius, float radialBias)
    {
        // 중심 각도
        float centerRad = Mathf.Atan2(spreadDir.y, spreadDir.x);

        // 각도 범위
        float halfRad = Mathf.Abs(angleDeg) * 0.5f * Mathf.Deg2Rad;
        if (halfRad <= 0f) halfRad = 1e-6f;
        float theta = Random.Range(centerRad - halfRad, centerRad + halfRad);

        // 반경 샘플링
        float gamma = Mathf.Max(radialBias, 0.0001f);
        float u = Random.value;
        float r = radius * Mathf.Pow(u, gamma);

        // 부채꼴 좌표 (float)
        Vector2 pos = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r;

        // 정수 변환
        return Vector2Int.RoundToInt(pos);
    }

    private static List<Vector2> RandomPointsInSector(
        Vector2 pos, int count, Vector2 spreadDir, float angleDeg, float radius, float radialBias)
    {
        float specialIntervalX = intervalX * 0.5f;

        var list = new List<Vector2>(Mathf.Max(0, count));
        for (int i = 0; i < count; i++)
        {
            Vector2 vec = RandomPointInSectorInt(spreadDir, angleDeg, radius, radialBias);
            vec = pos + new Vector2(vec.x * intervalX, vec.y * intervalY);
            float y = 0 <= vec.y ? vec.y : -vec.y;
            if (y != 0 && y % (intervalY * 2) != 0)
            {
                float x = vec.x;
                x += spreadDir.x > 0 ? specialIntervalX : -specialIntervalX;

                vec = new Vector2(x, vec.y);
            }

            list.Add(vec);
        }
        
        return list;
    }

    #endregion

    #region Points

    public List<Vector2> Get_RandomPointsInSector_Self()
    {
        return RandomPointsInSector(transform.position, amount, spreadDir, angle, radius, radialBias);
    }

    #endregion

    #endregion
}
