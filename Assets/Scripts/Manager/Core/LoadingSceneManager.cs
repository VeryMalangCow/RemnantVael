using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoadingSceneManager : PersistentSingleton<LoadingSceneManager>
{
    #region Value

    [SerializeField] private CanvasGroup LoadingCG;
    [SerializeField] private Image LoadingBarImg;

    [SerializeField] private List<RectTransform> LoadingIconRT_Clockwise;
    [SerializeField] private List<RectTransform> LoadingIconRT_CounterClockwise;

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

    private Sequence Get_CogSeq(List<RectTransform> _RTList, float _Angle, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < _RTList.Count; i++)
        {
            int index = i;
            seq.Join(_RTList[index]
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

    #region Load

    public void Play_LoadScene(string _SceneName)
    {
        ResourceManager.Instance.Clear_LanguageTxt();

        StartCoroutine(Play_LoadSceneAsync_Cor(_SceneName));
    }

    private IEnumerator Play_LoadSceneAsync_Cor(string _SceneName)
    {
        CogwheelSeq.Play();
        SlidingImgSeq.Play();

        LoadingCG.alpha = 0.0f;
        LoadingBarImg.fillAmount = 0.0f;

        LoadingCG.gameObject.SetActive(true);

        LoadingCG.DOFade(1f, 1f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation oper = SceneManager.LoadSceneAsync(_SceneName);

        while (!oper.isDone)
        {
            float progressValue = Mathf.Clamp01(oper.progress / 0.9f);
            LoadingBarImg.fillAmount = progressValue;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.8f);

        LoadingCG.DOFade(0f, 0.5f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.6f);

        CogwheelSeq.Pause();
        SlidingImgSeq.Pause();

        LoadingCG.gameObject.SetActive(false);

        if (Time.timeScale != 1) Time.timeScale = 1f;
    }

    #endregion
}
