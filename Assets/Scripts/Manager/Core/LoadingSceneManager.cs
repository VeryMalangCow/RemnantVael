using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingSceneManager : PersistentSingleton<LoadingSceneManager>
{
    #region Value

    [SerializeField] private CanvasGroup loadingCG;
    [SerializeField] private Image loadingBarImg;

    [SerializeField] private RectTransform[] loadingIconRT_Clockwise;
    [SerializeField] private RectTransform[] loadingIconRT_CounterClockwise;

    [SerializeField] private RectTransform slidingImgRT;
    

    [HideInInspector] private Sequence cogwheelSeq = null;

    [HideInInspector] private float slidingImgX = 0;
    [HideInInspector] private Sequence slidingImgSeq = null;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        slidingImgSeq = Get_SlidingSeq(1f);
        slidingImgSeq.Pause();

        cogwheelSeq = Get_CogSeq(1f);
        cogwheelSeq.Pause();
    }

    #endregion

    #region Cog

    private Sequence Get_CogSeq(float _DurTime = 1f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(Get_CogSeq(loadingIconRT_Clockwise, 360, _DurTime));
        seq.Join(Get_CogSeq(loadingIconRT_CounterClockwise, -360, _DurTime));
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
        slidingImgX = slidingImgRT.anchoredPosition.x;

        Sequence seq = DOTween.Sequence();

        seq.Append(slidingImgRT.DOAnchorPosX(-slidingImgX, _DurTime * 0.8f).SetEase(Ease.Linear));
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
        cogwheelSeq.Play();
        slidingImgSeq.Play();

        loadingCG.alpha = 0.0f;
        loadingBarImg.fillAmount = 0.0f;
        loadingCG.gameObject.SetActive(true);
        loadingCG.DOFade(1f, 1f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1f);

        yield return Resources.UnloadUnusedAssets();       // 네이티브 리소스 해제
        GC.Collect();                                       // 관리 힙 수거
        GC.WaitForPendingFinalizers();
        GC.Collect();

        AsyncOperation oper = SceneManager.LoadSceneAsync(_SceneName);

        while (!oper.isDone)
        {
            float progressValue = Mathf.Clamp01(oper.progress / 0.9f);
            loadingBarImg.fillAmount = progressValue;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.8f);
        loadingCG.DOFade(0f, 0.5f).SetUpdate(true);
        Set_InstanceNull(SceneManager.GetActiveScene().name);

        // Sound
        SoundManager.Instance.Set_MasterVolume(0f, 1f, 1f);
        yield return new WaitForSecondsRealtime(0.6f);

        cogwheelSeq.Pause();
        slidingImgSeq.Pause();
        loadingCG.gameObject.SetActive(false);

        if (Time.timeScale != 1) Time.timeScale = 1f;
    }

    #endregion
}
