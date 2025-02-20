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
        }
        else
        {
            txt.text = "First!";
            File.WriteAllText(firstRunFilePath, "initialized"); // 파일 생성
        }
    }
}