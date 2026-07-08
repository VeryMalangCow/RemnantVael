using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>, IMainGameInitializer
{
    #region Value

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [Space(20)]
    [Header("<><><><><> Event")]

    [Space(10)]
    [Header("=== Managing Value")]
    [SerializeField] private bool isPlayingEvent = false;

    [Space(10)]
    [Header("=== Base Comp")]
    [SerializeField] private RectTransform blackUpsideRT;
    [SerializeField] private RectTransform blackDownsideRT;

    [Space(10)]
    [Header("=== BlackScreen")]
    [SerializeField] private Image blackScreenImg;

    [Space(10)]
    [Header("=== Dialogue")]
    [SerializeField] private bool isPlayingDialogue = false;

    [SerializeField] private SideDialogueComp leftDialogueComp;
    [SerializeField] private SideDialogueComp rightDialogueComp;

    [Serializable]
    public class SideDialogueComp
    {
        [SerializeField] public GameObject dialogueGO;
        [SerializeField] public Image dialogueImg;
        [SerializeField] public TMP_Text nameTxt;
        [SerializeField] public TMP_Text dialogueTxt;
    }

    [HideInInspector] private List<DialogueElement> currentDialogues;

    [Space(10)]
    [Header("=== Cutscene")]
    [SerializeField] public bool isPlayingCutscene = false;
    [SerializeField] private GameObject cutsceneGO;
    [SerializeField] private ImgQueueSet imgQueueSet;
    [SerializeField] private TMP_Text cutsceneTxt;
    [SerializeField] private Image cutsceneNextImg;
    [HideInInspector] private List<CutsceneElement> currentCutscenes;
    

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private EventData currentEvent = null;

    private Dictionary<string, Dictionary<string, Sprite>> dialogueSpriteDict;

    #region Event (CSV Data)


    [HideInInspector] private EventID[] eventIds;
    [HideInInspector] private EventElement[] eventElements;

    [HideInInspector] private CutsceneID[] cutsceneIds;
    [HideInInspector] private CutsceneElement[][] cutsceneElements;

    [HideInInspector] private DialogueID[] dialogueIds;
    [HideInInspector] private DialogueElement[][] dialogueElements;

    #endregion

    #endregion

    #region Init
    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var reso = StaticResourceManager.instance.EventReso;

        // Event
        eventElements = CSVReader.GetEventElements(reso.eventElementCsv);
        eventIds = CSVReader.GetEventIds(reso.eventIdCsv);

        // Cutscene
        cutsceneElements = new CutsceneElement[reso.cutsceneElementCsvs.Length][];
        for (int i = 0; i < cutsceneElements.Length; i++)
            cutsceneElements[i] = CSVReader.GetCutsceneElements(reso.cutsceneElementCsvs[i]);
        cutsceneIds = CSVReader.GetCutsceneIds(reso.cutsceneIdCsv);

        // Dialogue
        dialogueElements = new DialogueElement[reso.dialogueElementCsvs.Length][];
        for (int i = 0; i < dialogueElements.Length; i++)
            dialogueElements[i] = CSVReader.GetDialogueElements(reso.dialogueElementCsvs[i]);
        dialogueIds = CSVReader.GetDialogueIds(reso.dialogueIdCsv);

        // Dialogue Img
        imgQueueSet.Offset();
        dialogueSpriteDict = new Dictionary<string, Dictionary<string, Sprite>>();
        Sprite[] sprites = StaticResourceManager.instance.EventReso.dialogueSprites;
        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];
            string[] s = sprite.name.Split("_");

            if (!dialogueSpriteDict.ContainsKey(s[0]))
                dialogueSpriteDict.Add(s[0], new Dictionary<string, Sprite>());
           
            dialogueSpriteDict[s[0]].Add(s[1], sprite);
        }

        sw.Stop();
        UnityEngine.Debug.Log($"EventManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

        yield return null;
    }

    #endregion

    #region Event Start

    public void TryStart_Event(int id)
    {
        if (SaveDataManager.instance.jsonData.gameProgressData.currentProgressing + 1 == id)
            Start_Event(id);
    }

    // 이벤트 시작
    private void Start_Event(int id)
    {
        // 다른 이벤트 중이라면 취소
        if (isPlayingEvent)
        { return; }

        SetOn_EventOption();

        currentEvent = new EventData(id, new List<EventElement>(GetCorrectEvents(id)));
        Play_Event();
    }

    // ID에 맞는 EventID를 가져온 후, 그에 맞는 EventElement List를 가져옴
    public List<EventElement> GetCorrectEvents(int id)
    {
        List<EventElement> result = new List<EventElement>();

        int[] ids = eventIds[id].eventIds;

        for (int i = 0; i < ids.Length; i++)
        {
            EventElement eventElement = eventElements[ids[i]];

            result.Add(eventElement);
        }

        return result;
    }

    // 이벤트 실행
    private void Play_Event()
    {
        if (currentEvent.events.Count <= 0)
        {
            SetOff_EventOption();
            return;
        }

        if (currentEvent.events[0] is EventElement_Stay stay)
        { StartCoroutine(Play_Stay_Cor(stay)); }
        else if (currentEvent.events[0] is EventElement_Move move)
        { StartCoroutine(Play_Move_Cor(move)); }
        else if (currentEvent.events[0] is EventElement_Look look)
        { StartCoroutine(Play_Look_Cor(look)); }
        else if (currentEvent.events[0] is EventElement_BlackScreenIn blackScreenIn)
        { StartCoroutine(Play_BlackScreenIn_Cor(blackScreenIn)); }
        else if (currentEvent.events[0] is EventElement_BlackScreenOut blackScreenOut)
        { StartCoroutine(Play_BlackScreenOut_Cor(blackScreenOut)); }
        else if (currentEvent.events[0] is EventElement_Dialogue dialogue)
        { StartCoroutine(Play_Dialogue_Cor(dialogue)); }
        else if (currentEvent.events[0] is EventElement_Cutscene cutscene)
        { StartCoroutine(Play_Cutscene_Cor(cutscene)); }

        currentEvent.Remove_OnePart();
    }


    // 이벤트 실행 시, 설정 온
    private void SetOn_EventOption()
    {
        Set_Input(false);
        isPlayingEvent = true;

        Set_BlackUpDownCover(true);
        Set_AnotherUI(false);
    }

    // 이벤트 실행 시, 설정 오프
    private void SetOff_EventOption()
    {
        bool needSave = false;
        switch (currentEvent.id)
        {
            case 0:
                SaveDataManager.instance.jsonData.gameProgressData.currentProgressing++;
                needSave = true;
                break;

            default:
                break; 
        }

        if (needSave) SaveDataManager.instance.Save_JsonData();

        currentEvent = null;
        isPlayingEvent = false;

        Set_Input(true);
        Set_BlackUpDownCover(false);
        Set_AnotherUI(true);
    }

    // 인풋 => On / Off
    public void Set_Input(bool onOff)
    {
        if (isPlayingEvent) return;

        UnityEngine.Debug.Log("Input " + (onOff ? "On" : "Off"));
        if (onOff)
        {
            InputManager.instance.SetOnOff_InputAction(StageManager.instance.targetStageId, true);

            InputManager.instance.Set_AllPointer(aim: true, mouse: false);
            InputManager.instance.canMouseInput = true;
        }
        else
        {
            InputManager.instance.SetOnOff_InputAction(StageManager.instance.targetStageId, false);

            InputManager.instance.Set_AllPointer(false);

            InputManager.instance.inputMoveDir = Vector2.zero;
            InputManager.instance.canMouseInput = false;
        }
    }

    // 블랙커버 위 아래 => On / Off
    public void Set_BlackUpDownCover(bool onOff)
    {
        if (DOTween.IsTweening(blackUpsideRT))
        { DOTween.Kill(blackUpsideRT); }

        if (DOTween.IsTweening(blackDownsideRT))
        { DOTween.Kill(blackDownsideRT); }

        if (onOff)
        {
            blackUpsideRT.DOAnchorPosY(0, 0.3f)
                .OnStart(() =>
                {
                    blackUpsideRT.gameObject.SetActive(true);
                });

            blackDownsideRT.DOAnchorPosY(0, 0.3f)
                .OnStart(() =>
                {
                    blackDownsideRT.gameObject.SetActive(true);
                });
        }
        else
        {
            float upsideY = blackUpsideRT.rect.height;
            blackUpsideRT.DOAnchorPosY(upsideY, 0.3f)
                .OnComplete(() =>
                {
                    blackUpsideRT.gameObject.SetActive(false);
                });

            float downsideY = blackDownsideRT.rect.height;
            blackDownsideRT.DOAnchorPosY(-downsideY, 0.3f)
                .OnComplete(() =>
                {
                    blackDownsideRT.gameObject.SetActive(false);
                });
        }
    }

    // 다른 UI => On / Off
    private void Set_AnotherUI(bool onOff)
    {
        MainGameUIManager.instance.uiParent.gameObject.SetActive(onOff);
    }

    #endregion

    #region Play (Kind of Condition)

    private IEnumerator Play_Stay_Cor(EventElement_Stay _event)
    {
        InputManager.instance.inputMoveDir = Vector2.zero; 

        yield return new WaitForSeconds(_event.targetTime);

        Play_Event();
    }

    private IEnumerator Play_Move_Cor(EventElement_Move _event)
    {
        Vector2 targetPos = _event.targetPos;
        Vector2 dir = Vector2.zero;

        InputManager.instance.inputMoveDir = Vector2.zero;
        if (_event.targetType != "None") // NPC 등 목표가 들어갈 부분
        {
            if (_event.targetType == "NPC") // NPC 등 목표가 들어갈 부분
            {
                NpcController npc = NpcManager.instance.Get_CorrectNPC(_event.targetId);
                if (npc != null)
                {
                    Vector2 npcPos = npc.transform.position;
                    targetPos += npcPos;
                    UnityEngine.Debug.Log(targetPos);
                }
            }
            else // 다른 목표가 있다면
            {

            }
        }

        while (0.01f < Vector2.Distance(targetPos, (Vector2)PlayerManager.instance.playerController.transform.position))
        {
            dir = (targetPos - (Vector2)PlayerManager.instance.playerController.transform.position).normalized;

            InputManager.instance.dirFromPlayerPos = dir;
            InputManager.instance.inputMoveDir = dir;

            yield return null;
        }
        InputManager.instance.inputMoveDir = Vector2.zero;

        Play_Event();
    }

    private IEnumerator Play_Look_Cor(EventElement_Look _event)
    {
        yield return new WaitForSeconds(0.5f);

        InputManager.instance.dirFromPlayerPos = _event.targetDir;

        PlayerManager.instance.playerController.rbLower.rb.velocity = Vector2.zero;

        PlayerManager.instance.playerController.rbLower.Set_Rot(_event.targetDir);


        yield return null;

        Play_Event();
    }

    private IEnumerator Play_BlackScreenIn_Cor(EventElement_BlackScreenIn _event)
    {
        if (DOTween.IsTweening(blackScreenImg))
        { DOTween.Kill(blackScreenImg); }

        blackScreenImg.DOFade(1, _event.targetTime)
            .OnStart(() =>
            {
                blackScreenImg.gameObject.SetActive(true);
            });

        yield return new WaitForSeconds(_event.targetTime);

        Play_Event();
    }

    private IEnumerator Play_BlackScreenOut_Cor(EventElement_BlackScreenOut _event)
    {
        if (DOTween.IsTweening(blackScreenImg))
        { DOTween.Kill(blackScreenImg); }

        blackScreenImg.DOFade(0, _event.targetTime)
            .OnComplete(() =>
            {
                blackScreenImg.gameObject.SetActive(false);
            });

        yield return new WaitForSeconds(_event.targetTime);

        Play_Event();
    }

    private IEnumerator Play_Dialogue_Cor(EventElement_Dialogue _event)
    {
        Start_Dialogue(_event);

        yield return new WaitUntil(() => !isPlayingDialogue);

        Play_Event();
    }

    private IEnumerator Play_Cutscene_Cor(EventElement_Cutscene _event)
    {
        Start_Cutscene(_event);

        yield return new WaitUntil(() => !isPlayingCutscene);

        Play_Event();
    }

    #endregion

    #region Dialogue

    private void Start_Dialogue(EventElement_Dialogue _event)
    {
        if (isPlayingDialogue) return;

        isPlayingDialogue = true;
        currentDialogues = GetCorrectDialogueElements(_event.targetDialogueId);
        StartCoroutine(Play_Dialogue_Cor());
    }

    // ID에 맞는 DialogueID를 가져온 후, 그에 맞는 DialogueElement List를 가져옴
    public List<DialogueElement> GetCorrectDialogueElements(int id)
    {
        List<DialogueElement> result = new List<DialogueElement>();

        int[] IDs = dialogueIds[id].dialogus;

        for (int i = 0; i < IDs.Length; i++)
        {
            DialogueElement cutsceneElement = dialogueElements[GameManager.languageID][IDs[i]];

            result.Add(cutsceneElement);
        }

        return result;
    }


    private IEnumerator Play_Dialogue_Cor()
    {
        bool canInteract = false;
        bool isScripting = false;
        Tween scriptingTween;

        while (true)
        {
            if (currentDialogues.Count <= 0)
            { break; }  

            DialogueElement currentDialogue = currentDialogues[0];

            // 기본 세팅
            SideDialogueComp targetDialogueComp = null;
            SideDialogueComp noneDialogueComp = null;
            if (currentDialogue.isLeft)
            { targetDialogueComp = leftDialogueComp; noneDialogueComp = rightDialogueComp; }
            else
            { targetDialogueComp = rightDialogueComp; noneDialogueComp = leftDialogueComp; }

            targetDialogueComp.dialogueGO.SetActive(true);
            noneDialogueComp.dialogueGO.SetActive(false);

            // Character Img
            string imgId = currentDialogue.imgID.StartsWith("Player") ?
                currentDialogue.imgID.Replace("Player", $"Player{DevTool.Get_LengthString(PlayerManager.instance.playerController.Get_ID(), 2)}") :
                currentDialogue.imgID;

            string[] imgString = imgId.Split("_");

            targetDialogueComp.dialogueImg.sprite = dialogueSpriteDict[imgString[0]][imgString[1]];

            // Name
            targetDialogueComp.nameTxt.text = ReplaceNPlaceholders(currentDialogue.name);

            // Script
            string targetScript = Get_ProductionString(currentDialogue.script);
            targetDialogueComp.dialogueTxt.text = "";
            isScripting = true;
            scriptingTween = targetDialogueComp.dialogueTxt
                .DOText(targetScript, targetScript.Length / 20f)
                .SetEase(Ease.Linear)
                .OnComplete(() => { isScripting = false; });

            while (true)
            {
                if (canInteract && Input.anyKeyDown)
                {
                    if (isScripting) // 스크립팅 중을 넘기기
                    {
                        scriptingTween.Complete();
                    }
                    else // 다음 스크립트로 넘기기
                    {
                        canInteract = false;
                        scriptingTween = null;
                        currentDialogues.Remove(currentDialogue);
                        break;
                    }
                }
                // 이것이 없으면, 한 프레임에
                // [다음 스크립트 넘기기]와 [스크립팅 중 넘기기]가 1프레임에 인식함
                canInteract = true; 
                yield return null;
            }

        }
        
        End_Dialogue();
    }

    private void End_Dialogue()
    {
        leftDialogueComp.dialogueGO.SetActive(false);
        rightDialogueComp.dialogueGO.SetActive(false);
        currentDialogues = null;

        isPlayingDialogue = false;
    }

    #endregion

    #region Cutscene

    private void Start_Cutscene(EventElement_Cutscene _event)
    {
        if (isPlayingCutscene) return; 

        SoundManager.instance.PlayCutsceneBgm(_event.targetSoundId);

        isPlayingCutscene = true;
        currentCutscenes = GetCorrectCutsceneElements(_event.targetCutsceneId);
        StartCoroutine(Play_Cutscene_Cor());
    }

    // ID에 맞는 CutsceneID를 가져온 후, 그에 맞는 CutsceneElement List를 가져옴
    public List<CutsceneElement> GetCorrectCutsceneElements(int id)
    {
        List<CutsceneElement> result = new List<CutsceneElement>();

        int[] ids = cutsceneIds[id].cutscenes;

        for (int i = 0; i < ids.Length; i++)
        {
            CutsceneElement cutsceneElement = cutsceneElements[GameManager.languageID][i];

            result.Add(cutsceneElement);
        }

        return result;
    }


    private IEnumerator Play_Cutscene_Cor()
    {
        cutsceneGO.gameObject.SetActive(true);

        bool canInteract = false;

        bool isAppearing = false;
        bool isDisappearing = false;

        bool eachComplete = false;

        Sequence seq;

        while (true)
        {
            if (currentCutscenes.Count <= 0)
            { break; }

            eachComplete = false;

            CutsceneElement currentCutscene = currentCutscenes[0];

            // Character Img
            Image img = imgQueueSet.Get_T();
            img.gameObject.SetActive(true);
            img.color = new Color(1f, 1f, 1f, 0f);
            img.sprite = StaticResourceManager.instance.EventReso.cutsceneSprites[currentCutscene.id];

            // Script
            seq = DOTween.Sequence();

            cutsceneTxt.text = "";

            string targetScrpit = Get_ProductionString(currentCutscene.script);

            isAppearing = true;
            seq.Join(cutsceneTxt.DOText(targetScrpit, targetScrpit.Length / 35f).SetEase(Ease.Linear));
            seq.Join(img.DOFade(1f, 3f).SetEase(Ease.Linear));
            seq.OnComplete(() => 
            { 
                isAppearing = false;
                cutsceneNextImg.gameObject.SetActive(true);
            });

            while (true)
            {
                if (!isAppearing && !isDisappearing && canInteract && Input.anyKeyDown)
                {
                    isDisappearing = true;
                    canInteract = false;
                    cutsceneNextImg.gameObject.SetActive(false);

                    seq = DOTween.Sequence();

                    cutsceneTxt.text = "";
                    seq.Join(img.DOFade(0f, 2f).SetEase(Ease.Linear));
                    seq.OnComplete(() =>
                    {
                        isDisappearing = false;
                        img.gameObject.SetActive(false);
                        currentCutscenes.Remove(currentCutscene);
                        eachComplete = true;
                    });
                }

                canInteract = true;

                yield return null;

                if (eachComplete)
                    break;
            }
        }

        End_Cutscene();
    }


    private void End_Cutscene()
    {
        cutsceneGO.gameObject.SetActive(false);

        currentCutscenes = null;

        isPlayingCutscene = false;

        SoundManager.instance.PlayCurrentStageBgm();
    }

    #endregion

    #region Prod. String

    private string Get_ProductionString(string s)
    {
        string result = s
            .Replace("<c>", ",")
            .Replace("<el>", "\n");

        result = ReplaceNPlaceholders(result);

        return result;
    }

    public string ReplaceNPlaceholders(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        const string marker = "<n>(";
        StringBuilder sb = null;
        int pos = 0;

        while (true)
        {
            int idx = s.IndexOf(marker, pos, StringComparison.Ordinal);
            if (idx < 0) break;

            int digitsStart = idx + marker.Length; // 숫자 시작 위치
            int i = digitsStart;

            // 숫자 파싱 (수동, 빠름)
            int val = 0;
            bool hasDigit = false;

            while (i < s.Length)
            {
                char c = s[i];
                if ((uint)(c - '0') <= 9)
                {
                    hasDigit = true;
                    // overflow 방지용 (int 범위 넘어가면 실패 처리)
                    int next = val * 10 + (c - '0');
                    if (next < val) { hasDigit = false; break; }
                    val = next;
                    i++;
                }
                else break;
            }

            // 패턴 유효성: 최소 1개 숫자 + 닫는 괄호 ')'
            bool valid = hasDigit && i < s.Length && s[i] == ')';

            if (!valid)
            {
                // 유효하지 않으면 "<n>"까지만 지나가며 계속
                if (sb == null) sb = new StringBuilder(s.Length);
                sb.Append(s, pos, (idx + 3) - pos); // "<n>"까지 복사
                pos = idx + 3;
                continue;
            }

            // 유효: 교체 수행
            if (sb == null) sb = new StringBuilder(s.Length + 16);
            sb.Append(s, pos, idx - pos); // 패턴 앞부분 복사

            string replacement = StaticResourceManager.instance.properNouns.GetLanguage(val);
            sb.Append(replacement);

            pos = i + 1; // ')' 다음 위치로 진행
        }

        if (sb == null) return s; // 교체된 게 하나도 없으면 원문 반환
        if (pos < s.Length) sb.Append(s, pos, s.Length - pos); // 꼬리 붙이기
        return sb.ToString();
    }

    #endregion
}

