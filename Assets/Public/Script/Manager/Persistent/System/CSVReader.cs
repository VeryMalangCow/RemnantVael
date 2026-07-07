using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVReader
{
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static string WORD_SPLIT_RE = @",";

    public static ModuleBaseData[] GetModuleBaseDatas(TextAsset textAsset)
    {
        string[][] csvData = GetCsvData(textAsset.text);
        ModuleBaseData[] result = new ModuleBaseData[csvData.Length - 1];

        for (int i = 1; i < csvData.Length; i++)
        {
            if (csvData[i][0] == "") break;

            int id = int.Parse(csvData[i][0]);
            int r1mainChip = int.Parse(csvData[i][1]);
            int r3mainChip = int.Parse(csvData[i][2]);
            int r5mainChip = int.Parse(csvData[i][3]);

            result[i - 1] = new ModuleBaseData(id, new List<int> { r1mainChip, r3mainChip, r5mainChip });
        }

        return result;
    }

    public static AllyCardBaseData[] GetAllyCardBaseDatas(TextAsset textAsset)
    {
        string[][] csvData = GetCsvData(textAsset.text);
        AllyCardBaseData[] result = new AllyCardBaseData[csvData.Length - 1];

        for (int i = 1; i < csvData.Length; i++)
        {
            if (csvData[i][0] == "") break;

            int id = int.Parse(csvData[i][0]);
            int rank = int.Parse(csvData[i][1]);
            int essentialID = int.Parse(csvData[i][2]);

            result[i - 1] = new AllyCardBaseData(id, rank, essentialID);
        }

        return result;
    }

    public static LanguageSet[] GetLanguageSets(TextAsset textAsset)
    {
        // ID -> (Index -> Languages)
        Dictionary<int, Dictionary<int, string[]>> tempDict = new Dictionary<int, Dictionary<int, string[]>>();
        string[][] csvData = GetCsvData(textAsset.text);

        int maxID = -1;

        for (int i = 1; i < csvData.Length; i++)
        {
            // ID
            if (!int.TryParse(csvData[i][0], out int id))
                continue;

            // Index
            if (!int.TryParse(csvData[i][1], out int index))
                continue;

            if (!tempDict.TryGetValue(id, out Dictionary<int, string[]> languageDict))
            {
                languageDict = new Dictionary<int, string[]>();
                tempDict.Add(id, languageDict);
            }

            string[] languages = new string[csvData[i].Length - 2];
            for (int j = 2; j < csvData[i].Length; j++)
                languages[j - 2] = csvData[i][j];

            languageDict.Add(index, languages);

            if (id > maxID)
                maxID = id;
        }

        LanguageSet[] result = new LanguageSet[maxID + 1];

        foreach (var pair in tempDict)
            result[pair.Key] = new LanguageSet(pair.Value);

        return result;
    }

    public static LanguageSet GetLanguageSet(TextAsset textAsset)
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

        return new LanguageSet(csvDict);
    }


    public static LanguageColorSet GetLanguageColorSet(TextAsset textAsset)
    {
        Dictionary<int, ColorStringArray> csvDict = new Dictionary<int, ColorStringArray>();
        string[][] csvData = GetCsvData(textAsset.text);

        int i, j;
        for (i = 1; i < csvData.Length; i++)
        {
            int id = int.TryParse(csvData[i][0], out int _id) ? _id : -1;

            if (id == -1)
                continue;

            string clrHex = csvData[i][1];
            string[] languages = new string[csvData[i].Length - 2];
            for (j = 2; j < csvData[i].Length; j++)
                languages[j - 2] = csvData[i][j];

            csvDict.Add(id, new ColorStringArray(clrHex, languages));
        }

        return new LanguageColorSet(csvDict);
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

    // 해당 라인만 가져오기
    public static string[] GetCsvLine(string text, int index)
    {
        string[] lines = Regex.Split(text, LINE_SPLIT_RE);
        if (lines.Length - 1 < index)
            return null;

        string[] words = Regex.Split(lines[index], WORD_SPLIT_RE);
        string[] result = new string[words.Length - 1];
        for (int i = 1; i < words.Length; i++)
            result[i - 1] = words[i];

        return result;
    }
}
