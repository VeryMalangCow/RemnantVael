using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVManager : Singleton<CSVManager>
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

    [SerializeField] private Texture2D CharacterImg_000;

    // === Data
    [HideInInspector] private List<EventID> EventID_Data;
    [HideInInspector] private List<EventElement> EventElement_Data;

    [HideInInspector] private List<DialogueElement> DialogueElement_Data;
    [HideInInspector] private List<DialogueID> DialogueID_Data;

    [HideInInspector] private List<Sprite> CharacterImgList_Data;

    #endregion

    #region Offset

    private void Offset()
    {
        EventElement_Data = GetOffset_EventElementList(EventElement_CSV);
        EventID_Data = GetOffset_EventIDList(EventID_CSV);

        DialogueElement_Data = GetOffset_DialougeEleventList(DialogueElement_CSV); // 다이얼로그 ID보다 먼저 와야함
        DialogueID_Data = GetOffset_DialougeIDList(DialougeID_CSV);

        CharacterImgList_Data = new List<Sprite>();
        CharacterImgList_Data.AddRange(GetOffset_CharacterImgList(CharacterImg_000));
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        if (CSVManager.Instance == this)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        Offset();
    }

    #endregion

    #region Get

    [HideInInspector] private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    [HideInInspector] private static string WORD_SPLIT_RE = @",";

    // 파일 => 스트링
    private string GetFileString(TextAsset _TextAsset)
    {
        return _TextAsset.text;
    }

    // 행 길이 구하기
    private int GetFileRowAmount(TextAsset _TextAsset)
    {
        return GetAllLine(_TextAsset).Length;
    }

    // 행 받아오기
    private string[] GetAllLine(TextAsset _TextAsset)
    {
        return Regex.Split(GetFileString(_TextAsset), LINE_SPLIT_RE);
    }

    // 열 하나를 받아오기 (인자: 행)
    private string GetLine(TextAsset _TextAsset, int _Row)
    {
        return GetAllLine(_TextAsset)[_Row];
    }

    // 열을 쉼표로 나누기
    private string[] GetWords_FromLine(TextAsset _TextAsset, int _Row)
    {
        return Regex.Split(GetAllLine(_TextAsset)[_Row], WORD_SPLIT_RE);
    }

    // 파일을 이중 리스트(string)으로 변경
    private List<List<string>> GetDoubleList(TextAsset _TextAsset)
    {
        List<List<string>> doubleList = new List<List<string>>();
        for (int i = 0; i < GetFileRowAmount(_TextAsset); i++)
        {
            doubleList.Add(GetWords_FromLine(_TextAsset, i).ToList());
        }
        return doubleList;
    }

    #endregion

    #region To Event ID

    // 오프셋
    private List<EventID> GetOffset_EventIDList(TextAsset _TextAsset)
    {
        List<EventID> result = new List<EventID>();

        List<List<string>> stringList = GetDoubleList(_TextAsset);

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
    private EventID GetCorrectEventID(int _ID)
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
    private List<EventElement> GetOffset_EventElementList(TextAsset _TextAsset)
    {
        List<EventElement> result = new List<EventElement>();

        List<List<string>> stringList = GetDoubleList(_TextAsset);

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
    public EventElement GetCorrectEvent(int _ID)
    {
        for (int i = 0; i < EventElement_Data.Count; i++)
        {
            if (EventElement_Data[i].ID == _ID)
            { return EventElement_Data[i]; }
        }
        return null;
    }

    // ID에 맞는 EventID를 가져온 후, 그에 맞는 EventElement List를 가져옴
    public List<EventElement> GetCorrectEventList(int _ID)
    {
        List<EventElement> result = new List<EventElement>();

        List<int> IDs = GetCorrectEventID(_ID).EventIDs;
        if (IDs == null)
        { return null; }

        for (int i = 0; i < IDs.Count; i++)
        {
            EventElement eventElement = GetCorrectEvent(IDs[i]);
            if (eventElement == null)
            { return null; }

            result.Add(eventElement);
        }

        return result;
    }


    #endregion

    #region To DialougeID

    // 오프셋
    private List<DialogueID> GetOffset_DialougeIDList(TextAsset _TextAsset)
    {
        List<DialogueID> result = new List<DialogueID>();

        List<List<string>> stringList = GetDoubleList(_TextAsset);

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
                dialogueList.Add(GetCorrectDialogueElement(elementId));
            }

            result.Add(new DialogueID(id, dialogueList));
        }

        return result;
    }

    // ID에 맞는 다이얼로그 리스트를 구함
    public DialogueID GetCorrectDialogueID(int _ID)
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

    private List<DialogueElement> GetOffset_DialougeEleventList(TextAsset _TextAsset)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        List<List<string>> stringList = GetDoubleList(_TextAsset);

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
    private DialogueElement GetCorrectDialogueElement(int _ID)
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

    private List<Sprite> GetOffset_CharacterImgList(Texture2D _Texture2D)
    {
        List<Sprite> result = new List<Sprite>();
        if (_Texture2D != null)
        {
            Debug.Log("Reso_Texture2D/" + _Texture2D.name);
            return Resources.LoadAll<Sprite>("Reso_Texture2D/" + _Texture2D.name).ToList();
        }
        else
        {
            return result;
        }
    }

    public Sprite GetCorrectCharacterImg(int _ID)
    {
        return CharacterImgList_Data[_ID];
    }

    #endregion
}