#region Event

[Serializable]
public class EventData
{
    public int id;
    public List<EventElement> events;

    public EventData(int id, List<EventElement> events)
    {
        this.id = id;
        this.events = events;
    }

    public void Remove_OnePart()
    {
        events.Remove(events[0]);
    }
}

#region Event ID

[Serializable]
public class EventID
{
    public int id;
    public int[] eventIds;

    public EventID() { }

    public EventID(int id, int[] eventIds)
    {
        this.id = id;
        this.eventIds = eventIds;
    }
}

#endregion

#region Event Kind of Element

[Serializable]
public class EventElement
{
    public int id;

    public EventElement() { }

    public EventElement(int id)
    {
        this.id = id;
    }
}

[Serializable]
public class EventElement_Stay : EventElement
{
    public float targetTime;

    public EventElement_Stay(int id, float targetTime) : base(id)
    {
        this.targetTime = targetTime;
    }
}

[Serializable]
public class EventElement_Look : EventElement
{
    public Vector2 targetDir;

    public EventElement_Look(int id, Vector2 targetDir) : base(id)
    {
        this.targetDir = targetDir;
    }
}

[Serializable]
public class EventElement_Move : EventElement
{
    public int targetId;
    public string targetType;
    public Vector2 targetPos;

    public EventElement_Move(int id, int targetId, string targetType, Vector2 targetPos) : base(id)
    {
        this.targetId = targetId;
        this.targetType = targetType;
        this.targetPos = targetPos;
    }
}

