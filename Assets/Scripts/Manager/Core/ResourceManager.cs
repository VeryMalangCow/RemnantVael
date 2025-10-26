using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class ResourceManager : PersistentSingleton<ResourceManager>
{
    #region File

    #region T

    private T[] GetAsset_Arr<T>(string _Path, string _FileName) where T : UnityEngine.Object
        => Resources.LoadAll<T>(_Path + _FileName);


    private T GetAsset<T>(string _Path, string _FileName) where T : UnityEngine.Object
        => Resources.Load<T>(_Path + _FileName);

    #endregion

    #region CSV

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

    // 열을 쉼표로 나누기
    private string[] Get_Words(TextAsset _TextAsset, int _Row)
    {
        return Regex.Split(Get_AllLine(_TextAsset)[_Row], WORD_SPLIT_RE);
    }

    // 파일을 이중 리스트(string)으로 변경
    private string[][] Get_DoubleArr(TextAsset _TextAsset)
    {
        List<string[]> result = new List<string[]>();
        int amount = Get_FileRowAmount(_TextAsset);
        for (int i = 0; i < amount; i++)
        {
            result.Add(Get_Words(_TextAsset, i));
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
        Offset_CSV_Module();
        Offset_CSV_AllyCard();
        Offset_CSV_AllyRequest();
        Offset_CSV_Map();
        Offset_CSV_Skill();
        Offset_CSV_Tuner();
    }

    private void Offset_Sprite()
    {
        Offset_Sprite_Cutscene();
        Offset_Sprite_Dialogue();
        Offset_Sprite_ModuleItem();
        Offset_Sprite_AllyCard();
        Offset_Sprite_Map();
        Offset_Sprite_Ally();
    }

    private void Offset_Prefab()
    {
        Offset_Prefab_Ally();
        Offset_Prefab_FieldObj();
        Offset_Prefab_ModuleItem();
    }

    private void Offset()
    {
        Offset_Other();
        Offset_CSV();
        Offset_Sprite();
        Offset_Prefab();
    }

    #endregion

    #region SDF + Language

    // SDF
    [HideInInspector] public LanguageTxt[] LanguageTxtArr;

    // 언어 변경을 위한 컴포넌트
    [HideInInspector] private HashSet<LanguageTxtController> AllLanguageTxtController = new HashSet<LanguageTxtController>();
    [HideInInspector] public HashSet<PrisonController> AllPrison = new HashSet<PrisonController>();

    // string
    [HideInInspector] public string RatingString;
    [HideInInspector] public string[] PrisonRateStringArr;
    [HideInInspector] public string StrikeTeamString;
    [HideInInspector] public string UplinkTeamString;
    [HideInInspector] public string NeoTeamString;
    [HideInInspector] public string[] AllyCardRateArr;


    private void Offset_SDF()
    {
        string sdfPath = "SDF/";

        List<LanguageTxt> result = new List<LanguageTxt>();
        for (int i = 0; i < GameManager.KindOfLanguage.Length; i++)
            result.Add(new LanguageTxt(i, GetAsset_SDF(sdfPath, GameManager.KindOfLanguage[i], 3)));

        LanguageTxtArr = result.ToArray();
    }

    private TMP_FontAsset[] GetAsset_SDF(string _Path, string _Type, int _Amount)
    {
        List<TMP_FontAsset> result = new List<TMP_FontAsset>();
        for (int i = 0; i < _Amount; i++)
        {
            string name = $"{_Type}_{i}_SDF";
            result.Add(Resources.Load<TMP_FontAsset>(_Path + name));
        }

        return result.ToArray();
    }


    public void Add_LanguageTxt(LanguageTxtController _LangTxt)
    {
        AllLanguageTxtController.Add(_LangTxt);
    }

    public void Clear_LanguageTxt()
    {
        AllLanguageTxtController.Clear();
    }

    public void Set_LanguageFont(int _LangID)
    {
        if (GameManager.LanguageID == _LangID) return;
        GameManager.LanguageID = _LangID;
        SaveDataManager.Instance.JsonData.OptionData.LanguageID = GameManager.LanguageID;

        // Change String
        Set_LanguageTxt();

        // Change Font Asset
        foreach (LanguageTxtController ltc in AllLanguageTxtController)
            ltc.Set_Font(GameManager.LanguageID);

        string sceneName = SceneManager.GetActiveScene().name;
        // Change UI
        if (sceneName == "MainGame")
        {
            // UI
            MainGameUIManager.Instance.Set_LanguageTxt();

            // Ally
            AllyManager.Instance.Set_Language();

            // Change PrisonInfo
            foreach (PrisonController prison in AllPrison)
                prison.Set_LanguageTxt();
        }
        else if (sceneName == "TitleLobby")
        {
            // UI
            TitleLobbyUIManager.Instance.Set_LanguageTxt();
        }

    }

    private void Set_LanguageTxt()
    {
        RatingString = Get_StaticWord(69);
        PrisonRateStringArr = new string[]
        {
            Get_StaticWord(64),
            Get_StaticWord(65),
            Get_StaticWord(66),
            Get_StaticWord(67),
            Get_StaticWord(68)
        };

        StrikeTeamString = $"{Get_StaticWord(61)}<size=85%> ({Get_StaticWord(71)})</size>";
        UplinkTeamString = $"{Get_StaticWord(62)}<size=85%> ({Get_StaticWord(72)})</size>";
        NeoTeamString = $"{Get_StaticWord(63)}<size=85%> ({Get_StaticWord(73)})</size>";

        AllyCardRateArr = new string[]
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
    [HideInInspector] private WordData StaticWord_Data;
    [HideInInspector] private WordData StaticDesc_Data;

    [HideInInspector] private WordData PlayerName_Data;
    [HideInInspector] private WordData EnemyName_Data;

    [HideInInspector] private WordData_WithClr ProperNoun_Data;
    [HideInInspector] private WordData RandomName_Data;

    // Offset
    private void Offset_CSV_Static()
    {
        string path = "CSV/Static/";

        StaticWord_Data = GetAsset_WordData(path, "StaticWord_CSV");
        StaticDesc_Data = GetAsset_WordData(path, "StaticDesc_CSV");

        PlayerName_Data = GetAsset_WordData(path, "PlayerName_CSV");
        EnemyName_Data = GetAsset_WordData(path, "EnemyName_CSV");

        ProperNoun_Data = GetAsset_WordData_Clr(path, "ProperNoun_CSV");
        RandomName_Data = GetAsset_WordData(path, "RandomName_CSV");
    }


    // Get
    public string Get_StaticWord(int _ID) => StaticWord_Data.Get_Word(_ID);
    public string Get_StaticDesc(int _ID) => StaticDesc_Data.Get_Word(_ID);

    public string Get_PlayerName(int _ID) => PlayerName_Data.Get_Word(_ID);
    public string Get_EnemyName(int _ID) => EnemyName_Data.Get_Word(_ID);

    public string Get_ProperNounWord(int _ID) => ProperNoun_Data.Get_Word(_ID);

    public string[][] Get_AllAllyRandomName()
    {
        List<string[]> result = new List<string[]>();

        for (int i = 0; i < RandomName_Data.AllWordData.Count; i++)
            result.Add(RandomName_Data.AllWordData[i].Words);
        
        return result.ToArray();
    }

    #endregion

    #region Event (CSV)

    [HideInInspector] private EventID[] EventID_Data;
    [HideInInspector] private EventElement[] EventElement_Data;

    // Offset
    private void Offset_CSV_Event()
    {
        // Event
        string path = "CSV/Event/";
        EventElement_Data = Get_EventElement(path, "EventElement_CSV");
        EventID_Data = Get_EventID(path, "EventID_CSV");
    }

    private EventElement[] Get_EventElement(string _Path, string _FileName)
    {
        List<EventElement> result = new List<EventElement>();

        string[][] stringArr = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

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

    private EventID[] Get_EventID(string _Path, string _FileName)
    {
        List<EventID> result = new List<EventID>();

        string[][] stringArr = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

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
    public List<EventElement> Get_CorrectEventArr(int _ID)
    {
        List<EventElement> result = new List<EventElement>();

        int[] IDs = EventID_Data[_ID].EventIDs;

        for (int i = 0; i < IDs.Length; i++)
        {
            EventElement eventElement = EventElement_Data[IDs[i]];

            result.Add(eventElement);
        }

        return result;
    }

    #endregion

    #region Cutscene (CSV)

    // Value
    [HideInInspector] private CutsceneID[] CutsceneID_Data;
    [HideInInspector] private CutsceneElement[][] CutsceneElement_Data;

    // Offset
    private void Offset_CSV_Cutscene()
    {
        string path = "CSV/Cutscene/";

        CutsceneElement_Data = new CutsceneElement[GameManager.KindOfLanguage.Length][];
        for (int i = 0; i < CutsceneElement_Data.Length; i++)
            CutsceneElement_Data[i] = GetAsset_CutsceneElement(path, $"CutsceneElement_{GameManager.KindOfLanguage[i]}_CSV");
        
        CutsceneID_Data = GetAsset_CutsceneID(path, "CutsceneID_CSV");
    }

    private CutsceneElement[] GetAsset_CutsceneElement(string _Path, string _FileName)
    {
        List<CutsceneElement> result = new List<CutsceneElement>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break; 

            int id = int.Parse(stringList[i][0]);
            string script = stringList[i][1];

            result.Add(new CutsceneElement(id, script));
        }

        return result.ToArray();
    }

    private CutsceneID[] GetAsset_CutsceneID(string _Path, string _FileName)
    {
        List<CutsceneID> result = new List<CutsceneID>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

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
    public List<CutsceneElement> Get_CorrectCutsceneElementList(int _ID)
    {
        List<CutsceneElement> result = new List<CutsceneElement>();

        int[] IDs = CutsceneID_Data[_ID].Cutscenes;

        for (int i = 0; i < IDs.Length; i++)
        {
            CutsceneElement cutsceneElement = CutsceneElement_Data[GameManager.LanguageID][i];

            result.Add(cutsceneElement);
        }

        return result;
    }


    #endregion
    #region Cutscene (Sprite)


    [HideInInspector] private Sprite[] CutsceneSprite_Data;

    // Offset
    private void Offset_Sprite_Cutscene()
    {
        string path = "Sprite/UI/Cutscene/";
        CutsceneSprite_Data = GetAsset_Arr<Sprite>(path, "CutsceneSet_00");
    }

    // Get
    public Sprite Get_CutsceneImg(int _ID) => CutsceneSprite_Data[_ID];

    #endregion

    #region Dialogue (CSV)

    // Value
    [HideInInspector] private DialogueID[] DialogueID_Data;
    [HideInInspector] private DialogueElement[][] DialogueElement_Data;

    // Offset
    private void Offset_CSV_Dialogue()
    {
        string path = "CSV/Dialogue/";

        DialogueElement_Data = new DialogueElement[GameManager.KindOfLanguage.Length][];
        for (int i = 0; i < DialogueElement_Data.Length; i++)
            DialogueElement_Data[i] = GetAsset_DialogueElement(path, $"DialogueElement_{GameManager.KindOfLanguage[i]}_CSV");

        DialogueID_Data = GetAsset_DialogueID(path, "DialogueID_CSV");
    }

    private DialogueElement[] GetAsset_DialogueElement(string _Path, string _FileName)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break; 

            int id = int.Parse(stringList[i][0]);
            string name = stringList[i][1];
            string script = stringList[i][2];
            int imgId = int.Parse(stringList[i][3]);
            bool isLeft = bool.Parse(stringList[i][4]);

            result.Add(new DialogueElement(id, name, script, imgId, isLeft));
        }

        return result.ToArray();
    }

    private DialogueID[] GetAsset_DialogueID(string _Path, string _FileName)
    {
        List<DialogueID> result = new List<DialogueID>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

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
    public List<DialogueElement> Get_CorrectDialogueElementList(int _ID)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        int[] IDs = DialogueID_Data[_ID].Dialogus;

        for (int i = 0; i < IDs.Length; i++)
        {
            DialogueElement cutsceneElement = DialogueElement_Data[GameManager.LanguageID][i];

            result.Add(cutsceneElement);
        }

        return result;
    }

    #endregion
    #region Dialogue (Sprite)

    // Value
    [HideInInspector] private Sprite[] DialoguCharSprite_Data;

    // Offset
    private void Offset_Sprite_Dialogue()
    {
        string path = "Sprite/UI/Dialogue/";
        DialoguCharSprite_Data = GetAsset_Arr<Sprite>(path, "CharacterSet_000");
    }

    // Get
    public Sprite Get_DialogueCharImg(int _ID) => DialoguCharSprite_Data[_ID];

    #endregion

    #region Item - Module (CSV)

    // Value
    [HideInInspector] private ModuleBaseData[] ModuleBaseList_Data;

    // 모듈
    [HideInInspector] private WordData ModuleItemName_Data;
    [HideInInspector] private WordData MainChipName_Data;

    [HideInInspector] private WordData ModuleItemDesc_Data;
    [HideInInspector] private WordData ModuleItemEquipDesc_Data;

    [HideInInspector] private WordData[] MainChipDesc_Data;
    [HideInInspector] private WordData MainChipAllyDesc_Data;

    // Offset
    private void Offset_CSV_Module()
    {
        string path = "CSV/Module/";
        ModuleBaseList_Data = GetAsset_ModuleBaseData(path, "Module_CSV");

        ModuleItemName_Data = GetAsset_WordData(path, "ModuleName_CSV");
        MainChipName_Data = GetAsset_WordData(path, "MainChipName_CSV");

        ModuleItemDesc_Data = GetAsset_WordData(path, "ModuleDesc_CSV");
        ModuleItemEquipDesc_Data = GetAsset_WordData(path, "ModuleEquipDesc_CSV");
        MainChipDesc_Data = GetAsset_WordDataArr_ForParentID(path, "MainChipDesc_CSV", MainChipName_Data.AllWordData.Count);
        MainChipAllyDesc_Data = GetAsset_WordData(path, "MainChipAllyDesc_CSV");
    }

    private ModuleBaseData[] GetAsset_ModuleBaseData(string _Path, string _FileName)
    {
        List<ModuleBaseData> result = new List<ModuleBaseData>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

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
        ItemData[] data = new ItemData[ModuleBaseList_Data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new ItemData(i, ModuleItemSprite_Data[i],
                ModuleBaseList_Data[i].ModuleMainChip[0],
                ModuleBaseList_Data[i].ModuleMainChip[1],
                ModuleBaseList_Data[i].ModuleMainChip[2]);

            Set_DataLanguage(data[i], i);
        }
        return data;
    }

    public MainChipData[] Get_MainChipDataArr()
    {
        MainChipData[] data = new MainChipData[MainChipName_Data.AllWordData.Count];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new MainChipData(i, ModuleSynhronySpritet_Data[i]);

            Set_DataLanguage(data[i], i);
        }
        return data;
    }

    // Get Name
    public string Get_ModuleName(int _ID) => ModuleItemName_Data.Get_Word(_ID);
    public string Get_SynergyName(int _ID) => MainChipName_Data.Get_Word(_ID);
    public string Get_MainChipBaseDesc(int _ID) => MainChipAllyDesc_Data.Get_Word(_ID);


    // Set
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
        _MainChipData.AmalgamationDescArr = new string[]
        {
            MainChipDesc_Data[_ID].Get_Word(0),
            MainChipDesc_Data[_ID].Get_Word(1),
            MainChipDesc_Data[_ID].Get_Word(2)
        };

        return _MainChipData;
    }

    #endregion
    #region Item - Module (Sprite)

    // Value
    [HideInInspector] private Sprite[] ModuleItemSprite_Data;
    [HideInInspector] private Sprite[] ModuleSynhronySpritet_Data;

    [HideInInspector] private Sprite[] RankIconArr;
    [HideInInspector] private Sprite[] MUUIDescRankIconArr;
    
    // Offset
    private void Offset_Sprite_ModuleItem()
    {
        string path = "Sprite/UI/MU/";
        ModuleItemSprite_Data = GetAsset_Arr<Sprite>(path, "MUItemUI_000");
        ModuleSynhronySpritet_Data = GetAsset_Arr<Sprite>(path, "MUSynchronyUI_000");

        RankIconArr = new Sprite[5];
        MUUIDescRankIconArr = new Sprite[5];

        Sprite[] allMUUI = GetAsset_Arr<Sprite>(path, "MUUI_000");
        string rankSpriteName = "MUUI_Rank_";
        string rankDescSpriteName = "MUUI_DescRank_";
        for (int i = 0; i < allMUUI.Length; i++)
        {
            Sprite sprite = allMUUI[i];

            if (sprite.name.Length > rankSpriteName.Length && sprite.name.StartsWith(rankSpriteName))
            {
                int index = int.Parse(sprite.name.Replace(rankSpriteName, "")) - 1;
                RankIconArr[index] = sprite;
            }
            else if (sprite.name.Length > rankDescSpriteName.Length && sprite.name.StartsWith(rankDescSpriteName))
            {
                int index = int.Parse(sprite.name.Replace(rankDescSpriteName, "")) - 1;
                MUUIDescRankIconArr[index] = sprite;
            }
        }
    }

    // Get
    public Sprite Get_RankIcon(int _Rank) => RankIconArr[_Rank - 1];
    public Sprite Get_DescRankIcon(int _Rank) => MUUIDescRankIconArr[_Rank - 1];

    #endregion
    #region Item - Module (Prefab)

    // Value
    [SerializeField] private GameObject InventoryItemPrefab;
    [SerializeField] private GameObject InventorySlotPrefab;

    // Offset
    private void Offset_Prefab_ModuleItem()
    {
        string path = "Prefab/UI/MainGame/Build/Player/MU/Inventory/";
        InventoryItemPrefab = GetAsset<GameObject>(path, "Panel_Item_Prefab");
        InventorySlotPrefab = GetAsset<GameObject>(path, "Panel_ItemSlot_Prefab");
    }

    // Get
    public GameObject Get_ModuleItemUI_Prefab() => InventoryItemPrefab;
    public GameObject Get_ModuleSlotUI_Prefab() => InventorySlotPrefab;

    #endregion

    #region Item - AllyCard (CSV)

    // Value
    [HideInInspector] private AllyCardBaseData[] StrikeTeam_AllyCard_Data;
    [HideInInspector] private AllyCardBaseData[] UplinkTeam_AllyCard_Data;
    [HideInInspector] private AllyCardBaseData[] NeoTeam_AllyCard_Data;


    [HideInInspector] private WordData StrikeTeam_AllyCardName_Data;
    [HideInInspector] private WordData UplinkTeam_AllyCardName_Data;
    [HideInInspector] private WordData NeoTeam_AllyCardName_Data;

    [HideInInspector] private WordData StrikeTeam_AllyCardDesc_Data;
    [HideInInspector] private WordData UplinkTeam_AllyCardDesc_Data;
    [HideInInspector] private WordData NeoTeam_AllyCardDesc_Data;

    // Offset
    private void Offset_CSV_AllyCard()
    {
        string path = "CSV/AllyCard/";

        StrikeTeam_AllyCard_Data = GetAsset_AllyCard(path,
            "AllyCard_StrikeTeam_CSV");
        UplinkTeam_AllyCard_Data = GetAsset_AllyCard(path,
            "AllyCard_UplinkTeam_CSV");
        NeoTeam_AllyCard_Data = GetAsset_AllyCard(path,
            "AllyCard_NeoTeam_CSV");

        StrikeTeam_AllyCardName_Data = GetAsset_WordData(path,
            "AllyCard_StrikeTeam_Name_CSV");
        UplinkTeam_AllyCardName_Data = GetAsset_WordData(path,
            "AllyCard_UplinkTeam_Name_CSV");
        NeoTeam_AllyCardName_Data = GetAsset_WordData(path,
            "AllyCard_NeoTeam_Name_CSV");

        StrikeTeam_AllyCardDesc_Data = GetAsset_WordData(path,
            "AllyCard_StrikeTeam_Desc_CSV");
        UplinkTeam_AllyCardDesc_Data = GetAsset_WordData(path,
            "AllyCard_UplinkTeam_Desc_CSV");
        NeoTeam_AllyCardDesc_Data = GetAsset_WordData(path,
            "AllyCard_NeoTeam_Desc_CSV");
    }

    private AllyCardBaseData[] GetAsset_AllyCard(string _Path, string _FileName)
    {
        List<AllyCardBaseData> result = new List<AllyCardBaseData>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

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
    public AllyCardData[] Get_StrikeTeam_AllAllyCardData() => Get_Team_AllAllyCardData(StrikeTeam_AllyCard_Data, StrikeTeam_AllyCardName_Data, StrikeTeam_AllyCardDesc_Data);
    public AllyCardData[] Get_UplinkTeam_AllAllyCardData() => Get_Team_AllAllyCardData(UplinkTeam_AllyCard_Data, UplinkTeam_AllyCardName_Data, UplinkTeam_AllyCardDesc_Data);
    public AllyCardData[] Get_NeoTeam_AllAllyCardData() => Get_Team_AllAllyCardData(NeoTeam_AllyCard_Data, NeoTeam_AllyCardName_Data, NeoTeam_AllyCardDesc_Data);
    
    public AllyCardData[] Get_Team_AllAllyCardData(AllyCardBaseData[] _Data, WordData _NameWord, WordData _DescWord)
    {
        List<AllyCardData> result = new List<AllyCardData>();
        for (int i = 0; i < _Data.Length; i++)
            result.Add(new AllyCardData(_Data[i], _NameWord.Get_Word(i), _DescWord.Get_Word(i)));

        return result.ToArray();
    }

    #endregion
    #region Item - AllyCard (Sprite)

    // Value
    [HideInInspector] private Sprite[][] AllyCardIcon_Data;

    [HideInInspector] private static readonly int STIconAmount = 1;
    [HideInInspector] private static readonly int UTIconAmount = 1;
    [HideInInspector] private static readonly int NTIconAmount = 1;

    // Offset
    private void Offset_Sprite_AllyCard()
    {
        AllyCardIcon_Data = new Sprite[][]
        {
            GetAsset_AllyCardIcon(STIconAmount, "ST"),
            GetAsset_AllyCardIcon(UTIconAmount, "UT"),
            GetAsset_AllyCardIcon(NTIconAmount, "NT")
        };
    }

    private Sprite[] GetAsset_AllyCardIcon(int _SpriteAmount, string _TypeName)
    {
        List<Sprite> result = new List<Sprite>();
        for (int i = 0; i < _SpriteAmount; i++)
        {
            result.AddRange(
                GetAsset_Arr<Sprite>(
                    $"Sprite/UI/Ally/",
                    $"AllyCardIcon_{_TypeName}_{DevTool.Get_LengthString(i, 3)}"));
        }
        return result.ToArray();
    }

    // Get
    public Sprite[] Get_AllyCardSpriteIcon(int _Type) => AllyCardIcon_Data[_Type];

    #endregion

    #region Ally (Sprite)

    // Value
    private static readonly string[] directionOrder = new string[] { "UL", "U", "UR", "R", "DR", "D", "DL", "L" };
    [HideInInspector] private Sprite[] AllySprite_Data;

    // Offset
    private void Offset_Sprite_Ally()
    {
        string path = "Sprite/Ally/";
        AllySprite_Data = GetAsset_Arr<Sprite>(path,"Ally_001");
    }

    // Get
    public List<Sprite> Get_AllySprite(string _Name, string _Type)
    {
        List<Sprite> result = new List<Sprite>();

        int stringLength = 7 + _Name.Length + _Type.Length;

        // 맞는 아트 리소스 가져오기
        for (int i = 0; i < AllySprite_Data.Length; i++)
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
    #region Ally (Prefab)

    // Value
    [HideInInspector] private Dictionary<string, GameObject> AllyFieldUnit_PrefabDict;
    [HideInInspector] private Dictionary<string, GameObject> AllyNoneUnit_PrefabDict;

    // Offset
    private void Offset_Prefab_Ally()
    {
        string path = "Prefab/Ally/";

        string fieldUnitPath = path + "FieldUnit/";
        AllyFieldUnit_PrefabDict = new Dictionary<string, GameObject>
        {
            { "Grunt", GetAsset<GameObject>(fieldUnitPath, "GruntAlly_Prefab") },

            { "Ignis", GetAsset<GameObject>(fieldUnitPath, "IgnisAlly_Prefab") },
            { "Glacia", GetAsset<GameObject>(fieldUnitPath, "GlaciaAlly_Prefab") },
            { "Volt", GetAsset<GameObject>(fieldUnitPath, "VoltAlly_Prefab") },
            { "Tox", GetAsset<GameObject>(fieldUnitPath, "ToxAlly_Prefab") }
        };

        string noneUnitPath = path + "NoneUnit/";
        AllyNoneUnit_PrefabDict = new Dictionary<string, GameObject>
        {
            { "Booma", GetAsset<GameObject>(noneUnitPath, "BoomaAlly_Prefab") },
            { "Totis", GetAsset<GameObject>(noneUnitPath, "TotisAlly_Prefab") }
        };
    }

    // Get
    public GameObject Get_FieldUnitAlly(string _Name) => AllyFieldUnit_PrefabDict[_Name];
    public GameObject Get_NoneUnitAlly(string _Name) => AllyNoneUnit_PrefabDict[_Name];

    #endregion

    #region AllyRequest (CSV)

    // Value
    [HideInInspector] private WordData RequestName_Data;
    [HideInInspector] private WordData RequestCompleteDesc_Data;
    [HideInInspector] private WordData RequestFailDesc_Data;

    // Offset
    private void Offset_CSV_AllyRequest()
    {
        string path = "CSV/AllyRequest/";
        RequestName_Data = GetAsset_WordData(path, "RequestName_CSV");
        RequestCompleteDesc_Data = GetAsset_WordData(path, "RequestCompleteDesc_CSV");
        RequestFailDesc_Data = GetAsset_WordData(path, "RequestFailDesc_CSV");
    }

    // Get
    public string Get_RequestName(int _ID) => RequestName_Data.Get_Word(_ID);
    public string Get_RequestCompleteDesc(int _ID) => RequestCompleteDesc_Data.Get_Word(_ID);
    public string Get_RequestFailDesc(int _ID) => RequestFailDesc_Data.Get_Word(_ID);

    #endregion

    #region Map (CSV)

    // Value
    [HideInInspector] private Dictionary<int, MapNextIndex> MapNextIndex_Data;

    [HideInInspector] private WordData MapName_Data;
    [HideInInspector] private WordData MapDesc_Data;


    // Offset
    private void Offset_CSV_Map()
    {
        string path = "CSV/Map/";
        MapNextIndex_Data = Offset_MapNextIndex(path, "MapEntranceIndex_CSV");
        MapName_Data = GetAsset_WordData(path, "MapName_CSV");
        MapDesc_Data = GetAsset_WordData(path, "MapDesc_CSV");
    }

    private Dictionary<int, MapNextIndex> Offset_MapNextIndex(string _Path, string _FileName)
    {
        Dictionary<int, MapNextIndex> result = new Dictionary<int, MapNextIndex>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break; 

            int pastIndex = int.Parse(stringList[i][0]);
            int nextIndex = int.Parse(stringList[i][1]);

            List<MapNextIndex> indexList = new List<MapNextIndex>();
            if (Is_ExistMapIndex(result, pastIndex, out MapNextIndex mapNextIndex)) // 이미 존재한다면
            {
                mapNextIndex.NextIndexList.Add(nextIndex);
            }
            else // 존재하지 않는다면
            {
                result.Add(pastIndex, new MapNextIndex(pastIndex, nextIndex));
            }
        }

        return result;
    }

    private bool Is_ExistMapIndex(Dictionary<int, MapNextIndex> _AllMapNextIndex, int _PastIndex, out MapNextIndex _MapNextIndex)
    {
        _MapNextIndex = null;
        if (_AllMapNextIndex.ContainsKey(_PastIndex))
        {
            _MapNextIndex = _AllMapNextIndex[_PastIndex];
            return true;
        }
        return false;
    }


    // Get
    public string Get_MapName(int _ID) => MapName_Data.Get_Word(_ID);
    public string Get_MapDesc(int _ID) => MapDesc_Data.Get_Word(_ID);

    public List<int> Get_CorrectIndexList(int _PastIndex)
    {
        if (MapNextIndex_Data.ContainsKey(_PastIndex))
            return MapNextIndex_Data[_PastIndex].NextIndexList;
        
        return null;
    }

    #endregion
    #region Map (Sprite)

    // Value
    [HideInInspector] private int EachKindOfMapAmount = 2;

    [HideInInspector] private MapReso LobbyMapReso;

    [HideInInspector] public static int KindOfMapAmount = 2;
    [HideInInspector] private MapReso[] StageMapReso;

    [HideInInspector] private MapReso PassageMapReso;

    [HideInInspector] private int KindOfFieldObjType = 3;
    [HideInInspector] private Sprite[][][] MapFieldObjList_Data;

    // Offset
    private void Offset_Sprite_Map()
    {
        string path = $"Sprite/Map/";

        // Lobby Map
        string lobbyName = $"MapLobby";
        LobbyMapReso = GetAsset_MapReso(path, lobbyName);

        // Stage Map
        List<MapReso> resos = new List<MapReso>();
        for (int i = 0; i < KindOfMapAmount; i++)
        {
            string stageName = $"Map{DevTool.Get_LengthString(i, 2)}";
            resos.Add(GetAsset_MapReso(path, stageName));
        }
        StageMapReso = resos.ToArray();

        // Passage Map
        string passageName = $"MapPassage/";
        PassageMapReso = GetAsset_MapReso(path, passageName);

        // Kind of Map / Type / List
        List<Sprite[][]> mapFieldObjList_Data = new List<Sprite[][]>();
        for (int i = 0; i < StageMapReso.Length; i++)
        {
            mapFieldObjList_Data.Add(Get_FieldObj(StageMapReso[i]));
        }
        MapFieldObjList_Data = mapFieldObjList_Data.ToArray();
    }

    private MapReso GetAsset_MapReso(string _Path, string _Name)
    {
        List<MapResoElement> mapResoElements = new List<MapResoElement>();

        for (int i = 0; i < EachKindOfMapAmount; i++)
        {
            Sprite[] spriteArr = GetAsset_Arr<Sprite>(_Path + _Name + "/", $"{_Name}_{DevTool.Get_LengthString(i, 3)}");

            for (int j = 0; j < spriteArr.Length; j++)
            {
                mapResoElements.Add(new MapResoElement(spriteArr[j], i));
            }
        }

        return new MapReso(mapResoElements.ToArray());
    }

    private Sprite[][] Get_FieldObj(MapReso _Reso) // Type / SpriteList
    {
        List<List<Sprite>> result = new List<List<Sprite>>();

        for (int i = 0; i < KindOfFieldObjType; i++)
        {
            result.Add(new List<Sprite>());
        }

        for (int i = 0; i < _Reso.MapResoElements.Length; i++)
        {
            string[] name = _Reso.MapResoElements[i].Sprite.name.Split("_");
            if (name[1] == "FieldObj")
            {
                int type = Int32.Parse(name[2].Substring(1, 2));
                result[type].Add(_Reso.MapResoElements[i].Sprite);
            }
        }

        List<Sprite[]> result2 = new List<Sprite[]>();
        for (int i = 0; i < result.Count; i++)
        {
            result2.Add(result[i].ToArray());
        }

        return result2.ToArray();
    }

    // Get
    public MapReso Get_LobbyMapReso() => LobbyMapReso;
    public MapReso Get_StageMapReso(int _ID) => StageMapReso[_ID];
    public MapReso Get_PassageMapReso() => PassageMapReso;

    public Sprite Get_RandomFieldObjSprite(int _StageID, int _TypeID)
    {
        Sprite[] spriteArr = MapFieldObjList_Data[_StageID][_TypeID];
        return spriteArr[UnityEngine.Random.Range(0, spriteArr.Length)];
    }


    #endregion
    #region Map (Prefab)

    // Value
    [HideInInspector] private GameObject[] FieldObjArray;

    // Offset
    private void Offset_Prefab_FieldObj()
    {
        string path = "Prefab/FieldObj/";

        FieldObjArray = new GameObject[KindOfFieldObjType];

        for (int i = 0; i < KindOfFieldObjType; i++)
            FieldObjArray[i] = GetAsset<GameObject>(path, $"FieldObj_T{DevTool.Get_LengthString(i, 2)}");
    }

    // Get
    public GameObject Get_RandomFieldObj_Prefab() => FieldObjArray[UnityEngine.Random.Range(0, FieldObjArray.Length)];

    #endregion

    #region Skill (CSV)

    // Value
    [HideInInspector] private WordData[] SkillName_Data;
    [HideInInspector] private WordData[] SkillDesc_Data;

    private void Offset_CSV_Skill()
    {
        string path = "CSV/Skill/";
        SkillName_Data = GetAsset_WordDataArr_ForParentID(path, "SkillName_CSV", PlayerManager.KindOfPlayerAmount);
        SkillDesc_Data = GetAsset_WordDataArr_ForParentID(path, "SkillDesc_CSV", PlayerManager.KindOfPlayerAmount);
    }

    // Get
    public string Get_SkillName(int _PlayerID, int _ID) => SkillName_Data[_PlayerID].Get_Word(_ID);
    public string Get_SkillDesc(int _PlayerID, int _ID) => SkillDesc_Data[_PlayerID].Get_Word(_ID);

    #endregion

    #region Tuner (CSV)

    // Value
    [HideInInspector] private WordData TunerStateName_Data;

    // Offset
    private void Offset_CSV_Tuner()
    {
        string path = "CSV/Tuner/";
        TunerStateName_Data = GetAsset_WordData(path, "TunerStateName_CSV");
    }

    // Get
    public string Get_TunerDescName(int _Index) => TunerStateName_Data.Get_Word(_Index);

    #endregion

    #region GetAsset_WordData

    private WordData[] GetAsset_WordDataArr_ForParentID(string _Path, string _FileName, int _Amount)
    {
        List<WordData> result = new List<WordData>();
        for (int i = 0; i < _Amount; i++)
            result.Add(GetAsset_WordData_ForParentID(_Path, _FileName, i));

        return result.ToArray();
    }

    private WordData GetAsset_WordData_ForParentID(string _Path, string _FileName, int _TargetParentID)
    {
        List<WordElementData> element = new List<WordElementData>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            if (int.Parse(stringList[i][0]) != _TargetParentID) continue;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][1]);

            for (int j = 1; j < GameManager.KindOfLanguage.Length + 1; j++)
                nameList.Add(stringList[i][j + 1]);

            element.Add(new WordElementData(id, nameList.ToArray()));
        }

        return new WordData(element);
    }

    private WordData GetAsset_WordData(string _Path, string _FileName)
    {
        List<WordElementData> element = new List<WordElementData>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][0]);

            for (int j = 0; j < GameManager.KindOfLanguage.Length; j++)
                nameList.Add(stringList[i][j + 1]);

            element.Add(new WordElementData(id, nameList.ToArray()));
        }

        return new WordData(element);
    }

    private WordData_WithClr GetAsset_WordData_Clr(string _Path, string _FileName)
    {
        List<WordElementData_WithClr> element = new List<WordElementData_WithClr>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            string clrHex = stringList[i][1];

            List<string> nameList = new List<string>();
            for (int j = 0; j < GameManager.KindOfLanguage.Length; j++)
                nameList.Add(stringList[i][j + 2]);

            element.Add(new WordElementData_WithClr(id, clrHex, nameList.ToArray()));
        }

        return new WordData_WithClr(element);
    }

    #endregion
}