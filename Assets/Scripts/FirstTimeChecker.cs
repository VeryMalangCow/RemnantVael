using TMPro;
using UnityEngine;

public class FirstTimeChecker : MonoBehaviour
{
    private const string FirstRunKey = "FirstRun";
    [SerializeField] private TMP_Text txt;

    void Start()
    {
        if (IsFirstRun())
        {
            Debug.Log("게임을 처음 실행했습니다!");
            txt.text = "First";
            // 여기에 처음 실행 시 필요한 로직 추가
        }
        else
        {
            txt.text = "Already";
            Debug.Log("이전에 실행한 적이 있습니다.");
        }
    }

    bool IsFirstRun()
    {
        if (!PlayerPrefs.HasKey(FirstRunKey))
        {
            PlayerPrefs.SetInt(FirstRunKey, 1);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}