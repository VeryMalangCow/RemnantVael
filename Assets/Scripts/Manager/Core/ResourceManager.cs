using System;
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

    private T[] GetAsset_Arr<T>(string _Path, string _FileName = "") where T : UnityEngine.Object
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
        Offset_CSV_Info();
        Offset_CSV_Module();
        Offset_CSV_AllyCard();
        Offset_CSV_AllyRequest();
        Offset_CSV_Map();
        Offset_CSV_Skill();
        Offset_CSV_Tuner();
    }

    private void Offset_Sprite()
    {
        Offset_Sprite_Static();
        Offset_Sprite_Cutscene();
        Offset_Sprite_Dialogue();
        Offset_Sprite_ModuleItem();
        Offset_Sprite_Core();
        Offset_Sprite_AllyCard();
        Offset_Sprite_AllyRequest();
        Offset_Sprite_Prison();
        Offset_Sprite_Map();
        Offset_Sprite_Ally();
        Offset_Sprite_PuzzleNSC();
        Offset_Sprite_Minimap();
    }

    private void Offset_Material()
    {
        Offset_Material_Static();
        Offset_Material_Map();
    }

    private void Offset_Prefab()
    {
        Offset_Prefab_Ally();
        Offset_Prefab_ModuleItem();
        Offset_Prefab_Map();
        Offset_Prefab_Build(); 
        Offset_Prefab_CanvasUI();
    }

    private void Offset_Anim()
    {
        Offset_Anim_Static();
        Offset_Anim_Module();
        Offset_Anim_Keycard();
        Offset_Anim_PlayerShop();
        Offset_Anim_AllyShop();
        Offset_Anim_Vault();
        Offset_Anim_Prison();
        Offset_Anim_Operator();
        Offset_Anim_Converter();
        Offset_Anim_Core();
    }

    private void Offset()
    {
        Offset_Other();
        Offset_CSV();
        Offset_Sprite();
        Offset_Material();
        Offset_Prefab(); 
        Offset_Anim();
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
    [HideInInspector] private WordSet_Just StaticWord_Data;
    [HideInInspector] private WordSet_Just StaticDesc_Data;

    [HideInInspector] private WordSet_Just PlayerName_Data;
    [HideInInspector] private WordSet_Just EnemyName_Data;

    [HideInInspector] private WordSet_WithClr ProperNoun_Data;
    [HideInInspector] private WordSet_Just RandomName_Data;

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

    public string[] Get_AllyRandomName(int _ID) => RandomName_Data.Get_Words(_ID);
    public int Get_AllAllyRandomNameAmount() => RandomName_Data.Get_Amount();

    #endregion
    #region Static (Sprite)

    // Value
    [HideInInspector] private Sprite[] EnemyPhaseSpriteArr;
    [HideInInspector] public Sprite BuildingDurFrame { get; private set; }
    [HideInInspector] public Sprite BuildingDurInner { get; private set; }

    [HideInInspector] public Sprite SpaceBarSprite { get; private set; }
    [HideInInspector] public Sprite MLBSprite { get; private set; }
    [HideInInspector] public Sprite MRBSprite { get; private set; }

    [HideInInspector] public Color LockedClr { get; private set; }
    [HideInInspector] public Color UnlockedClr;

    [HideInInspector] public CoupleData<Sprite> CvtMaterialConditionIcon { get; private set; }

    // Offset
    private void Offset_Sprite_Static()
    {
        string path = "Sprite/";

        #region Module UI

        string moduleUIPath = path + "UI/ModuleUI/";
        Sprite[] moduleUISprites = GetAsset_Arr<Sprite>(moduleUIPath, "ModuleUI_000");
        CvtMaterialConditionIcon = new CoupleData<Sprite>(null, null);
        for (int i = 0; i < moduleUISprites.Length; i++)
        {
            Sprite sprite = moduleUISprites[i];

            if (sprite.name == "ModuleUI_Input_SpaceBar")
                SpaceBarSprite = sprite;
            else if (sprite.name == "ModuleUI_MLB")
                MLBSprite = sprite;
            else if (sprite.name == "ModuleUI_MRB")
                MRBSprite = sprite;

            else if (sprite.name == "ModuleUI_13s_X")
                CvtMaterialConditionIcon.TypeBase = sprite;
            else if (sprite.name == "ModuleUI_13s_O")
                CvtMaterialConditionIcon.TypeSpecial = sprite;
        }

        #endregion

        #region Enemy

        string enemyPath = path + "Enemy/";
        EnemyPhaseSpriteArr = new Sprite[3]; // 3
        Sprite[] enemySprites = GetAsset_Arr<Sprite>(enemyPath, "Enemy00_000");
        for (int i = 0; i < enemySprites.Length; i++)
        {
            Sprite sprite = enemySprites[i];

            if (Get_InSpriteName(sprite, "Enemy_Icon_BossPhase_", out int index0))
                EnemyPhaseSpriteArr[index0] = sprite;
        }

        #endregion

        #region Building

        string buildingPath = path + "Building/";
        PrisonRateIconArr = new Sprite[5]; // 5
        Sprite[] building000Sprites = GetAsset_Arr<Sprite>(buildingPath, "Building_000");
        for (int i = 0; i < building000Sprites.Length; i++)
        {
            Sprite sprite = building000Sprites[i];

            if (sprite.name == "Building000_Durablity_Frame")
                BuildingDurFrame = sprite;
            else if (sprite.name == "Building000_Durablity_Inner")
                BuildingDurInner = sprite;
        }

        #endregion

        #region Color

        ColorUtility.TryParseHtmlString("#C388FF", out Color lockedClr);
        LockedClr = lockedClr;
        ColorUtility.TryParseHtmlString("#FFFFFF", out Color unlockedClr);
        UnlockedClr = unlockedClr;

        #endregion
    }

    // Get
    public Sprite Get_BossPhaseSprite(int _ID) => EnemyPhaseSpriteArr[_ID];

    #endregion
    #region Static (Material)

    // Value
    [HideInInspector] private Dictionary<string, Material> StaticMaterialDict;

    // Offset
    private void Offset_Material_Static()
    {
        string path = "Material/";
        string modulePath = path + "Module/";
        string enemyPath = path + "Enemy/";
        string buildPath = path + "Build/";

        StaticMaterialDict = new Dictionary<string, Material>()
        {
            { "Module_Explosion", GetAsset<Material>(modulePath, "Module_000_Explosion") },
            { "Module_Hitted", GetAsset<Material>(modulePath, "Module_000_Hitted") },

            { "Enemy_Explosion", GetAsset<Material>(enemyPath, "Enemy00_000") },

            { "Build_Durablity", GetAsset<Material>(buildPath, "Build_000") },
            { "Build_PrisonOff", GetAsset<Material>(buildPath, "Build_001") },
            { "Build_PrisonOn", GetAsset<Material>(buildPath, "Build_002") }
        };

    }

    // Get
    public Material Get_ModuleMaterial(string _Name) => StaticMaterialDict[$"Module_{_Name}"];
    public Material Get_EnemyMaterial(string _Name) => StaticMaterialDict[$"Enemy_{_Name}"];
    public Material Get_BuildMaterial(string _Name) => StaticMaterialDict[$"Build_{_Name}"];
    public CoupleData<Material> Get_CoupleBuildMaterial(string _BaseName, string _SpecialName)
        => new CoupleData<Material>(Get_BuildMaterial(_BaseName), Get_BuildMaterial(_SpecialName));

    #endregion
    #region Static (Anim)

    // Value
    [HideInInspector] public AnimationClip ExplosionAC { get; private set; }
    [HideInInspector] private AnimationClip[] AttributeExplosionACArr;

    // Offset
    private void Offset_Anim_Static()
    {
        string path = "Anim/";

        string explosionPath = path + "Explosion/";

        AttributeExplosionACArr = new AnimationClip[4];
        AttributeExplosionACArr[0] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Fire");
        AttributeExplosionACArr[1] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Cold");
        AttributeExplosionACArr[2] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Electicity");
        AttributeExplosionACArr[3] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Corrosion");
    }

    // Get
    public AnimationClip Get_AttributeExplosionAC(int _ID) => AttributeExplosionACArr[_ID];

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
            string imgId = stringList[i][3];
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
            DialogueElement cutsceneElement = DialogueElement_Data[GameManager.LanguageID][IDs[i]];

            result.Add(cutsceneElement);
        }

        return result;
    }

    #endregion
    #region Dialogue (Sprite)

    // Value
    [HideInInspector] private Dictionary<string, Sprite> DialogueCharSprite_Data;

    // Offset
    private void Offset_Sprite_Dialogue()
    {
        string path = "Sprite/UI/Dialogue/";

        DialogueCharSprite_Data = new Dictionary<string, Sprite>();
        Sprite[] sprites = GetAsset_Arr<Sprite>(path, "CharacterSet_000");
        for (int i = 0; i < sprites.Length; i++)
            DialogueCharSprite_Data.Add(sprites[i].name, sprites[i]);
    }

    // Get
    public Sprite Get_DialogueCharImg(string _ID) => DialogueCharSprite_Data[_ID];

    #endregion

    #region Info (CSV)

    // Value
    [HideInInspector] private WordSet_Just InfoName_Data;
    [HideInInspector] private WordSet_Just[] InfoDetail_Data;

    // Offset
    private void Offset_CSV_Info()
    {
        string path = "CSV/Info/";

        InfoName_Data = GetAsset_WordData(path, "InfoName_CSV");
        int amount = InfoName_Data.Get_Amount();
        InfoDetail_Data = GetAsset_WordDataArr_ForParentID(path, "InfoDetail_CSV", amount);
    }

    public string Get_InfoName(int _ID) => InfoName_Data.Get_Word(_ID);
    public WordSet_Just Get_InfoDetail(int _ID) => InfoDetail_Data[_ID];

    #endregion

    #region Item - Module (CSV)

    // Value
    [HideInInspector] private ModuleBaseData[] ModuleBaseList_Data;

    // 모듈
    [HideInInspector] private WordSet_Just ModuleItemName_Data;
    [HideInInspector] private WordSet_Just MainChipName_Data;

    [HideInInspector] private WordSet_Just ModuleItemDesc_Data;
    [HideInInspector] private WordSet_Just ModuleItemEquipDesc_Data;

    [HideInInspector] private WordSet_Just[] MainChipDesc_Data;
    [HideInInspector] private WordSet_Just MainChipAllyDesc_Data;

    // Offset
    private void Offset_CSV_Module()
    {
        string path = "CSV/Module/";
        ModuleBaseList_Data = GetAsset_ModuleBaseData(path, "Module_CSV");

        ModuleItemName_Data = GetAsset_WordData(path, "ModuleName_CSV");
        MainChipName_Data = GetAsset_WordData(path, "MainChipName_CSV");

        ModuleItemDesc_Data = GetAsset_WordData(path, "ModuleDesc_CSV");
        ModuleItemEquipDesc_Data = GetAsset_WordData(path, "ModuleEquipDesc_CSV");
        MainChipDesc_Data = GetAsset_WordDataArr_ForParentID(path, "MainChipDesc_CSV", MainChipName_Data.Get_Amount());
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
        MainChipData[] data = new MainChipData[MainChipName_Data.Get_Amount()];
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
    [HideInInspector] private Sprite[] DescRankIconArr;

    // Offset
    private void Offset_Sprite_ModuleItem()
    {
        string path = "Sprite/UI/MU/";
        ModuleItemSprite_Data = GetAsset_Arr<Sprite>(path, "MUItemUI_000");
        ModuleSynhronySpritet_Data = GetAsset_Arr<Sprite>(path, "MUSynchronyUI_000");

        RankIconArr = new Sprite[5]; // 5
        DescRankIconArr = new Sprite[5]; // 5

        Sprite[] allMUUI = GetAsset_Arr<Sprite>(path, "MUUI_000");
        for (int i = 0; i < allMUUI.Length; i++)
        {
            Sprite sprite = allMUUI[i];

            if (Get_InSpriteName(sprite, "MUUI_Rank_", out int index0))
                RankIconArr[index0 - 1] = sprite;
            else if (Get_InSpriteName(sprite, "MUUI_DescRank_", out int index1))
                DescRankIconArr[index1 - 1] = sprite;
            
        }
    }

    // Get
    public Sprite Get_RankIcon(int _Rank) => RankIconArr[_Rank - 1];
    public Sprite Get_DescRankIcon(int _Rank) => DescRankIconArr[_Rank - 1];

    #endregion
    #region Item - Module (Prefab)

    // Value
    [HideInInspector] private GameObject InventoryItemPrefab;
    [HideInInspector] private GameObject InventorySlotPrefab;

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
    #region Item - Module (Anim)

    // Value
    [HideInInspector] private AnimationClip[] ModuleOutlineACs;

    // Offset
    private void Offset_Anim_Module()
    {
        string path = "Anim/";

        string modulePath = path + "Module/";

        ModuleOutlineACs = new AnimationClip[5]; // 5
        for (int i = 0; i < ModuleOutlineACs.Length; i++)
            ModuleOutlineACs[i] = GetAsset<AnimationClip>(modulePath, $"Clip_ModuleItemOutline_R{i + 1}");
    }

    // Get
    public AnimationClip Get_ModuleOutlineAC(int _Rank) => ModuleOutlineACs[_Rank];

    #endregion

    #region Item - Keycard (Anim & Color)

    // Value
    [HideInInspector] public AnimationClip KeycardOutlineAC { get; private set; }
    [HideInInspector] public Color[] KeycardOutlineColorArr;

    // Offset
    private void Offset_Anim_Keycard()
    {
        string path = "Anim/";

        string modulePath = path + "Module/";

        KeycardOutlineAC = GetAsset<AnimationClip>(modulePath, "Clip_KeycardOutline");
        KeycardOutlineColorArr = new Color[5];
        ColorUtility.TryParseHtmlString("#FF727F", out KeycardOutlineColorArr[0]);
        ColorUtility.TryParseHtmlString("#FFF49B", out KeycardOutlineColorArr[1]);
        ColorUtility.TryParseHtmlString("#A0FFFC", out KeycardOutlineColorArr[2]);
        ColorUtility.TryParseHtmlString("#A0FF99", out KeycardOutlineColorArr[3]);
        ColorUtility.TryParseHtmlString("#99FFD0", out KeycardOutlineColorArr[4]);
    }

    // Get
    public Color Get_KeycardColor(int _ID) => KeycardOutlineColorArr[_ID];

    #endregion

    #region Item - Core (Sprite)

    // Value
    [HideInInspector] private Dictionary<int, Sprite> CoreSpriteDict;

    // Offset
    private void Offset_Sprite_Core()
    {
        string path = "Sprite/";

        string modulePath = path + "Module/";
        CoreSpriteDict = new Dictionary<int, Sprite>();
        Sprite[] moduleSprites = GetAsset_Arr<Sprite>(modulePath, "Module_000");
        for (int i = 0; i < moduleSprites.Length; i++)
        {
            Sprite sprite = moduleSprites[i];

            if (sprite.name == "Module_ProtoCore")
                CoreSpriteDict.Add(1, sprite);
            else if (sprite.name == "Module_EtherCore")
                CoreSpriteDict.Add(2, sprite);
            else if (sprite.name == "Module_OriginCore")
                CoreSpriteDict.Add(3, sprite);
        }
    }

    // Get
    public Sprite Get_CoreSprite(int _ID) => CoreSpriteDict[_ID];

    #endregion
    #region Item - Core (Anim)

    // Value
    [SerializeField] public AnimationClip CoreOutlineAC { get; private set; }

    // Offset
    private void Offset_Anim_Core()
    {
        string path = "Anim/";

        string modulePath = path + "Module/";

        CoreOutlineAC = GetAsset<AnimationClip>(modulePath, "Clip_CoreOutline");
    }

    // Get

    #endregion

    #region AllyCard (CSV)

    // Value
    [HideInInspector] private AllyCardBaseData[] StrikeTeam_AllyCard_Data;
    [HideInInspector] private AllyCardBaseData[] UplinkTeam_AllyCard_Data;
    [HideInInspector] private AllyCardBaseData[] NeoTeam_AllyCard_Data;


    [HideInInspector] private WordSet_Just StrikeTeam_AllyCardName_Data;
    [HideInInspector] private WordSet_Just UplinkTeam_AllyCardName_Data;
    [HideInInspector] private WordSet_Just NeoTeam_AllyCardName_Data;

    [HideInInspector] private WordSet_Just StrikeTeam_AllyCardDesc_Data;
    [HideInInspector] private WordSet_Just UplinkTeam_AllyCardDesc_Data;
    [HideInInspector] private WordSet_Just NeoTeam_AllyCardDesc_Data;

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

    public AllyCardData[] Get_Team_AllAllyCardData(AllyCardBaseData[] _Data, WordSet_Just _NameWord, WordSet_Just _DescWord)
    {
        List<AllyCardData> result = new List<AllyCardData>();
        for (int i = 0; i < _Data.Length; i++)
            result.Add(new AllyCardData(_Data[i], _NameWord.Get_Word(i), _DescWord.Get_Word(i)));

        return result.ToArray();
    }

    #endregion
    #region AllyCard (Sprite & Color)

    // Value
    [HideInInspector] private Sprite[][] AllyCardIcon_Data;

    [HideInInspector] private static readonly int STIconAmount = 1;
    [HideInInspector] private static readonly int UTIconAmount = 1;
    [HideInInspector] private static readonly int NTIconAmount = 1;

    [HideInInspector] private Sprite[] AllyCardFrameArr;
    [HideInInspector] private Sprite[] AllyCardLightArr;
    [HideInInspector] private Sprite[] AllyCardBGArr;

    [HideInInspector] private Color[] AllyCardColorArr;
    [HideInInspector] public Sprite AllyNullIcon { get; private set; }

    [HideInInspector] private Sprite[] KeyCardSpriteArr;

    // Offset
    private void Offset_Sprite_AllyCard()
    {
        string path = "Sprite/";

        AllyCardIcon_Data = new Sprite[][]
        {
            GetAsset_AllyCardIcon(STIconAmount, "ST"),
            GetAsset_AllyCardIcon(UTIconAmount, "UT"),
            GetAsset_AllyCardIcon(NTIconAmount, "NT")
        };

        string allyCardPath = path + "UI/Ally/";

        Sprite[] allyCardFrameSprites = GetAsset_Arr<Sprite>(allyCardPath, "AllyCardFrame_000");
        AllyCardFrameArr = new Sprite[6]; // 6
        AllyCardLightArr = new Sprite[6];
        AllyCardBGArr = new Sprite[6];
        for (int i = 0; i < allyCardFrameSprites.Length; i++)
        {
            Sprite sprite = allyCardFrameSprites[i];

            if (Get_InSpriteName(sprite, "AllyCardFrame_000_Frame_", out int indexf))
                AllyCardFrameArr[indexf] = sprite;
            else if (Get_InSpriteName(sprite, "AllyCardFrame_000_Light_", out int indexl))
                AllyCardLightArr[indexl] = sprite;
            else if (Get_InSpriteName(sprite, "AllyCardFrame_000_BG_", out int indexB))
                AllyCardBGArr[indexB] = sprite;
        }


        AllyCardColorArr = new Color[6]; // Keycard Color: 6
        ColorUtility.TryParseHtmlString("#FFFFFF", out AllyCardColorArr[0]);
        ColorUtility.TryParseHtmlString("#D4FACA", out AllyCardColorArr[1]);
        ColorUtility.TryParseHtmlString("#64F9F8", out AllyCardColorArr[2]);
        ColorUtility.TryParseHtmlString("#B366FD", out AllyCardColorArr[3]);
        ColorUtility.TryParseHtmlString("#FE4C31", out AllyCardColorArr[4]);
        ColorUtility.TryParseHtmlString("#FFFFE1", out AllyCardColorArr[5]);

        KeyCardSpriteArr = new Sprite[5]; // Keycard: 5
        string moduleUiPath = path + "UI/ModuleUI/";

        Sprite[] moduleUiSprites = GetAsset_Arr<Sprite>(moduleUiPath, "ModuleUI_000");
        for (int i = 0; i < moduleUiSprites.Length; i++)
        {
            Sprite sprite = moduleUiSprites[i];

            if (sprite.name == "ModuleUI_Icon_NullCard")
                AllyNullIcon = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_KeyCard_", out string s))
            {
                switch (s)
                {
                    case "Boss": KeyCardSpriteArr[0] = sprite; break;
                    case "Vault": KeyCardSpriteArr[1] = sprite; break;
                    case "Prison": KeyCardSpriteArr[2] = sprite; break;
                    case "Shop": KeyCardSpriteArr[3] = sprite; break;
                    case "AllyShop": KeyCardSpriteArr[4] = sprite; break;

                    default: break;
                }
            }
        }
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

    public Sprite Get_AllyCardFrame(int _Rank) => AllyCardFrameArr[_Rank];
    public Sprite Get_AllyCardLight(int _Rank) => AllyCardLightArr[_Rank];
    public Sprite Get_AllyCardBG(int _Rank) => AllyCardBGArr[_Rank];

    public Color Get_AllyCardColor(int _Rank) => AllyCardColorArr[_Rank];
    public Sprite Get_KeyCardSprite(int _ID) => KeyCardSpriteArr[_ID];
    public int Get_KeycardAmount() => KeyCardSpriteArr.Length;

    #endregion

    #region Ally (Sprite)

    // Value
    private static readonly string[] directionOrder = new string[] { "UL", "U", "UR", "R", "DR", "D", "DL", "L" };
    [HideInInspector] private Sprite[] AllySprite_Data;

    [HideInInspector] private CoupleData<Sprite> StrikeTeamIcon = new CoupleData<Sprite>(null, null);
    [HideInInspector] private CoupleData<Sprite> UplinkTeamIcon = new CoupleData<Sprite>(null, null);
    [HideInInspector] private CoupleData<Sprite> NeoTeamIcon = new CoupleData<Sprite>(null, null);

    [HideInInspector] public PrisonAllySprite StrikeTeamAllySprites { get; private set; }
    [HideInInspector] public PrisonAllySprite UplinkTeamAllySprites { get; private set; }
    [HideInInspector] public PrisonAllySprite NeoTeamAllySprites { get; private set; }

    // Offset
    private void Offset_Sprite_Ally()
    {
        string path = "Sprite/";

        string allyPath = path + "Ally/";

        AllySprite_Data = GetAsset_Arr<Sprite>(allyPath, "Ally_001");

        string buildingPath = path + "Building/";
        Sprite[] building000Sprites = GetAsset_Arr<Sprite>(buildingPath, "Building_000");
        for (int i = 0; i < building000Sprites.Length; i++)
        {
            Sprite sprite = building000Sprites[i];

            if (Get_InSpriteName(sprite, StrikeTeamIcon, "Building000_StrikeTeamMark_", "Small", "Big"))
                continue;
            else if (Get_InSpriteName(sprite, UplinkTeamIcon, "Building000_UplinkTeamMark_", "Small", "Big"))
                continue;
            else if (Get_InSpriteName(sprite, NeoTeamIcon, "Building000_NeoTeamMark_", "Small", "Big"))
                continue;
        }

        StrikeTeamAllySprites = new PrisonAllySprite();
        UplinkTeamAllySprites = new PrisonAllySprite();
        NeoTeamAllySprites = new PrisonAllySprite();
        Sprite[] ally000Sprites = GetAsset_Arr<Sprite>(allyPath, "Ally_000");
        for (int i = 0; i < ally000Sprites.Length; i++)
        {
            Sprite sprite = ally000Sprites[i];

            if (Get_InSpriteName(sprite, StrikeTeamAllySprites, "Ally000_ST_InPrison_"))
                continue;
            if (Get_InSpriteName(sprite, UplinkTeamAllySprites, "Ally000_UT_InPrison_"))
                continue;
            if (Get_InSpriteName(sprite, NeoTeamAllySprites, "Ally000_NT_InPrison_"))
                continue;
        }
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


    public Sprite Get_STPrisonIcon(bool _IsBase) => StrikeTeamIcon.Get_Base(_IsBase);
    public Sprite Get_UTPrisonIcon(bool _IsBase) => UplinkTeamIcon.Get_Base(_IsBase);
    public Sprite Get_NTPrisonIcon(bool _IsBase) => NeoTeamIcon.Get_Base(_IsBase);


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
    [HideInInspector] private WordSet_Just RequestName_Data;
    [HideInInspector] private WordSet_Just RequestCompleteDesc_Data;
    [HideInInspector] private WordSet_Just RequestFailDesc_Data;

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
    #region AllyRequest (Sprite)

    // Value
    [HideInInspector] private Sprite[] RequestRankSpriteList;
    [HideInInspector] private Dictionary<string, Sprite> RequestRewardDict;

    // Offset
    private void Offset_Sprite_AllyRequest()
    {
        string path = "Sprite/";

        string moduleUIPath = path + "UI/ModuleUI/";
        Sprite[] moduleUISprites = GetAsset_Arr<Sprite>(moduleUIPath, "ModuleUI_000");
        RequestRankSpriteList = new Sprite[5];
        RequestRewardDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < moduleUISprites.Length; i++)
        {
            Sprite sprite = moduleUISprites[i];

            if (Get_InSpriteName(sprite, "ModuleUI_RequestRank_", out int index))
                RequestRankSpriteList[index] = sprite;

            else if (sprite.name == "ModuleUI_13s_BC")
                RequestRewardDict.Add("BC", sprite);
            else if (sprite.name == "ModuleUI_13s_Credit")
                RequestRewardDict.Add("Credit", sprite);
            else if (sprite.name == "ModuleUI_13s_EP")
                RequestRewardDict.Add("EP", sprite);
        }

    }

    // Get
    public Sprite Get_AllyRequestRank(int _Rank) => RequestRankSpriteList[_Rank];
    public Sprite Get_AllyRequestReward(string _Type) => RequestRewardDict[_Type];

    #endregion

    #region Map (CSV)

    // Value
    [HideInInspector] private Dictionary<int, MapNextIndex> MapNextIndex_Data;

    [HideInInspector] private WordSet_Just MapName_Data;
    [HideInInspector] private WordSet_Just MapDesc_Data;


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
    #region Map (Sprite & Material)

    // Value
    [HideInInspector] private int EachKindOfMapAmount = 2;

    [HideInInspector] private MapReso LobbyMapReso;

    [HideInInspector] public static int KindOfMapAmount = 2;
    [HideInInspector] private MapReso[] StageMapReso;

    [HideInInspector] private MapReso PassageMapReso;

    [HideInInspector] private int KindOfFieldObjType = 3;
    [HideInInspector] private Sprite[][][] MapFieldObjList_Data;

    [HideInInspector] private Material[] PassageMiddleMaterialArr;

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
    private void Offset_Material_Map()
    {
        string path = "Material/";

        string mapPassagePath = path + "Map/MapPassage/";
        PassageMiddleMaterialArr = new Material[1];
        for (int i = 0; i < PassageMiddleMaterialArr.Length; i++)
            PassageMiddleMaterialArr[i] = GetAsset<Material>(mapPassagePath, $"MapPassage_{DevTool.Get_LengthString(i, 3)}");

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

    public Material Get_PassageMiddleMaterial(int _Idx) => PassageMiddleMaterialArr[_Idx];

    #endregion
    #region Map (Prefab)

    // Value
    [HideInInspector] public GameObject LobbyRoomRulePrefab { get; private set; } // Lobby
    [HideInInspector] public GameObject[] SRoomPrefabList { get; private set; }
    [HideInInspector] public GameObject LobbyEntranceRoomRulePrefab { get; private set; }

    [HideInInspector] public GameObject StartRoomRulePrefab { get; private set; }  // Start

    [HideInInspector] public GameObject[] RoomPrefabArr { get; private set; } // Room
    [HideInInspector] public GameObject[] RoomDesignatedPrefabArr { get; private set; }

    [HideInInspector] public GameObject[] RoomRulePrefabArr { get; private set; } // Rule
    [HideInInspector] public GameObject[] RoomRuleEntrancePrefabArr { get; private set; }
    [HideInInspector] public GameObject[] RoomRuleVaultPrefabArr { get; private set; }
    [HideInInspector] public GameObject[] RoomRuleShopPrefabArr { get; private set; }
    [HideInInspector] public GameObject[] RoomRuleAllyShopPrefabArr { get; private set; }
    [HideInInspector] public GameObject[] RoomRulePrisonPrefabArr { get; private set; }


    [SerializeField] public GameObject PassageRoomPrefab{ get; private set; } // Passage // Room
    [SerializeField] public GameObject PassageRulePrefab { get; private set; } // Rule

    [HideInInspector] private GameObject[] FieldObjArray;

    // Offset
    private void Offset_Prefab_Map()
    {
        string mapPath = "Prefab/Map/";
        string roomPath = mapPath + "Rooms/";

        // Dict
        Dictionary<string, Action<GameObject>> prefabDict = new Dictionary<string, Action<GameObject>>
        {
            { "R00_LobbyR00", go => LobbyRoomRulePrefab = go },
            { "RS00_EntranceR00", go => LobbyEntranceRoomRulePrefab = go },
            { "R00_StartR00", go => StartRoomRulePrefab = go },
            { "RP01", go => PassageRoomPrefab = go },
            { "R01_PassageR00", go => PassageRulePrefab = go }
        };

        // 지역 복사본 (클로즈 캡쳐 주의)
        SRoomPrefabList = new GameObject[1]; // Small: 1
        for (int i = 0; i < SRoomPrefabList.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"RS{DevTool.Get_LengthString(idx, 2)}", go => SRoomPrefabList[idx] = go);
        }

        RoomPrefabArr = new GameObject[8]; // Room: 8
        for (int i = 0; i < RoomPrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R{DevTool.Get_LengthString(idx, 2)}", go => RoomPrefabArr[idx] = go);
        }

        RoomDesignatedPrefabArr = new GameObject[3]; // Room - Designated: 3
        for (int i = 0; i < RoomDesignatedPrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R00_EliteEnemyR{DevTool.Get_LengthString(idx, 2)}", go => RoomDesignatedPrefabArr[idx] = go);
        }

        int[] rAmount = new int[] { /*0*/5, /*1*/2, /*2*/2, /*3*/2, /*4*/2, /*5*/2, /*6*/2, /*7*/2 };
        RoomRulePrefabArr = new GameObject[19]; // Rule - Base: 19
        int roomIdx = 0;
        int ruleIdx = 0;
        int orderIdx = 0;
        for (int i = 0; i < rAmount.Length; i++)
        {
            for (int j = 0; j < rAmount[i]; j++)
            {
                int idx = roomIdx;
                int idx2 = ruleIdx;
                int order = orderIdx;
                prefabDict.Add($"R{DevTool.Get_LengthString(idx, 2)}_RR{DevTool.Get_LengthString(idx2, 2)}", go => RoomRulePrefabArr[order] = go);
                ruleIdx++;
                orderIdx++;
            }
            ruleIdx = 0;
            roomIdx++;
        }

        RoomRuleEntrancePrefabArr = new GameObject[2]; // Rule - Entrance: 2
        for (int i = 0; i < RoomRuleEntrancePrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R03_EntranceR{DevTool.Get_LengthString(idx, 2)}", go => RoomRuleEntrancePrefabArr[idx] = go);
        }

        RoomRuleVaultPrefabArr = new GameObject[1]; // Rule - Vault: 1
        for (int i = 0; i < RoomRuleVaultPrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R00_VaultR{DevTool.Get_LengthString(idx, 2)}", go => RoomRuleVaultPrefabArr[idx] = go);
        }

        RoomRuleShopPrefabArr = new GameObject[1]; // Rule - Shop: 1
        for (int i = 0; i < RoomRuleShopPrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R00_ShopR{DevTool.Get_LengthString(idx, 2)}", go => RoomRuleShopPrefabArr[idx] = go);
        }

        RoomRuleAllyShopPrefabArr = new GameObject[1]; // Rule - Ally Shop: 1
        for (int i = 0; i < RoomRuleAllyShopPrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R00_AllyShopR{DevTool.Get_LengthString(idx, 2)}", go => RoomRuleAllyShopPrefabArr[idx] = go);
        }

        RoomRulePrisonPrefabArr = new GameObject[1]; // Rule - Prison: 1
        for (int i = 0; i < RoomRulePrisonPrefabArr.Length; i++)
        {
            int idx = i;
            prefabDict.Add($"R00_PrisonR{DevTool.Get_LengthString(idx, 2)}", go => RoomRulePrisonPrefabArr[idx] = go);
        }

        // Init
        GameObject[] allRoomReso = GetAsset_Arr<GameObject>(roomPath);
        for (int i = 0; i < allRoomReso.Length; i++)
        {
            GameObject prefab = allRoomReso[i];
            if (prefabDict.TryGetValue(prefab.name, out Action<GameObject> set))
            {
                set(prefab);
            }
        }

        // Field Obj
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
    [HideInInspector] private WordSet_Just[] SkillName_Data;
    [HideInInspector] private WordSet_Just[] SkillDesc_Data;

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
    [HideInInspector] private WordSet_Just TunerStateName_Data;

    // Offset
    private void Offset_CSV_Tuner()
    {
        string path = "CSV/Tuner/";
        TunerStateName_Data = GetAsset_WordData(path, "TunerStateName_CSV");
    }

    // Get
    public string Get_TunerDescName(int _Index) => TunerStateName_Data.Get_Word(_Index);

    #endregion

    #region Puzzle NSC (Sprite & Color)

    // Value
    [HideInInspector] public Sprite[] NSC_NumSpriteArr { get; private set; }
    [HideInInspector] public Sprite[] NSC_ShapeSpriteArr { get; private set; }
    [HideInInspector] public Color[] NSC_ColorArr { get; private set; }
    [HideInInspector] public Sprite NSC_ColorSprite { get; private set; }


    [HideInInspector] private NSCAnswerSpriteSet[] AllNSCAnswerSpriteSet;

    // Offset
    private void Offset_Sprite_PuzzleNSC()
    {
        string path = "Sprite/";

        string moduleUIPath = path + "UI/ModuleUI/";
        NSC_NumSpriteArr = new Sprite[5]; //5
        NSC_ShapeSpriteArr = new Sprite[5];
        AllNSCAnswerSpriteSet = new NSCAnswerSpriteSet[5];
        for (int i = 0; i < AllNSCAnswerSpriteSet.Length; i++)
        {
            AllNSCAnswerSpriteSet[i] = new NSCAnswerSpriteSet();
            AllNSCAnswerSpriteSet[i].ShapeIndex = i;
            AllNSCAnswerSpriteSet[i].AllAnswerSet = new Sprite[5];
        }
        Sprite[] moduleUISprites = GetAsset_Arr<Sprite>(moduleUIPath, "ModuleUI_000");
        for (int i = 0; i < moduleUISprites.Length; i++)
        {
            Sprite sprite = moduleUISprites[i];

            if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_Num_", out int index))
                NSC_NumSpriteArr[index - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_Shape_", out string indexS))
            {
                switch (indexS)
                {
                    case "Circle": NSC_ShapeSpriteArr[0] = sprite; break;
                    case "Triangle": NSC_ShapeSpriteArr[1] = sprite; break;
                    case "Rectangle": NSC_ShapeSpriteArr[2] = sprite; break;
                    case "X": NSC_ShapeSpriteArr[3] = sprite; break;
                    case "HalfCircle": NSC_ShapeSpriteArr[4] = sprite; break;
                    default: break;
                }
            }
            else if (sprite.name == "ModuleUI_RollSelect_Color")
                NSC_ColorSprite = sprite;

            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_C_", out int index_a_c))
                AllNSCAnswerSpriteSet[0].AllAnswerSet[index_a_c - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_T_", out int index_a_t))
                AllNSCAnswerSpriteSet[1].AllAnswerSet[index_a_t - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_R_", out int index_a_r))
                AllNSCAnswerSpriteSet[2].AllAnswerSet[index_a_r - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_X_", out int index_a_x))
                AllNSCAnswerSpriteSet[3].AllAnswerSet[index_a_x - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_H_", out int index_a_h))
                AllNSCAnswerSpriteSet[4].AllAnswerSet[index_a_h - 1] = sprite;
        }


        NSC_ColorArr = new Color[5];
        ColorUtility.TryParseHtmlString("#FF0000", out NSC_ColorArr[0]);
        ColorUtility.TryParseHtmlString("#FFFF00", out NSC_ColorArr[1]);
        ColorUtility.TryParseHtmlString("#00FF00", out NSC_ColorArr[2]);
        ColorUtility.TryParseHtmlString("#0000FF", out NSC_ColorArr[3]);
        ColorUtility.TryParseHtmlString("#FF00FF", out NSC_ColorArr[4]);
    }

    // Get
    public Sprite Get_NSCAnswerSprite(int _ShapeIndex, int _NumIndex) => AllNSCAnswerSpriteSet[_ShapeIndex].AllAnswerSet[_NumIndex];

    #endregion
    #region Build (Prefab)

    // Value
    [HideInInspector] public GameObject BUShopPrefab { get; private set; } // Player Shop
    [HideInInspector] public GameObject MUShopPrefab { get; private set; }

    [HideInInspector] public GameObject ABUShopPrefab { get; private set; }  // Ally Shop
    [HideInInspector] public GameObject AMUShopPrefab { get; private set; }

    [HideInInspector] public GameObject[] VaultPrefabArr { get; private set; } // Value
    [HideInInspector] public GameObject[] PrisonPrefabArr { get; private set; } // Prison


    [HideInInspector] public GameObject RepairOperatorPrefab { get; private set; }  // Oper

    [HideInInspector] public GameObject VaultRerollOperatorPrefab { get; private set; }
    [HideInInspector] public GameObject VaultUpgradeOperatorPrefab { get; private set; }

    [HideInInspector] public GameObject PrisonPayOperatorPrefab { get; private set; }
    [HideInInspector] public GameObject PrisonPuzzleOperatorPrefab { get; private set; }

    // Offset
    private void Offset_Prefab_Build()
    {
        string shop = "Shop";
        string vault = "Vault";
        string prison = "Prison";
        string oper = "Operator";

        string path = "Prefab/Build/MainGame/";

        string playerPath = path + shop + "/";
        BUShopPrefab = GetAsset<GameObject>(playerPath, $"BaseUpgrade{shop}");
        MUShopPrefab = GetAsset<GameObject>(playerPath, $"ModuleUpgrade{shop}");

        string allyPath = path + "Ally" + shop + "/";
        ABUShopPrefab = GetAsset<GameObject>(allyPath, $"AllyBaseUpgrade{shop}");
        AMUShopPrefab = GetAsset<GameObject>(allyPath, $"AllyModuleUpgrade{shop}");

        string vaultPath = path + vault + "/";
        VaultPrefabArr = new GameObject[3]; // 3
        VaultPrefabArr[0] = GetAsset<GameObject>(vaultPath, $"BetteryShard{vault}");
        VaultPrefabArr[1] = GetAsset<GameObject>(vaultPath, $"Joule{vault}");
        VaultPrefabArr[2] = GetAsset<GameObject>(vaultPath, $"Module{vault}");

        string prisonPath = path + prison + "/";
        PrisonPrefabArr = new GameObject[3]; // 3
        PrisonPrefabArr[0] = GetAsset<GameObject>(prisonPath, $"{prison}_StrikeTeam");
        PrisonPrefabArr[1] = GetAsset<GameObject>(prisonPath, $"{prison}_UplinkTeam");
        PrisonPrefabArr[2] = GetAsset<GameObject>(prisonPath, $"{prison}_NeoTeam");


        string operPath = path + oper + "/";
        RepairOperatorPrefab = GetAsset<GameObject>(operPath, $"Repair{oper}");
        string vaultOperPath = operPath + vault + "/";
        VaultRerollOperatorPrefab = GetAsset<GameObject>(vaultOperPath, $"VaultReroll{oper}");
        VaultUpgradeOperatorPrefab = GetAsset<GameObject>(vaultOperPath, $"VaultUpgrade{oper}");
        string prisonOperPath = operPath + prison + "/";
        PrisonPayOperatorPrefab = GetAsset<GameObject>(prisonOperPath, $"PrisonPay{oper}");
        PrisonPuzzleOperatorPrefab = GetAsset<GameObject>(prisonOperPath, $"PrisonPuzzle{oper}");
    }

    #endregion

    #region Shop - BU & MU (Anim)

    // Value
    [HideInInspector] public CoupleData<AnimationClip> BUShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip BUShop_BrokenAC { get; private set; }
    [HideInInspector] public CoupleData<AnimationClip> MUShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip MUShop_BrokenAC { get; private set; }


    [HideInInspector] public AnimationClip BrokenStateAC { get; private set; }
    [HideInInspector] public CoupleData<AnimationClip> NeedChargeBettery_OnOffStateAC { get; private set; }

    // Offset
    private void Offset_Anim_PlayerShop()
    {
        string path = "Anim/";

        string shopPath = path + "Building/Shop/";

        string buPath = shopPath + "BU/";
        BUShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(buPath, "Clip_BUShop_Off"), GetAsset<AnimationClip>(buPath, "Clip_BUShop_Off"));
        BUShop_BrokenAC = GetAsset<AnimationClip>(buPath, "Clip_BUShop_Broken");

        string muPath = shopPath + "MU/";
        MUShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(muPath, "Clip_MUShop_Off"), GetAsset<AnimationClip>(muPath, "Clip_MUShop_Off"));
        MUShop_BrokenAC = GetAsset<AnimationClip>(muPath, "Clip_MUShop_Broken");

        BrokenStateAC = GetAsset<AnimationClip>(shopPath, "Clip_Broken");
        NeedChargeBettery_OnOffStateAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(shopPath, "Clip_NeedEC"), GetAsset<AnimationClip>(shopPath, "Clip_Upgrade"));
    }

    #endregion
    #region Shop - ABU & AMU (Anim)

    // Value
    [HideInInspector] public CoupleData<AnimationClip> AllyBUShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip AllyBUShop_BrokenAC { get; private set; }
    [HideInInspector] public CoupleData<AnimationClip> AllyMUShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip AllyMUShop_BrokenAC { get; private set; }

    // Offset
    private void Offset_Anim_AllyShop()
    {
        string path = "Anim/";

        string shopPath = path + "Building/AllyShop/";

        string buPath = shopPath + "BU/";
        AllyBUShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(buPath, "Clip_AllyBUShop_Off"), GetAsset<AnimationClip>(buPath, "Clip_AllyBUShop_Off"));
        AllyBUShop_BrokenAC = GetAsset<AnimationClip>(buPath, "Clip_AllyBUShop_Broken");

        string muPath = shopPath + "MU/";
        AllyMUShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(muPath, "Clip_AllyMUShop_Off"), GetAsset<AnimationClip>(muPath, "Clip_AllyMUShop_Off"));
        AllyMUShop_BrokenAC = GetAsset<AnimationClip>(muPath, "Clip_AllyMUShop_Broken");
    }

    #endregion

    #region Vault (Anim)

    // Value
    [HideInInspector] private CoupleData<AnimationClip>[] Vault_AC;
    [HideInInspector] private AnimationClip[] Vault_BrokenAC;

    [HideInInspector] public AnimationClip Vault_ModuleIconAC { get; private set; }
    [HideInInspector] public AnimationClip Vault_BSIconAC { get; private set; }
    [HideInInspector] public AnimationClip Vault_JIconAC { get; private set; }
    
    [HideInInspector] public CoupleData<AnimationClip> Vault_StateAC { get; private set; }

    // Offset
    private void Offset_Anim_Vault()
    {
        string path = "Anim/";

        string vaultPath = path + "Building/Vault/";

        Vault_AC = new CoupleData<AnimationClip>[5];
        Vault_BrokenAC = new AnimationClip[5];
        for (int i = 0; i < Vault_AC.Length; i++)
        {
            AnimationClip ac = GetAsset<AnimationClip>(vaultPath, $"Clip_Vault_G{i}");
            Vault_AC[i] = new CoupleData<AnimationClip>(ac, ac);

            Vault_BrokenAC[i] = GetAsset<AnimationClip>(vaultPath, $"Clip_Vault_G{i}_Broken");
        }

        string iconName = "Clip_Vault_Icon_";
        Vault_ModuleIconAC = GetAsset<AnimationClip>(vaultPath, $"{iconName}Module");
        Vault_BSIconAC = GetAsset<AnimationClip>(vaultPath, $"{iconName}BetteryShard");
        Vault_JIconAC = GetAsset<AnimationClip>(vaultPath, $"{iconName}Joule");

        Vault_StateAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(vaultPath, $"Clip_VaultEmpty_State"), GetAsset<AnimationClip>(vaultPath, $"Clip_Vault_State"));
    }

    // Get
    public CoupleData<AnimationClip> Get_VaultAnim(int _Grade) => Vault_AC[_Grade];
    public AnimationClip Get_VaultBrokenAnim(int _Grade) => Vault_BrokenAC[_Grade];

    #endregion

    #region Prison (Sprite)

    // Value
    [HideInInspector] private Sprite[] PrisonRateIconArr;

    // Offset
    private void Offset_Sprite_Prison()
    {
        string path = "Sprite/";
        string buildingPath = path + "Building/";
        PrisonRateIconArr = new Sprite[5]; // 5
        Sprite[] building000Sprites = GetAsset_Arr<Sprite>(buildingPath, "Building_000");
        for (int i = 0; i < building000Sprites.Length; i++)
        {
            Sprite sprite = building000Sprites[i];

            if (Get_InSpriteName(sprite, "Building000_DangerRate", out int index))
                PrisonRateIconArr[index - 1] = sprite;
        }
    }

    // Get
    public Sprite Get_PrisonRankSprite(int _ID) => PrisonRateIconArr[_ID];

    #endregion
    #region Prison (Anim)

    // Value 
    [SerializeField] public CoupleData<AnimationClip> Prison_OnOffAC { get; private set; }
    [SerializeField] public CoupleData<AnimationClip> Prison_OnOffUpsideAC { get; private set; }

    [SerializeField] public CoupleData<AnimationClip> Prison_StateAC { get; private set; }

    // Offset
    private void Offset_Anim_Prison()
    {
        string path = "Anim/";

        string prisonPath = path + "Building/Prison/";

        string prisonName = "Clip_Prison";
        string iconName = "Clip_Icon_";

        string downName = "_Downside";
        string upName = "_Upside";
        string lockName = "Lock";
        string UnlockName = "Unlock";

        Prison_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(prisonPath, $"{prisonName}{lockName}{downName}"), GetAsset<AnimationClip>(prisonPath, $"{prisonName}{UnlockName}{downName}"));
        Prison_OnOffUpsideAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(prisonPath, $"{prisonName}{lockName}{upName}"), GetAsset<AnimationClip>(prisonPath, $"{prisonName}{UnlockName}{upName}"));

        Prison_StateAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(prisonPath, $"{iconName}{lockName}"), GetAsset<AnimationClip>(prisonPath, $"{iconName}{UnlockName}"));
    }

    // Get

    #endregion

    #region Operator (Anim)

    // Value
    [SerializeField] public CoupleData<AnimationClip> Operator_OnOffAC { get; private set; }
    [SerializeField] public AnimationClip Operator_RepairAC { get; private set; }
    [SerializeField] public AnimationClip Operator_RerollAC { get; private set; }
    [SerializeField] public AnimationClip Operator_UpgradeAC { get; private set; }
    [SerializeField] public AnimationClip Operator_AllyAC { get; private set; }

    [SerializeField] public CoupleData<AnimationClip> Operator_LightAC { get; private set; }

    // Offset
    private void Offset_Anim_Operator()
    {
        string path = "Anim/";

        string OperPath = path + "Building/Operator/";

        string operName = "Clip_Operator";
        Operator_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(OperPath, $"{operName}Off"), GetAsset<AnimationClip>(OperPath, $"{operName}On"));

        string iconName = "Clip_Icon_";
        Operator_RepairAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Repair");
        Operator_RerollAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Reroll");
        Operator_UpgradeAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Upgrade");
        Operator_AllyAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Ally");

        Operator_LightAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(OperPath, $"Clip_LightOff"), GetAsset<AnimationClip>(OperPath, $"Clip_LightOn"));
    }

    #endregion

    #region Shop - Converter (Material & Anim)

    // Value
    [HideInInspector] private ConverterReso ConverterReso;

    // Offset
    private void Offset_Anim_Converter()
    {
        string materialPath = "Material/Build/";
        string animPath = "Anim/Building/Converter/";

        string converterAnimName = "Clip_Converter_";
        string converterMaterialName = "Build_003";
        ConverterReso = new ConverterReso();

        ConverterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "PremiumCredit"),
            GetAsset<Material>(materialPath, converterMaterialName)));
        ConverterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "ProtoCore"),
            GetAsset<Material>(materialPath, converterMaterialName)));
        ConverterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "EtherCore"),
            GetAsset<Material>(materialPath, converterMaterialName)));
        ConverterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "OriginCore"),
            GetAsset<Material>(materialPath, converterMaterialName)));
    }

    // Get
    public EachConverterReso Get_ConverterReso(int _ID) => ConverterReso.ConverterResoList[_ID];

    #endregion

    #region Minimap (Sprite)

    // Value
    [HideInInspector] public CoupleData<Sprite> Vault_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> Elevator_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> Shop_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> AllyShop_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> ST_Prison_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> UT_Prison_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> NT_Prison_Icon { get; private set; }

    [HideInInspector] private MinimapIcon[] MinimapIcons;

    [HideInInspector] private Dictionary<int, Sprite> StageIconDict;


    // Offset
    private void Offset_Sprite_Minimap()
    {
        string path = "Sprite/UI/HUD/";


        string mmName = "PlayerHUD_MM_";
        string mmoName = "PlayerHUD_MMO_";
        string immName = "PlayerHUD_IMM_";
        string immoName = "PlayerHUD_IMMO_";

        string mmIconName = "PlayerHUD_MM_Icon_";
        string immIconName = "PlayerHUD_IMM_Icon_";

        Sprite[] hud000S = GetAsset_Arr<Sprite>(path, "PlayerHUD_000");
        Dictionary<string, Sprite> hud000dict = new Dictionary<string, Sprite>();
        for (int i = 0; i < hud000S.Length; i++)
        {
            hud000dict.Add(hud000S[i].name, hud000S[i]);
        }

        Vault_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}Vault"], hud000dict[$"{immIconName}Vault"]);
        Elevator_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}Elevator"], hud000dict[$"{immIconName}Elevator"]);
        Shop_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}Shop"], hud000dict[$"{immIconName}Shop"]);
        AllyShop_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}AllyShop"], hud000dict[$"{immIconName}AllyShop"]);
        ST_Prison_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}ST_Prison"], hud000dict[$"{immIconName}ST_Prison"]);
        UT_Prison_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}UT_Prison"], hud000dict[$"{immIconName}UT_Prison"]);
        NT_Prison_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}NT_Prison"], hud000dict[$"{immIconName}NT_Prison"]);

        MinimapIcons = new MinimapIcon[8]; // 8

        MinimapIcons[0] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}000"], hud000dict[$"{mmoName}000"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}000"], hud000dict[$"{immoName}000"])),
            new Vector2Int[] { new Vector2Int(0, 0) }, 
            new Vector2(0.5f, 0.5f));

        MinimapIcons[1] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}001"], hud000dict[$"{mmoName}001"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}001"], hud000dict[$"{immoName}001"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) }, 
            new Vector2(0.25f, 0.5f));

        MinimapIcons[2] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}002"], hud000dict[$"{mmoName}002"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}002"], hud000dict[$"{immoName}002"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) },
            new Vector2(0.5f, 0.25f));

        MinimapIcons[3] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}003"], hud000dict[$"{mmoName}003"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}003"], hud000dict[$"{immoName}003"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
            new Vector2(0.25f, 0.25f));

        MinimapIcons[4] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}004"], hud000dict[$"{mmoName}004"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}004"], hud000dict[$"{immoName}004"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
            new Vector2(0.25f, 0.75f));

        MinimapIcons[5] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}005"], hud000dict[$"{mmoName}005"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}005"], hud000dict[$"{immoName}005"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) },
            new Vector2(0.25f, 0.25f));
        
        MinimapIcons[6] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}006"], hud000dict[$"{mmoName}006"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}006"], hud000dict[$"{immoName}006"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
            new Vector2(0.25f, 0.25f));

        MinimapIcons[7] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}007"], hud000dict[$"{mmoName}007"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}007"], hud000dict[$"{immoName}007"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) },
            new Vector2(0.25f, 0.25f));

        string stageIconName = "ModuleUI_StageIcon_";

        string moduleUIPath = "Sprite/UI/ModuleUI/";
        StageIconDict = new Dictionary<int, Sprite>();
        Sprite[] moduleUIS = GetAsset_Arr<Sprite>(moduleUIPath);
        for (int i = 0; i < moduleUIS.Length; i++)
        {
            Sprite s = moduleUIS[i];
            if (Get_InSpriteName(s, stageIconName, out int idx))
            {
                StageIconDict.Add(idx, s);
            }
            else if (Get_InSpriteName(s, stageIconName, out string idxS) && idxS == "Lobby")
            {
                StageIconDict.Add(99, s);
            }
        }
    }

    // Get
    public MinimapIcon Get_MinimapIcon(int _ID) => MinimapIcons[_ID];
    public Sprite Get_StageIcon(int _ID) => StageIconDict[_ID];

    #endregion

    #region Canvas UI (Prefab)

    // Value
    [SerializeField] public GameObject TitleLobby_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject PlayerHUD_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject BaseUpgrade_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject ModuleUpgrade_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject AllyBaseUpgrade_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject AllyModuleUpgrade_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject OutMainGame_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject InteractAnno_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject MapIntro_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject AllyCard_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject Puzzle_BoxLineConnector_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject Puzzle_NumShapeColorPassword_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject Puzzle_InOrderLocker_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject Cvt_PremiumCredit_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject Cvt_ProtoCore_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject Cvt_EtherCore_CanvasPrefab { get; private set; }
    [SerializeField] public GameObject Cvt_OriginCore_CanvasPrefab { get; private set; }

    [SerializeField] public GameObject BattleProd_CanvasPrefab { get; private set; }

    // Offset
    private void Offset_Prefab_CanvasUI()
    {
        string path = "Prefab/UI/";

        Dictionary<string, GameObject> dict = new Dictionary<string, GameObject>();
        GameObject[] prefabs = GetAsset_Arr<GameObject>(path);
        for (int i = 0; i < prefabs.Length; i++)
            dict.Add(prefabs[i].name, prefabs[i]);

        TitleLobby_CanvasPrefab = dict[Get_CanvasName("TitleLobby")];

        PlayerHUD_CanvasPrefab = dict[Get_CanvasName("PlayerInfoHUD")];

        BaseUpgrade_CanvasPrefab = dict[Get_CanvasName("BUUI")];
        ModuleUpgrade_CanvasPrefab = dict[Get_CanvasName("MUUI")];

        AllyBaseUpgrade_CanvasPrefab = dict[Get_CanvasName("ABUUI")];
        AllyModuleUpgrade_CanvasPrefab = dict[Get_CanvasName("AMUUI")];

        OutMainGame_CanvasPrefab = dict[Get_CanvasName("OutMainGame")];
        InteractAnno_CanvasPrefab = dict[Get_CanvasName("InteractAnnoHUD")];
        MapIntro_CanvasPrefab = dict[Get_CanvasName("MapIntroHUD")];

        AllyCard_CanvasPrefab = dict[Get_CanvasName("AllyUpgrade")];

        Puzzle_BoxLineConnector_CanvasPrefab = dict[Get_CanvasName("Puzzle_BoxLineConnector")];
        Puzzle_NumShapeColorPassword_CanvasPrefab = dict[Get_CanvasName("Puzzle_NumShapeColorPassword")];
        Puzzle_InOrderLocker_CanvasPrefab = dict[Get_CanvasName("Puzzle_InOrderLocker")];

        Cvt_PremiumCredit_CanvasPrefab = dict[Get_CanvasName("PremiumCreditCvt")];
        Cvt_ProtoCore_CanvasPrefab = dict[Get_CanvasName("ProtoCoreCvt")];
        Cvt_EtherCore_CanvasPrefab = dict[Get_CanvasName("EtherCoreCvt")];
        Cvt_OriginCore_CanvasPrefab = dict[Get_CanvasName("OriginCoreCvt")];

        BattleProd_CanvasPrefab = dict[Get_CanvasName("Battle")];

        string Get_CanvasName(string _Name) => "Canvas_" + _Name + "_Prefab";
    }

    #endregion

    #region GetAsset_WordData

    private WordSet_Just[] GetAsset_WordDataArr_ForParentID(string _Path, string _FileName, int _Amount)
    {
        List<WordSet_Just> result = new List<WordSet_Just>();
        for (int i = 0; i < _Amount; i++)
            result.Add(GetAsset_WordData_ForParentID(_Path, _FileName, i));

        return result.ToArray();
    }

    private WordSet_Just GetAsset_WordData_ForParentID(string _Path, string _FileName, int _TargetParentID)
    {
        Dictionary<int, WordElement_Just> elementDict = new Dictionary<int, WordElement_Just>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            if (int.Parse(stringList[i][0]) != _TargetParentID) continue;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][1]);

            for (int j = 1; j < GameManager.KindOfLanguage.Length + 1; j++)
                nameList.Add(stringList[i][j + 1]);

            elementDict.Add(id, new WordElement_Just(id, nameList.ToArray()));
        }

        return new WordSet_Just(elementDict);
    }

    private WordSet_Just GetAsset_WordData(string _Path, string _FileName)
    {
        Dictionary<int, WordElement_Just> elementDict = new Dictionary<int, WordElement_Just>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            List<string> nameList = new List<string>();
            int id = int.Parse(stringList[i][0]);

            for (int j = 0; j < GameManager.KindOfLanguage.Length; j++)
                nameList.Add(stringList[i][j + 1]);

            elementDict.Add(id, new WordElement_Just(id, nameList.ToArray()));
        }

        return new WordSet_Just(elementDict);
    }

    private WordSet_WithClr GetAsset_WordData_Clr(string _Path, string _FileName)
    {
        Dictionary<int, WordElement_WithClr> elementDict = new Dictionary<int, WordElement_WithClr>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(_Path + _FileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int id = int.Parse(stringList[i][0]);
            string clrHex = stringList[i][1];

            List<string> nameList = new List<string>();
            for (int j = 0; j < GameManager.KindOfLanguage.Length; j++)
                nameList.Add(stringList[i][j + 2]);

            elementDict.Add(id, new WordElement_WithClr(id, clrHex, nameList.ToArray()));
        }

        return new WordSet_WithClr(elementDict);
    }

    #endregion

    #region GetAsset_SpriteName

    private bool Get_InSpriteName(Sprite _Sprite, string _Name, out int _Index)
    {
        _Index = -1;
        if (_Sprite.name.Length > _Name.Length && _Sprite.name.StartsWith(_Name))
        {
            if (int.TryParse(_Sprite.name.Replace(_Name, ""), out _Index))
            {
                return true;
            }
        }
        return false;
    }

    private bool Get_InSpriteName(Sprite _Sprite, string _Name, out string _Index)
    {
        _Index = "";
        if (_Sprite.name.Length > _Name.Length && _Sprite.name.StartsWith(_Name))
        {
            _Index = _Sprite.name.Replace(_Name, "");
            return true;
        }
        return false;
    }

    private bool Get_InSpriteName(Sprite _Sprite, CoupleData<Sprite> _Data, string _Name, string _Base, string _Special)
    {
        if (Get_InSpriteName(_Sprite, _Name, out string _index))
        {
            if (_index == _Base)
            {
                _Data.TypeBase = _Sprite;
                return true;
            }  
            else if (_index == _Special)
            {
                _Data.TypeSpecial = _Sprite;
                return true;
            }
        }
        return false;
    }

    private bool Get_InSpriteName(Sprite _Sprite, PrisonAllySprite _Data, string _Name)
    {
        if (Get_InSpriteName(_Sprite, _Name, out string _index))
        {
            if (_index == "Bind")
            { 
                _Data.Bind = _Sprite;
                return true;
            }
            else if (_index == "Fall")
            { 
                _Data.Fall = _Sprite;
                return true;
            }
            else if (_index == "Stand")
            { 
                _Data.Stand = _Sprite;
                return true;
            }
            else if (_index == "Salute")
            { 
                _Data.Salute = _Sprite;
                return true;
            }
        }
        return false;
    }

    #endregion
}