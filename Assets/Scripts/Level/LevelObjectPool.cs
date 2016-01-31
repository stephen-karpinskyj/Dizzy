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
        
        T obj;
        
        switch (queue.Count)
        {
            case 0: obj = InstantiateLevelObject<T>(); break;
            default: obj = queue.Dequeue() as T; break;
        }
        
        obj.gameObject.SetActive(true);
        
        return obj;
    }
    
    public void Return(LevelObjectController obj)
    {
        Debug.Assert(obj != null);
        
        var t = obj.GetType();
        var queue = GetQueue(t);
        
        obj.gameObject.SetActive(false);
        obj.transform.parent = this.transform;
        obj.transform.localScale = Vector3.one;
        
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
        Type t = typeof(T); 
        
        if (t == typeof(JunkController))
        {
            prefab = Data.Instance.JunkPrefab as T;
        }
        else if (t == typeof(MineController))
        {
            prefab = Data.Instance.MinePrefab as T;
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
