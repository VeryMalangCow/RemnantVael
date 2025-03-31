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

    #region - Event

    [Space(5)]
    [Header("-- Event")]
    [SerializeField] private TextAsset EventID_CSV;
    [SerializeField] private TextAsset EventElement_CSV;

    // 이벤트
    [HideInInspector] private List<EventID> EventID_Data;
    [HideInInspector] private List<EventElement> EventElement_Data;

    #endregion

    #region - Dialogue

    [Space(5)]
    [Header("-- Dialogue")]
    [SerializeField] private List<TextAsset> DialogueElement_CSVList;
    [SerializeField] private TextAsset DialougeID_CSV;


    // 다이얼로그
    [HideInInspector] private List<List<DialogueElement>> DialogueElement_DataList;
    [HideInInspector] private List<DialogueID> DialogueID_Data;

    #endregion

    #region - Word

    [Space(5)]
    [Header("-- Word")]
    [SerializeField] private TextAsset MapName_CSV;
    [SerializeField] private TextAsset MapDesc_CSV;

    // Word
    // 맵 이름
    [HideInInspector] private WordData MapName_Data;
    [HideInInspector] private WordData MapDesc_Data;

    #endregion

    #region - Sprite

    [Space(10)]
    [Header("=== Sprite")]

    [Space(5)]
    [Header("-- Character")]
    [SerializeField] private Texture2D CharacterImg_000;

    [Space(5)]
    [Header("-- Map")]
    [Space(5)]
    [SerializeField] private List<Texture2D> Map00;

    [SerializeField] private List<Texture2D> Map01;


    // Data
    [HideInInspector] private List<Sprite> CharacterImgList_Data;

    [HideInInspector] public List<List<Sprite>> MapImgList_Data;
    [HideInInspector] public List<List<int>> MapMaterialIndexList_Data;

    // Path
    [HideInInspector] private string SpritePath = "Sprite/";

    [HideInInspector] private string CharacterImg_Path = "Character/";

    [HideInInspector] private string Map_Path = "Map/";
    [HideInInspector] private string Map00_Path = "Map00/";
    [HideInInspector] private string Map01_Path = "Map01/";

    #endregion

    #endregion

    #region Offset

    private void Offset_CSV()
    {
        EventElement_Data = Offset_EventElementList(EventElement_CSV);
        EventID_Data = Offset_EventIDList(EventID_CSV);

        DialogueElement_DataList = new List<List<DialogueElement>>();
        for (int i = 0; i < DialogueElement_CSVList.Count; i++)
            DialogueElement_DataList.Add(Offset_DialougeEleventList(DialogueElement_CSVList[i]));
        
        DialogueID_Data = Offset_DialougeIDList(DialougeID_CSV);

        MapName_Data = Offset_WordData(MapName_CSV);
        MapDesc_Data = Offset_WordData(MapDesc_CSV);
    }

    private void Offset_CharImg()
    {
        // Char
        CharacterImgList_Data = new List<Sprite>();
        CharacterImgList_Data.AddRange(
            Offset_ImgPath(CharacterImg_000, SpritePath + CharacterImg_Path));

    }

    private void Offset_MapImg()
    {
        // Map
        List<List<Texture2D>> spriteDoubleList = new List<List<Texture2D>>
        { Map00, Map01 };

        List<string> spriteMap_Path = new List<string>()
        { Map00_Path, Map01_Path };

        MapImgList_Data = new List<List<Sprite>>();
        MapMaterialIndexList_Data = new List<List<int>>();

        for (int i = 0; i < spriteDoubleList.Count; i++)
        {
            MapImgList_Data.Add(new List<Sprite>());
            MapMaterialIndexList_Data.Add(new List<int>());

            for (int j = 0; j < spriteDoubleList[i].Count; j++)
            {
                MapImgList_Data[i].AddRange(
                    Offset_ImgPath(spriteDoubleList[i][j], SpritePath + Map_Path + spriteMap_Path[i]));

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
        for (int i = 0; i < DialogueElement_DataList[GameManager.LanguageID].Count; i++)
            if (DialogueElement_DataList[GameManager.LanguageID][i].ID == _ID)
                return DialogueElement_DataList[GameManager.LanguageID][i]; 
        

        return null;
    }

    #endregion


    #region To MapName

    #region Offset

    private WordData Offset_WordData(TextAsset _TextAsset)
    {
        List<WordElementData> element = new List<WordElementData>();

        List<List<string>> stringList = Get_DoubleList(_TextAsset);

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "") break; 

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][0]);
            
            for (int j = 0; j < GameManager.KindOfLanguageAmount; j++)
            {
                nameList.Add(stringList[i][j + 1]);
            }
            element.Add(new WordElementData(id, nameList));
        }

        return new WordData(element);
    }

    #endregion

    #region Get 

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