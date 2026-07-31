using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public abstract class TotemeController : DroppingDepthController, IPoolable
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Toteme")]
    [SerializeField] protected string poolingString = "";

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private int playerBuffId;
    [SerializeField] private string allyBuffId;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float thisDur;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SpriteRenderer holoSr;
    [SerializeField] private Light2D holoLight2d;

    [Space(10)]
    [Header("=== Buff Area")]
    [SerializeField] private GameObject buffAreaGo;
    [SerializeField] private CapsuleCollider2D buffCol;
    [SerializeField] private Transform buffPointParentTf;

    #endregion

    #region - Hide

    // Activating
    [HideInInspector] private bool isActivating = false;
    [HideInInspector] private static readonly Vector2 buffColBaseSize = new Vector2(2, 1);
    [HideInInspector] private static readonly int pointAmountPerSize = 15;
    [HideInInspector] private List<PoolableSpriteRenderer> buffSpoterList = new List<PoolableSpriteRenderer>();

    // Buff
    [HideInInspector] private bool inAreaPlayer = false;
    [HideInInspector] private List<AllyController> inAreaAllies = null;
    #endregion

    #endregion

    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;
    
    #region Pool
    
    public void PoolOffset()
    {
        gameObject.SetActive(false);
    }

    public void SetActiveOn()
    {
        gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        gameObject.SetActive(true);
    }

    public void SetActiveOff()
    {
        Reset_State();

        gameObject.SetActive(false);
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        if (isActivating)
            holoSr.transform.Rotate(new Vector3(0, 180f * Time.deltaTime, 0));
    }

    #endregion

    #region Light

    public override void SetOn_LightIntensity(float intensity)
    {
        base.SetOn_LightIntensity(intensity);

        holoLight2d.intensity = intensity;
        holoLight2d.lightCookieSprite = holoSr.sprite;
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

        isActivating = false;
        holoSr.gameObject.SetActive(false);
        buffAreaGo.gameObject.SetActive(false);

        inAreaPlayer = false;
        inAreaAllies = null;
    }

    #endregion

    #region State

    public void Set_State(
        float droppingTime, float topYPos, float bottomYPos, float dur,
        Sprite holoSprite, Color clr, float buffAreaSize,
        BulletState_PosAndRot state_PosAndRot,
        BulletState_Size state_Size)
    {
        base.Set_State_Base(null, droppingTime, topYPos, bottomYPos);

        Set_State_PosAndRot(state_PosAndRot);
        Set_State_ShadowSize(state_Size);
        Set_State_Toteme(dur, holoSprite);

        Set_State_BuffAreaPoint(state_Size.objSize, clr);

        SetOn_State();
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot state_PosAndRot)
    {
        this.transform.position = state_PosAndRot.spawnPos + (state_PosAndRot.dir * state_PosAndRot.dis);
        this.transform.localRotation = DevTool.GetRotFromDir(state_PosAndRot.dir);

        DevTool.Add_RotZValue(transform, state_PosAndRot.spreadAngle);
    }

    public override void Set_State_ShadowSize(BulletState_Size state_Size)
    {
        base.Set_State_ShadowSize(state_Size);

        buffCol.size = buffColBaseSize * state_Size.objSize;
    }

    private void Set_State_BuffAreaPoint(Vector2 areaSize, Color clr)
    {
        Vector2 targetArea = buffCol.size;
        int amount = (int)(areaSize.x * pointAmountPerSize);

        List<Vector2> pointPosList = Get_PointPosList(targetArea, amount);

        VfxManager.instance.SpawnAreaSpoters(amount, buffSpoterList);

        for (int i = 0; i < amount; i++)
        {
            buffSpoterList[i].spriteRenderer.color = clr;
            buffSpoterList[i].transform.SetParent(buffPointParentTf, false); // 로컬 좌표 유지
            buffSpoterList[i].transform.localPosition = pointPosList[i]; // 로컬 좌표로 설정
            buffSpoterList[i].gameObject.SetActive(true);
        }
    }


    private void Set_State_Toteme(float dur, Sprite holoSprite)
    {
        thisDur = dur;
        holoSr.sprite = holoSprite;
    }

    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();

        base.SetOn_State();
    }


    public void Set_State_BuffID(int playerBuffId, string allyBuffId)
    {
        this.playerBuffId = playerBuffId;
        this.allyBuffId = allyBuffId;
    }

    #endregion

    #region Active

    protected override void Active()
    {
        SetOff_Trail();
        if (gameObject.activeSelf)
            StartCoroutine(this.Play_BuffArea_Cor());
        else
        {
            RemoveAct();
            RemoveObject();
        }
    }

    private IEnumerator Play_BuffArea_Cor()
    {
        // 시작점
        Active_StartSetting();
        yield return new WaitForSeconds(thisDur - 1);

        // 그라데이션 되는 부분
        Active_FadeOut();
        yield return new WaitForSeconds(1f);

        RemoveAct();
        RemoveObject();
    }

    private void Active_StartSetting()
    {
        AllyManager.instance.Add_Toteme(this);
        inAreaAllies = new List<AllyController>();

        isActivating = true;
        holoSr.gameObject.SetActive(true);
        holoSr.DOFade(1f, 0.5f).SetEase(Ease.Linear);
        buffAreaGo.gameObject.SetActive(true);
    }

    private void Active_FadeOut()
    {
        thisSr.DOFade(0f, 0.9f).SetEase(Ease.Linear);
        holoSr.DOFade(0f, 0.9f).SetEase(Ease.Linear);
    }

    #endregion

    #region Active Buff

    public void Active_Buff()
    {
        if (!isActivating) return;

        if (inAreaPlayer)
            BuffManager.instance.Gain_Buff(playerBuffId);
        
        for (int i = 0; i < inAreaAllies.Count; i++)
        {
            AllyBuff buff = inAreaAllies[i].buffController.Get_AllyBuff(allyBuffId);
            buff.SetAndGain_Buff(1);
        }
    }

    #endregion

    #region Sorting Order

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        holoSr.sortingOrder = sortingOrder;
        trail.sortingOrder = sortingOrder - 1;
    }

    #endregion

    #region Area Point

    private List<Vector2> Get_PointPosList(Vector2 capsuleSize, int amount)
    {
        List<Vector2> points = new List<Vector2>();

        float width = capsuleSize.x;
        float height = capsuleSize.y;

        float radius = height / 2f;
        float straight = width - (radius * 2f);
        if (straight < 0f) straight = 0f;

        float arcLength = Mathf.PI * radius;
        float perimeter = (straight * 2f) + (arcLength * 2f);

        for (int i = 0; i < amount; i++)
        {
            float dist = (perimeter * i) / amount;
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
        trail.Clear();

        trail.emitting = true;
        trail.enabled = true;
    }

    private void SetOff_Trail()
    {
        trail.emitting = false;
        trail.enabled = false;
    }

    #endregion

    #region Remove

    private void RemoveAct()
    {
        StopCoroutine(this.Play_BuffArea_Cor());

        SetOff_BuffPoint();
        AllyManager.instance.Remove_Toteme(this);

    }

    protected abstract void RemoveObject();

    private void SetOff_BuffPoint()
    {
        if (buffSpoterList == null) return;
        for (int i = 0; i < buffSpoterList.Count; i++)
        {
            VfxManager.instance.RemoveAreaSpoter(buffSpoterList[i]);
        }
        buffSpoterList = null;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            inAreaPlayer = true; 
        }
        else if (col.tag == "Ally")
        {
            if (col.gameObject.transform.parent.gameObject.TryGetComponent(out AllyController ally))
            {
                DevTool.Add_InList(inAreaAllies, ally);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            inAreaPlayer = false;
        }
        else if (col.tag == "Ally")
        {
            if (col.gameObject.transform.parent.gameObject.TryGetComponent(out AllyController ally))
            {
                DevTool.Remove_InList(inAreaAllies, ally);
            }
        }
    }

    #endregion
}