[Serializable]
public class EventElement_BlackScreenIn : EventElement
{
    public float targetTime;

    public EventElement_BlackScreenIn(int id, float targetTime) : base(id)
    {
        this.targetTime = targetTime;
    }
}

[Serializable]
public class EventElement_BlackScreenOut : EventElement
{
    public float targetTime;

    public EventElement_BlackScreenOut(int id, float targetTime) : base(id)
    {
        this.targetTime = targetTime;
    }
}

[Serializable]
public class EventElement_Dialogue : EventElement
{
    public int targetDialogueId;

    public EventElement_Dialogue(int id, int targetId) : base(id)
    {
        targetDialogueId = targetId;
    }
    
}

[Serializable]
public class EventElement_Cutscene : EventElement
{
    public int targetCutsceneId;
    public int targetSoundId;

    public EventElement_Cutscene(int id, int targetCutsceneId, int targetSoundId) : base(id)
    {
        this.targetCutsceneId = targetCutsceneId;
        this.targetSoundId = targetSoundId;
    }
}


#endregion

#region Cutscene

[Serializable]
public class CutsceneID
{
    public int id;
    public int[] cutscenes;

    public CutsceneID(int id, int[] cutscenes)
    {
        this.id = id;
        this.cutscenes = cutscenes;
    }
}

[Serializable]
public class CutsceneElement
{
    public int id;
    public string script;

    public CutsceneElement(int id, string script)
    {
        this.id = id;
        this.script = script;
    }
}

#endregion

#region Dialogue

[Serializable]
public class DialogueID
{
    public int id;
    public int[] dialogus;

    public DialogueID(int _ID, int[] _Dialogus)
    {
        id = _ID;
        dialogus = _Dialogus;
    }
}

[Serializable]
public class DialogueElement
{
    public int id;
    public string name;
    public string script;
    public string imgID;
    public bool isLeft;

    public DialogueElement(int id, string name, string script, string imgId, bool isLeft)
    {
        this.id = id;
        this.name = name;
        this.script = script;
        imgID = imgId;
        this.isLeft = isLeft;
    }
}

#endregion

#endregion
