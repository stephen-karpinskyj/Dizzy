using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectPool : MonoBehaviour
{
    private Dictionary<Type, Queue<LevelObjectController>> unusedObjects;
        
    private void Awake()
    {
        this.unusedObjects = new Dictionary<Type, Queue<LevelObjectController>>();
    }
        
    public T Get<T>() where T : LevelObjectController
    {
        var t = typeof(T);
        var queue = GetQueue(t);
                
        switch (queue.Count)
        {
            case 0: return InstantiateLevelObject<T>();
            default: return queue.Dequeue() as T;
        }
    }
    
    public void Return<T>(T obj) where T : LevelObjectController
    {
        Debug.Assert(obj != null);
        
        var t = typeof(T);
        var queue = GetQueue(t);
        
        queue.Enqueue(obj);
    }
    
    private Queue<LevelObjectController> GetQueue(Type t)
    {
        Queue<LevelObjectController> queue;
    
        if (!this.unusedObjects.TryGetValue(t, out queue))
        {
            queue = new Queue<LevelObjectController>();
            this.unusedObjects[t] = queue;
        }
        
        Debug.Assert(queue != null);
        
        return queue;
    }
    
    private T InstantiateLevelObject<T>() where T : LevelObjectController
    {
        T prefab;
        
        if (typeof(T) == typeof(JunkController))
        {
            prefab = Data.Instance.JunkPrefab as T;
        }
        else
        {
            throw new NotSupportedException();
        }
        
        var obj = GameObjectUtility.InstantiatePrefab(prefab, this.transform);
        
        Debug.Assert(obj != null);
        
        return obj;
    }
}
