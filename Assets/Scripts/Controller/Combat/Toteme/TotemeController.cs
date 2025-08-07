using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private int AllyBuffID;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float ThisDur;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SpriteRenderer HoloSR;
    [SerializeField] private Light2D HoloLight;

    #endregion

    #region - Hide

    [HideInInspector] private bool Is_Activating = false;

    #endregion

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        if (Is_Activating)
            HoloSR.transform.Rotate(new Vector3(0, 135f * Time.deltaTime, 0));
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
    }

    #endregion

    #region State

    public void Set_State(
        float _DroppingTime, float _TopYPos, float _BottomYPos, float _Dur,
        Sprite _HoloSprite,
        BulletState_PosAndRot _State_PosAndRot,
        BulletState_Size _State_Size)
    {
        base.Set_State_Base(null, _DroppingTime, _TopYPos, _BottomYPos);

        Set_State_PosAndRot(_State_PosAndRot);
        Set_State_ShadowSize(_State_Size);
        Set_State_Toteme(_Dur, _HoloSprite);

        SetOn_State();
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot _State_PosAndRot)
    {
        this.transform.position = _State_PosAndRot.SpawnPos + (_State_PosAndRot.Dir * _State_PosAndRot.Dis);
        this.transform.localRotation = DevTool.Get_RotFromDir(_State_PosAndRot.Dir);

        DevTool.Add_RotZValue(transform, _State_PosAndRot.SpreadAngle);
    }

    private void Set_State_Toteme(float _Dur, Sprite _HoloSprite)
    {
        ThisDur = _Dur;
        HoloSR.sprite = _HoloSprite;
    }

    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();

        base.SetOn_State();
    }

    #endregion

    #region Active

    protected override void Active()
    {
        SetOff_Trail();

        StartCoroutine(Play_BuffArea_Cor());
    }

    private IEnumerator Play_BuffArea_Cor()
    {
        // 시작점
        Active_StartSetting();
        yield return new WaitForSeconds(ThisDur - 1);

        // 그라데이션 되는 부분
        Active_FadeOut();
        yield return new WaitForSeconds(1);

        Remove_Object();
    }

    private void Active_StartSetting()
    {
        Is_Activating = true;
        HoloSR.gameObject.SetActive(true);
        HoloSR.DOFade(1f, 0.5f).SetEase(Ease.Linear);
    }

    private void Active_FadeOut()
    {
        ThisSR.DOFade(0f, 0.9f).SetEase(Ease.Linear);
        HoloSR.DOFade(0f, 0.9f).SetEase(Ease.Linear);
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

    #region Remove

    protected abstract void Remove_Condition();

    protected void Remove_Object()
    {
        SetOff_Trail();

        Remove_Condition();
        Reset_State();

        this.gameObject.SetActive(false);
    }

    #endregion
}
