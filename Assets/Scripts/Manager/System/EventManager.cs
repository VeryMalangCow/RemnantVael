using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Event")]

    [Space(10)]
    [Header("=== Managing Value")]
    [SerializeField] private bool IsPlayingEvent = false;

    [Space(10)]
    [Header("=== Base Comp")]
    [SerializeField] private RectTransform BlackUpsideRT;
    [SerializeField] private RectTransform BlackDownsideRT;

    [Space(10)]
    [Header("=== BlackScreen")]
    [SerializeField] private Image BlackScreenImg;

    [Space(10)]
    [Header("=== Dialogue")]
    [SerializeField] private bool IsPlayingDialogue = false;

    [SerializeField] private SideDialogueComp LeftDialogueComp;
    [SerializeField] private SideDialogueComp RightDialogueComp;

    [Serializable]
    public class SideDialogueComp
    {
        [SerializeField] public GameObject DialogueGO;
        [SerializeField] public Image DialogueImg;
        [SerializeField] public TMP_Text NameTxt;
        [SerializeField] public TMP_Text DialogueTxt;
    }

    [HideInInspector] private List<DialogueElement> CurrentDialogues;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private EventData CurrentEvent = new EventData();

    [HideInInspector] private PlayerController PC;
    [HideInInspector] private TitlePlayerManager TitlePC;

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
        if (EventManager.Instance == this)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        
    }

    private void Update()
    {
        // Test Input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartEvent(0);
        }
    }

    #endregion

    #region Event Start

    // 이벤트 시작
    public void StartEvent(int _ID)
    {
        // 다른 이벤트 중이라면 취소
        if (IsPlayingEvent)
        { return; }

        EventSettingOn();

        CurrentEvent = new EventData();
        CurrentEvent.Events = new List<EventElement>(CSVManager.Instance.GetCorrectEventList(_ID));
        PlayEvent();
    }

    // 이벤트 실행
    private void PlayEvent()
    {
        if (CurrentEvent.Events.Count <= 0)
        {
            EventSettingOff();
            return;
        }

        if (CurrentEvent.Events[0] is EventElement_Stay stay)
        { StartCoroutine(PlayEvent_Stay(stay)); }
        else if (CurrentEvent.Events[0] is EventElement_Move move)
        { StartCoroutine(PlayEvent_Move(move)); }
        else if (CurrentEvent.Events[0] is EventElement_Look look)
        { StartCoroutine(PlayEvent_Look(look)); }
        else if (CurrentEvent.Events[0] is EventElement_BlackScreenIn blackScreenIn)
        { StartCoroutine(PlayEvent_BlackScreenIn(blackScreenIn)); }
        else if (CurrentEvent.Events[0] is EventElement_BlackScreenOut blackScreenOut)
        { StartCoroutine(PlayEvent_BlackScreenOut(blackScreenOut)); }
        else if (CurrentEvent.Events[0] is EventElement_Dialogue dialogue)
        { StartCoroutine(PlayEvent_Dialogue(dialogue)); }

        CurrentEvent.ClearOnePart();
    }


    // 이벤트 실행 시, 설정 온
    private void EventSettingOn()
    {
        IsPlayingEvent = true;

        SetInputSetting(false);
        SetBlackUpDownCover(true);
        SetAnotherUISetting(false);
    }

    // 이벤트 실행 시, 설정 오프
    private void EventSettingOff()
    {
        CurrentEvent = null;
        IsPlayingEvent = false;

        SetInputSetting(true);
        SetBlackUpDownCover(false);
        SetAnotherUISetting(true);
    }

    // 인풋 => On / Off
    public void SetInputSetting(bool _OnOff)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (_OnOff)
        {
            if (sceneName == "TitleLobby")
            {
                TitleInputManager.Instance.OnEnableInput();
            }
            else if (sceneName == "MainGame")
            {
                InputManager.Instance.OnEnableInput();
                InputManager.Instance.SetAim(true);

                InputManager.Instance.CanMouseInput = true;
            }
        }
        else
        {
            if (sceneName == "TitleLobby")
            {
                TitleInputManager.Instance.OnDisableInput();
            }
            else if (sceneName == "MainGame")
            {
                InputManager.Instance.OnDisableInput();
                InputManager.Instance.SetAimAllOff();

                InputManager.Instance.InputMoveDir = Vector2.zero;
                InputManager.Instance.CanMouseInput = false;
            }
        }
    }

    // 블랙커버 위 아래 => On / Off
    public void SetBlackUpDownCover(bool _OnOff)
    {
        if (DOTween.IsTweening(BlackUpsideRT))
        { DOTween.Kill(BlackUpsideRT); }

        if (DOTween.IsTweening(BlackDownsideRT))
        { DOTween.Kill(BlackDownsideRT); }

        if (_OnOff)
        {
            BlackUpsideRT.DOAnchorPosY(0, 0.3f)
                .OnStart(() =>
                {
                    BlackUpsideRT.gameObject.SetActive(true);
                });

            BlackDownsideRT.DOAnchorPosY(0, 0.3f)
                .OnStart(() =>
                {
                    BlackDownsideRT.gameObject.SetActive(true);
                });
        }
        else
        {
            float upsideY = BlackUpsideRT.rect.height;
            BlackUpsideRT.DOAnchorPosY(upsideY, 0.3f)
                .OnComplete(() =>
                {
                    BlackUpsideRT.gameObject.SetActive(false);
                });

            float downsideY = BlackDownsideRT.rect.height;
            BlackDownsideRT.DOAnchorPosY(-downsideY, 0.3f)
                .OnComplete(() =>
                {
                    BlackDownsideRT.gameObject.SetActive(false);
                });
        }
    }

    // 다른 UI => On / Off
    private void SetAnotherUISetting(bool _OnOff)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TitleLobby")
        {
#if UNITY_EDITOR
            Debug.Log("필요: 타이틀 로비의 UI 끄기");
#endif
        }
        else if (sceneName == "MainGame")
        {
            MainGameUIManager.Instance.UIParent.gameObject.SetActive(_OnOff);
        }
    }
    #endregion

    #region Play (Kind of Condition)

    private IEnumerator PlayEvent_Stay(EventElement_Stay _Event)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TitleLobby") // 맵 구분
        { 
            TitleInputManager.Instance.InputMoveDir = Vector2.zero; 
        }
        else if (sceneName == "MainGame") // 맵 구분
        { 
            InputManager.Instance.InputMoveDir = Vector2.zero; 
        }
        yield return new WaitForSeconds(_Event.TargetTime);

        PlayEvent();
    }

    private IEnumerator PlayEvent_Move(EventElement_Move _Event)
    {
        Vector2 targetPos = _Event.TargetPos;
        Vector2 dir = Vector2.zero;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TitleLobby") // 맵 구분
        {
            TitleInputManager.Instance.InputMoveDir = Vector2.zero;
            if (_Event.TargetType != "None")
            {
                if (_Event.TargetType == "NPC") // NPC 등 목표가 들어갈 부분
                {
                    Vector2 npcPos = NPCManager.Instance.GetNPC(_Event.TargetID).transform.position;
                    targetPos += npcPos;
                }
                else // 다른 목표가 있다면
                {

                }
            }
            

            while (0.01f < Vector2.Distance(targetPos, (Vector2)TitlePlayerManager.Instance.PlayerController.transform.position))
            {
                dir = (targetPos - (Vector2)TitlePlayerManager.Instance.PlayerController.transform.position).normalized;
                
                TitleInputManager.Instance.InputMoveDir = dir;

                yield return null;
            }
            TitleInputManager.Instance.InputMoveDir = Vector2.zero;
        }
        else if (sceneName == "MainGame") // 맵 구분
        {
            InputManager.Instance.InputMoveDir = Vector2.zero;
            if (_Event.TargetType != "None") // NPC 등 목표가 들어갈 부분
            {
                if (_Event.TargetType == "NPC") // NPC 등 목표가 들어갈 부분
                {
                    NPCController npc = NPCManager.Instance.GetNPC(_Event.TargetID);
                    if (npc != null)
                    {
                        Vector2 npcPos = npc.transform.position;
                        targetPos += npcPos;
                    }
                }
                else // 다른 목표가 있다면
                {

                }
            }

            while (0.01f < Vector2.Distance(targetPos, (Vector2)PlayerManager.Instance.PlayerController.transform.position))
            {
                dir = (targetPos - (Vector2)PlayerManager.Instance.PlayerController.transform.position).normalized;
                
                InputManager.Instance.DirFromPlayerPos = dir;
                InputManager.Instance.InputMoveDir = dir; 

                yield return null;
            }
            InputManager.Instance.InputMoveDir = Vector2.zero;
        }

        PlayEvent();
    }

    private IEnumerator PlayEvent_Look(EventElement_Look _Event)
    {
        yield return new WaitForSeconds(0.5f);
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TitleLobby") // 맵 구분
        {
            TitleInputManager.Instance.InputMoveDir = Vector2.zero;

            TitlePlayerManager.Instance.PlayerController.HigherBody.SetRotation(_Event.TargetDir);
        }
        else if (sceneName == "MainGame") // 맵 구분
        {
            InputManager.Instance.DirFromPlayerPos = _Event.TargetDir;

            PlayerManager.Instance.PlayerController.LowerController.ThisRb.velocity = Vector2.zero;

            PlayerManager.Instance.PlayerController.LowerController.SetRotation(_Event.TargetDir);
        }

        yield return null;

        PlayEvent();
    }

    private IEnumerator PlayEvent_BlackScreenIn(EventElement_BlackScreenIn _Event)
    {
        if (DOTween.IsTweening(BlackScreenImg))
        { DOTween.Kill(BlackScreenImg); }

        BlackScreenImg.DOFade(1, _Event.TargetTime)
            .OnStart(() =>
            {
                BlackScreenImg.gameObject.SetActive(true);
            });

        yield return new WaitForSeconds(_Event.TargetTime);

        PlayEvent();
    }

    private IEnumerator PlayEvent_BlackScreenOut(EventElement_BlackScreenOut _Event)
    {
        if (DOTween.IsTweening(BlackScreenImg))
        { DOTween.Kill(BlackScreenImg); }

        BlackScreenImg.DOFade(0, _Event.TargetTime)
            .OnComplete(() =>
            {
                BlackScreenImg.gameObject.SetActive(false);
            });

        yield return new WaitForSeconds(_Event.TargetTime);

        PlayEvent();
    }

    private IEnumerator PlayEvent_Dialogue(EventElement_Dialogue _Event)
    {
        DialogueID id = _Event.GetDialogueList();

        StartDialogue(id);
        yield return new WaitUntil(() => !IsPlayingDialogue);

        PlayEvent();
    }

    #endregion

    #region Dialogue

    private void StartDialogue(DialogueID _DialogueID)
    {
        if (IsPlayingDialogue)
        { return; }

        IsPlayingDialogue = true;
        CurrentDialogues = new List<DialogueElement>(_DialogueID.Dialogues);
        StartCoroutine(PlayDialogue());
    }

    private IEnumerator PlayDialogue()
    {
        bool canInteract = false;
        bool isScripting = false;
        Tween scriptingTween;

        while (true)
        {
            if (CurrentDialogues.Count <= 0)
            { break; }  

            DialogueElement currentDialogue = CurrentDialogues[0];

            // 기본 세팅
            SideDialogueComp targetDialogueComp = null;
            SideDialogueComp noneDialogueComp = null;
            if (currentDialogue.IsLeft)
            { targetDialogueComp = LeftDialogueComp; noneDialogueComp = RightDialogueComp; }
            else
            { targetDialogueComp = RightDialogueComp; noneDialogueComp = LeftDialogueComp; }

            targetDialogueComp.DialogueGO.SetActive(true);
            noneDialogueComp.DialogueGO.SetActive(false);

            // Character Img
            targetDialogueComp.DialogueImg.sprite = CSVManager.Instance.GetCorrectCharacterImg(currentDialogue.ImgID);

            // Name
            targetDialogueComp.NameTxt.text = currentDialogue.Name;

            // Script
            targetDialogueComp.DialogueTxt.text = "";
            isScripting = true;
            scriptingTween = targetDialogueComp.DialogueTxt
                .DOText(currentDialogue.Script, currentDialogue.Script.Length / 10f)
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
                        CurrentDialogues.Remove(currentDialogue);
                        break;
                    }
                }
                // 이것이 없으면, 한 프레임에
                // [다음 스크립트 넘기기]와 [스크립팅 중 넘기기]가 1프레임에 인식함
                canInteract = true; 
                yield return null;
            }

        }
        
        EndDialogue();
    }

    private void EndDialogue()
    {
        LeftDialogueComp.DialogueGO.SetActive(false);
        RightDialogueComp.DialogueGO.SetActive(false);
        CurrentDialogues = null;

        IsPlayingDialogue = false;
    }

    #endregion

}

#region Event

[Serializable]
public class EventData
{
    public List<EventElement> Events;

    public void ClearOnePart()
    {
        Events.Remove(Events[0]);
    }
}

#region Event ID

[Serializable]
public class EventID
{
    public int ID;
    public List<int> EventIDs;

    public EventID() { }

    public EventID(int _ID, List<int> _EventIDs)
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
    public int TargetID;

    public EventElement_Dialogue(int _ID, int _TargetID) : base(_ID)
    {
        TargetID = _TargetID;
    }

    public DialogueID GetDialogueList()
    {
       return CSVManager.Instance.GetCorrectDialogueID(TargetID);
    }

}

#endregion

#region Dialogue

[Serializable]
public class DialogueID
{
    public int ID;
    public List<DialogueElement> Dialogues;

    public DialogueID(int _ID, List<DialogueElement> _Dialogues)
    {
        ID = _ID;
        Dialogues = _Dialogues;
    }
}

[Serializable]
public class DialogueElement
{
    public int ID;
    public string Name;
    public string Script;
    public int ImgID;
    public bool IsLeft;

    public DialogueElement(int _ID, string _Name, string _Script, int _ImgID, bool _IsLeft)
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
