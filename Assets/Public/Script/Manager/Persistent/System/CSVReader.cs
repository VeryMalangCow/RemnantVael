using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVReader
{
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static string WORD_SPLIT_RE = @",";

    public static Dictionary<int, string[]> GetLanguageSet(TextAsset textAsset)
    {
        Dictionary<int, string[]> csvDict = new Dictionary<int, string[]>();
        string[][] csvData = GetCsvData(textAsset.text);

        int i, j;
        for (i = 1; i < csvData.Length; i++)
        {
            int id = int.TryParse(csvData[i][0], out int _id) ? _id : -1;

            if (id == -1)
                continue;

            string[] languages = new string[csvData[i].Length - 1];
            for (j = 1; j < csvData[i].Length; j++)
                languages[j - 1] = csvData[i][j];

            csvDict.Add(id, languages);
        }

        return csvDict;
    }

    // 파일을 이중 리스트(string)으로 변경
    private static string[][] GetCsvData(string text)
    {
        string[] lines = Regex.Split(text, LINE_SPLIT_RE);
        string[][] result = new string[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
            result[i] = Regex.Split(lines[i], WORD_SPLIT_RE);

        return result;
    }
}
