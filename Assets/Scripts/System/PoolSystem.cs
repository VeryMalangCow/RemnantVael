using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolSystem<T> where T : MonoBehaviour, IPoolable
{
    [SerializeField] private T prefab;
    [SerializeField] private Transform parentTf;

    public T[] objs { get; private set; }
    private Stack<int> freeIndices;
    public List<int> activeIndices { get; private set; }

    public void Init(int size)
    {
        objs = new T[size];
        freeIndices = new Stack<int>(size);
        activeIndices = new List<int>(size);

        for (int i = 0; i < size; i++)
        {
            T obj = Object.Instantiate(prefab, parentTf);
            obj.PoolOffset();
            obj.PoolIndex = i;
            obj.ActiveIndex = -1;

            objs[i] = obj;
            freeIndices.Push(i);
        }
    }

    // 사용 가능한 Free 추가
    public void Enqueue(T obj)
    {
        if (obj == null || obj.ActiveIndex < 0)
            return;

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
}

public interface IPoolable
{
    int PoolIndex { get; set; }
    int ActiveIndex { get; set; }

    public void PoolOffset();
    public void SetActiveOn();
    public void SetActiveOff();
}