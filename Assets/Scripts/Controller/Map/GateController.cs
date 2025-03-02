using System.Collections.Generic;
using UnityEngine;

public class GateController : StaticDepthController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Gate Controller")]

    [SerializeField] public GameObject ExtraTargetObject;

    [Space(10)]
    [Header("=== Room State")]
    [SerializeField] public Vector2Int RoomPosGate;
    [SerializeField] public Vector2Int GateDir;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public bool IsOpen = false;
    [HideInInspector] public RoomController ThisRoom;
    [HideInInspector] public bool HadParter = false;
    [HideInInspector] public bool SettedPos = false;
    [HideInInspector] public GateController ParterGate = null;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] public Animator ThisAnimator;
    [SerializeField] private AnimationClip ThisAC;

    [Space(10)]
    [Header("=== Other")]
    [SerializeField] private GameObject OnThingsGO;
    [SerializeField] private GameObject OffThingsGO;
    [SerializeField] private GameObject EntranceGO;
    [SerializeField] public List<SortLayerObjectController> NeedSetAllLayer;


    [HideInInspector] private AnimatorOverrideController aoc;
    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        ExtraTargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    private void Update()
    {
        if (OnThingsGO.gameObject.activeSelf && ThisAnimator.enabled && Get_AnimIsDone())
        {
            ThisAnimator.enabled = false;
        }
    }

    #endregion

    #region On Off

    public void Set_ExistDoorState(bool _IsExist)
    {
        if (_IsExist)
        {
            OnThingsGO.gameObject.SetActive(true);
            OffThingsGO.gameObject.SetActive(false);
            Set_Anim(ThisAC, 0f);
        }
        else
        {
            OnThingsGO.gameObject.SetActive(false);
            OffThingsGO.gameObject.SetActive(true);
        }
    }

    public void Set_OnOff(bool _IsOn)
    {
        IsOpen = _IsOn;

        if (OnThingsGO.gameObject.activeSelf && IsOpen)
        {
            Set_Anim(ThisAC, 1f);
            EntranceGO.SetActive(true);
        }
    }

    #endregion

    #region Interact

    public void Play_Interact()
    {
        if (IsOpen && ParterGate != null)
        {
            PlayerManager.Instance.PlayerController.gameObject.transform.position = ParterGate.gameObject.transform.position
            + new Vector3(GateDir.x, GateDir.y, 0);
            StageManager.Instance.Play_CurrentRoom(ParterGate.ThisRoom);
        }
    }

    #endregion

    #region Anim

    public void Set_Anim(AnimationClip _AC, float _AnimSpeed = 1f)
    {
        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;
        ThisAnimator.speed = _AnimSpeed;
    }

    private bool Get_AnimIsDone()
    {
        // 현재 애니메이터 상태 정보 가져오기
        AnimatorStateInfo animatorStateInfo = ThisAnimator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션이 종료되었는지 판별
        if (animatorStateInfo.normalizedTime >= 1 && !ThisAnimator.IsInTransition(0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
