using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager : PersistentSingleton<ResourceManager>
{
    #region File

    #region T

    private T[] GetAsset_Arr<T>(string path, string fileName = "") where T : UnityEngine.Object
        => Resources.LoadAll<T>(path + fileName);

    private T GetAsset<T>(string path, string fileName) where T : UnityEngine.Object
        => Resources.Load<T>(path + fileName);

    #endregion

    #region CSV

    [HideInInspector] private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    [HideInInspector] private static string WORD_SPLIT_RE = @",";

    // 파일 => 스트링
    private string Get_FileString(TextAsset textAsset)
    {
        return textAsset.text;
    }

    // 행 길이 구하기
    private int Get_FileRowAmount(TextAsset textAsset)
    {
        return Get_AllLine(textAsset).Length;
    }

    // 행 받아오기
    private string[] Get_AllLine(TextAsset textAsset)
    {
        return Regex.Split(Get_FileString(textAsset), LINE_SPLIT_RE);
    }

    // 열을 쉼표로 나누기
    private string[] Get_Words(TextAsset textAsset, int _Row)
    {
        return Regex.Split(Get_AllLine(textAsset)[_Row], WORD_SPLIT_RE);
    }

    // 파일을 이중 리스트(string)으로 변경
    private string[][] Get_DoubleArr(TextAsset textAsset)
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

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();

        Offset();
        Debug.Log("ResouceManager : Offset Complete");
    }

    private void Start()
    {
        Set_LanguageTxt();
    }

    #endregion

    #region Offset

    private void Offset_Other()
    {
        Offset_SDF();
    }

    private void Offset_CSV()
    {
        Offset_CSV_Static();
        Offset_CSV_Event();
        Offset_CSV_Cutscene();
        Offset_CSV_Dialogue();
        Offset_CSV_Info();
        Offset_CSV_Module();
        Offset_CSV_AllyCard();
        Offset_CSV_AllyRequest();
        Offset_CSV_Map();
        Offset_CSV_Skill();
        Offset_CSV_Tuner();
    }

    public void Offset()
    {
        Offset_Other();
        Offset_CSV();
    }

    #endregion


    #region SDF + Language

    // SDF
    [HideInInspector] public LanguageTxt[] languageTxtArr;

    // 언어 변경을 위한 컴포넌트
    [HideInInspector] private HashSet<LanguageTxtController> allLanguageTxtControllers = new HashSet<LanguageTxtController>();
    [HideInInspector] public HashSet<PrisonController> allPrisons = new HashSet<PrisonController>();

    // string
    [HideInInspector] public string ratingString;
    [HideInInspector] public string[] prisonRateStringArr;
    [HideInInspector] public string strikeTeamString;
    [HideInInspector] public string uplinkTeamString;
    [HideInInspector] public string neoTeamString;
    [HideInInspector] public string[] allyCardRateArr;

    private void Offset_SDF()
    {
        string sdfPath = "SDF/";

        List<LanguageTxt> result = new List<LanguageTxt>();
        for (int i = 0; i < GameManager.kindOfLanguage.Length; i++)
            result.Add(new LanguageTxt(i, GetAsset_SDF(sdfPath, GameManager.kindOfLanguage[i], 3)));

        languageTxtArr = result.ToArray();
    }

    private TMP_FontAsset[] GetAsset_SDF(string path, string type, int amount)
    {
        List<TMP_FontAsset> result = new List<TMP_FontAsset>();
        for (int i = 0; i < amount; i++)
        {
            string name = $"{type}_{i}_SDF";
            result.Add(Resources.Load<TMP_FontAsset>(path + name));
        }

        return result.ToArray();
    }


    public void Add_LanguageTxt(LanguageTxtController langTxt)
    {
        allLanguageTxtControllers.Add(langTxt);
    }

    public void Clear_LanguageTxt()
    {
        allLanguageTxtControllers.Clear();
    }

    public void Set_LanguageFont(int langID)
    {
        if (GameManager.languageID == langID) return;
        GameManager.languageID = langID;
        SaveDataManager.instance.jsonData.optionData.languageID = GameManager.languageID;

        // Change String
        Set_LanguageTxt();

        // Change Font Asset
        foreach (LanguageTxtController ltc in allLanguageTxtControllers)
            ltc.Set_Font(GameManager.languageID);

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

    private void Set_LanguageTxt()
    {
        ratingString = Get_StaticWord(69);
        prisonRateStringArr = new string[]
        {
            Get_StaticWord(64),
            Get_StaticWord(65),
            Get_StaticWord(66),
            Get_StaticWord(67),
            Get_StaticWord(68)
        };

        strikeTeamString = $"{Get_StaticWord(61)}<size=85%> ({Get_StaticWord(71)})</size>";
        uplinkTeamString = $"{Get_StaticWord(62)}<size=85%> ({Get_StaticWord(72)})</size>";
        neoTeamString = $"{Get_StaticWord(63)}<size=85%> ({Get_StaticWord(73)})</size>";

        allyCardRateArr = new string[]
        {
            Get_StaticWord(76),
            Get_StaticWord(77),
            Get_StaticWord(78),
            Get_StaticWord(79),
            Get_StaticWord(80),
            Get_StaticWord(81)
        };
    }

    #endregion

    #region Static (CSV)

    // Value
    [HideInInspector] private WordSet_Just staticWord_Data;
    [HideInInspector] private WordSet_Just staticDesc_Data;

    [HideInInspector] private WordSet_Just playerName_Data;
    [HideInInspector] private WordSet_Just enemyName_Data;

    [HideInInspector] private WordSet_WithClr properNoun_Data;
    [HideInInspector] private WordSet_Just randomName_Data;

    // Offset
    private void Offset_CSV_Static()
    {
        string path = "CSV/Static/";

        staticWord_Data = GetAsset_WordData(path, "StaticWord_CSV");
        staticDesc_Data = GetAsset_WordData(path, "StaticDesc_CSV");

        playerName_Data = GetAsset_WordData(path, "PlayerName_CSV");
        enemyName_Data = GetAsset_WordData(path, "EnemyName_CSV");

        properNoun_Data = GetAsset_WordData_Clr(path, "ProperNoun_CSV");
        randomName_Data = GetAsset_WordData(path, "RandomName_CSV");
    }


    // Get
    public string Get_StaticWord(int id) => staticWord_Data.Get_Word(id);
    public string Get_StaticDesc(int id) => staticDesc_Data.Get_Word(id);

    public string Get_PlayerName(int id) => playerName_Data.Get_Word(id);
    public string Get_EnemyName(int id) => enemyName_Data.Get_Word(id);

    public string Get_ProperNounWord(int id) => properNoun_Data.Get_Word(id);

    public string[] Get_AllyRandomName(int id) => randomName_Data.Get_Words(id);
    public int Get_AllAllyRandomNameAmount() => randomName_Data.Get_Amount();

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

    #region Info (CSV)

    // Value
    [HideInInspector] private WordSet_Just infoName_Data;
    [HideInInspector] private WordSet_Just[] infoDetail_Data;

    // Offset
    private void Offset_CSV_Info()
    {
        string path = "CSV/Info/";

        infoName_Data = GetAsset_WordData(path, "InfoName_CSV");
        int amount = infoName_Data.Get_Amount();
        infoDetail_Data = GetAsset_WordDataArr_ForParentID(path, "InfoDetail_CSV", amount);
    }

    public string Get_InfoName(int id) => infoName_Data.Get_Word(id);
    public WordSet_Just Get_InfoDetail(int id) => infoDetail_Data[id];

    #endregion

    #region Item - Module (CSV)

    // Value
    [HideInInspector] private ModuleBaseData[] moduleBaseList_Data;

    // 모듈
    [HideInInspector] private WordSet_Just moduleItemName_Data;
    [HideInInspector] private WordSet_Just mainChipName_Data;

    [HideInInspector] private WordSet_Just moduleItemDesc_Data;
    [HideInInspector] private WordSet_Just moduleItemEquipDesc_Data;

    [HideInInspector] private WordSet_Just[] mainChipDesc_Data;
    [HideInInspector] private WordSet_Just mainChipAllyDesc_Data;

    // Offset
    private void Offset_CSV_Module()
    {
        string path = "CSV/Module/";
        moduleBaseList_Data = GetAsset_ModuleBaseData(path, "Module_CSV");

        moduleItemName_Data = GetAsset_WordData(path, "ModuleName_CSV");
        mainChipName_Data = GetAsset_WordData(path, "MainChipName_CSV");

        moduleItemDesc_Data = GetAsset_WordData(path, "ModuleDesc_CSV");
        moduleItemEquipDesc_Data = GetAsset_WordData(path, "ModuleEquipDesc_CSV");
        mainChipDesc_Data = GetAsset_WordDataArr_ForParentID(path, "MainChipDesc_CSV", mainChipName_Data.Get_Amount());
        mainChipAllyDesc_Data = GetAsset_WordData(path, "MainChipAllyDesc_CSV");
    }

    private ModuleBaseData[] GetAsset_ModuleBaseData(string path, string fileName)
    {
        List<ModuleBaseData> result = new List<ModuleBaseData>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            int r1mainChip = int.Parse(stringList[i][1]);
            int r3mainChip = int.Parse(stringList[i][2]);
            int r5mainChip = int.Parse(stringList[i][3]);

            result.Add(new ModuleBaseData(id, new List<int> { r1mainChip, r3mainChip, r5mainChip }));
        }

        return result.ToArray();
    }



    // Get Data
    public ItemData[] Get_ItemDataArr()
    {
        ItemData[] data = new ItemData[moduleBaseList_Data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new ItemData(i, StaticResourceManager.instance.ItemReso.moduleItemSprites[i],
                moduleBaseList_Data[i].moduleMainChip[0],
                moduleBaseList_Data[i].moduleMainChip[1],
                moduleBaseList_Data[i].moduleMainChip[2]);

            Set_DataLanguage(data[i], i);
        }
        return data;
    }

    public MainChipData[] Get_MainChipDataArr()
    {
        MainChipData[] data = new MainChipData[mainChipName_Data.Get_Amount()];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new MainChipData(i, StaticResourceManager.instance.ItemReso.moduleSynhronySprites[i]);

            Set_DataLanguage(data[i], i);
        }
        return data;
    }

    // Get Name
    public string Get_ModuleName(int id) => moduleItemName_Data.Get_Word(id);
    public string Get_SynergyName(int id) => mainChipName_Data.Get_Word(id);
    public string Get_MainChipBaseDesc(int id) => mainChipAllyDesc_Data.Get_Word(id);


    // Set
    public ItemData Set_DataLanguage(ItemData itemData, int id)
    {
        itemData.name = moduleItemName_Data.Get_Word(id);
        itemData.desc = moduleItemDesc_Data.Get_Word(id);
        itemData.equipDesc = moduleItemEquipDesc_Data.Get_Word(id);

        return itemData;
    }

    public MainChipData Set_DataLanguage(MainChipData mainChipData, int id)
    {
        mainChipData.name = mainChipName_Data.Get_Word(id);
        mainChipData.amalgamationDescArr = new string[]
        {
            mainChipDesc_Data[id].Get_Word(0),
            mainChipDesc_Data[id].Get_Word(1),
            mainChipDesc_Data[id].Get_Word(2)
        };

        return mainChipData;
    }

    #endregion

    #region AllyCard (CSV)

    // Value
    [HideInInspector] private AllyCardBaseData[] strikeTeam_AllyCard_Data;
    [HideInInspector] private AllyCardBaseData[] uplinkTeam_AllyCard_Data;
    [HideInInspector] private AllyCardBaseData[] neoTeam_AllyCard_Data;


    [HideInInspector] private WordSet_Just strikeTeam_AllyCardName_Data;
    [HideInInspector] private WordSet_Just uplinkTeam_AllyCardName_Data;
    [HideInInspector] private WordSet_Just neoTeam_AllyCardName_Data;

    [HideInInspector] private WordSet_Just strikeTeam_AllyCardDesc_Data;
    [HideInInspector] private WordSet_Just uplinkTeam_AllyCardDesc_Data;
    [HideInInspector] private WordSet_Just neoTeam_AllyCardDesc_Data;

    // Offset
    private void Offset_CSV_AllyCard()
    {
        string path = "CSV/AllyCard/";

        strikeTeam_AllyCard_Data = GetAsset_AllyCard(path,
            "AllyCard_StrikeTeam_CSV");
        uplinkTeam_AllyCard_Data = GetAsset_AllyCard(path,
            "AllyCard_UplinkTeam_CSV");
        neoTeam_AllyCard_Data = GetAsset_AllyCard(path,
            "AllyCard_NeoTeam_CSV");

        strikeTeam_AllyCardName_Data = GetAsset_WordData(path,
            "AllyCard_StrikeTeam_Name_CSV");
        uplinkTeam_AllyCardName_Data = GetAsset_WordData(path,
            "AllyCard_UplinkTeam_Name_CSV");
        neoTeam_AllyCardName_Data = GetAsset_WordData(path,
            "AllyCard_NeoTeam_Name_CSV");

        strikeTeam_AllyCardDesc_Data = GetAsset_WordData(path,
            "AllyCard_StrikeTeam_Desc_CSV");
        uplinkTeam_AllyCardDesc_Data = GetAsset_WordData(path,
            "AllyCard_UplinkTeam_Desc_CSV");
        neoTeam_AllyCardDesc_Data = GetAsset_WordData(path,
            "AllyCard_NeoTeam_Desc_CSV");
    }

    private AllyCardBaseData[] GetAsset_AllyCard(string path, string fileName)
    {
        List<AllyCardBaseData> result = new List<AllyCardBaseData>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            int rank = int.Parse(stringList[i][1]);
            int essentialID = int.Parse(stringList[i][2]);

            result.Add(new AllyCardBaseData(id, rank, essentialID));
        }

        return result.ToArray();
    }


    // Get
    public AllyCardData[] Get_StrikeTeam_AllAllyCardData() => Get_Team_AllAllyCardData(strikeTeam_AllyCard_Data, strikeTeam_AllyCardName_Data, strikeTeam_AllyCardDesc_Data);
    public AllyCardData[] Get_UplinkTeam_AllAllyCardData() => Get_Team_AllAllyCardData(uplinkTeam_AllyCard_Data, uplinkTeam_AllyCardName_Data, uplinkTeam_AllyCardDesc_Data);
    public AllyCardData[] Get_NeoTeam_AllAllyCardData() => Get_Team_AllAllyCardData(neoTeam_AllyCard_Data, neoTeam_AllyCardName_Data, neoTeam_AllyCardDesc_Data);

    public AllyCardData[] Get_Team_AllAllyCardData(AllyCardBaseData[] data, WordSet_Just nameWord, WordSet_Just descWord)
    {
        List<AllyCardData> result = new List<AllyCardData>();
        for (int i = 0; i < data.Length; i++)
            result.Add(new AllyCardData(data[i], nameWord.Get_Word(i), descWord.Get_Word(i)));

        return result.ToArray();
    }

    #endregion

    #region AllyRequest (CSV)

    // Value
    [HideInInspector] private WordSet_Just requestName_Data;
    [HideInInspector] private WordSet_Just requestCompleteDesc_Data;
    [HideInInspector] private WordSet_Just requestFailDesc_Data;

    // Offset
    private void Offset_CSV_AllyRequest()
    {
        string path = "CSV/AllyRequest/";
        requestName_Data = GetAsset_WordData(path, "RequestName_CSV");
        requestCompleteDesc_Data = GetAsset_WordData(path, "RequestCompleteDesc_CSV");
        requestFailDesc_Data = GetAsset_WordData(path, "RequestFailDesc_CSV");
    }

    // Get
    public string Get_RequestName(int id) => requestName_Data.Get_Word(id);
    public string Get_RequestCompleteDesc(int id) => requestCompleteDesc_Data.Get_Word(id);
    public string Get_RequestFailDesc(int id) => requestFailDesc_Data.Get_Word(id);

    #endregion

    #region Map (CSV)


    // Value

    [HideInInspector] private WordSet_Just mapName_Data;
    [HideInInspector] private WordSet_Just mapDesc_Data;


    // Offset
    private void Offset_CSV_Map()
    {
        string path = "CSV/Map/";
        mapName_Data = GetAsset_WordData(path, "MapName_CSV");
        mapDesc_Data = GetAsset_WordData(path, "MapDesc_CSV");
    }

    // Get
    public string Get_MapName(int id) => mapName_Data.Get_Word(id);
    public string Get_MapDesc(int id) => mapDesc_Data.Get_Word(id);

    #endregion

    #region Skill (CSV)

    // Value
    [HideInInspector] private WordSet_Just[] skillName_Data;
    [HideInInspector] private WordSet_Just[] skillDesc_Data;

    private void Offset_CSV_Skill()
    {
        string path = "CSV/Skill/";
        skillName_Data = GetAsset_WordDataArr_ForParentID(path, "SkillName_CSV", PlayerManager.kindOfPlayerAmount);
        skillDesc_Data = GetAsset_WordDataArr_ForParentID(path, "SkillDesc_CSV", PlayerManager.kindOfPlayerAmount);
    }

    // Get
    public string Get_SkillName(int playerID, int id) => skillName_Data[playerID].Get_Word(id);
    public string Get_SkillDesc(int playerID, int id) => skillDesc_Data[playerID].Get_Word(id);

    #endregion

    #region Tuner (CSV)

    // Value
    [HideInInspector] private WordSet_Just tunerStateName_Data;

    // Offset
    private void Offset_CSV_Tuner()
    {
        string path = "CSV/Tuner/";
        tunerStateName_Data = GetAsset_WordData(path, "TunerStateName_CSV");
    }

    // Get
    public string Get_TunerDescName(int index) => tunerStateName_Data.Get_Word(index);

    #endregion


    #region GetAsset_WordData

    private WordSet_Just[] GetAsset_WordDataArr_ForParentID(string path, string fileName, int amount)
    {
        List<WordSet_Just> result = new List<WordSet_Just>();
        for (int i = 0; i < amount; i++)
            result.Add(GetAsset_WordData_ForParentID(path, fileName, i));

        return result.ToArray();
    }

    private WordSet_Just GetAsset_WordData_ForParentID(string path, string fileName, int targetParentID)
    {
        Dictionary<int, WordElement_Just> elementDict = new Dictionary<int, WordElement_Just>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            if (int.Parse(stringList[i][0]) != targetParentID) continue;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][1]);

            for (int j = 1; j < GameManager.kindOfLanguage.Length + 1; j++)
                nameList.Add(stringList[i][j + 1]);

            elementDict.Add(id, new WordElement_Just(id, nameList.ToArray()));
        }

        return new WordSet_Just(elementDict);
    }

    private WordSet_Just GetAsset_WordData(string path, string fileName)
    {
        Dictionary<int, WordElement_Just> elementDict = new Dictionary<int, WordElement_Just>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][0]);

            for (int j = 0; j < GameManager.kindOfLanguage.Length; j++)
                nameList.Add(stringList[i][j + 1]);

            elementDict.Add(id, new WordElement_Just(id, nameList.ToArray()));
        }

        return new WordSet_Just(elementDict);
    }

    private WordSet_WithClr GetAsset_WordData_Clr(string path, string fileName)
    {
        Dictionary<int, WordElement_WithClr> elementDict = new Dictionary<int, WordElement_WithClr>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            string clrHex = stringList[i][1];

            List<string> nameList = new List<string>();
            for (int j = 0; j < GameManager.kindOfLanguage.Length; j++)
                nameList.Add(stringList[i][j + 2]);

            elementDict.Add(id, new WordElement_WithClr(id, clrHex, nameList.ToArray()));
        }

        return new WordSet_WithClr(elementDict);
    }

    #endregion
}