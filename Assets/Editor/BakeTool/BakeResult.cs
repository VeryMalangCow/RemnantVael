using System;
using System.Collections.Generic;

/*
Bake 작업 결과를 저장하는 데이터 객체
Logger와 Baker 간의 결과 전달 역할
데이터만 보관하며, Bake 수행이나 출력 기능은 담당하지 않음
*/

/// <summary> Bake 작업의 결과를 저장하는 데이터 셋 </summary>
public sealed class BakeResult
{
    /// <summary> Bake 대상 이름</summary>
    public string TargetName { get; set; }

    /// <summary> 전체 처리 대상 개수 </summary>
    public int TotalCount { get; set; }

    /// <summary> Runtime 저장 경로</summary>
    public string RuntimePath { get; set; }

    /// <summary> 소요 시간 </summary>
    public TimeSpan ElapsedTime { get; set; }


    // Success
    /// <summary> 성공 개수 </summary>
    public int SuccessCount { get; set; }

    /// <summary> 모든 작업이 성공? </summary>
    public bool IsSuccess => WarningCount == 0;

    // Warning
    private readonly List<string> warnings = new();

    /// <summary> Warning 목록 (읽기 전용) </summary>
    public IReadOnlyList<string> WarningMessages => warnings;

    /// <summary> Warning 개수 </summary>
    public int WarningCount => warnings.Count;

    /// <summary> Warning 존재 여부 </summary>
    public bool HasWarning => WarningCount > 0;

    /// <summary> Warning을 추가 </summary>
    public void AddWarning(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        warnings.Add(message);
    }

    /// <summary> 성공/전체 개수 요약 </summary>
    public string Summary => $"{SuccessCount} / {TotalCount}";


    /// <summary> 결과를 초기화 </summary>
    public void Clear()
    {
        TargetName = string.Empty;
        RuntimePath = string.Empty;

        TotalCount = 0;
        SuccessCount = 0;

        ElapsedTime = TimeSpan.Zero;

        warnings.Clear();
    }
}
