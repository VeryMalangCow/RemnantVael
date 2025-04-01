using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVManager : PersistentSingleton<CSVManager>
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> CSV")]

    [Space(10)]
    [Header("=== CSV")]

    [Space(5)]
    [Header("=== Map")]
    [SerializeField] private int KindOfMapAmount;
    [SerializeField] private List<int> EachKindOfMapAmount;

    #endregion

    #region - Hide

    // 이벤트
    [HideInInspector] private List<EventID> EventID_Data;
    [HideInInspector] private List<EventElement> EventElement_Data;

    // 다이얼로그
    [HideInInspector] private List<List<DialogueElement>> DialogueElement_DataList = new List<List<DialogueElement>>();
    [HideInInspector] private List<DialogueID> DialogueID_Data;

    // 워드
    [HideInInspector] private WordData StaticWord_Data;
    // 맵 이름
    [HideInInspector] private WordData MapName_Data;
    [HideInInspector] private WordData MapDesc_Data;

    // 스프라이트
    [HideInInspector] private List<Sprite> CharacterImgList_Data;

    [HideInInspector] public List<List<Sprite>> MapImgList_Data;
    [HideInInspector] public List<List<int>> MapMaterialIndexList_Data;

    #endregion

    #endregion

    #region Offset

    private void Offset_CSV()
    {
        // Event
        string eventPath = "CSV/Event/";
        EventElement_Data = Offset_EventElementList(eventPath, 
            "EventElement");
        EventID_Data = Offset_EventIDList(eventPath, 
            "EventID");

        // Dialogue
        string dialoguePath = "CSV/Dialogue/";
        for (int i = 0; i < GameManager.KindOfLanguage.Count; i++)
            DialogueElement_DataList.Add(Offset_DialougeEleventList(dialoguePath, 
                $"DialogueElement_{GameManager.KindOfLanguage[i]}"));
        
        DialogueID_Data = Offset_DialougeIDList(dialoguePath, 
            "DialogueID");

        // Word
        string wordPath = "CSV/Word/";
        StaticWord_Data = Offset_WordData(wordPath,
            "StaticWordCSV");

        MapName_Data = Offset_WordData(wordPath, 
            "MapNameCSV");
        MapDesc_Data = Offset_WordData(wordPath, 
            "MapDescCSV");
    }

    private void Offset_CharImg()
    {
        // Char
        CharacterImgList_Data = new List<Sprite>();
        CharacterImgList_Data.AddRange(
            Offset_ImgPath(
                "Sprite/Character/",
                "UI_CharacterImg_000"));
    }

    private void Offset_MapImg()
    {
        // Map
        MapImgList_Data = new List<List<Sprite>>();
        MapMaterialIndexList_Data = new List<List<int>>();

        for (int i = 0; i < KindOfMapAmount; i++)
        {
            MapImgList_Data.Add(new List<Sprite>());
            MapMaterialIndexList_Data.Add(new List<int>());

            for (int j = 0; j < EachKindOfMapAmount[i]; j++)
            {
                MapImgList_Data[i].AddRange(
                    Offset_ImgPath(
                        $"Sprite/Map/Map{DevTool.Get_LengthString(i, 2)}/",
                        $"Map{DevTool.Get_LengthString(i, 2)}_{DevTool.Get_LengthString(j, 3)}"));

                for (int k = 0; k < MapImgList_Data[i].Count; k++)
                    MapMaterialIndexList_Data[i].Add(j);
            }
        }
    }

    private void Offset()
    {
        Offset_CSV();
        Offset_CharImg();
        Offset_MapImg();
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Offset();
    }

    #endregion

    #region Getting for File

    [HideInInspector] private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    [HideInInspector] private static string WORD_SPLIT_RE = @",";

    // 파일 => 스트링
    private string Get_FileString(TextAsset _TextAsset)
    {
        return _TextAsset.text;
    }

    // 행 길이 구하기
    private int Get_FileRowAmount(TextAsset _TextAsset)
    {
        return Get_AllLine(_TextAsset).Length;
    }

    // 행 받아오기
    private string[] Get_AllLine(TextAsset _TextAsset)
    {
        return Regex.Split(Get_FileString(_TextAsset), LINE_SPLIT_RE);
    }

    // 열 하나를 받아오기 (인자: 행)
    private string Get_Line(TextAsset _TextAsset, int _Row)
    {
        return Get_AllLine(_TextAsset)[_Row];
    }

    // 열을 쉼표로 나누기
    private string[] Get_Words(TextAsset _TextAsset, int _Row)
    {
        return Regex.Split(Get_AllLine(_TextAsset)[_Row], WORD_SPLIT_RE);
    }

    // 파일을 이중 리스트(string)으로 변경
    private List<List<string>> Get_DoubleList(TextAsset _TextAsset)
    {
        List<List<string>> doubleList = new List<List<string>>();
        for (int i = 0; i < Get_FileRowAmount(_TextAsset); i++)
        {
            doubleList.Add(Get_Words(_TextAsset, i).ToList());
        }
        return doubleList;
    }

    #endregion

    #region To Event ID

    // 오프셋
    private List<EventID> Offset_EventIDList(string _Path, string _FileName)
    {
        List<EventID> result = new List<EventID>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "")
            { break; }

            int id = int.Parse(stringList[i][0]);
            List<int> idList = new List<int>();
            
            for (int j = 1; j < stringList[i].Count; j++)
            {
                if (stringList[i][j] == "" || stringList[i][j] == null)
                {  break; }
                idList.Add(int.Parse(stringList[i][j]));
            }

            result.Add(new EventID(id, idList));
        }

        return result;
    }

    // ID에 맞는 EventID
    private EventID Get_CorrectEventID(int _ID)
    {
        for (int i = 0; i < EventID_Data.Count; i++)
        {
            if (EventID_Data[i].ID == _ID)
            { return EventID_Data[i]; }
        }

        return null;
    }

    #endregion

    #region To Event Element

    // 오프셋
    private List<EventElement> Offset_EventElementList(string _Path, string _FileName)
    {
        List<EventElement> result = new List<EventElement>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "")
            { break; }

            EventElement eventElement = new EventElement();

            int id = int.Parse(stringList[i][0]);
            string name = stringList[i][1];

            // 정지
            if (name == "Stay")
            {
                float targetTime = float.Parse(stringList[i][2]);

                eventElement = new EventElement_Stay(id, targetTime);
            }
            else if (name == "Look")
            {
                string[] vectorString = stringList[i][2].Split("/");
                Vector2 vector = new Vector2(float.Parse(vectorString[0]), float.Parse(vectorString[1]));

                eventElement = new EventElement_Look(id, vector);
            }
            // 이동
            else if (name == "Move")
            {
                int targetId = int.Parse(stringList[i][2]);
                string targetType = stringList[i][3];
                string[] vectorString = stringList[i][4].Split("/");
                Vector2 vector = new Vector2(float.Parse(vectorString[0]), float.Parse(vectorString[1]));

                eventElement = new EventElement_Move(id, targetId, targetType, vector);
            }
            // 검은 화면 키기
            else if (name == "BlackScreenIn")
            {
                float targetTime = float.Parse(stringList[i][2]);

                eventElement = new EventElement_BlackScreenIn(id, targetTime);
            }
            // 검은 화면 끄기
            else if (name == "BlackScreenOut")
            {
                float targetTime = float.Parse(stringList[i][2]);

                eventElement = new EventElement_BlackScreenOut(id, targetTime);
            }
            // 다이얼로그
            else if (name == "Dialogue")
            {
                int targetId = int.Parse(stringList[i][2]);

                eventElement = new EventElement_Dialogue(id, targetId);
            }

            result.Add(eventElement);
        }

        return result;
    }

    // ID에 맞는 EventElement
    public EventElement Get_CorrectEvent(int _ID)
    {
        for (int i = 0; i < EventElement_Data.Count; i++)
        {
            if (EventElement_Data[i].ID == _ID)
            { return EventElement_Data[i]; }
        }
        return null;
    }

    // ID에 맞는 EventID를 가져온 후, 그에 맞는 EventElement List를 가져옴
    public List<EventElement> Get_CorrectEventList(int _ID)
    {
        List<EventElement> result = new List<EventElement>();

        List<int> IDs = Get_CorrectEventID(_ID).EventIDs;
        if (IDs == null)
        { return null; }

        for (int i = 0; i < IDs.Count; i++)
        {
            EventElement eventElement = Get_CorrectEvent(IDs[i]);
            if (eventElement == null)
            { return null; }

            result.Add(eventElement);
        }

        return result;
    }


    #endregion

    #region To DialougeID

    // 오프셋
    private List<DialogueID> Offset_DialougeIDList(string _Path, string _FileName)
    {
        List<DialogueID> result = new List<DialogueID>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "")
            { break; }

            int id = int.Parse(stringList[i][0]);

            List<DialogueElement> dialogueList = new List<DialogueElement>();
            for (int j = 1; j < stringList[i].Count; j++)
            {
                if (stringList[i][j] == "" || stringList[i][j] == null)
                { break; }
                int elementId = int.Parse(stringList[i][j]);
                dialogueList.Add(Get_CorrectDialogueElement(elementId));
            }

            result.Add(new DialogueID(id, dialogueList));
        }

        return result;
    }

    // ID에 맞는 다이얼로그 리스트를 구함
    public DialogueID Get_CorrectDialogueID(int _ID)
    {
        for (int i = 0; i < DialogueID_Data.Count; i++)
        {
            if (DialogueID_Data[i].ID == _ID)
            { return DialogueID_Data[i]; }
        }

        return null;
    }

    #endregion

    #region To Dialogue Element

    // 오프셋

    private List<DialogueElement> Offset_DialougeEleventList(string _Path, string _FileName)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "")
            { break; }

            int id = int.Parse(stringList[i][0]);
            string name = stringList[i][1];
            string script = stringList[i][2];
            int imgId = int.Parse(stringList[i][3]);
            bool isLeft = bool.Parse(stringList[i][4]);

            result.Add(new DialogueElement(id, name, script, imgId, isLeft));
        }

        return result;
    }


    // ID에 맞는 다이얼로그 1개를 구함
    private DialogueElement Get_CorrectDialogueElement(int _ID)
    {
        for (int i = 0; i < DialogueElement_DataList[GameManager.LanguageID].Count; i++)
            if (DialogueElement_DataList[GameManager.LanguageID][i].ID == _ID)
                return DialogueElement_DataList[GameManager.LanguageID][i]; 
        

        return null;
    }

    #endregion


    #region To MapName

    #region Offset

    private WordData Offset_WordData(string _Path, string _FileName)
    {
        List<WordElementData> element = new List<WordElementData>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "") break; 

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][0]);
            
            for (int j = 0; j < GameManager.KindOfLanguage.Count; j++)
            {
                nameList.Add(stringList[i][j + 1]);
            }
            element.Add(new WordElementData(id, nameList));
        }

        return new WordData(element);
    }

    #endregion

    #region Get 

    // Static
    public string Get_StaticWord(int _ID)
    {
        return StaticWord_Data.Get_Word(_ID);
    }

    // Map
    public string Get_MapName(int _ID)
    {
        return MapName_Data.Get_Word(_ID);
    }

    public string Get_MapDesc(int _ID)
    {
        return MapDesc_Data.Get_Word(_ID);
    }

    #endregion

    #endregion

    #region To SpriteList

    private List<Sprite> Offset_ImgPath(string _Path, string _FileName)
    {
        List<Sprite> result = new List<Sprite>();
        return Resources.LoadAll<Sprite>(_Path + _FileName).ToList();
    }

    public Sprite Get_CorrectCharacterImg(int _ID)
    {
        return CharacterImgList_Data[_ID];
    }

    #endregion
}