using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*
BakeResult를 사람이 보기 쉬운 형태로 Console에 출력
출력 형식을 통일하여 Baker는 출력 방식에 대해 신경 쓰지 않도록 분리
*/

/// <summary> Bake 결과를 Console에 출력 </summary>
public static class BakeLogger
{
    private const string Separator = "==================================================";
    private const int BuilderCapacity = 512;

    /// <summary> 단일 Bake 결과를 출력 </summary>
    public static void Log(BakeResult result)
    {
        if (result == null)
            return;

        StringBuilder builder = new(BuilderCapacity);

        builder.AppendLine(Separator);
        builder.AppendLine("<b>Bake Result</b>");
        builder.AppendLine(Separator);

        builder.AppendLine($"Target      : {result.TargetName}");
        builder.AppendLine($"RuntimePath : {result.RuntimePath}");
        builder.AppendLine($"Processed   : {result.Summary}");
        builder.AppendLine($"Elapsed     : {result.ElapsedTime.TotalMilliseconds:F1} ms");

        if (result.HasWarning)
        {
            builder.AppendLine();
            builder.AppendLine($"Warnings ({result.WarningCount})");

            foreach (string warning in result.WarningMessages)
            {
                builder.AppendLine($" • {warning}");
            }

            builder.AppendLine();
            builder.AppendLine("<b>Bake Finished With Warnings.</b>");
            builder.AppendLine(Separator);

            Debug.LogWarning(builder.ToString());
        }
        else
        {
            builder.AppendLine();
            builder.AppendLine("<b>Bake Completed Successfully.</b>");
            builder.AppendLine(Separator);

            Debug.Log(builder.ToString());
        }
    }

    /// <summary> 여러 Bake 결과를 하나로 요약하여 출력 </summary>
    public static void LogSummary(IEnumerable<BakeResult> results)
    {
        if (results == null)
            return;

        SummaryData summary = BuildSummary(results);

        StringBuilder builder = new(BuilderCapacity);

        builder.AppendLine(Separator);
        builder.AppendLine("<b>Bake Summary</b>");
        builder.AppendLine(Separator);

        builder.AppendLine($"Bake Count : {summary.BakeCount}");
        builder.AppendLine($"Processed  : {summary.SuccessCount} / {summary.TotalCount}");
        builder.AppendLine($"Warnings   : {summary.WarningCount}");
        builder.AppendLine($"Elapsed    : {summary.ElapsedTime.TotalMilliseconds:F1} ms");

        builder.AppendLine();

        if (summary.WarningCount == 0)
        {
            builder.AppendLine("<b>All Bake Completed Successfully.</b>");
            builder.AppendLine(Separator);

            Debug.Log(builder.ToString());
        }
        else
        {
            builder.AppendLine("<b>Bake Finished With Warnings.</b>");
            builder.AppendLine(Separator);

            Debug.LogWarning(builder.ToString());
        }
    }

    private static SummaryData BuildSummary(IEnumerable<BakeResult> results)
    {
        SummaryData summary = new();

        foreach (BakeResult result in results)
        {
            if (result == null)
                continue;

            summary.BakeCount++;
            summary.TotalCount += result.TotalCount;
            summary.SuccessCount += result.SuccessCount;
            summary.WarningCount += result.WarningCount;
            summary.ElapsedTime += result.ElapsedTime;
        }

        return summary;
    }

    /// <summary> Summary 계산용 내부 데이터 </summary>
    private struct SummaryData
    {
        public int BakeCount;
        public int TotalCount;
        public int SuccessCount;
        public int WarningCount;
        public TimeSpan ElapsedTime;
    }
}