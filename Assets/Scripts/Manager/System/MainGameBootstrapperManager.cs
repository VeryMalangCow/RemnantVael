using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainGameBootstrapperManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] initializerObjs;

    private readonly List<IMainGameInitializer> initializers = new List<IMainGameInitializer>();

#if UNITY_EDITOR
    [SerializeField] private GameObject[] preAwakePersistentSingletons;
#endif
    private void Awake()
    {
#if UNITY_EDITOR
        for (int i = 0; i < preAwakePersistentSingletons.Length; i++)
            preAwakePersistentSingletons[i].gameObject.SetActive(true);

        Debug.Log("<color=orange>PreAwakeManagers : All Offset Complete</color>");
#endif
        StartCoroutine(InitStart());
    }

    private IEnumerator InitStart()
    {
        CollectInitializers();
        SortInitialzers();

        int index = 0;
        while (index < initializers.Count)
        {
            int currentOrder = initializers[index].InitOrder;

            int startIndex = index;
            int endIndex = index;

            while (endIndex < initializers.Count && initializers[endIndex].InitOrder == currentOrder)
            {
                endIndex++;
            }

            int completedCount = 0;
            int totalCount = endIndex - startIndex;

            for (int i = startIndex; i < endIndex; i++)
            {
                StartCoroutine(RunInitializer(initializers[i], delegate
                {
                    completedCount++;
                }));
            }

            while (completedCount < totalCount)
            {
                yield return null;
            }

            index = endIndex;
        }

        EndInit();
    }

    private IEnumerator RunInitializer(IMainGameInitializer initializer, Action onComplete)
    {
        bool hasError = false;

        IEnumerator routine = initializer.Initialize();

        while (true)
        {
            bool moveNext = false;

            try
            {
                moveNext = routine.MoveNext();
            }
            catch (Exception e)
            {
                Debug.LogError("Initializer Failed : " + initializer + " - " + e);
                hasError = true;
            }

            if (hasError || !moveNext)
                break;

            yield return routine.Current;
        }

        if (onComplete != null) onComplete();
    }
    
    // 타입 초기화
    private void CollectInitializers()
    {
        initializers.Clear();

        for (int i = 0; i < initializerObjs.Length; i++)
        {
            var mono = initializerObjs[i];

            if (mono == null)
            {
                Debug.LogWarning("Initializer Empty : index " + i);
                continue;
            }

            var initializer = mono as IMainGameInitializer;

            if (initializer == null)
            {
                Debug.LogWarning(mono.name + " is not Contain IMainGameInitializer");
                continue;
            }
            
            if (initializers.Contains(initializer))
            {
                Debug.LogWarning(mono.name + " is already Contain");
                continue;
            }
            
            initializers.Add(initializer);
        }
    }

    // 순서 정렬
    private void SortInitialzers()
    {
        initializers.Sort(CompareOrder);
    }

    // Order
    private int CompareOrder(IMainGameInitializer a, IMainGameInitializer b)
        => a.InitOrder.CompareTo(b.InitOrder);
    
    private void EndInit()
    {
        MainGameUIManager.instance.EndProd();
    }
}

public interface IMainGameInitializer
{
    int InitOrder { get; }
    IEnumerator Initialize();
}
