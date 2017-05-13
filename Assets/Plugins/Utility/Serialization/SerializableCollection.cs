using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
/// <remarks>Overcomes UnityEngine.JsonUtility's inability to implicitly (de-)serialize derived types in a collection.</remarks>
public abstract class SerializableCollection<T> : ICollection<T>, ISerializationCallbackReceiver
{
    /// <remarks>Temporary list for serialization purposes only.</remarks>
    [SerializeField]
    private List<SerializableDerivedType> elements = new List<SerializableDerivedType>();

    protected List<T> Elements = new List<T>();

    protected abstract bool TryJsonSerialize(T element, out string json);
    protected abstract bool TryJsonDeserialize(string type, string json, out T element);

    protected virtual void OnAdd(T element) { }


    #region ICollection implementation


    public void Add(T item)
    {
        this.Elements.Add(item);
        this.OnAdd(item);
    }

    public void Clear()
    {
        this.Elements.Clear();
    }

    public bool Contains(T item)
    {
        return this.Elements.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        this.Elements.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return this.Elements.Remove(item);
    }

    public int Count
    {
        get { return this.Elements.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }


    #endregion


    #region IEnumerable implementation


    public IEnumerator<T> GetEnumerator()
    {
        return this.Elements.GetEnumerator();
    }


    #endregion


    #region IEnumerable implementation


    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.Elements.GetEnumerator();
    }


    #endregion


    #region ISerializationCallbackReceiver


    public void OnBeforeSerialize()
    {
        this.elements.Clear();

        foreach (var e in this.Elements)
        {
            string json;
            var success = this.TryJsonSerialize(e, out json);

            if (success)
            {
                this.elements.Add(new SerializableDerivedType { Type = e.GetType().Name, Json = json });
            }
            else
            {
                Debug.LogWarning("[SerializableCollection] Skipping serialization of element with unknown type=" + e.GetType());
            }
        }
    }

    public void OnAfterDeserialize()
    {
        this.Elements.Clear();

        foreach (var se in this.elements)
        {
            T element;
            var success = this.TryJsonDeserialize(se.Type, se.Json, out element);

            if (success)
            {
                this.Elements.Add(element);
            }
            else
            {
                Debug.LogWarning("[SerializableCollection] Skipping deserialization of element with unknown type=" + se.Type);
            }
        }
    }


    #endregion
}
