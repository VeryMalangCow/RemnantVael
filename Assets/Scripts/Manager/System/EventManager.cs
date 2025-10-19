using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    [Header("=== Cutscene")]
    [SerializeField] private bool IsPlayingCutscene = false;
    [SerializeField] private GameObject CutsceneGO;
    [SerializeField] private ImgQueueSet ImgQueueSet;
    [SerializeField] private TMP_Text CutsceneTxt;
    [HideInInspector] private List<CutsceneElement> CurrentCutscenes;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private EventData CurrentEvent = new EventData();

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake(); 

        Offset();
    }

    #endregion

    #region Offset

    private void Offset()
    {
        ImgQueueSet.Offset();
    }

    #endregion

    #region Event Start

    // 이벤트 시작
    public void Start_Event(int _ID)
    {
        // 다른 이벤트 중이라면 취소
        if (IsPlayingEvent)
        { return; }

        SetOn_EventOption();

        CurrentEvent = new EventData();
        CurrentEvent.Events = new List<EventElement>(ResourceManager.Instance.Get_CorrectEventList(_ID));
        Play_Event();
    }

    // 이벤트 실행
    private void Play_Event()
    {
        if (CurrentEvent.Events.Count <= 0)
        {
            SetOff_EventOption();
            return;
        }

        if (CurrentEvent.Events[0] is EventElement_Stay stay)
        { StartCoroutine(Play_Stay_Cor(stay)); }
        else if (CurrentEvent.Events[0] is EventElement_Move move)
        { StartCoroutine(Play_Move_Cor(move)); }
        else if (CurrentEvent.Events[0] is EventElement_Look look)
        { StartCoroutine(Play_Look_Cor(look)); }
        else if (CurrentEvent.Events[0] is EventElement_BlackScreenIn blackScreenIn)
        { StartCoroutine(Play_BlackScreenIn_Cor(blackScreenIn)); }
        else if (CurrentEvent.Events[0] is EventElement_BlackScreenOut blackScreenOut)
        { StartCoroutine(Play_BlackScreenOut_Cor(blackScreenOut)); }
        else if (CurrentEvent.Events[0] is EventElement_Dialogue dialogue)
        { StartCoroutine(Play_Dialogue_Cor(dialogue)); }
        else if (CurrentEvent.Events[0] is EventElement_Cutscene cutscene)
        { StartCoroutine(Play_Cutscene_Cor(cutscene)); }

        CurrentEvent.Remove_OnePart();
    }


    // 이벤트 실행 시, 설정 온
    private void SetOn_EventOption()
    {
        IsPlayingEvent = true;

        Set_Input(false);
        Set_BlackUpDownCover(true);
        Set_AnotherUI(false);
    }

    // 이벤트 실행 시, 설정 오프
    private void SetOff_EventOption()
    {
        CurrentEvent = null;
        IsPlayingEvent = false;

        Set_Input(true);
        Set_BlackUpDownCover(false);
        Set_AnotherUI(true);
    }

    // 인풋 => On / Off
    public void Set_Input(bool _OnOff)
    {
        Debug.Log("Input " + (_OnOff ? "On" : "Off"));
        if (_OnOff)
        {
            InputManager.Instance.SetOnOff_InputAction(StageManager.Instance.TargetStageID, true);

            InputManager.Instance.Set_AllPointer(_Aim: true, _Mouse: false);
            InputManager.Instance.CanMouseInput = true;
        }
        else
        {
            InputManager.Instance.SetOnOff_InputAction(StageManager.Instance.TargetStageID, false);

            InputManager.Instance.Set_AllPointer(false);

            InputManager.Instance.InputMoveDir = Vector2.zero;
            InputManager.Instance.CanMouseInput = false;
        }
    }

    // 블랙커버 위 아래 => On / Off
    public void Set_BlackUpDownCover(bool _OnOff)
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
    private void Set_AnotherUI(bool _OnOff)
    {
        MainGameUIManager.Instance.UIParent.gameObject.SetActive(_OnOff);
    }

    #endregion

    #region Play (Kind of Condition)

    private IEnumerator Play_Stay_Cor(EventElement_Stay _Event)
    {
        InputManager.Instance.InputMoveDir = Vector2.zero; 

        yield return new WaitForSeconds(_Event.TargetTime);

        Play_Event();
    }

    private IEnumerator Play_Move_Cor(EventElement_Move _Event)
    {
        Vector2 targetPos = _Event.TargetPos;
        Vector2 dir = Vector2.zero;

        InputManager.Instance.InputMoveDir = Vector2.zero;
        if (_Event.TargetType != "None") // NPC 등 목표가 들어갈 부분
        {
            if (_Event.TargetType == "NPC") // NPC 등 목표가 들어갈 부분
            {
                NPCController npc = NPCManager.Instance.Get_CorrectNPC(_Event.TargetID);
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

        Play_Event();
    }

    private IEnumerator Play_Look_Cor(EventElement_Look _Event)
    {
        yield return new WaitForSeconds(0.5f);

        InputManager.Instance.DirFromPlayerPos = _Event.TargetDir;

        PlayerManager.Instance.PlayerController.LowerController.ThisRb.velocity = Vector2.zero;

        PlayerManager.Instance.PlayerController.LowerController.Set_Rot(_Event.TargetDir);


        yield return null;

        Play_Event();
    }

    private IEnumerator Play_BlackScreenIn_Cor(EventElement_BlackScreenIn _Event)
    {
        if (DOTween.IsTweening(BlackScreenImg))
        { DOTween.Kill(BlackScreenImg); }

        BlackScreenImg.DOFade(1, _Event.TargetTime)
            .OnStart(() =>
            {
                BlackScreenImg.gameObject.SetActive(true);
            });

        yield return new WaitForSeconds(_Event.TargetTime);

        Play_Event();
    }

    private IEnumerator Play_BlackScreenOut_Cor(EventElement_BlackScreenOut _Event)
    {
        if (DOTween.IsTweening(BlackScreenImg))
        { DOTween.Kill(BlackScreenImg); }

        BlackScreenImg.DOFade(0, _Event.TargetTime)
            .OnComplete(() =>
            {
                BlackScreenImg.gameObject.SetActive(false);
            });

        yield return new WaitForSeconds(_Event.TargetTime);

        Play_Event();
    }

    private IEnumerator Play_Dialogue_Cor(EventElement_Dialogue _Event)
    {
        DialogueID id = _Event.Get_DialogueList();

        Start_Dialogue(id);
        yield return new WaitUntil(() => !IsPlayingDialogue);

        Play_Event();
    }

    private IEnumerator Play_Cutscene_Cor(EventElement_Cutscene _Event)
    {
        CutsceneID id = _Event.Get_CutsceneList();

        Start_Cutscene(id);

        // 현재 사운드 저장해서
        //SoundManager.Instance.Play_2D_BGM(_Event.TargetID);
        yield return new WaitUntil(() => !IsPlayingCutscene);
        // 이곳에 다시 재생

        Play_Event();
    }

    #endregion

    #region Dialogue

    private void Start_Dialogue(DialogueID _DialogueID)
    {
        if (IsPlayingDialogue)
        { return; }

        IsPlayingDialogue = true;
        CurrentDialogues = new List<DialogueElement>(_DialogueID.Dialogues);
        StartCoroutine(Play_Dialogue_Cor());
    }

    private IEnumerator Play_Dialogue_Cor()
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
            targetDialogueComp.DialogueImg.sprite = ResourceManager.Instance.Get_CorrectCharacterImg(currentDialogue.ImgID);

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
        
        End_Dialogue();
    }

    private void End_Dialogue()
    {
        LeftDialogueComp.DialogueGO.SetActive(false);
        RightDialogueComp.DialogueGO.SetActive(false);
        CurrentDialogues = null;

        IsPlayingDialogue = false;
    }

    #endregion

    #region Cutscene

    private void Start_Cutscene(CutsceneID _CutsceneID)
    {
        if (IsPlayingCutscene)
        { return; }

        IsPlayingCutscene = true;
        CurrentCutscenes = new List<CutsceneElement>(_CutsceneID.Cutscenes);
        StartCoroutine(Play_Cutscene_Cor());
    }

    private IEnumerator Play_Cutscene_Cor()
    {
        CutsceneGO.gameObject.SetActive(true);

        bool canInteract = false;

        bool isAppearing = false;
        bool isDisappearing = false;

        bool eachComplete = false;

        Sequence seq;

        while (true)
        {
            if (CurrentCutscenes.Count <= 0)
            { break; }

            eachComplete = false;

            CutsceneElement currentCutscene = CurrentCutscenes[0];

            // Character Img
            Image img = ImgQueueSet.Get_T();
            img.gameObject.SetActive(true);
            img.color = new Color(1f, 1f, 1f, 0f);
            img.sprite = ResourceManager.Instance.Get_Cutscene(currentCutscene.ID);

            // Script
            seq = DOTween.Sequence();

            CutsceneTxt.text = "";

            isAppearing = true;
            seq.Join(CutsceneTxt.DOText(currentCutscene.Script, currentCutscene.Script.Length / 5f));
            seq.Join(img.DOFade(1f, 3f));
            seq.OnComplete(() => isAppearing = false);

            while (true)
            {
                if (canInteract && Input.anyKeyDown)
                {
                    if (isAppearing) // 나타나기
                    {
                        seq.Complete();
                    }
                    else if (!isDisappearing) // 사라지기
                    {
                        isDisappearing = true;
                        canInteract = false;

                        seq = DOTween.Sequence();

                        CutsceneTxt.text = "";
                        seq.Join(img.DOFade(0f, 3f));
                        seq.OnComplete(() => 
                        { 
                            isDisappearing = false;
                            img.gameObject.SetActive(false);
                            CurrentCutscenes.Remove(currentCutscene);
                            eachComplete = true;
                        });
                    }
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
        CutsceneGO.gameObject.SetActive(false);

        CurrentCutscenes = null;

        IsPlayingCutscene = false;
    }

    #endregion
}

#region Event

[Serializable]
public class EventData
{
    public List<EventElement> Events;

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

    public DialogueID Get_DialogueList() => ResourceManager.Instance.Get_CorrectDialogueID(TargetID);
    
}

[Serializable]
public class EventElement_Cutscene : EventElement
{
    public int TargetID;

    public EventElement_Cutscene(int _ID, int _TargetID) : base(_ID)
    {
        TargetID = _TargetID;
    }

    public CutsceneID Get_CutsceneList() => ResourceManager.Instance.Get_CorrectCutsceneID(TargetID);

}


#endregion

#region Cutscene

[Serializable]
public class CutsceneID
{
    public int ID;
    public List<CutsceneElement> Cutscenes;

    public CutsceneID(int _ID, List<CutsceneElement> _Cutscenes)
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
