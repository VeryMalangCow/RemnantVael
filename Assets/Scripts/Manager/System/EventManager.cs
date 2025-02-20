using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Event")]

    [Space(10)]
    [Header("=== Managing Value")]
    [SerializeField] private bool IsPlayingEvent = false;
    [SerializeField] private bool IsStartEachOne = false;
    [SerializeField] private bool IsEndEachOne = false;

    [Space(10)]
    [Header("=== Test")]
    [SerializeField] private EventData CurrentEvent = new EventData();

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

        
    }

    private void Update()
    {
        // Test Input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayEvent();
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

        IsPlayingEvent = true;

        CurrentEvent.Events = CSVManager.Instance.GetCorrectEventList(_ID);
        PlayEvent();
    }

    // 이벤트 실행
    private void PlayEvent()
    {
        string s = "";

        if (CurrentEvent.Events.Count <= 0)
        {
            CurrentEvent = null;
            return;
        }

        if (CurrentEvent.Events[0] is EventElement_Stay)
        {
            s += " Stay";
        }
        else if (CurrentEvent.Events[0] is EventElement_Move)
        {
            s += " Move";
        }
        else if (CurrentEvent.Events[0] is EventElement_BlackScreenIn)
        {
            s += " BlackScreenIn";
        }
        else if (CurrentEvent.Events[0] is EventElement_BlackScreenOut)
        {
            s += " BlackScreenOut";
        }

        CurrentEvent.ClearOnePart();
    }


    #endregion

    #region Play (Kind of Condition)

    private IEnumerator PlayEvent_Stay()
    {
        yield return null;
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
public class EventElement_Move : EventElement
{
    public Vector2 TargetPos;

    public EventElement_Move(int _ID, Vector2 _TargetPos) : base(_ID)
    {
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

#endregion

#endregion
