#if UNITY_EDITOR

using UnityEngine;

public struct PatternPreviewElement
{
    public string name;
    public PatternPreviewDetail left;
    public PatternPreviewDetail oper;
    public PatternPreviewDetail right;
    public PatternPreviewDescription desc;

    public PatternPreviewElement(
        string name, 
        PatternPreviewDetail left, 
        PatternPreviewDetail oper, 
        PatternPreviewDetail right,
        PatternPreviewDescription desc)
    {
        this.name = name;

        this.left = left;
        this.oper = oper;
        this.right = right;

        this.desc = desc;
    }
}

public struct PatternPreviewDetail
{
    public string name;
    public Color clr;

    public PatternPreviewDetail(string name, Color clr)
    {
        this.name = name;
        this.clr = clr;
    }

    public PatternPreviewDetail(string name)
    {
        this.name = name;
        this.clr = new Color(0.8f, 0.8f, 0.8f, 1f);
    }
}

public struct PatternPreviewDescription
{
    public string desc;

    public PatternPreviewDescription(string desc)
    {
        this.desc = desc;
    }
}

public static class PatternDraw
{
    private static readonly float showDuration = 1f;
    public static void DrawCircleCast(Vector2 start, Vector2 end, float radius)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-dir.y, dir.x);

        // 양쪽 경계선
        Debug.DrawLine(
            start + perpendicular * radius,
            end + perpendicular * radius,
            Color.yellow,
            showDuration);

        Debug.DrawLine(
            start - perpendicular * radius,
            end - perpendicular * radius,
            Color.yellow,
            showDuration);

        // 시작/끝 원
        DrawCircle(start, radius, Color.yellow);
        DrawCircle(end, radius, Color.yellow);
    }

    private static void DrawCircle(Vector2 center, float radius, Color color)
    {
        const int segmentCount = 32;

        float angleStep = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float angleA = angleStep * i * Mathf.Deg2Rad;
            float angleB = angleStep * (i + 1) * Mathf.Deg2Rad;

            Vector2 a = center + new Vector2(
                Mathf.Cos(angleA),
                Mathf.Sin(angleA)) * radius;

            Vector2 b = center + new Vector2(
                Mathf.Cos(angleB),
                Mathf.Sin(angleB)) * radius;

            Debug.DrawLine(a, b, color, showDuration);
        }
    }
}

#endif