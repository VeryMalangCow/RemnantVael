using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingSceneManager : PersistentSingleton<LoadingSceneManager>
{
    #region Value

    [SerializeField] private CanvasGroup LoadingCG;
    [SerializeField] private Image LoadingBarImg;

    [SerializeField] private RectTransform[] LoadingIconRT_Clockwise;
    [SerializeField] private RectTransform[] LoadingIconRT_CounterClockwise;

    [SerializeField] private RectTransform SlidingImgRT;
    

    [HideInInspector] private Sequence CogwheelSeq = null;

    [HideInInspector] private float SlidingImgX = 0;
    [HideInInspector] private Sequence SlidingImgSeq = null;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        SlidingImgSeq = Get_SlidingSeq(1f);
        SlidingImgSeq.Pause();

        CogwheelSeq = Get_CogSeq(1f);
        CogwheelSeq.Pause();
    }

    #endregion

    #region Cog

    private Sequence Get_CogSeq(float _DurTime = 1f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(Get_CogSeq(LoadingIconRT_Clockwise, 360, _DurTime));
        seq.Join(Get_CogSeq(LoadingIconRT_CounterClockwise, -360, _DurTime));
        seq.SetLoops(-1, LoopType.Restart);
        seq.SetUpdate(true);

        return seq;
    }

    private Sequence Get_CogSeq(RectTransform[] _RTArr, float _Angle, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < _RTArr.Length; i++)
        {
            int index = i;
            seq.Join(_RTArr[index]
                .DORotate(new Vector3(0, 0, _Angle), _DurTime, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear));
        }

        seq.SetUpdate(true);
        return seq;
    }

    #endregion

    #region SlidingImg

    private Sequence Get_SlidingSeq(float _DurTime = 1f)
    {
        SlidingImgX = SlidingImgRT.anchoredPosition.x;

        Sequence seq = DOTween.Sequence();

        seq.Append(SlidingImgRT.DOAnchorPosX(-SlidingImgX, _DurTime * 0.8f).SetEase(Ease.Linear));
        seq.AppendInterval(_DurTime * 0.2f);
        seq.SetLoops(-1, LoopType.Restart);
        seq.SetUpdate(true);

        return seq;
    }

    #endregion

    #region Instance

    private void Set_InstanceNull(string _SceneName)
    {
        if (_SceneName == "TitleLobby")
        {
            Set_TitleLobby_InstanceNull();
        }
        else if (_SceneName == "MainGame")
        {
            Set_MainGame_InstanceNull();
        }
    }

    private void Set_MainGame_InstanceNull()
    {
        TitleInputManager.Instance = null;
        TitleLobbyUIManager.Instance = null;
    }

    private void Set_TitleLobby_InstanceNull()
    {
        PlayerManager.Instance = null;
        PoolingManager.Instance = null;
        LayerOrderManager.Instance = null;
        InputManager.Instance = null;
        EnemyManager.Instance = null;
        BaseUpgradeManager.Instance = null;
        ModuleItemManager.Instance = null;
        MainGameUIManager.Instance = null;
        UnitManager.Instance = null;
        StageManager.Instance = null;
        BuffManager.Instance = null;
        AllyManager.Instance = null;
        NPCManager.Instance = null;
        EventManager.Instance = null;
        TimerManager.Instance = null;
    }

    #endregion

    #region Load

    public void Play_LoadScene(string _SceneName)
    {
        ResourceManager.Instance.Clear_LanguageTxt();
        StartCoroutine(Play_LoadSceneAsync_Cor(_SceneName));
    }

    private IEnumerator Play_LoadSceneAsync_Cor(string _SceneName)
    {
        // Sound
        SoundManager.Instance.Set_MasterVolume(1f, 0f, 1f);
        CogwheelSeq.Play();
        SlidingImgSeq.Play();

        LoadingCG.alpha = 0.0f;
        LoadingBarImg.fillAmount = 0.0f;
        LoadingCG.gameObject.SetActive(true);
        LoadingCG.DOFade(1f, 1f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1f);

        yield return Resources.UnloadUnusedAssets();       // 네이티브 리소스 해제
        GC.Collect();                                       // 관리 힙 수거
        GC.WaitForPendingFinalizers();
        GC.Collect();

        AsyncOperation oper = SceneManager.LoadSceneAsync(_SceneName);

        while (!oper.isDone)
        {
            float progressValue = Mathf.Clamp01(oper.progress / 0.9f);
            LoadingBarImg.fillAmount = progressValue;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.8f);
        LoadingCG.DOFade(0f, 0.5f).SetUpdate(true);
        Set_InstanceNull(SceneManager.GetActiveScene().name);

        // Sound
        SoundManager.Instance.Set_MasterVolume(0f, 1f, 1f);
        yield return new WaitForSecondsRealtime(0.6f);

        CogwheelSeq.Pause();
        SlidingImgSeq.Pause();
        LoadingCG.gameObject.SetActive(false);

        if (Time.timeScale != 1) Time.timeScale = 1f;
    }

    #endregion
}
