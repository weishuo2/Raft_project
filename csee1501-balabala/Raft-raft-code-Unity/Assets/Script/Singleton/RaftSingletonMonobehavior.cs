using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftSingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Start()
    {
        if (this != Instance)
        {
            GameObject go = this.gameObject; 

            Destroy(this);
            Destroy(go);
        }
    }                 
}
