using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : PersistentSingleton<LoadingSceneManager>
{
    #region Value

    [SerializeField] private CanvasGroup LoadingCG;
    [SerializeField] private CanvasGroup LoadingExtraCG;
    [SerializeField] private Image LoadingBarImg;

    #endregion

    #region Framework

    protected override void Awake()
    {
        //Singleton
        base.Awake();
    }

    #endregion

    #region Load

    public void LoadScene(string _SceneName)
    {
        StartCoroutine(LoadSceneAsync(_SceneName));
    }

    private IEnumerator LoadSceneAsync(string _SceneName)
    {
        LoadingCG.alpha = 0.0f;
        LoadingExtraCG.alpha = 0.0f;
        LoadingBarImg.fillAmount = 0.0f;

        LoadingCG.gameObject.SetActive(true);

        LoadingCG.DOFade(1f, 1f);

        yield return new WaitForSeconds(1f);

        LoadingExtraCG.DOFade(1f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation oper = SceneManager.LoadSceneAsync(_SceneName);

        while (!oper.isDone)
        {
            float progressValue = Mathf.Clamp01(oper.progress / 0.9f);
            LoadingBarImg.fillAmount = progressValue;

            yield return null;
        }

        LoadingExtraCG.DOFade(0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        LoadingCG.DOFade(0f, 1f);

        yield return new WaitForSeconds(1f);

        LoadingCG.gameObject.SetActive(false);
    }

    #endregion
}
