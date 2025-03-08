using System.Collections.Generic;
using UnityEngine;

public class GateController : StaticDepthController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Gate")]
    [SerializeField] public GameObject ExtraTargetObject;

    [Space(10)]
    [Header("=== Room State")]
    [SerializeField] public Vector2Int RoomPosGate;
    [SerializeField] public Vector2Int GateDir;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public bool IsOpen = false;

    [HideInInspector] public bool HadParter = false;
    [HideInInspector] public bool SettedPos = false;
    [HideInInspector] public RoomController ThisRoom;
    [HideInInspector] public GateController ParterGate = null;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] public Animator ThisAnimator;
    [SerializeField] private AnimationClip ThisAC;

    [Space(10)]
    [Header("=== Other")]
    [SerializeField] private CoupleData<GameObject> ThingsGO;
    [SerializeField] private GameObject EntranceGO;
    [SerializeField] public List<SortingObjectController> NeedSetAllLayer;

    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        // 같은 방법으로 Y축을 위로 올리는데, 다른 객체가 추가로 필요하니 이처럼 사용
        ExtraTargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    private void Update()
    {
        // 문 애니메이션이 종료되면 애니메이터를 끈다.
        Try_AT_Disable();
    }

    #endregion

    #region On Off

    // 처음 문의 상태를 (벽이거나 문이거나) 판별해서 세팅
    public void Set_ExistDoorState(bool _IsExist)
    {
        ThingsGO.TypeBase.SetActive(!_IsExist);
        ThingsGO.TypeSpecial.SetActive(_IsExist);

        if (_IsExist)
        {
            DevTool.Set_Anim(ref AOC, ThisAnimator, ThisAC);
            DevTool.Set_AnimSpeed(ThisAnimator, 0f);
        }
    }

    // 문 열기/닫기
    public void Set_OpenClose(bool _IsOpen)
    {
        IsOpen = _IsOpen;

        if (ThingsGO.TypeSpecial.activeSelf && IsOpen)
        {
            DevTool.Set_Anim(ref AOC, ThisAnimator, ThisAC);
            DevTool.Set_AnimSpeed(ThisAnimator, 1f);

            // 상호작용 판정을 가진 오브젝트
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

    // Animator 종료 시도
    private void Try_AT_Disable()
    {
        if (Can_AT_Disable())
        {
            ThisAnimator.enabled = false;
        }
    }

    // Animator가 종료될 수 있는가 판별
    private bool Can_AT_Disable()
    {
        return
            ThingsGO.TypeSpecial.activeSelf &&
            ThisAnimator.enabled &&
            DevTool.Is_AnimIsDone(ThisAnimator);
    }

    #endregion
}
