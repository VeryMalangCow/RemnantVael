using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance = null;

    protected virtual void Awake()
    {
        if (null == Instance)
        {
            Instance = (T)this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}

public class PersistentSingleton<T> : Singleton<T> where T : PersistentSingleton<T>
{
    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}