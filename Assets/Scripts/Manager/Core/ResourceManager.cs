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

    private void Offset_Sprite()
    {
        Offset_Sprite_Static();
        Offset_Sprite_Cutscene();
        Offset_Sprite_Dialogue();
        Offset_Sprite_Info();
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

    public void Offset()
    {
        Offset_Other();
        Offset_CSV();
        Offset_Sprite();
        Offset_Material();
        Offset_Anim();
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
    #region Static (Sprite)

    // Value
    [HideInInspector] private Sprite[] enemyPhaseSpriteArr;
    [HideInInspector] public Sprite buildingDurFrame { get; private set; }
    [HideInInspector] public Sprite buildingDurInner { get; private set; }

    [HideInInspector] public Sprite spaceBarSprite { get; private set; }
    [HideInInspector] public Sprite mlbSprite { get; private set; }
    [HideInInspector] public Sprite mrbSprite { get; private set; }

    [HideInInspector] public Color lockedClr { get; private set; }
    [HideInInspector] public Color unlockedClr;

    [HideInInspector] public CoupleData<Sprite> cvtMaterialConditionIcon { get; private set; }

    // Offset
    private void Offset_Sprite_Static()
    {
        string path = "Sprite/";

        #region Module UI

        string moduleUIPath = path + "UI/ModuleUI/";
        Sprite[] moduleUISprites = GetAsset_Arr<Sprite>(moduleUIPath, "ModuleUI_000");
        cvtMaterialConditionIcon = new CoupleData<Sprite>(null, null);
        for (int i = 0; i < moduleUISprites.Length; i++)
        {
            Sprite sprite = moduleUISprites[i];

            if (sprite.name == "ModuleUI_Input_SpaceBar")
                spaceBarSprite = sprite;
            else if (sprite.name == "ModuleUI_MLB")
                mlbSprite = sprite;
            else if (sprite.name == "ModuleUI_MRB")
                mrbSprite = sprite;

            else if (sprite.name == "ModuleUI_13s_X")
                cvtMaterialConditionIcon.typeBase = sprite;
            else if (sprite.name == "ModuleUI_13s_O")
                cvtMaterialConditionIcon.typeSpecial = sprite;
        }

        #endregion

        #region Enemy

        string enemyPath = path + "Enemy/";
        enemyPhaseSpriteArr = new Sprite[3]; // 3
        Sprite[] enemySprites = GetAsset_Arr<Sprite>(enemyPath, "Enemy00_000");
        for (int i = 0; i < enemySprites.Length; i++)
        {
            Sprite sprite = enemySprites[i];

            if (Get_InSpriteName(sprite, "Enemy_Icon_BossPhase_", out int index0))
                enemyPhaseSpriteArr[index0] = sprite;
        }

        #endregion

        #region Building

        string buildingPath = path + "Building/";
        prisonRateIconArr = new Sprite[5]; // 5
        Sprite[] building000Sprites = GetAsset_Arr<Sprite>(buildingPath, "Building_000");
        for (int i = 0; i < building000Sprites.Length; i++)
        {
            Sprite sprite = building000Sprites[i];

            if (sprite.name == "Building000_Durablity_Frame")
                buildingDurFrame = sprite;
            else if (sprite.name == "Building000_Durablity_Inner")
                buildingDurInner = sprite;
        }

        #endregion

        #region Color

        ColorUtility.TryParseHtmlString("#C388FF", out Color lockedClr);
        this.lockedClr = lockedClr;
        ColorUtility.TryParseHtmlString("#FFFFFF", out Color unlockedClr);
        this.unlockedClr = unlockedClr;

        #endregion
    }

    // Get
    public Sprite Get_BossPhaseSprite(int id) => enemyPhaseSpriteArr[id];

    #endregion
    #region Static (Material)

    // Value
    [HideInInspector] private Dictionary<string, Material> staticMaterialDict;

    // Offset
    private void Offset_Material_Static()
    {
        string path = "Material/";
        string modulePath = path + "Module/";
        string enemyPath = path + "Enemy/";
        string buildPath = path + "Build/";

        staticMaterialDict = new Dictionary<string, Material>()
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
    public Material Get_ModuleMaterial(string name) => staticMaterialDict[$"Module_{name}"];
    public Material Get_EnemyMaterial(string name) => staticMaterialDict[$"Enemy_{name}"];
    public Material Get_BuildMaterial(string name) => staticMaterialDict[$"Build_{name}"];
    public CoupleData<Material> Get_CoupleBuildMaterial(string baseName, string specialName)
        => new CoupleData<Material>(Get_BuildMaterial(baseName), Get_BuildMaterial(specialName));

    #endregion
    #region Static (Anim)

    // Value
    [HideInInspector] public AnimationClip explosionAC { get; private set; }
    [HideInInspector] private AnimationClip[] attributeExplosionACArr;

    // Offset
    private void Offset_Anim_Static()
    {
        string path = "Anim/";

        string explosionPath = path + "Explosion/";

        explosionAC = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion");

        attributeExplosionACArr = new AnimationClip[4];
        attributeExplosionACArr[0] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Fire");
        attributeExplosionACArr[1] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Cold");
        attributeExplosionACArr[2] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Electicity");
        attributeExplosionACArr[3] = GetAsset<AnimationClip>(explosionPath, "Clip_Explosion_Corrosion");
    }

    // Get
    public AnimationClip Get_AttributeExplosionAC(int id) => attributeExplosionACArr[id];

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
    #region Cutscene (Sprite)


    [HideInInspector] private Sprite[] cutsceneSprite_Data;

    // Offset
    private void Offset_Sprite_Cutscene()
    {
        string path = "Sprite/UI/Cutscene/";
        cutsceneSprite_Data = GetAsset_Arr<Sprite>(path, "CutsceneSet_00");
    }

    // Get
    public Sprite Get_CutsceneImg(int id) => cutsceneSprite_Data[id];

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
    #region Dialogue (Sprite)

    // Value
    [HideInInspector] private Dictionary<string, Sprite> dialogueCharSprite_Data;

    // Offset
    private void Offset_Sprite_Dialogue()
    {
        string path = "Sprite/UI/Dialogue/";

        dialogueCharSprite_Data = new Dictionary<string, Sprite>();
        Sprite[] sprites = GetAsset_Arr<Sprite>(path, "CharacterSet_000");
        for (int i = 0; i < sprites.Length; i++)
            dialogueCharSprite_Data.Add(sprites[i].name, sprites[i]);
    }

    // Get
    public Sprite Get_DialogueCharImg(string id) => dialogueCharSprite_Data[id];

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
    #region Info (Sprite)

    // Value
    [HideInInspector] private Sprite[] infoSprite_Data;

    // Offset
    private void Offset_Sprite_Info()
    {
        string path = "Sprite/UI/Info/";
        infoSprite_Data = GetAsset_Arr<Sprite>(path, "Info_00");
    }

    // Get
    public Sprite Get_InfoImg(int id) => infoSprite_Data[id];

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
            data[i] = new ItemData(i, moduleItemSprite_Data[i],
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
            data[i] = new MainChipData(i, moduleSynhronySpritet_Data[i]);

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
    #region Item - Module (Sprite)

    // Value
    [HideInInspector] private Sprite[] moduleItemSprite_Data;
    [HideInInspector] private Sprite[] moduleSynhronySpritet_Data;

    [HideInInspector] private Sprite[] rankIconArr;
    [HideInInspector] private Sprite[] descRankIconArr;

    // Offset
    private void Offset_Sprite_ModuleItem()
    {
        string path = "Sprite/UI/MU/";
        moduleItemSprite_Data = GetAsset_Arr<Sprite>(path, "MUItemUI_000");
        moduleSynhronySpritet_Data = GetAsset_Arr<Sprite>(path, "MUSynchronyUI_000");

        rankIconArr = new Sprite[5]; // 5
        descRankIconArr = new Sprite[5]; // 5

        Sprite[] allMUUI = GetAsset_Arr<Sprite>(path, "MUUI_000");
        for (int i = 0; i < allMUUI.Length; i++)
        {
            Sprite sprite = allMUUI[i];

            if (Get_InSpriteName(sprite, "MUUI_Rank_", out int index0))
                rankIconArr[index0 - 1] = sprite;
            else if (Get_InSpriteName(sprite, "MUUI_DescRank_", out int index1))
                descRankIconArr[index1 - 1] = sprite;
            
        }
    }

    // Get
    public Sprite Get_RankIcon(int rank) => rankIconArr[rank - 1];
    public Sprite Get_DescRankIcon(int rank) => descRankIconArr[rank - 1];

    #endregion
    #region Item - Module (Prefab)

    #endregion
    #region Item - Module (Anim)

    // Value
    [HideInInspector] private AnimationClip[] moduleOutlineACs;

    // Offset
    private void Offset_Anim_Module()
    {
        string path = "Anim/";

        string modulePath = path + "Module/";

        moduleOutlineACs = new AnimationClip[5]; // 5
        for (int i = 0; i < moduleOutlineACs.Length; i++)
            moduleOutlineACs[i] = GetAsset<AnimationClip>(modulePath, $"Clip_ModuleItemOutline_R{i + 1}");
    }

    // Get
    public AnimationClip Get_ModuleOutlineAC(int rank) => moduleOutlineACs[rank];

    #endregion

    #region Item - Keycard (Anim & Color)

    // Value
    [HideInInspector] public AnimationClip keycardOutlineAC { get; private set; }
    [HideInInspector] public Color[] keycardOutlineColorArr;

    // Offset
    private void Offset_Anim_Keycard()
    {
        string path = "Anim/";

        string modulePath = path + "Module/";

        keycardOutlineAC = GetAsset<AnimationClip>(modulePath, "Clip_KeycardOutline");
        keycardOutlineColorArr = new Color[5];
        ColorUtility.TryParseHtmlString("#FF727F", out keycardOutlineColorArr[0]);
        ColorUtility.TryParseHtmlString("#FFF49B", out keycardOutlineColorArr[1]);
        ColorUtility.TryParseHtmlString("#A0FFFC", out keycardOutlineColorArr[2]);
        ColorUtility.TryParseHtmlString("#A0FF99", out keycardOutlineColorArr[3]);
        ColorUtility.TryParseHtmlString("#99FFD0", out keycardOutlineColorArr[4]);
    }

    // Get
    public Color Get_KeycardColor(int id) => keycardOutlineColorArr[id];

    #endregion

    #region Item - Core (Sprite)

    // Value
    [HideInInspector] private Dictionary<int, Sprite> coreSpriteDict;

    // Offset
    private void Offset_Sprite_Core()
    {
        string path = "Sprite/";

        string modulePath = path + "Module/";
        coreSpriteDict = new Dictionary<int, Sprite>();
        Sprite[] moduleSprites = GetAsset_Arr<Sprite>(modulePath, "Module_000");
        for (int i = 0; i < moduleSprites.Length; i++)
        {
            Sprite sprite = moduleSprites[i];

            if (sprite.name == "Module_ProtoCore")
                coreSpriteDict.Add(1, sprite);
            else if (sprite.name == "Module_EtherCore")
                coreSpriteDict.Add(2, sprite);
            else if (sprite.name == "Module_OriginCore")
                coreSpriteDict.Add(3, sprite);
        }
    }

    // Get
    public Sprite Get_CoreSprite(int id) => coreSpriteDict[id];

    #endregion
    #region Item - Core (Anim)

    // Value
    [SerializeField] public AnimationClip coreOutlineAC { get; private set; }

    // Offset
    private void Offset_Anim_Core()
    {
        string path = "Anim/";

        string modulePath = path + "Module/";

        coreOutlineAC = GetAsset<AnimationClip>(modulePath, "Clip_CoreOutline");
    }

    // Get

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
    #region AllyCard (Sprite & Color)

    // Value
    [HideInInspector] private Sprite[][] allyCardIcon_Data;

    [HideInInspector] private static readonly int stIconAmount = 1;
    [HideInInspector] private static readonly int utIconAmount = 1;
    [HideInInspector] private static readonly int ntIconAmount = 1;

    [HideInInspector] private Sprite[] allyCardFrameArr;
    [HideInInspector] private Sprite[] allyCardLightArr;
    [HideInInspector] private Sprite[] allyCardBGArr;

    [HideInInspector] private Color[] allyCardColorArr;
    [HideInInspector] public Sprite allyNullIcon { get; private set; }

    [HideInInspector] private Sprite[] keyCardSpriteArr;

    // Offset
    private void Offset_Sprite_AllyCard()
    {
        string path = "Sprite/";

        allyCardIcon_Data = new Sprite[][]
        {
            GetAsset_AllyCardIcon(stIconAmount, "ST"),
            GetAsset_AllyCardIcon(utIconAmount, "UT"),
            GetAsset_AllyCardIcon(ntIconAmount, "NT")
        };

        string allyCardPath = path + "UI/Ally/";

        Sprite[] allyCardFrameSprites = GetAsset_Arr<Sprite>(allyCardPath, "AllyCardFrame_000");
        allyCardFrameArr = new Sprite[6]; // 6
        allyCardLightArr = new Sprite[6];
        allyCardBGArr = new Sprite[6];
        for (int i = 0; i < allyCardFrameSprites.Length; i++)
        {
            Sprite sprite = allyCardFrameSprites[i];

            if (Get_InSpriteName(sprite, "AllyCardFrame_000_Frame_", out int indexf))
                allyCardFrameArr[indexf] = sprite;
            else if (Get_InSpriteName(sprite, "AllyCardFrame_000_Light_", out int indexl))
                allyCardLightArr[indexl] = sprite;
            else if (Get_InSpriteName(sprite, "AllyCardFrame_000_BG_", out int indexB))
                allyCardBGArr[indexB] = sprite;
        }


        allyCardColorArr = new Color[6]; // Keycard Color: 6
        ColorUtility.TryParseHtmlString("#FFFFFF", out allyCardColorArr[0]);
        ColorUtility.TryParseHtmlString("#D4FACA", out allyCardColorArr[1]);
        ColorUtility.TryParseHtmlString("#64F9F8", out allyCardColorArr[2]);
        ColorUtility.TryParseHtmlString("#B366FD", out allyCardColorArr[3]);
        ColorUtility.TryParseHtmlString("#FE4C31", out allyCardColorArr[4]);
        ColorUtility.TryParseHtmlString("#FFFFE1", out allyCardColorArr[5]);

        keyCardSpriteArr = new Sprite[5]; // Keycard: 5
        string moduleUiPath = path + "UI/ModuleUI/";

        Sprite[] moduleUiSprites = GetAsset_Arr<Sprite>(moduleUiPath, "ModuleUI_000");
        for (int i = 0; i < moduleUiSprites.Length; i++)
        {
            Sprite sprite = moduleUiSprites[i];

            if (sprite.name == "ModuleUI_Icon_NullCard")
                allyNullIcon = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_KeyCard_", out string s))
            {
                switch (s)
                {
                    case "Boss": keyCardSpriteArr[0] = sprite; break;
                    case "Vault": keyCardSpriteArr[1] = sprite; break;
                    case "Prison": keyCardSpriteArr[2] = sprite; break;
                    case "Shop": keyCardSpriteArr[3] = sprite; break;
                    case "AllyShop": keyCardSpriteArr[4] = sprite; break;

                    default: break;
                }
            }
        }
    }

    private Sprite[] GetAsset_AllyCardIcon(int spriteAmount, string typeName)
    {
        List<Sprite> result = new List<Sprite>();
        for (int i = 0; i < spriteAmount; i++)
        {
            result.AddRange(
                GetAsset_Arr<Sprite>(
                    $"Sprite/UI/Ally/",
                    $"AllyCardIcon_{typeName}_{DevTool.Get_LengthString(i, 3)}"));
        }
        return result.ToArray();
    }

    // Get
    public Sprite[] Get_AllyCardSpriteIcon(int type) => allyCardIcon_Data[type];

    public Sprite Get_AllyCardFrame(int rank) => allyCardFrameArr[rank];
    public Sprite Get_AllyCardLight(int rank) => allyCardLightArr[rank];
    public Sprite Get_AllyCardBG(int rank) => allyCardBGArr[rank];

    public Color Get_AllyCardColor(int rank) => allyCardColorArr[rank];
    public Sprite Get_KeyCardSprite(int id) => keyCardSpriteArr[id];
    public int Get_KeycardAmount() => keyCardSpriteArr.Length;

    #endregion

    #region Ally (Sprite)

    // Value
    private static readonly string[] directionOrder = new string[] { "UL", "U", "UR", "R", "DR", "D", "DL", "L" };
    [HideInInspector] private Sprite[] allySprite_Data;

    [HideInInspector] private CoupleData<Sprite> strikeTeamIcon = new CoupleData<Sprite>(null, null);
    [HideInInspector] private CoupleData<Sprite> uplinkTeamIcon = new CoupleData<Sprite>(null, null);
    [HideInInspector] private CoupleData<Sprite> neoTeamIcon = new CoupleData<Sprite>(null, null);

    [HideInInspector] public PrisonAllySprite strikeTeamAllySprites { get; private set; }
    [HideInInspector] public PrisonAllySprite uplinkTeamAllySprites { get; private set; }
    [HideInInspector] public PrisonAllySprite neoTeamAllySprites { get; private set; }

    // Offset
    private void Offset_Sprite_Ally()
    {
        string path = "Sprite/";

        string allyPath = path + "Ally/";

        allySprite_Data = GetAsset_Arr<Sprite>(allyPath, "Ally_001");

        string buildingPath = path + "Building/";
        Sprite[] building000Sprites = GetAsset_Arr<Sprite>(buildingPath, "Building_000");
        for (int i = 0; i < building000Sprites.Length; i++)
        {
            Sprite sprite = building000Sprites[i];

            if (Get_InSpriteName(sprite, strikeTeamIcon, "Building000_StrikeTeamMark_", "Small", "Big"))
                continue;
            else if (Get_InSpriteName(sprite, uplinkTeamIcon, "Building000_UplinkTeamMark_", "Small", "Big"))
                continue;
            else if (Get_InSpriteName(sprite, neoTeamIcon, "Building000_NeoTeamMark_", "Small", "Big"))
                continue;
        }

        strikeTeamAllySprites = new PrisonAllySprite();
        uplinkTeamAllySprites = new PrisonAllySprite();
        neoTeamAllySprites = new PrisonAllySprite();
        Sprite[] ally000Sprites = GetAsset_Arr<Sprite>(allyPath, "Ally_000");
        for (int i = 0; i < ally000Sprites.Length; i++)
        {
            Sprite sprite = ally000Sprites[i];

            if (Get_InSpriteName(sprite, strikeTeamAllySprites, "Ally000_ST_InPrison_"))
                continue;
            if (Get_InSpriteName(sprite, uplinkTeamAllySprites, "Ally000_UT_InPrison_"))
                continue;
            if (Get_InSpriteName(sprite, neoTeamAllySprites, "Ally000_NT_InPrison_"))
                continue;
        }
    }

    // Get
    public List<Sprite> Get_AllySprite(string name, string type)
    {
        List<Sprite> result = new List<Sprite>();

        int stringLength = 7 + name.Length + type.Length;

        // 맞는 아트 리소스 가져오기
        for (int i = 0; i < allySprite_Data.Length; i++)
        {
            if (allySprite_Data[i].name.Length >= stringLength &&
                allySprite_Data[i].name.Substring(0, stringLength) == $"Ally_{name}_{type}_")
            {
                result.Add(allySprite_Data[i]);
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


    public Sprite Get_STPrisonIcon(bool isBase) => strikeTeamIcon.Get_Base(isBase);
    public Sprite Get_UTPrisonIcon(bool isBase) => uplinkTeamIcon.Get_Base(isBase);
    public Sprite Get_NTPrisonIcon(bool isBase) => neoTeamIcon.Get_Base(isBase);


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
    #region AllyRequest (Sprite)

    // Value
    [HideInInspector] private Sprite[] requestRankSpriteList;
    [HideInInspector] private Dictionary<string, Sprite> requestRewardDict;

    // Offset
    private void Offset_Sprite_AllyRequest()
    {
        string path = "Sprite/";

        string moduleUIPath = path + "UI/ModuleUI/";
        Sprite[] moduleUISprites = GetAsset_Arr<Sprite>(moduleUIPath, "ModuleUI_000");
        requestRankSpriteList = new Sprite[5];
        requestRewardDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < moduleUISprites.Length; i++)
        {
            Sprite sprite = moduleUISprites[i];

            if (Get_InSpriteName(sprite, "ModuleUI_RequestRank_", out int index))
                requestRankSpriteList[index] = sprite;

            else if (sprite.name == "ModuleUI_13s_BC")
                requestRewardDict.Add("BC", sprite);
            else if (sprite.name == "ModuleUI_13s_Credit")
                requestRewardDict.Add("Credit", sprite);
            else if (sprite.name == "ModuleUI_13s_EP")
                requestRewardDict.Add("EP", sprite);
        }

    }

    // Get
    public Sprite Get_AllyRequestRank(int rank) => requestRankSpriteList[rank];
    public Sprite Get_AllyRequestReward(string type) => requestRewardDict[type];

    #endregion

    #region Map (CSV)

    // Value
    [HideInInspector] private Dictionary<int, MapNextIndex> mapNextIndex_Data;

    [HideInInspector] private WordSet_Just mapName_Data;
    [HideInInspector] private WordSet_Just mapDesc_Data;


    // Offset
    private void Offset_CSV_Map()
    {
        string path = "CSV/Map/";
        mapNextIndex_Data = Offset_MapNextIndex(path, "MapEntranceIndex_CSV");
        mapName_Data = GetAsset_WordData(path, "MapName_CSV");
        mapDesc_Data = GetAsset_WordData(path, "MapDesc_CSV");
    }

    private Dictionary<int, MapNextIndex> Offset_MapNextIndex(string path, string fileName)
    {
        Dictionary<int, MapNextIndex> result = new Dictionary<int, MapNextIndex>();

        string[][] stringList = Get_DoubleArr(Resources.Load<TextAsset>(path + fileName));

        for (int i = 1; i < stringList.Length; i++)
        {
            if (stringList[i][0] == "") break;

            int pastIndex = int.Parse(stringList[i][0]);
            int nextIndex = int.Parse(stringList[i][1]);

            List<MapNextIndex> indexList = new List<MapNextIndex>();
            if (Is_ExistMapIndex(result, pastIndex, out MapNextIndex mapNextIndex)) // 이미 존재한다면
            {
                mapNextIndex.nextIndexList.Add(nextIndex);
            }
            else // 존재하지 않는다면
            {
                result.Add(pastIndex, new MapNextIndex(pastIndex, nextIndex));
            }
        }

        return result;
    }

    private bool Is_ExistMapIndex(Dictionary<int, MapNextIndex> allMapNextIndex, int pastIndex, out MapNextIndex mapNextIndex)
    {
        mapNextIndex = null;
        if (allMapNextIndex.ContainsKey(pastIndex))
        {
            mapNextIndex = allMapNextIndex[pastIndex];
            return true;
        }
        return false;
    }


    // Get
    public string Get_MapName(int id) => mapName_Data.Get_Word(id);
    public string Get_MapDesc(int id) => mapDesc_Data.Get_Word(id);

    public List<int> Get_CorrectIndexList(int pastIndex)
    {
        if (mapNextIndex_Data.ContainsKey(pastIndex))
            return mapNextIndex_Data[pastIndex].nextIndexList;

        return null;
    }

    #endregion
    #region Map (Sprite & Material)

    [HideInInspector] private Material[] passageMiddleMaterialArr;

    // Offset
    private void Offset_Sprite_Map()
    {

    }

    private void Offset_Material_Map()
    {
        string path = "Material/";

        string mapPassagePath = path + "Map/MapPassage/";
        passageMiddleMaterialArr = new Material[1];
        for (int i = 0; i < passageMiddleMaterialArr.Length; i++)
            passageMiddleMaterialArr[i] = GetAsset<Material>(mapPassagePath, $"MapPassage_{DevTool.Get_LengthString(i, 3)}");

    }



    // Get
    public Material Get_PassageMiddleMaterial(int index) => passageMiddleMaterialArr[index];

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

    #region Puzzle NSC (Sprite & Color)

    // Value
    [HideInInspector] public Sprite[] nsc_numSpriteArr { get; private set; }
    [HideInInspector] public Sprite[] nsc_shapeSpriteArr { get; private set; }
    [HideInInspector] public Color[] nsc_colorArr { get; private set; }
    [HideInInspector] public Sprite nsc_colorSprite { get; private set; }


    [HideInInspector] private NSCAnswerSpriteSet[] allNscAnswerSpriteSet;

    // Offset
    private void Offset_Sprite_PuzzleNSC()
    {
        string path = "Sprite/";

        string moduleUIPath = path + "UI/ModuleUI/";
        nsc_numSpriteArr = new Sprite[5]; //5
        nsc_shapeSpriteArr = new Sprite[5];
        allNscAnswerSpriteSet = new NSCAnswerSpriteSet[5];
        for (int i = 0; i < allNscAnswerSpriteSet.Length; i++)
        {
            allNscAnswerSpriteSet[i] = new NSCAnswerSpriteSet();
            allNscAnswerSpriteSet[i].shapeIndex = i;
            allNscAnswerSpriteSet[i].allAnswerSet = new Sprite[5];
        }
        Sprite[] moduleUISprites = GetAsset_Arr<Sprite>(moduleUIPath, "ModuleUI_000");
        for (int i = 0; i < moduleUISprites.Length; i++)
        {
            Sprite sprite = moduleUISprites[i];

            if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_Num_", out int index))
                nsc_numSpriteArr[index - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_Shape_", out string indexS))
            {
                switch (indexS)
                {
                    case "Circle": nsc_shapeSpriteArr[0] = sprite; break;
                    case "Triangle": nsc_shapeSpriteArr[1] = sprite; break;
                    case "Rectangle": nsc_shapeSpriteArr[2] = sprite; break;
                    case "X": nsc_shapeSpriteArr[3] = sprite; break;
                    case "HalfCircle": nsc_shapeSpriteArr[4] = sprite; break;
                    default: break;
                }
            }
            else if (sprite.name == "ModuleUI_RollSelect_Color")
                nsc_colorSprite = sprite;

            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_C_", out int index_a_c))
                allNscAnswerSpriteSet[0].allAnswerSet[index_a_c - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_T_", out int index_a_t))
                allNscAnswerSpriteSet[1].allAnswerSet[index_a_t - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_R_", out int index_a_r))
                allNscAnswerSpriteSet[2].allAnswerSet[index_a_r - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_X_", out int index_a_x))
                allNscAnswerSpriteSet[3].allAnswerSet[index_a_x - 1] = sprite;
            else if (Get_InSpriteName(sprite, "ModuleUI_RollSelect_A_H_", out int index_a_h))
                allNscAnswerSpriteSet[4].allAnswerSet[index_a_h - 1] = sprite;
        }


        nsc_colorArr = new Color[5];
        ColorUtility.TryParseHtmlString("#FF0000", out nsc_colorArr[0]);
        ColorUtility.TryParseHtmlString("#FFFF00", out nsc_colorArr[1]);
        ColorUtility.TryParseHtmlString("#00FF00", out nsc_colorArr[2]);
        ColorUtility.TryParseHtmlString("#0000FF", out nsc_colorArr[3]);
        ColorUtility.TryParseHtmlString("#FF00FF", out nsc_colorArr[4]);
    }

    // Get
    public Sprite Get_NSCAnswerSprite(int shapeIndex, int numIndex) => allNscAnswerSpriteSet[shapeIndex].allAnswerSet[numIndex];

    #endregion

    #region Shop - BU & MU (Anim)

    // Value
    [HideInInspector] public CoupleData<AnimationClip> buShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip buShop_BrokenAC { get; private set; }
    [HideInInspector] public CoupleData<AnimationClip> muShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip muShop_BrokenAC { get; private set; }


    [HideInInspector] public AnimationClip brokenStateAC { get; private set; }
    [HideInInspector] public CoupleData<AnimationClip> needChargeBettery_OnOffStateAC { get; private set; }

    // Offset
    private void Offset_Anim_PlayerShop()
    {
        string path = "Anim/";

        string shopPath = path + "Building/Shop/";

        string buPath = shopPath + "BU/";
        buShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(buPath, "Clip_BUShop_Off"), GetAsset<AnimationClip>(buPath, "Clip_BUShop_Off"));
        buShop_BrokenAC = GetAsset<AnimationClip>(buPath, "Clip_BUShop_Broken");

        string muPath = shopPath + "MU/";
        muShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(muPath, "Clip_MUShop_Off"), GetAsset<AnimationClip>(muPath, "Clip_MUShop_Off"));
        muShop_BrokenAC = GetAsset<AnimationClip>(muPath, "Clip_MUShop_Broken");

        brokenStateAC = GetAsset<AnimationClip>(shopPath, "Clip_Broken");
        needChargeBettery_OnOffStateAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(shopPath, "Clip_NeedEC"), GetAsset<AnimationClip>(shopPath, "Clip_Upgrade"));
    }

    #endregion
    #region Shop - ABU & AMU (Anim)

    // Value
    [HideInInspector] public CoupleData<AnimationClip> allyBuShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip allyBuShop_BrokenAC { get; private set; }
    [HideInInspector] public CoupleData<AnimationClip> allyMuShop_OnOffAC { get; private set; }
    [HideInInspector] public AnimationClip allyMuShop_BrokenAC { get; private set; }

    // Offset
    private void Offset_Anim_AllyShop()
    {
        string path = "Anim/";

        string shopPath = path + "Building/AllyShop/";

        string buPath = shopPath + "BU/";
        allyBuShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(buPath, "Clip_AllyBUShop_Off"), GetAsset<AnimationClip>(buPath, "Clip_AllyBUShop_Off"));
        allyBuShop_BrokenAC = GetAsset<AnimationClip>(buPath, "Clip_AllyBUShop_Broken");

        string muPath = shopPath + "MU/";
        allyMuShop_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(muPath, "Clip_AllyMUShop_Off"), GetAsset<AnimationClip>(muPath, "Clip_AllyMUShop_Off"));
        allyMuShop_BrokenAC = GetAsset<AnimationClip>(muPath, "Clip_AllyMUShop_Broken");
    }

    #endregion

    #region Vault (Anim)

    // Value
    [HideInInspector] private CoupleData<AnimationClip>[] vault_AC;
    [HideInInspector] private AnimationClip[] vault_BrokenAC;

    [HideInInspector] public AnimationClip vault_ModuleIconAC { get; private set; }
    [HideInInspector] public AnimationClip vault_BSIconAC { get; private set; }
    [HideInInspector] public AnimationClip vault_JIconAC { get; private set; }
    
    [HideInInspector] public CoupleData<AnimationClip> vault_StateAC { get; private set; }

    // Offset
    private void Offset_Anim_Vault()
    {
        string path = "Anim/";

        string vaultPath = path + "Building/Vault/";

        vault_AC = new CoupleData<AnimationClip>[5];
        vault_BrokenAC = new AnimationClip[5];
        for (int i = 0; i < vault_AC.Length; i++)
        {
            AnimationClip ac = GetAsset<AnimationClip>(vaultPath, $"Clip_Vault_G{i}");
            vault_AC[i] = new CoupleData<AnimationClip>(ac, ac);

            vault_BrokenAC[i] = GetAsset<AnimationClip>(vaultPath, $"Clip_Vault_G{i}_Broken");
        }

        string iconName = "Clip_Vault_Icon_";
        vault_ModuleIconAC = GetAsset<AnimationClip>(vaultPath, $"{iconName}Module");
        vault_BSIconAC = GetAsset<AnimationClip>(vaultPath, $"{iconName}BetteryShard");
        vault_JIconAC = GetAsset<AnimationClip>(vaultPath, $"{iconName}Joule");

        vault_StateAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(vaultPath, $"Clip_VaultEmpty_State"), GetAsset<AnimationClip>(vaultPath, $"Clip_Vault_State"));
    }

    // Get
    public CoupleData<AnimationClip> Get_VaultAnim(int grade) => vault_AC[grade];
    public AnimationClip Get_VaultBrokenAnim(int grade) => vault_BrokenAC[grade];

    #endregion

    #region Prison (Sprite)

    // Value
    [HideInInspector] private Sprite[] prisonRateIconArr;

    // Offset
    private void Offset_Sprite_Prison()
    {
        string path = "Sprite/";
        string buildingPath = path + "Building/";
        prisonRateIconArr = new Sprite[5]; // 5
        Sprite[] building000Sprites = GetAsset_Arr<Sprite>(buildingPath, "Building_000");
        for (int i = 0; i < building000Sprites.Length; i++)
        {
            Sprite sprite = building000Sprites[i];

            if (Get_InSpriteName(sprite, "Building000_DangerRate", out int index))
                prisonRateIconArr[index - 1] = sprite;
        }
    }

    // Get
    public Sprite Get_PrisonRankSprite(int id) => prisonRateIconArr[id];

    #endregion
    #region Prison (Anim)

    // Value 
    [SerializeField] public CoupleData<AnimationClip> prison_OnOffAC { get; private set; }
    [SerializeField] public CoupleData<AnimationClip> prison_OnOffUpsideAC { get; private set; }

    [SerializeField] public CoupleData<AnimationClip> prison_StateAC { get; private set; }

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

        prison_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(prisonPath, $"{prisonName}{lockName}{downName}"), GetAsset<AnimationClip>(prisonPath, $"{prisonName}{UnlockName}{downName}"));
        prison_OnOffUpsideAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(prisonPath, $"{prisonName}{lockName}{upName}"), GetAsset<AnimationClip>(prisonPath, $"{prisonName}{UnlockName}{upName}"));

        prison_StateAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(prisonPath, $"{iconName}{lockName}"), GetAsset<AnimationClip>(prisonPath, $"{iconName}{UnlockName}"));
    }

    // Get

    #endregion

    #region Operator (Anim)

    // Value
    [SerializeField] public CoupleData<AnimationClip> operator_OnOffAC { get; private set; }
    [SerializeField] public AnimationClip operator_RepairAC { get; private set; }
    [SerializeField] public AnimationClip operator_RerollAC { get; private set; }
    [SerializeField] public AnimationClip operator_UpgradeAC { get; private set; }
    [SerializeField] public AnimationClip operator_AllyAC { get; private set; }

    [SerializeField] public CoupleData<AnimationClip> operator_LightAC { get; private set; }

    // Offset
    private void Offset_Anim_Operator()
    {
        string path = "Anim/";

        string OperPath = path + "Building/Operator/";

        string operName = "Clip_Operator";
        operator_OnOffAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(OperPath, $"{operName}Off"), GetAsset<AnimationClip>(OperPath, $"{operName}On"));

        string iconName = "Clip_Icon_";
        operator_RepairAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Repair");
        operator_RerollAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Reroll");
        operator_UpgradeAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Upgrade");
        operator_AllyAC = GetAsset<AnimationClip>(OperPath, $"{iconName}Ally");

        operator_LightAC = new CoupleData<AnimationClip>(
            GetAsset<AnimationClip>(OperPath, $"Clip_LightOff"), GetAsset<AnimationClip>(OperPath, $"Clip_LightOn"));
    }

    #endregion

    #region Shop - Converter (Material & Anim)

    // Value
    [HideInInspector] private ConverterReso converterReso;

    // Offset
    private void Offset_Anim_Converter()
    {
        string materialPath = "Material/Build/";
        string animPath = "Anim/Building/Converter/";

        string converterAnimName = "Clip_Converter_";
        string converterMaterialName = "Build_003";
        converterReso = new ConverterReso();

        converterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "PremiumCredit"),
            GetAsset<Material>(materialPath, converterMaterialName)));
        converterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "ProtoCore"),
            GetAsset<Material>(materialPath, converterMaterialName)));
        converterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "EtherCore"),
            GetAsset<Material>(materialPath, converterMaterialName)));
        converterReso.Add(new EachConverterReso(
            GetAsset<AnimationClip>(animPath, converterAnimName + "OriginCore"),
            GetAsset<Material>(materialPath, converterMaterialName)));
    }

    // Get
    public EachConverterReso Get_ConverterReso(int id) => converterReso.converterResoList[id];

    #endregion

    #region Minimap (Sprite)

    // Value
    [HideInInspector] public CoupleData<Sprite> vault_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> elevator_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> shop_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> allyShop_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> st_Prison_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> ut_Prison_Icon { get; private set; }
    [HideInInspector] public CoupleData<Sprite> nt_Prison_Icon { get; private set; }

    [HideInInspector] private MinimapIcon[] minimapIcons;

    [HideInInspector] private Dictionary<int, Sprite> stageIconDict;


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

        vault_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}Vault"], hud000dict[$"{immIconName}Vault"]);
        elevator_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}Elevator"], hud000dict[$"{immIconName}Elevator"]);
        shop_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}Shop"], hud000dict[$"{immIconName}Shop"]);
        allyShop_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}AllyShop"], hud000dict[$"{immIconName}AllyShop"]);
        st_Prison_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}ST_Prison"], hud000dict[$"{immIconName}ST_Prison"]);
        ut_Prison_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}UT_Prison"], hud000dict[$"{immIconName}UT_Prison"]);
        nt_Prison_Icon = new CoupleData<Sprite>(
            hud000dict[$"{mmIconName}NT_Prison"], hud000dict[$"{immIconName}NT_Prison"]);

        minimapIcons = new MinimapIcon[8]; // 8

        minimapIcons[0] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}000"], hud000dict[$"{mmoName}000"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}000"], hud000dict[$"{immoName}000"])),
            new Vector2Int[] { new Vector2Int(0, 0) }, 
            new Vector2(0.5f, 0.5f));

        minimapIcons[1] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}001"], hud000dict[$"{mmoName}001"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}001"], hud000dict[$"{immoName}001"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) }, 
            new Vector2(0.25f, 0.5f));

        minimapIcons[2] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}002"], hud000dict[$"{mmoName}002"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}002"], hud000dict[$"{immoName}002"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) },
            new Vector2(0.5f, 0.25f));

        minimapIcons[3] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}003"], hud000dict[$"{mmoName}003"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}003"], hud000dict[$"{immoName}003"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
            new Vector2(0.25f, 0.25f));

        minimapIcons[4] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}004"], hud000dict[$"{mmoName}004"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}004"], hud000dict[$"{immoName}004"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
            new Vector2(0.25f, 0.75f));

        minimapIcons[5] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}005"], hud000dict[$"{mmoName}005"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}005"], hud000dict[$"{immoName}005"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) },
            new Vector2(0.25f, 0.25f));
        
        minimapIcons[6] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}006"], hud000dict[$"{mmoName}006"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}006"], hud000dict[$"{immoName}006"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
            new Vector2(0.25f, 0.25f));

        minimapIcons[7] = new MinimapIcon(
            new CouplePair<Sprite>(
                new CoupleData<Sprite>(hud000dict[$"{mmName}007"], hud000dict[$"{mmoName}007"]),
                new CoupleData<Sprite>(hud000dict[$"{immName}007"], hud000dict[$"{immoName}007"])),
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) },
            new Vector2(0.25f, 0.25f));

        string stageIconName = "ModuleUI_StageIcon_";

        string moduleUIPath = "Sprite/UI/ModuleUI/";
        stageIconDict = new Dictionary<int, Sprite>();
        Sprite[] moduleUIS = GetAsset_Arr<Sprite>(moduleUIPath);
        for (int i = 0; i < moduleUIS.Length; i++)
        {
            Sprite s = moduleUIS[i];
            if (Get_InSpriteName(s, stageIconName, out int idx))
            {
                stageIconDict.Add(idx, s);
            }
            else if (Get_InSpriteName(s, stageIconName, out string idxS) && idxS == "Lobby")
            {
                stageIconDict.Add(99, s);
            }
        }
    }

    // Get
    public MinimapIcon Get_MinimapIcon(int id) => minimapIcons[id];
    public Sprite Get_StageIcon(int id) => stageIconDict[id];

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

    #region GetAsset_SpriteName

    private bool Get_InSpriteName(Sprite sprite, string name, out int index)
    {
        index = -1;
        if (sprite.name.Length > name.Length && sprite.name.StartsWith(name))
        {
            if (int.TryParse(sprite.name.Replace(name, ""), out index))
            {
                return true;
            }
        }
        return false;
    }

    private bool Get_InSpriteName(Sprite sprite, string name, out string index)
    {
        index = "";
        if (sprite.name.Length > name.Length && sprite.name.StartsWith(name))
        {
            index = sprite.name.Replace(name, "");
            return true;
        }
        return false;
    }

    private bool Get_InSpriteName(Sprite sprite, CoupleData<Sprite> data, string name, string _base, string special)
    {
        if (Get_InSpriteName(sprite, name, out string _index))
        {
            if (_index == _base)
            {
                data.typeBase = sprite;
                return true;
            }  
            else if (_index == special)
            {
                data.typeSpecial = sprite;
                return true;
            }
        }
        return false;
    }

    private bool Get_InSpriteName(Sprite sprite, PrisonAllySprite data, string name)
    {
        if (Get_InSpriteName(sprite, name, out string _index))
        {
            if (_index == "Bind")
            { 
                data.bind = sprite;
                return true;
            }
            else if (_index == "Fall")
            { 
                data.fall = sprite;
                return true;
            }
            else if (_index == "Stand")
            { 
                data.stand = sprite;
                return true;
            }
            else if (_index == "Salute")
            { 
                data.salute = sprite;
                return true;
            }
        }
        return false;
    }

    #endregion
}