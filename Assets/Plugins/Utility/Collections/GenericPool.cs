using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class GenericPool<T>
{
    private List<T> available;
    private List<T> used;

    public int AvailableCount
    {
        get { return this.available.Count; }
    }

    public GenericPool(IEnumerable<T> elements)
    {
        Debug.Assert(this.available == null, this);

        this.available = new List<T>(elements);
        this.used = new List<T>();
    }

    public T FindAvailable(Predicate<T> match)
    {
        return this.available.Find(match);
    }

    public T FindUsed(Predicate<T> match)
    {
        return this.used.Find(match);
    }

    public void ForEachAvailable(Action<T> action)
    {
        foreach (var obj in this.available)
        {
            action(obj);
        }
    }

    public void ForEachUsed(Action<T> action)
    {
        foreach (var obj in this.used)
        {
            action(obj);
        }
    }

    public bool TryUseRandom(out T obj)
    {
        obj = default(T);

        if (this.AvailableCount <= 0)
        {
            return false;
        }

        obj = this.available[Random.Range(0, this.AvailableCount)];

        Debug.Assert(!this.used.Contains(obj), this);

        this.available.Remove(obj);
        this.used.Add(obj);

        return true;
    }

    public void Unuse(T obj)
    {
        Debug.Assert(this.used.Contains(obj), this);
        Debug.Assert(!this.available.Contains(obj), this);

        this.used.Remove(obj);
        this.available.Add(obj);
    }

    public void UnuseAll()
    {
        foreach (var obj in this.used)
        {
            Debug.Assert(!this.available.Contains(obj), this);

            this.available.Add(obj);
        }

        this.used.Clear();
    }
}
