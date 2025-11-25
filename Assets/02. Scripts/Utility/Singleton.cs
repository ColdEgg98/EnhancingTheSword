using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    public static bool IsInstance => _instance != null;
    public bool isDontDestroy = true;
    protected virtual void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            Instance = (T)this;
            if (isDontDestroy)
                DontDestroyOnLoad(this);
        }
    }
}