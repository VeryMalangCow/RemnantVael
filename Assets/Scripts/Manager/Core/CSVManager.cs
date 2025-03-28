using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVManager : PersistentSingleton<CSVManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> SCV")]

    [Space(10)]
    [Header("=== CSV")]
    [SerializeField] private TextAsset EventID_CSV;
    [SerializeField] private TextAsset EventElement_CSV;

    [SerializeField] private TextAsset DialogueElement_CSV;
    [SerializeField] private TextAsset DialougeID_CSV;

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] private string SpritePath = "Sprite/";

    [Space(5)]
    [Header("-- Character")]
    [SerializeField] private string CharacterImg_Path = "Character/";
    [SerializeField] private Texture2D CharacterImg_000;

    [Space(5)]
    [Header("-- Map")]
    [SerializeField] private string Map_Path = "Map/";
    [Space(5)]
    [SerializeField] private string Map00_Path = "Map00/";
    [SerializeField] private List<Texture2D> Map00;

    [SerializeField] private string Map01_Path = "Map01/";
    [SerializeField] private List<Texture2D> Map01;

    // === Data
    [HideInInspector] private List<EventID> EventID_Data;
    [HideInInspector] private List<EventElement> EventElement_Data;

    [HideInInspector] private List<DialogueElement> DialogueElement_Data;
    [HideInInspector] private List<DialogueID> DialogueID_Data;

    [HideInInspector] private List<Sprite> CharacterImgList_Data;

    [HideInInspector] public List<List<Sprite>> MapImgList_Data;

    #endregion

    #region Offset

    private void Offset()
    {
        EventElement_Data = Offset_EventElementList(EventElement_CSV);
        EventID_Data = Offset_EventIDList(EventID_CSV);

        DialogueElement_Data = Offset_DialougeEleventList(DialogueElement_CSV); // 다이얼로그 ID보다 먼저 와야함
        DialogueID_Data = Offset_DialougeIDList(DialougeID_CSV);

        CharacterImgList_Data = new List<Sprite>();
        CharacterImgList_Data.AddRange(
            Offset_ImgPath(CharacterImg_000, SpritePath + CharacterImg_Path));

        List<List<Texture2D>> spriteDoubleList = new List<List<Texture2D>>
        { Map00, Map01 };
        List<string> spriteMap_Path = new List<string>()
        { Map00_Path, Map01_Path };

        MapImgList_Data = new List<List<Sprite>>();
        for (int i = 0; i < spriteDoubleList.Count; i++)
        {
            MapImgList_Data.Add(new List<Sprite>());
            for (int j = 0; j < spriteDoubleList[i].Count; j++)
            {
                MapImgList_Data[i].AddRange(
                    Offset_ImgPath(spriteDoubleList[i][j], SpritePath + Map_Path + spriteMap_Path[i]));
            }
        }
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
    private List<EventID> Offset_EventIDList(TextAsset _TextAsset)
    {
        List<EventID> result = new List<EventID>();

        List<List<string>> stringList = Get_DoubleList(_TextAsset);

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
    private List<EventElement> Offset_EventElementList(TextAsset _TextAsset)
    {
        List<EventElement> result = new List<EventElement>();

        List<List<string>> stringList = Get_DoubleList(_TextAsset);

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
    private List<DialogueID> Offset_DialougeIDList(TextAsset _TextAsset)
    {
        List<DialogueID> result = new List<DialogueID>();

        List<List<string>> stringList = Get_DoubleList(_TextAsset);

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

    private List<DialogueElement> Offset_DialougeEleventList(TextAsset _TextAsset)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        List<List<string>> stringList = Get_DoubleList(_TextAsset);

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
        for (int i = 0; i < DialogueElement_Data.Count; i++)
        {
            if (DialogueElement_Data[i].ID == _ID)
            { return DialogueElement_Data[i]; }
        }

        return null;
    }

    #endregion

    #region To SpriteList

    private List<Sprite> Offset_ImgPath(Texture2D _Texture2D, string _Path)
    {
        List<Sprite> result = new List<Sprite>();
        if (_Texture2D != null)
            return Resources.LoadAll<Sprite>(_Path + _Texture2D.name).ToList();
        else
            return result;
        
    }

    public Sprite Get_CorrectCharacterImg(int _ID)
    {
        return CharacterImgList_Data[_ID];
    }

    #endregion
}