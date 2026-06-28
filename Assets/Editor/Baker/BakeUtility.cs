using System.IO;

/*
모든 Baker가 공통으로 사용하는 유틸리티 클래스
특정 Bake에 종속되지 않는 공용 기능만 담당
*/

/// <summary> Bake 과정에서 공통으로 사용하는 Utility 함수 모음 </summary>
public static class BakeUtility
{
    private const string EditorFolderName = "/Editor/";
    private const string RuntimeFolderName = "/Runtime/";

    /// <summary> Editor Folder 내부의 에셋인지 확인 </summary>
    public static bool IsEditorPath(string assetPath)
        => !string.IsNullOrEmpty(assetPath) && assetPath.Contains(EditorFolderName);

    /// <summary> From Editor Path -> To Runtime Path </summary>
    public static string ConvertToRuntimePath(string editorPath)
        => editorPath.Replace(EditorFolderName, RuntimeFolderName);

    /// <summary> Runtime Path에 Folder 없으면 생성 </summary>
    public static void EnsureFolderExists(string assetPath)
    {
        string directory = Path.GetDirectoryName(assetPath);

        if (string.IsNullOrEmpty(directory)) return;
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }
}
