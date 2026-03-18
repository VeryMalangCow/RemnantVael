using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance = null;

    protected virtual void Awake()
    {
        if (null == instance)
        {
            instance = (T)this;
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
        if (instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}