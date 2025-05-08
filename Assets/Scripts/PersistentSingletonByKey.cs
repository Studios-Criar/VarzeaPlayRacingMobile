using System.Collections.Generic;
using UnityEngine;

public class PersistentSingletonByKey : MonoBehaviour
{
    [SerializeField] private string key;

    private static readonly Dictionary<string, PersistentSingletonByKey> Instances = new();
    
    private void Awake()
    {
        if (Instances.TryGetValue(key, out var instance) && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instances[key] = this;
        DontDestroyOnLoad(gameObject);
    }
}
