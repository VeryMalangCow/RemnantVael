using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ResourceManager : PersistentSingleton<ResourceManager>
{
    #region Value

    #region - Amount Set

    // 맵 종류
    [HideInInspector] private int KindOfMapAmount = 2;
    // 각 맵에 사용할 스프라이트의 양
    [HideInInspector] private int EachKindOfMapAmount = 2;
    // 카드 아이콘 양
    [HideInInspector] private int STIconAmount = 1;
    [HideInInspector] private int UTIconAmount = 1;
    [HideInInspector] private int NTIconAmount = 1;

    #endregion

    #region - Hide

    // 이벤트
    [HideInInspector] private List<EventID> EventID_Data;
    [HideInInspector] private List<EventElement> EventElement_Data;

    // 다이얼로그
    [HideInInspector] private List<List<DialogueElement>> DialogueElement_DataList = new List<List<DialogueElement>>();
    [HideInInspector] private List<DialogueID> DialogueID_Data;

    // 모듈
    [HideInInspector] private List<ModuleBaseData> ModuleBaseList_Data;

    // 동료 카드
    [HideInInspector] private List<AllyCardBaseData> StrikeTeam_AllyCard_Data;
    [HideInInspector] private List<AllyCardBaseData> UplinkTeam_AllyCard_Data;
    [HideInInspector] private List<AllyCardBaseData> NeoTeam_AllyCard_Data;

    // 워드
    // 스태틱
    [HideInInspector] private WordData StaticWord_Data;
    // 맵 이름
    [HideInInspector] private WordData MapName_Data;
    [HideInInspector] private WordData MapDesc_Data;
    // 스킬
    [HideInInspector] private List<WordData> SkillName_Data;
    // 모듈
    [HideInInspector] private WordData ModuleItemName_Data;
    [HideInInspector] private WordData MainChipName_Data;
    // 동료 카드 이름
    [HideInInspector] private WordData StrikeTeam_AllyCardName_Data;
    [HideInInspector] private WordData UplinkTeam_AllyCardName_Data;
    [HideInInspector] private WordData NeoTeam_AllyCardName_Data;

    // 문장
    // 스태틱
    [HideInInspector] private WordData StaticDesc_Data;
    // 스킬
    [HideInInspector] private List<WordData> SkillDesc_Data;
    // 모듈
    [HideInInspector] private WordData ModuleItemDesc_Data;
    [HideInInspector] private WordData ModuleItemEquipDesc_Data;
    [HideInInspector] private List<WordData> MainChipDescList_Data;
    // 동료 카드 설명
    [HideInInspector] private WordData StrikeTeam_AllyCardDesc_Data;
    [HideInInspector] private WordData UplinkTeam_AllyCardDesc_Data;
    [HideInInspector] private WordData NeoTeam_AllyCardDesc_Data;


    // 스프라이트
    // 캐릭터
    [HideInInspector] private List<Sprite> CharacterImgList_Data;

    // 맵
    [HideInInspector] public List<Sprite> MapLobbyImg_Data;
    [HideInInspector] public List<int> MapLobbyMaterialIndexList_Data;

    [HideInInspector] public List<List<Sprite>> MapImgList_Data;
    [HideInInspector] public List<List<int>> MapMaterialIndexList_Data;

    // 모듈
    [HideInInspector] private List<Sprite> ModuleItemImgList_Data;
    [HideInInspector] private List<Sprite> ModuleSynhronyImgList_Data;

    // 카드 아이콘
    [HideInInspector] private List<List<Sprite>> AllyCardIcon_Data;

    // 동료
    [HideInInspector] private List<Sprite> AllySprite_Data;
    private static readonly string[] directionOrder = new string[] { "UL", "U", "UR", "R", "DR", "D", "DL", "L" };

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

        // ModuleBase
        string modulePath = "CSV/Module/";
        ModuleBaseList_Data = Offset_ModuleBase(modulePath, 
            "ModuleCSV");

        // Ally Card
        string allyCardPath = "CSV/AllyCard/";
        StrikeTeam_AllyCard_Data = Offset_AllyCard(allyCardPath,
            "AllyCard_StrikeTeam_CSV");
        UplinkTeam_AllyCard_Data = Offset_AllyCard(allyCardPath,
            "AllyCard_UplinkTeam_CSV"); 
        NeoTeam_AllyCard_Data = Offset_AllyCard(allyCardPath,
            "AllyCard_NeoTeam_CSV");
        // Word
        // Static
        string wordPath = "CSV/Word/";
        StaticWord_Data = Offset_WordData(wordPath,
            "StaticWordCSV");
        // Map
        MapName_Data = Offset_WordData(wordPath, 
            "MapNameCSV");
        // Skill
        SkillName_Data = Offset_WordDataList_ForParentID(wordPath,
            "SkillNameCSV", PlayerManager.KindOfPlayerAmount);
        // Module
        ModuleItemName_Data = Offset_WordData(wordPath,
            "ModuleNameCSV");
        // MainChip
        MainChipName_Data = Offset_WordData(wordPath,
            "MainChipNameCSV");
        // Ally Card
        StrikeTeam_AllyCardName_Data = Offset_WordData(wordPath,
            "AllyCard_StrikeTeam_NameCSV");
        UplinkTeam_AllyCardName_Data = Offset_WordData(wordPath,
            "AllyCard_UplinkTeam_NameCSV");
        NeoTeam_AllyCardName_Data = Offset_WordData(wordPath,
            "AllyCard_NeoTeam_NameCSV");

        // Desc
        // Static
        string descPath = "CSV/Desc/";
        StaticDesc_Data = Offset_WordData(descPath,
            "StaticDescCSV");
        // Map
        MapDesc_Data = Offset_WordData(descPath,
            "MapDescCSV");
        // Skill
        SkillDesc_Data = Offset_WordDataList_ForParentID(descPath,
            "SkillDescCSV", PlayerManager.KindOfPlayerAmount);
        // Module
        ModuleItemDesc_Data = Offset_WordData(descPath,
            "ModuleDescCSV");
        ModuleItemEquipDesc_Data = Offset_WordData(descPath,
            "ModuleEquipDescCSV");
        MainChipDescList_Data = Offset_WordDataList_ForParentID(descPath,
            "MainChipDescCSV", MainChipName_Data.AllWordData.Count);
        // Ally Card
        StrikeTeam_AllyCardDesc_Data = Offset_WordData(descPath,
            "AllyCard_StrikeTeam_DescCSV");
        UplinkTeam_AllyCardDesc_Data = Offset_WordData(descPath,
            "AllyCard_UplinkTeam_DescCSV");
        NeoTeam_AllyCardDesc_Data = Offset_WordData(descPath,
            "AllyCard_NeoTeam_DescCSV");
    }

    private void Offset_CharImg()
    {
        // Char
        CharacterImgList_Data = new List<Sprite>();
        CharacterImgList_Data.AddRange(
            Offset_ImgPath(
                "Sprite/Character/",
                "CharacterImg_000"));
    }

    private void Offset_MapImg()
    {
        // Lobby Map
        MapLobbyImg_Data = new List<Sprite>();
        MapLobbyMaterialIndexList_Data = new List<int>();

        for (int j = 0; j < EachKindOfMapAmount; j++)
        {
            MapLobbyImg_Data.AddRange(
            Offset_ImgPath(
                $"Sprite/Map/MapLobby/",
                $"MapLobby_{DevTool.Get_LengthString(j, 3)}"));

            for (int k = 0; k < MapLobbyImg_Data.Count; k++)
                MapLobbyMaterialIndexList_Data.Add(j);
        }

        // Map
        MapImgList_Data = new List<List<Sprite>>();
        MapMaterialIndexList_Data = new List<List<int>>();

        for (int i = 0; i < KindOfMapAmount; i++)
        {
            MapImgList_Data.Add(new List<Sprite>());
            MapMaterialIndexList_Data.Add(new List<int>());

            for (int j = 0; j < EachKindOfMapAmount; j++)
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

    private void Offset_ModuleItemImg()
    {
        ModuleItemImgList_Data = new List<Sprite>();
        ModuleItemImgList_Data.AddRange(
            Offset_ImgPath(
                "Sprite/UI/MU/",
                "MUItemUI_000"));

        ModuleSynhronyImgList_Data = new List<Sprite>();
        ModuleSynhronyImgList_Data.AddRange(
            Offset_ImgPath(
                "Sprite/UI/MU/",
                "MUSynchronyUI_000"));
    }

    private void Offset_AllyCardIcon()
    {
        AllyCardIcon_Data = new List<List<Sprite>>
        {
            Get_AllyCardIcon(STIconAmount, "ST"),
            Get_AllyCardIcon(UTIconAmount, "UT"),
            Get_AllyCardIcon(NTIconAmount, "NT")
        };

        List<Sprite> Get_AllyCardIcon(int _SpriteAmount, string _TypeName)
        {
            List<Sprite> result = new List<Sprite>();
            for (int i = 0; i < _SpriteAmount; i++)
            {
                result.AddRange(
                    Offset_ImgPath(
                        $"Sprite/UI/Ally/",
                        $"AllyCardIcon_{_TypeName}_{DevTool.Get_LengthString(i, 3)}"));
            }
            return result;
        }
    }

    private void Offset_AllySprite()
    {
        AllySprite_Data = new List<Sprite>();
        AllySprite_Data.AddRange(
            Offset_ImgPath(
                "Sprite/Ally/",
                "Ally_001_00"));
    }

    private void Offset()
    {
        Offset_CSV();
        Offset_CharImg();
        Offset_MapImg();
        Offset_ModuleItemImg();
        Offset_AllyCardIcon();
        Offset_AllySprite();
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

    #region To Module Base

    private List<ModuleBaseData> Offset_ModuleBase(string _Path, string _FileName)
    {
        List<ModuleBaseData> result = new List<ModuleBaseData>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "")
            { break; }

            int id = int.Parse(stringList[i][0]);
            int r1mainChip = int.Parse(stringList[i][1]);
            int r3mainChip = int.Parse(stringList[i][2]);
            int r5mainChip = int.Parse(stringList[i][3]);

            result.Add(new ModuleBaseData(id, new List<int> { r1mainChip, r3mainChip, r5mainChip }));
        }

        return result;
    }

    public int Get_AllModuleItemAmount()
    {
        return ModuleBaseList_Data.Count;
    }

    public int Get_AllModuleSynchronyAmount()
    {
        return MainChipName_Data.AllWordData.Count;
    }

    #endregion

    #region To AllyCard

    private List<AllyCardBaseData> Offset_AllyCard(string _Path, string _FileName)
    {
        List<AllyCardBaseData> result = new List<AllyCardBaseData>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "")
            { break; }

            int id = int.Parse(stringList[i][0]);
            int rank = int.Parse(stringList[i][1]);
            int essentialID = int.Parse(stringList[i][2]);

            result.Add(new AllyCardBaseData(id, rank, essentialID));
        }

        return result;
    }

    public List<Sprite> Get_AllyCardSpriteIcon(int _Type)
    {
        return AllyCardIcon_Data[_Type];
    }

    #endregion

    #region To Word | Desc

    #region Offset

    private List<WordData> Offset_WordDataList_ForParentID(string _Path, string _FileName, int _Amount)
    {
        List<WordData> result = new List<WordData>();
        for (int i = 0; i < _Amount; i++)
            result.Add(Offset_WordData_ForParentID(_Path, _FileName, i));

        return result;
    }

    private WordData Offset_WordData_ForParentID(string _Path, string _FileName, int _TargetParentID)
    {
        List<WordElementData> element = new List<WordElementData>();

        List<List<string>> stringList = Get_DoubleList(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Count; i++)
        {
            if (stringList[i][0] == "") break;

            if (int.Parse(stringList[i][0]) != _TargetParentID) continue;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][1]);

            for (int j = 1; j < GameManager.KindOfLanguage.Count + 1; j++)
                nameList.Add(stringList[i][j + 1]);

            element.Add(new WordElementData(id, nameList));
        }

        return new WordData(element);
    }

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
                nameList.Add(stringList[i][j + 1]);
            
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

    public string Get_StaticDesc(int _ID)
    {
        return StaticDesc_Data.Get_Word(_ID);
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

    // Skill
    public string Get_SkillName(int _PlayerID, int _ID)
    {
        return SkillName_Data[_PlayerID].Get_Word(_ID);
    }
    
    public string Get_SkillDesc(int _PlayerID, int _ID)
    {
        return SkillDesc_Data[_PlayerID].Get_Word(_ID);
    }

    // Module
    public ItemData Get_ItemData(int _ID)
    {
        ItemData result = new ItemData(_ID);

        Set_DataLanguage(result, _ID);

        result.ItemIcon = ModuleItemImgList_Data[_ID];

        result.R1_MainChipID = ModuleBaseList_Data[_ID].ModuleMainChip[0];
        result.R3_MainChipID = ModuleBaseList_Data[_ID].ModuleMainChip[1];
        result.R5_MainChipID = ModuleBaseList_Data[_ID].ModuleMainChip[2];

        return result;
    }

    // MainChip
    public MainChipData Get_MainChipData(int _ID)
    {
        MainChipData result = new MainChipData();

        result.ID = _ID;

        Set_DataLanguage(result, _ID);

        result.ThisIcon = ModuleSynhronyImgList_Data[_ID];

        return result;
    }

    // AllyCard
    public List<AllyCardData> Get_StrikeTeam_AllAllyCardData()
    {
        List<AllyCardData> result = new List<AllyCardData>();
        for (int i = 0; i < StrikeTeam_AllyCard_Data.Count; i++)
            result.Add(new AllyCardData(StrikeTeam_AllyCard_Data[i], StrikeTeam_AllyCardName_Data.Get_Word(i), StrikeTeam_AllyCardDesc_Data.Get_Word(i)));

        return result;
    }
    public List<AllyCardData> Get_UplinkTeam_AllAllyCardData()
    {
        List<AllyCardData> result = new List<AllyCardData>();
        for (int i = 0; i < UplinkTeam_AllyCard_Data.Count; i++)
            result.Add(new AllyCardData(UplinkTeam_AllyCard_Data[i], UplinkTeam_AllyCardName_Data.Get_Word(i), UplinkTeam_AllyCardDesc_Data.Get_Word(i)));

        return result;
    }
    public List<AllyCardData> Get_NeoTeam_AllAllyCardData()
    {
        List<AllyCardData> result = new List<AllyCardData>();
        for (int i = 0; i < NeoTeam_AllyCard_Data.Count; i++)
            result.Add(new AllyCardData(NeoTeam_AllyCard_Data[i], NeoTeam_AllyCardName_Data.Get_Word(i), NeoTeam_AllyCardDesc_Data.Get_Word(i)));

        return result;
    }


    #endregion

    #region Set

    public ItemData Set_DataLanguage(ItemData _ItemData, int _ID)
    {
        _ItemData.Name = ModuleItemName_Data.Get_Word(_ID);
        _ItemData.Description = ModuleItemDesc_Data.Get_Word(_ID);
        _ItemData.EquipDescription = ModuleItemEquipDesc_Data.Get_Word(_ID);

        return _ItemData;
    }

    public MainChipData Set_DataLanguage(MainChipData _MainChipData, int _ID)
    {
        _MainChipData.Name = MainChipName_Data.Get_Word(_ID);
        _MainChipData.AmalgamationDescList = new List<string>
        {
            MainChipDescList_Data[_ID].Get_Word(0),
            MainChipDescList_Data[_ID].Get_Word(1),
            MainChipDescList_Data[_ID].Get_Word(2)
        };

        return _MainChipData;
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


    public List<Sprite> Get_LobbyStageMapSpriteList()
    {
        return MapLobbyImg_Data;
    }

    public List<Sprite> Get_StageMapSpriteList(int _ID)
    {
        return MapImgList_Data[_ID];
    }

    public List<int> Get_LobbyStageMapMaterialList()
    {
        return MapLobbyMaterialIndexList_Data;
    }

    public List<int> Get_StageMapMaterialList(int _ID)
    {
        return MapMaterialIndexList_Data[_ID];
    }

    #endregion

    #region To Ally Sprite

    public List<Sprite> Get_AllySprite(string _Name, string _Type)
    {
        List<Sprite> result = new List<Sprite>();

        int stringLength = 7 + _Name.Length + _Type.Length;

        // 맞는 아트 리소스 가져오기
        for (int i = 0; i < AllySprite_Data.Count; i++)
        {
            if (AllySprite_Data[i].name.Length >= stringLength &&
                AllySprite_Data[i].name.Substring(0, stringLength) == $"Ally_{_Name}_{_Type}_")
            {
                result.Add(AllySprite_Data[i]);
            }
        }

        // 방향에 따라 알맞는 순서 맞추기
        return Get_SortSpritesByDirection(result);
    }

    public static List<Sprite> Get_SortSpritesByDirection(List<Sprite> sprites)
    {
        return sprites
            .OrderBy(sprite => Get_DirectionIndex(sprite.name))
            .ToList();
    }

    private static int Get_DirectionIndex(string spriteName)
    {
        // 예: "Sprite_Head_UL" → "UL" 추출
        string[] parts = spriteName.Split('_');
        string dir = parts[parts.Length - 1];

        int index = System.Array.IndexOf(directionOrder, dir);
        return index >= 0 ? index : int.MaxValue;
    }

    #endregion

    #region Clear

    public void Clear_MapImgMaterial()
    {
        MapImgList_Data.Clear();
        MapMaterialIndexList_Data.Clear();
    }

    #endregion
}