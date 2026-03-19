using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public abstract class TotemeController : DroppingDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Toteme")]
    [SerializeField] protected string PoolingString = "";

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private int PlayerBuffID;
    [SerializeField] private string AllyBuffID;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float ThisDur;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SpriteRenderer HoloSR;
    [SerializeField] private Light2D HoloLight;

    [Space(10)]
    [Header("=== Buff Area")]
    [SerializeField] private GameObject BuffAreaGO;
    [SerializeField] private CapsuleCollider2D BuffCol;
    [SerializeField] private Transform BuffPointParentTF;

    #endregion

    #region - Hide

    // Activating
    [HideInInspector] private bool Is_Activating = false;
    [HideInInspector] private static readonly Vector2 BuffColBaseSize = new Vector2(2, 1);
    [HideInInspector] private static readonly int PointAmountPerSize = 15;
    [HideInInspector] private List<SpriteRenderer> BuffPointList = new List<SpriteRenderer>();

    // Buff
    [HideInInspector] private bool InAreaPlayer = false;
    [HideInInspector] private List<AllyController> InAreaAllies = null;

    #endregion

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        if (Is_Activating)
            HoloSR.transform.Rotate(new Vector3(0, 180f * Time.deltaTime, 0));
    }

    #endregion

    #region Light

    public override void SetOn_LightIntensity(float _Intensity)
    {
        base.SetOn_LightIntensity(_Intensity);

        HoloLight.intensity = _Intensity;
        HoloLight.lightCookieSprite = HoloSR.sprite;
    }

    #endregion

    #region Reset

    public void Reset_State()
    {
        Reset_BaseToteme();
    }

    private void Reset_BaseToteme()
    {
        transform.position = new Vector3(1000, 0, 0);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        Is_Activating = false;
        HoloSR.gameObject.SetActive(false);
        BuffAreaGO.gameObject.SetActive(false);

        InAreaPlayer = false;
        InAreaAllies = null;
    }

    #endregion

    #region State

    public void Set_State(
        float _DroppingTime, float _TopYPos, float _BottomYPos, float _Dur,
        Sprite _HoloSprite, Color _Clr, float _BuffAreaSize,
        BulletState_PosAndRot _State_PosAndRot,
        BulletState_Size _State_Size)
    {
        UnitManager.instance.Add_Unit(this);

        base.Set_State_Base(null, _DroppingTime, _TopYPos, _BottomYPos);

        Set_State_PosAndRot(_State_PosAndRot);
        Set_State_ShadowSize(_State_Size);
        Set_State_Toteme(_Dur, _HoloSprite);

        Set_State_BuffAreaPoint(_State_Size.objSize, _Clr);

        SetOn_State();
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot _State_PosAndRot)
    {
        this.transform.position = _State_PosAndRot.spawnPos + (_State_PosAndRot.dir * _State_PosAndRot.dis);
        this.transform.localRotation = DevTool.Get_RotFromDir(_State_PosAndRot.dir);

        DevTool.Add_RotZValue(transform, _State_PosAndRot.spreadAngle);
    }

    public override void Set_State_ShadowSize(BulletState_Size _State_Size)
    {
        base.Set_State_ShadowSize(_State_Size);

        BuffCol.size = BuffColBaseSize * _State_Size.objSize;
    }

    private void Set_State_BuffAreaPoint(Vector2 _AreaSize, Color _Clr)
    {
        Vector2 targetArea = BuffCol.size;
        int amount = (int)(_AreaSize.x * PointAmountPerSize);

        List<Vector2> pointPosList = Get_PointPosList(targetArea, amount);
        BuffPointList = PoolingManager.instance.Get_OP_AreaPointSRList(amount);

        for (int i = 0; i < amount; i++)
        {
            BuffPointList[i].color = _Clr;
            BuffPointList[i].transform.SetParent(BuffPointParentTF, false); // 로컬 좌표 유지
            BuffPointList[i].transform.localPosition = pointPosList[i]; // 로컬 좌표로 설정
            BuffPointList[i].gameObject.SetActive(true);
        }
    }


    private void Set_State_Toteme(float _Dur, Sprite _HoloSprite)
    {
        ThisDur = _Dur;
        HoloSR.sprite = _HoloSprite;
    }

    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();

        base.SetOn_State();
    }


    public void Set_State_BuffID(int _PlayerBuffID, string _AllyBuffID)
    {
        PlayerBuffID = _PlayerBuffID;
        AllyBuffID = _AllyBuffID;
    }

    #endregion

    #region Active

    protected override void Active()
    {
        SetOff_Trail();
        if (gameObject.activeSelf)
            StartCoroutine(this.Play_BuffArea_Cor());
        else
            Remove_Object();
    }

    private IEnumerator Play_BuffArea_Cor()
    {
        // 시작점
        Active_StartSetting();
        yield return new WaitForSeconds(ThisDur - 1);

        // 그라데이션 되는 부분
        Active_FadeOut();
        yield return new WaitForSeconds(1f);

        Remove_Object();
    }

    private void Active_StartSetting()
    {
        TimerManager.instance.Add_Toteme(this);
        InAreaAllies = new List<AllyController>();

        Is_Activating = true;
        HoloSR.gameObject.SetActive(true);
        HoloSR.DOFade(1f, 0.5f).SetEase(Ease.Linear);
        BuffAreaGO.gameObject.SetActive(true);
    }

    private void Active_FadeOut()
    {
        ThisSR.DOFade(0f, 0.9f).SetEase(Ease.Linear);
        HoloSR.DOFade(0f, 0.9f).SetEase(Ease.Linear);
    }

    #endregion

    #region Active Buff

    public void Active_Buff()
    {
        if (!Is_Activating) return;

        if (InAreaPlayer)
            BuffManager.instance.Gain_Buff(PlayerBuffID);
        
        for (int i = 0; i < InAreaAllies.Count; i++)
        {
            AllyBuff buff = InAreaAllies[i].BuffController.Get_AllyBuff(AllyBuffID);
            buff.SetAndGain_Buff(1);
        }
    }

    #endregion

    #region Sorting Order

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        HoloSR.sortingOrder = _SortingOrder;
        ThisTrail.sortingOrder = _SortingOrder - 1;
    }

    #endregion

    #region Area Point

    private List<Vector2> Get_PointPosList(Vector2 _CapsuleSize, int _Amount)
    {
        List<Vector2> points = new List<Vector2>();

        float width = _CapsuleSize.x;
        float height = _CapsuleSize.y;

        float radius = height / 2f;
        float straight = width - (radius * 2f);
        if (straight < 0f) straight = 0f;

        float arcLength = Mathf.PI * radius;
        float perimeter = (straight * 2f) + (arcLength * 2f);

        for (int i = 0; i < _Amount; i++)
        {
            float dist = (perimeter * i) / _Amount;
            Vector2 pos;

            // 1. 상단 직선 (왼 → 오)
            if (dist <= straight)
            {
                pos = new Vector2(-straight / 2f + dist, radius);
            }
            // 2. 오른쪽 반원 (위 → 아래)
            else if (dist <= straight + arcLength)
            {
                float arcDist = dist - straight;
                float t = arcDist / arcLength; // 0~1
                float angle = 90f - t * 180f; // 90 → -90
                pos = new Vector2(straight / 2f + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            }
            // 3. 하단 직선 (오 → 왼)
            else if (dist <= straight + arcLength + straight)
            {
                float lineDist = dist - (straight + arcLength);
                pos = new Vector2(straight / 2f - lineDist, -radius);
            }
            // 4. 왼쪽 반원 (아래 → 위)
            else
            {
                float arcDist = dist - (straight * 2f + arcLength);
                float t = arcDist / arcLength; // 0 ~ 1

                // 변경: -90 → 90 이 아니라 270 → 90 으로 (아래 -> 왼쪽 -> 위)
                float angle = 270f - t * 180f; // 270 -> 90
                float rad = angle * Mathf.Deg2Rad;

                pos = new Vector2(-straight / 2f + Mathf.Cos(rad) * radius,
                    Mathf.Sin(rad) * radius);
            }

            points.Add(pos);
        }

        return points;
    }

    #endregion

    #region Trail

    protected virtual void SetOn_Trail()
    {
        ThisTrail.Clear();

        ThisTrail.emitting = true;
        ThisTrail.enabled = true;
    }

    private void SetOff_Trail()
    {
        ThisTrail.emitting = false;
        ThisTrail.enabled = false;
    }

    #endregion

    #region Pooling

    protected abstract void PoolingSet();

    #endregion

    #region Remove

    private void Remove_Object()
    {
        UnitManager.instance.Remove_Unit(this);

        RemoveForce_Object();
    }

    public void RemoveForce_Object()
    {
        StopCoroutine(this.Play_BuffArea_Cor());

        Reset_State();
        SetOff_BuffPoint();
        TimerManager.instance.Remove_Toteme(this);

        PoolingSet();

        this.gameObject.SetActive(false);
    }

    private void SetOff_BuffPoint()
    {
        if (BuffPointList == null) return;
        for (int i = 0; i < BuffPointList.Count; i++)
        {
            BuffPointList[i].gameObject.SetActive(false);
            PoolingManager.instance.areaPointSRs.Enqueue(BuffPointList[i]);
        }
        BuffPointList = null;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Other)
    {
        if (_Other.tag == "Player")
        {
            InAreaPlayer = true; 
        }
        else if (_Other.tag == "Ally")
        {
            if (_Other.gameObject.transform.parent.gameObject.TryGetComponent(out AllyController ally))
            {
                DevTool.Add_InList(InAreaAllies, ally);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _Other)
    {
        if (_Other.tag == "Player")
        {
            InAreaPlayer = false;
        }
        else if (_Other.tag == "Ally")
        {
            if (_Other.gameObject.transform.parent.gameObject.TryGetComponent(out AllyController ally))
            {
                DevTool.Remove_InList(InAreaAllies, ally);
            }
        }
    }

    #endregion
}
