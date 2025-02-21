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

    // === Data
    [HideInInspector] private List<EventID> EventID_Data;
    [HideInInspector] private List<EventElement> EventElement_Data;

    #endregion

    #region Offset

    private void Offset()
    {
        EventElement_Data = GetOffset_EventElementList(EventElement_CSV);
        EventID_Data = GetOffset_EventIDList(EventID_CSV);
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        if (SaveDataManager.Instance == this)
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

    #region To EventID

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

    #region To EventElement

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
                string[] vectorString = stringList[i][2].Split("/");
                Vector2 vector = new Vector2(float.Parse(vectorString[0]), float.Parse(vectorString[1]));

                eventElement = new EventElement_Move(id, vector);
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
}