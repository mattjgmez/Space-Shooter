using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError($"{typeof(T)} is NULL.");
            return _instance;
        }
    }

    void Awake()
    {
        _instance = (T)this;

        Init();
    }

    public virtual void Init()
    {

    }
}
