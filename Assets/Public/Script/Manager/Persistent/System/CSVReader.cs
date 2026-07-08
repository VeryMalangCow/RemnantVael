using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVReader
{
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static string WORD_SPLIT_RE = @",";

    // Event
    public static EventID[] GetEventIds(TextAsset textAsset)
    {
        List<EventID> result = new List<EventID>();

        string[][] stringArr = GetCsvData(textAsset.text);

        for (int i = 1; i < stringArr.Length; i++)
        {
            if (stringArr[i][0] == "") break;

            int id = int.Parse(stringArr[i][0]);
            List<int> idList = new List<int>();

            for (int j = 1; j < stringArr[i].Length; j++)
            {
                if (stringArr[i][j] == "" || stringArr[i][j] == null) break;

                idList.Add(int.Parse(stringArr[i][j]));
            }

            result.Add(new EventID(id, idList.ToArray()));
        }

        return result.ToArray();
    }
    public static EventElement[] GetEventElements(TextAsset textAsset)
    {
        List<EventElement> result = new List<EventElement>();

        string[][] stringArr = GetCsvData(textAsset.text);

        for (int i = 1; i < stringArr.Length; i++)
        {
            if (stringArr[i][0] == "") break;

            EventElement eventElement = new EventElement();

            int id = int.Parse(stringArr[i][0]);
            string name = stringArr[i][1];

            // 정지
            if (name == "Stay")
            {
                float targetTime = float.Parse(stringArr[i][2]);

                eventElement = new EventElement_Stay(id, targetTime);
            }
            // 바라보기
            else if (name == "Look")
            {
                string[] vectorString = stringArr[i][2].Split("/");
                Vector2 vector = new Vector2(float.Parse(vectorString[0]), float.Parse(vectorString[1]));

                eventElement = new EventElement_Look(id, vector);
            }
            // 이동
            else if (name == "Move")
            {
                int targetId = int.Parse(stringArr[i][2]);
                string targetType = stringArr[i][3];
                string[] vectorString = stringArr[i][4].Split("/");
                Vector2 vector = new Vector2(float.Parse(vectorString[0]), float.Parse(vectorString[1]));

                eventElement = new EventElement_Move(id, targetId, targetType, vector);
            }
            // 검은 화면 키기
            else if (name == "BlackScreenIn")
            {
                float targetTime = float.Parse(stringArr[i][2]);

                eventElement = new EventElement_BlackScreenIn(id, targetTime);
            }
            // 검은 화면 끄기
            else if (name == "BlackScreenOut")
            {
                float targetTime = float.Parse(stringArr[i][2]);

                eventElement = new EventElement_BlackScreenOut(id, targetTime);
            }
            // 다이얼로그
            else if (name == "Dialogue")
            {
                int targetId = int.Parse(stringArr[i][2]);

                eventElement = new EventElement_Dialogue(id, targetId);
            }
            // 컷씬
            else if (name == "Cutscene")
            {
                int targetId = int.Parse(stringArr[i][2]);
                int soundId = int.Parse(stringArr[i][3]);

                eventElement = new EventElement_Cutscene(id, targetId, soundId);
            }

            result.Add(eventElement);
        }

        return result.ToArray();
    }

    // Cutscene
    public static CutsceneID[] GetCutsceneIds(TextAsset textAsset)
    {
        List<CutsceneID> result = new List<CutsceneID>();

        string[][] stringList = GetCsvData(textAsset.text);

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);

            List<int> idList = new List<int>();
            for (int j = 1; j < stringList[i].Length; j++)
            {
                if (stringList[i][j] == "" || stringList[i][j] == null) break;

                int elementId = int.Parse(stringList[i][j]);
                idList.Add(elementId);
            }

            result.Add(new CutsceneID(id, idList.ToArray()));
        }

        return result.ToArray();
    }
    public static CutsceneElement[] GetCutsceneElements(TextAsset textAsset)
    {
        List<CutsceneElement> result = new List<CutsceneElement>();

        string[][] stringList = GetCsvData(textAsset.text);

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            string script = stringList[i][1];

            result.Add(new CutsceneElement(id, script));
        }

        return result.ToArray();
    }

    // Dialogue
    public static DialogueID[] GetDialogueIds(TextAsset textAsset)
    {
        List<DialogueID> result = new List<DialogueID>();

        string[][] stringList = GetCsvData(textAsset.text);

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);

            List<int> idList = new List<int>();
            for (int j = 1; j < stringList[i].Length; j++)
            {
                if (stringList[i][j] == "" || stringList[i][j] == null) break;
                int elementId = int.Parse(stringList[i][j]);
                idList.Add(elementId);
            }

            result.Add(new DialogueID(id, idList.ToArray()));
        }

        return result.ToArray();
    }
    public static DialogueElement[] GetDialogueElements(TextAsset textAsset)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        string[][] stringList = GetCsvData(textAsset.text);

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            string name = stringList[i][1];
            string script = stringList[i][2];
            string imgId = stringList[i][3];
            bool isLeft = bool.Parse(stringList[i][4]);

            result.Add(new DialogueElement(id, name, script, imgId, isLeft));
        }

        return result.ToArray();
    }


    // Module
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

    // Ally Card
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


    // Language
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
