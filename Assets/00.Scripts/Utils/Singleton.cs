using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T s_Instance;

    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<T>();
                if (s_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    s_Instance = obj.AddComponent<T>();
                }
            }
            return s_Instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (s_Instance != this)
        {
            Debug.Log($"{gameObject.name} : {s_Instance}");
            Destroy(gameObject);
        }
    }
}
