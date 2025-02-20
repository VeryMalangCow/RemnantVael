using System.IO;
using TMPro;
using UnityEngine;

public class FirstTimeChecker : MonoBehaviour
{
    private string firstRunFilePath;
    [SerializeField] private TMP_Text txt;

    void Start()
    {
        firstRunFilePath = Path.Combine(Application.persistentDataPath, "firstRun.dat");

        Debug.Log(firstRunFilePath);

        if (File.Exists(firstRunFilePath))
        {
            txt.text = "Already!";
            Debug.Log("이전에 실행된 적 있음.");
        }
        else
        {
            txt.text = "First!";
            Debug.Log("처음 실행됨!");
            File.WriteAllText(firstRunFilePath, "initialized"); // 파일 생성
        }
    }
}