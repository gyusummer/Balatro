using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CustomList<T> : Collection<T>
{
    public Action<T> OnAdded;
    public Action<T> OnRemoved;
    public string Name;
    public int Max;

    public CustomList(int max = 256)
    {
        Name = "";
        Max = max;
    }
    
    public CustomList(IList<T> list, string name, int max = int.MaxValue) : base(list)
    {
        Name = name;
        Max = max;
    }
    
    public T First() => this[0];
	
    public new void Add(T element)
    {
        if (Count >= Max)
        {
            Debug.Log("Exceeded maximum");
            return;
        }
        base.Add(element);
        OnAdded?.Invoke(element);
    }

    public new void Remove(T element)
    {
        if (base.Remove(element))
        {
            OnRemoved?.Invoke(element);
        }
    }

    public void ClearWith(Action<T> callback)
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            T element = this[i];
            Remove(element);
            callback.Invoke(element);
        }
    }

    public new void Clear()
    {
        Debug.Log("Custom List Clear");
        for (int i = Count - 1; i >= 0; i--)
        {
            Remove(this[i]);
        }
    }

    public List<T> CloneList()
    {
        return new List<T>(this);
    }
}