using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager : PersistentSingleton<ResourceManager>
{
    #region File

    [HideInInspector] private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    [HideInInspector] private static string WORD_SPLIT_RE = @",";

    // 파일 => 스트링
    private static string Get_FileString(TextAsset textAsset)
    {
        return textAsset.text;
    }

    // 행 길이 구하기
    private static int Get_FileRowAmount(TextAsset textAsset)
    {
        return Get_AllLine(textAsset).Length;
    }

    // 행 받아오기
    private static string[] Get_AllLine(TextAsset textAsset)
    {
        return Regex.Split(Get_FileString(textAsset), LINE_SPLIT_RE);
    }

    // 열을 쉼표로 나누기
    private static string[] Get_Words(TextAsset textAsset, int _Row)
    {
        return Regex.Split(Get_AllLine(textAsset)[_Row], WORD_SPLIT_RE);
    }

    // 파일을 이중 리스트(string)으로 변경
    private static string[][] Get_DoubleArr(TextAsset textAsset)
    {
        List<string[]> result = new List<string[]>();
        int amount = Get_FileRowAmount(textAsset);
        for (int i = 0; i < amount; i++)
        {
            result.Add(Get_Words(textAsset, i));
        }
        return result.ToArray();
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Offset();
        Debug.Log("ResouceManager : Offset Complete");
    }

    #endregion

    #region Offset

    public void Offset()
    {
        Offset_CSV_Event();
        Offset_CSV_Cutscene();
        Offset_CSV_Dialogue();
    }

    #endregion


    #region Event (CSV)

    [HideInInspector] private EventID[] eventID_Data;
    [HideInInspector] private EventElement[] eventElement_Data;

    // Offset
    private void Offset_CSV_Event()
    {
        // Event
        string path = "CSV/Event/";
        eventElement_Data = Get_EventElement(path, "EventElement_CSV");
        eventID_Data = Get_EventID(path, "EventID_CSV");
    }

    private EventElement[] Get_EventElement(string path, string fileName)
    {
        List<EventElement> result = new List<EventElement>();

        string[][] stringArr = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

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

    private EventID[] Get_EventID(string path, string fileName)
    {
        List<EventID> result = new List<EventID>();

        string[][] stringArr = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

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


    // ID에 맞는 EventID를 가져온 후, 그에 맞는 EventElement List를 가져옴
    public List<EventElement> Get_CorrectEventArr(int id)
    {
        List<EventElement> result = new List<EventElement>();

        int[] ids = eventID_Data[id].eventIds;

        for (int i = 0; i < ids.Length; i++)
        {
            EventElement eventElement = eventElement_Data[ids[i]];

            result.Add(eventElement);
        }

        return result;
    }

    #endregion

    #region Cutscene (CSV)

    // Value
    [HideInInspector] private CutsceneID[] cutsceneID_Data;
    [HideInInspector] private CutsceneElement[][] cutsceneElement_Data;

    // Offset
    private void Offset_CSV_Cutscene()
    {
        string path = "CSV/Cutscene/";

        cutsceneElement_Data = new CutsceneElement[GameManager.kindOfLanguage.Length][];
        for (int i = 0; i < cutsceneElement_Data.Length; i++)
            cutsceneElement_Data[i] = GetAsset_CutsceneElement(path, $"CutsceneElement_{GameManager.kindOfLanguage[i]}_CSV");

        cutsceneID_Data = GetAsset_CutsceneID(path, "CutsceneID_CSV");
    }

    private CutsceneElement[] GetAsset_CutsceneElement(string path, string fileName)
    {
        List<CutsceneElement> result = new List<CutsceneElement>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            string script = stringList[i][1];

            result.Add(new CutsceneElement(id, script));
        }

        return result.ToArray();
    }

    private CutsceneID[] GetAsset_CutsceneID(string path, string fileName)
    {
        List<CutsceneID> result = new List<CutsceneID>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

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


    // ID에 맞는 CutsceneID를 가져온 후, 그에 맞는 CutsceneElement List를 가져옴
    public List<CutsceneElement> Get_CorrectCutsceneElementList(int id)
    {
        List<CutsceneElement> result = new List<CutsceneElement>();

        int[] ids = cutsceneID_Data[id].cutscenes;

        for (int i = 0; i < ids.Length; i++)
        {
            CutsceneElement cutsceneElement = cutsceneElement_Data[GameManager.languageID][i];

            result.Add(cutsceneElement);
        }

        return result;
    }


    #endregion

    #region Dialogue (CSV)

    // Value
    [HideInInspector] private DialogueID[] dialogueID_Data;
    [HideInInspector] private DialogueElement[][] dialogueElement_Data;

    // Offset
    private void Offset_CSV_Dialogue()
    {
        string path = "CSV/Dialogue/";

        dialogueElement_Data = new DialogueElement[GameManager.kindOfLanguage.Length][];
        for (int i = 0; i < dialogueElement_Data.Length; i++)
            dialogueElement_Data[i] = GetAsset_DialogueElement(path, $"DialogueElement_{GameManager.kindOfLanguage[i]}_CSV");

        dialogueID_Data = GetAsset_DialogueID(path, "DialogueID_CSV");
    }

    private DialogueElement[] GetAsset_DialogueElement(string path, string fileName)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

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

    private DialogueID[] GetAsset_DialogueID(string path, string fileName)
    {
        List<DialogueID> result = new List<DialogueID>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

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


    // ID에 맞는 DialogueID를 가져온 후, 그에 맞는 DialogueElement List를 가져옴
    public List<DialogueElement> Get_CorrectDialogueElementList(int id)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        int[] IDs = dialogueID_Data[id].dialogus;

        for (int i = 0; i < IDs.Length; i++)
        {
            DialogueElement cutsceneElement = dialogueElement_Data[GameManager.languageID][IDs[i]];

            result.Add(cutsceneElement);
        }

        return result;
    }

    #endregion










    #region Language

    // 언어 변경을 위한 컴포넌트
    [HideInInspector] private HashSet<LanguageTxtController> allLanguageTxtControllers = new HashSet<LanguageTxtController>();
    [HideInInspector] public HashSet<PrisonController> allPrisons = new HashSet<PrisonController>();

    public void Add_LanguageTxt(LanguageTxtController langTxt)
    {
        allLanguageTxtControllers.Add(langTxt);
    }

    public void Clear_LanguageTxt()
    {
        allLanguageTxtControllers.Clear();
    }

    public void SetLanguageFont(int langID)
    {
        if (GameManager.languageID == langID) return;
        GameManager.languageID = langID;
        SaveDataManager.instance.jsonData.optionData.languageID = GameManager.languageID;
        SetLanguageFont();
    }


    private void SetLanguageFont()
    {
        // Change Font Asset
        foreach (LanguageTxtController ltc in allLanguageTxtControllers)
            ltc.Set_Font(GameManager.languageID);

        SetLanguage();
    }

    private void SetLanguage()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        // Change UI
        if (sceneName == "MainGame")
        {
            // UI
            MainGameUIManager.instance.SetLanguageTxt();

            // Ally
            AllyManager.instance.Set_Language();

            // Change PrisonInfo
            foreach (PrisonController prison in allPrisons)
                prison.Set_LanguageTxt();
        }
        else if (sceneName == "TitleLobby")
        {
            // UI
            TitleLobbyUIManager.instance.Set_LanguageTxt();
        }
    }


    #endregion

}