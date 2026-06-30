using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Scripting;

public class MainGameBootstrapperManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] initializerObjs;
    [SerializeField] private GameObject initalizeingGo;
    [SerializeField] private TMP_Text initalizeingTxt; 

    private readonly List<IMainGameInitializer> initializers = new List<IMainGameInitializer>();

#if UNITY_EDITOR
    [Space(20)]
    [SerializeField] private GameObject[] preAwakePersistentSingletons;
#endif
    private void Awake()
    {
        StartCoroutine(InitStart());
    }

    private IEnumerator InitStart()
    {
        yield return null;

        StartProdInit();

#if UNITY_EDITOR
        for (int i = 0; i < preAwakePersistentSingletons.Length; i++)
            preAwakePersistentSingletons[i].gameObject.SetActive(true);

        UnityEngine.Debug.Log("<color=orange>PreAwakeManagers : All Offset Complete</color>");
        yield return new WaitForSeconds(1);
#endif
        initalizeingGo.gameObject.SetActive(true);

        // 타입 초기화
        CollectInitializers();
        // 순서 정렬
        initializers.Sort(CompareOrder);

        for (int i = 0; i < initializers.Count; i++)
        {
            IMainGameInitializer initializer = initializers[i];

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Initializer Start : <color=grey>{initializer}</color>");
#endif
            initalizeingTxt.text = initializer.InitPregressText;
            yield return initializer.Initialize();
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Initializer Complete : <color=green>{initializer}</color>");
#endif
        }

        initalizeingGo.gameObject.SetActive(false);

        yield return CustomGC.CollectAsync();

        EndProdInit();
    }

    private void CollectInitializers()
    {
        initializers.Clear();

        for (int i = 0; i < initializerObjs.Length; i++)
        {
            var mono = initializerObjs[i];

            if (mono == null)
            {
                UnityEngine.Debug.LogWarning("Initializer Empty : index " + i);
                continue;
            }

            var initializer = mono as IMainGameInitializer;

            if (initializer == null)
            {
                UnityEngine.Debug.LogWarning(mono.name + " is not Contain IMainGameInitializer");
                continue;
            }
            
            if (initializers.Contains(initializer))
            {
                UnityEngine.Debug.LogWarning(mono.name + " is already Contain");
                continue;
            }
            
            initializers.Add(initializer);
        }
    }

    // Order
    private int CompareOrder(IMainGameInitializer a, IMainGameInitializer b)
        => a.InitOrder.CompareTo(b.InitOrder);
    
    private void StartProdInit()
    {
        MainGameUIManager.instance.StartProd();
    }
    private void EndProdInit()
    {
        MainGameUIManager.instance.EndProd();
    }
}

public interface IMainGameInitializer
{
    int InitOrder { get; }
    string InitPregressText { get; }
    IEnumerator Initialize();
}
