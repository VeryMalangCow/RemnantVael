using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Diagnostics;

[System.Serializable]
public class PoolSystem<T> where T : MonoBehaviour, IPoolable
{
    [SerializeField] private T prefab;
    public T Prefab { get { return prefab; } }
    [SerializeField] private Transform parentTf;

    public T[] objs { get; private set; }
    private Stack<int> freeIndices;
    public List<int> activeIndices { get; private set; }

    // 코루틴 기반 순차 생성 (초기화 스파이크 방지 -> 초기에만 실행될 것)
    public IEnumerator InitAsync(Transform _parentTf, int size, float maxMsPerFrame = 1f)
    {
        parentTf = _parentTf;
        yield return InitAsync(size, maxMsPerFrame);
    }

    public IEnumerator InitAsync(int size, float maxMsPerFrame = 1f)
    {
        objs = new T[size];
        freeIndices = new Stack<int>(size);
        activeIndices = new List<int>(size);

#if UNITY_EDITOR
        List<int> createAmount = new List<int>();
        int allFrame = 0;
        int createdThisFrame = 0;
#endif

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < size; i++)
        {
            CreatePoolObjs(i);

#if UNITY_EDITOR
            createdThisFrame++;
#endif
            if (sw.Elapsed.TotalMilliseconds >= maxMsPerFrame)
            {
#if UNITY_EDITOR
                createAmount.Add(createdThisFrame);
                createdThisFrame = 0;
                allFrame++;
#endif
                yield return null;
                sw.Restart();
            }
        }
        sw.Stop();

#if UNITY_EDITOR
        if (createdThisFrame > 0)
        {
            createAmount.Add(createdThisFrame);
            allFrame++;
        }

        string s =
            $"PoolSystem : Create : <color=orange>{typeof(T).Name}</color>" +
            $"\nTotal Amount -> <color=red>{size}</color>" +
            $"\nFrame -> <color=red>{allFrame}</color>" +
            $"\nLimitMs -> <color=red>{maxMsPerFrame}</color>\n";

        for (int i = 0; i < createAmount.Count; i++)
        {
            s += $"<color=yellow>{createAmount[i]}</color> / ";
        }

        UnityEngine.Debug.Log(s);
#endif
        yield return null;
    }

    private void CreatePoolObjs(int index)
    {
        T obj = Object.Instantiate(prefab, parentTf);
        obj.PoolOffset();
        obj.PoolIndex = index;
        obj.ActiveIndex = -1;

        objs[index] = obj;
        freeIndices.Push(index);
    }

    // 사용 가능한 Free 추가
    public void Enqueue(T obj)
    {
        if (obj == null)
            return;

        if (obj.ActiveIndex < 0)
            return;

        if (activeIndices.Count <= 0)
        {
            UnityEngine.Debug.LogWarning($"Enqueue 실패: activeIndices가 비어있음. obj: {obj.name}, PoolIndex: {obj.PoolIndex}, ActiveIndex: {obj.ActiveIndex}");
            obj.ActiveIndex = -1;
            return;
        }

        if (obj.ActiveIndex >= activeIndices.Count)
        {
            UnityEngine.Debug.LogWarning($"Enqueue 실패: ActiveIndex 범위 초과. obj: {obj.name}, PoolIndex: {obj.PoolIndex}, ActiveIndex: {obj.ActiveIndex}, activeCount: {activeIndices.Count}");
            obj.ActiveIndex = -1;
            return;
        }

        if (activeIndices[obj.ActiveIndex] != obj.PoolIndex)
        {
            UnityEngine.Debug.LogWarning($"Enqueue 실패: ActiveIndex 불일치. obj: {obj.name}, PoolIndex: {obj.PoolIndex}, ActiveIndex: {obj.ActiveIndex}, activeIndices[ActiveIndex]: {activeIndices[obj.ActiveIndex]}");
            obj.ActiveIndex = -1;
            return;
        }

        // 바꾸어줄 Index들
        int activeIdx = obj.ActiveIndex;
        int lastIdx = activeIndices.Count - 1; 

        // 활성 인덱스 리스트에서 제거할 Index와 끝 Index를 바꿈
        int movedPoolIdx = activeIndices[lastIdx];
        activeIndices[activeIdx] = movedPoolIdx;
        objs[movedPoolIdx].ActiveIndex = activeIdx;

        // 활성 인덱스 리스트에서 마지막 요소 제거
        activeIndices.RemoveAt(lastIdx);
        obj.ActiveIndex = -1;

        // free에 추가
        freeIndices.Push(obj.PoolIndex);

        obj.transform.SetParent(parentTf, false);

        obj.SetActiveOff();
    }


    // 사용을 위해 Free에서 불러오기
    public T Dequeue()
    {
        if (freeIndices.Count == 0)
            return null;

        // 인덱스와 객체 가져오기
        int poolIdx = freeIndices.Pop();
        T obj = objs[poolIdx];

        // 활성 인덱스 리스트에 추가
        activeIndices.Add(poolIdx); 
        obj.ActiveIndex = activeIndices.Count - 1;

        obj.SetActiveOn();

        return obj;
    }

    // Free 한 번에 여러개 불러오기
    public void DequeueMany(int amount, List<T> list)
    {
        list.Clear();

        for (int i = 0; i < amount; i++)
        {
            T obj = Dequeue();
            if (obj == null)
                break;
            list.Add(obj);
        }
    }

    public void ReturnAll()
    {
        for (int i = activeIndices.Count - 1; i >= 0; i--)
        {
            Enqueue(objs[activeIndices[i]]);
        }
    }
}

public interface IPoolable
{
    int PoolIndex { get; set; }
    int ActiveIndex { get; set; }

    public void PoolOffset();
    public void SetActiveOn();
    public void SetActiveOff();
}