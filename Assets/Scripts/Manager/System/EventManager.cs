using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    #region Value

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
        TryStart_Event(0);
    }

    #endregion

    #region Offset

    private void Offset()
    {
        imgQueueSet.Offset();
    }

    #endregion

    #region Event Start

    public void TryStart_Event(int _ID)
    {
        if (SaveDataManager.instance.jsonData.gameProgressData.currentProgressing + 1 == _ID)
            Start_Event(_ID);
    }

    // 이벤트 시작
    private void Start_Event(int _ID)
    {
        // 다른 이벤트 중이라면 취소
        if (isPlayingEvent)
        { return; }

        SetOn_EventOption();

        currentEvent = new EventData(_ID, new List<EventElement>(ResourceManager.instance.Get_CorrectEventArr(_ID)));
        Play_Event();
    }

    // 이벤트 실행
    private void Play_Event()
    {
        if (currentEvent.Events.Count <= 0)
        {
            SetOff_EventOption();
            return;
        }

        if (currentEvent.Events[0] is EventElement_Stay stay)
        { StartCoroutine(Play_Stay_Cor(stay)); }
        else if (currentEvent.Events[0] is EventElement_Move move)
        { StartCoroutine(Play_Move_Cor(move)); }
        else if (currentEvent.Events[0] is EventElement_Look look)
        { StartCoroutine(Play_Look_Cor(look)); }
        else if (currentEvent.Events[0] is EventElement_BlackScreenIn blackScreenIn)
        { StartCoroutine(Play_BlackScreenIn_Cor(blackScreenIn)); }
        else if (currentEvent.Events[0] is EventElement_BlackScreenOut blackScreenOut)
        { StartCoroutine(Play_BlackScreenOut_Cor(blackScreenOut)); }
        else if (currentEvent.Events[0] is EventElement_Dialogue dialogue)
        { StartCoroutine(Play_Dialogue_Cor(dialogue)); }
        else if (currentEvent.Events[0] is EventElement_Cutscene cutscene)
        { StartCoroutine(Play_Cutscene_Cor(cutscene)); }

        currentEvent.Remove_OnePart();
    }


    // 이벤트 실행 시, 설정 온
    private void SetOn_EventOption()
    {
        isPlayingEvent = true;

        Set_Input(false);
        Set_BlackUpDownCover(true);
        Set_AnotherUI(false);
    }

    // 이벤트 실행 시, 설정 오프
    private void SetOff_EventOption()
    {
        bool needSave = false;
        switch (currentEvent.ID)
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
    public void Set_Input(bool _OnOff)
    {
        if (isPlayingEvent) return;

        Debug.Log("Input " + (_OnOff ? "On" : "Off"));
        if (_OnOff)
        {
            InputManager.instance.SetOnOff_InputAction(StageManager.instance.targetStageID, true);

            InputManager.instance.Set_AllPointer(_Aim: true, _Mouse: false);
            InputManager.instance.canMouseInput = true;
        }
        else
        {
            InputManager.instance.SetOnOff_InputAction(StageManager.instance.targetStageID, false);

            InputManager.instance.Set_AllPointer(false);

            InputManager.instance.inputMoveDir = Vector2.zero;
            InputManager.instance.canMouseInput = false;
        }
    }

    // 블랙커버 위 아래 => On / Off
    public void Set_BlackUpDownCover(bool _OnOff)
    {
        if (DOTween.IsTweening(blackUpsideRT))
        { DOTween.Kill(blackUpsideRT); }

        if (DOTween.IsTweening(blackDownsideRT))
        { DOTween.Kill(blackDownsideRT); }

        if (_OnOff)
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
    private void Set_AnotherUI(bool _OnOff)
    {
        MainGameUIManager.instance.uiParent.gameObject.SetActive(_OnOff);
    }

    #endregion

    #region Play (Kind of Condition)

    private IEnumerator Play_Stay_Cor(EventElement_Stay _Event)
    {
        InputManager.instance.inputMoveDir = Vector2.zero; 

        yield return new WaitForSeconds(_Event.TargetTime);

        Play_Event();
    }

    private IEnumerator Play_Move_Cor(EventElement_Move _Event)
    {
        Vector2 targetPos = _Event.TargetPos;
        Vector2 dir = Vector2.zero;

        InputManager.instance.inputMoveDir = Vector2.zero;
        if (_Event.TargetType != "None") // NPC 등 목표가 들어갈 부분
        {
            if (_Event.TargetType == "NPC") // NPC 등 목표가 들어갈 부분
            {
                NPCController npc = NPCManager.instance.Get_CorrectNPC(_Event.TargetID);
                if (npc != null)
                {
                    Vector2 npcPos = npc.transform.position;
                    targetPos += npcPos;
                    Debug.Log(targetPos);
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

    private IEnumerator Play_Look_Cor(EventElement_Look _Event)
    {
        yield return new WaitForSeconds(0.5f);

        InputManager.instance.dirFromPlayerPos = _Event.TargetDir;

        PlayerManager.instance.playerController.LowerController.ThisRb.velocity = Vector2.zero;

        PlayerManager.instance.playerController.LowerController.Set_Rot(_Event.TargetDir);


        yield return null;

        Play_Event();
    }

    private IEnumerator Play_BlackScreenIn_Cor(EventElement_BlackScreenIn _Event)
    {
        if (DOTween.IsTweening(blackScreenImg))
        { DOTween.Kill(blackScreenImg); }

        blackScreenImg.DOFade(1, _Event.TargetTime)
            .OnStart(() =>
            {
                blackScreenImg.gameObject.SetActive(true);
            });

        yield return new WaitForSeconds(_Event.TargetTime);

        Play_Event();
    }

    private IEnumerator Play_BlackScreenOut_Cor(EventElement_BlackScreenOut _Event)
    {
        if (DOTween.IsTweening(blackScreenImg))
        { DOTween.Kill(blackScreenImg); }

        blackScreenImg.DOFade(0, _Event.TargetTime)
            .OnComplete(() =>
            {
                blackScreenImg.gameObject.SetActive(false);
            });

        yield return new WaitForSeconds(_Event.TargetTime);

        Play_Event();
    }

    private IEnumerator Play_Dialogue_Cor(EventElement_Dialogue _Event)
    {
        Start_Dialogue(_Event);

        yield return new WaitUntil(() => !isPlayingDialogue);

        Play_Event();
    }

    private IEnumerator Play_Cutscene_Cor(EventElement_Cutscene _Event)
    {
        Start_Cutscene(_Event);

        yield return new WaitUntil(() => !isPlayingCutscene);

        Play_Event();
    }

    #endregion

    #region Dialogue

    private void Start_Dialogue(EventElement_Dialogue _Event)
    {
        if (isPlayingDialogue) return;

        isPlayingDialogue = true;
        currentDialogues = ResourceManager.instance.Get_CorrectDialogueElementList(_Event.TargetDialogueID);
        StartCoroutine(Play_Dialogue_Cor());
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
            if (currentDialogue.IsLeft)
            { targetDialogueComp = leftDialogueComp; noneDialogueComp = rightDialogueComp; }
            else
            { targetDialogueComp = rightDialogueComp; noneDialogueComp = leftDialogueComp; }

            targetDialogueComp.dialogueGO.SetActive(true);
            noneDialogueComp.dialogueGO.SetActive(false);

            // Character Img
            string imgId = currentDialogue.ImgID.StartsWith("Player") ?
                currentDialogue.ImgID.Replace("Player", $"Player{DevTool.Get_LengthString(PlayerManager.instance.playerController.Get_ID(), 2)}") :
                currentDialogue.ImgID;
            targetDialogueComp.dialogueImg.sprite = ResourceManager.instance.Get_DialogueCharImg(imgId);

            // Name
            targetDialogueComp.nameTxt.text = ReplaceNPlaceholders(currentDialogue.Name);

            // Script
            string targetScript = Get_ProductionString(currentDialogue.Script);
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

    private void Start_Cutscene(EventElement_Cutscene _Event)
    {
        if (isPlayingCutscene) return; 

        SoundManager.instance.Play_2D_BGM_Cutscene(_Event.TargetSoundID);

        isPlayingCutscene = true;
        currentCutscenes = ResourceManager.instance.Get_CorrectCutsceneElementList(_Event.TargetCutsceneID);
        StartCoroutine(Play_Cutscene_Cor());
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
            img.sprite = ResourceManager.instance.Get_CutsceneImg(currentCutscene.ID);

            // Script
            seq = DOTween.Sequence();

            cutsceneTxt.text = "";

            string targetScrpit = Get_ProductionString(currentCutscene.Script);

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

        SoundManager.instance.Play_2D_BGM_Stage(StageManager.instance.Get_CurrentStageData().infoData.stageId);
    }

    #endregion

    #region Prod. String

    private string Get_ProductionString(string _String)
    {
        string result = _String
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

            string replacement = ResourceManager.instance.Get_ProperNounWord(val);
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
    public int ID;
    public List<EventElement> Events;

    public EventData(int _ID, List<EventElement> _Events)
    {
        ID = _ID;
        Events = _Events;
    }

    public void Remove_OnePart()
    {
        Events.Remove(Events[0]);
    }
}

#region Event ID

[Serializable]
public class EventID
{
    public int ID;
    public int[] EventIDs;

    public EventID() { }

    public EventID(int _ID, int[] _EventIDs)
    {
        ID = _ID;
        EventIDs = _EventIDs;
    }
}

#endregion

#region Event Kind of Element

[Serializable]
public class EventElement
{
    public int ID;

    public EventElement() { }

    public EventElement(int _ID)
    {
        ID = _ID;
    }
}

[Serializable]
public class EventElement_Stay : EventElement
{
    public float TargetTime;

    public EventElement_Stay(int _ID, float _TargetTime) : base(_ID)
    {
        TargetTime = _TargetTime;
    }
}

[Serializable]
public class EventElement_Look : EventElement
{
    public Vector2 TargetDir;

    public EventElement_Look(int _ID, Vector2 _TargetDir) : base(_ID)
    {
        TargetDir = _TargetDir;
    }
}

[Serializable]
public class EventElement_Move : EventElement
{
    public int TargetID;
    public string TargetType;
    public Vector2 TargetPos;

    public EventElement_Move(int _ID, int _TargetID, string _TargetType, Vector2 _TargetPos) : base(_ID)
    {
        TargetID = _TargetID;
        TargetType = _TargetType;
        TargetPos = _TargetPos;
    }
}

[Serializable]
public class EventElement_BlackScreenIn : EventElement
{
    public float TargetTime;

    public EventElement_BlackScreenIn(int _ID, float _TargetTime) : base(_ID)
    {
        TargetTime = _TargetTime;
    }
}

[Serializable]
public class EventElement_BlackScreenOut : EventElement
{
    public float TargetTime;

    public EventElement_BlackScreenOut(int _ID, float _TargetTime) : base(_ID)
    {
        TargetTime = _TargetTime;
    }
}

[Serializable]
public class EventElement_Dialogue : EventElement
{
    public int TargetDialogueID;

    public EventElement_Dialogue(int _ID, int _TargetID) : base(_ID)
    {
        TargetDialogueID = _TargetID;
    }
    
}

[Serializable]
public class EventElement_Cutscene : EventElement
{
    public int TargetCutsceneID;
    public int TargetSoundID;

    public EventElement_Cutscene(int _ID, int _TargetID, int _TargetSoundID) : base(_ID)
    {
        TargetCutsceneID = _TargetID;
        TargetSoundID = _TargetSoundID;
    }
}


#endregion

#region Cutscene

[Serializable]
public class CutsceneID
{
    public int ID;
    public int[] Cutscenes;

    public CutsceneID(int _ID, int[] _Cutscenes)
    {
        ID = _ID;
        Cutscenes = _Cutscenes;
    }
}

[Serializable]
public class CutsceneElement
{
    public int ID;
    public string Script;

    public CutsceneElement(int _ID, string _Script)
    {
        ID = _ID;
        Script = _Script;
    }
}

#endregion

#region Dialogue

[Serializable]
public class DialogueID
{
    public int ID;
    public int[] Dialogus;

    public DialogueID(int _ID, int[] _Dialogus)
    {
        ID = _ID;
        Dialogus = _Dialogus;
    }
}

[Serializable]
public class DialogueElement
{
    public int ID;
    public string Name;
    public string Script;
    public string ImgID;
    public bool IsLeft;

    public DialogueElement(int _ID, string _Name, string _Script, string _ImgID, bool _IsLeft)
    {
        ID = _ID;
        Name = _Name;
        Script = _Script;
        ImgID = _ImgID;
        IsLeft = _IsLeft;
    }
}

#endregion

#endregion
