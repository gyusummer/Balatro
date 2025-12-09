using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CustomList<T>
{
    public Action<T> OnAdded;
    public Action<T> OnRemoved;
    [SerializeField] protected List<T> _list;
    public int Count => _list.Count;
    public string Name;
    public int Max;

    public CustomList(int max = 256)
    {
        _list = new List<T>(max);
        Name = "";
        Max = max;
    }
    
    public CustomList(IList<T> list, string name, int max = 256)
    {
        _list = new List<T>(list);
        Name = name;
        Max = max;
    }
    
    public T First() => _list[0];
    
    public bool Contains(T item) => _list.Contains(item);
	
    public void Add(T element)
    {
        if (Count >= Max)
        {
            Debug.Log("Exceeded maximum");
            return;
        }
        _list.Add(element);
        OnAdded?.Invoke(element);
    }

    public void Remove(T element)
    {
        if (_list.Remove(element))
        {
            OnRemoved?.Invoke(element);
        }
    }

    public void ClearWith(Action<T> callback)
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            T element = _list[i];
            Remove(element);
            callback.Invoke(element);
        }
    }

    public void Clear()
    {
        Debug.Log("Custom List Clear");
        for (int i = Count - 1; i >= 0; i--)
        {
            Remove(_list[i]);
        }
    }

    public List<T> CloneList()
    {
        return new List<T>(_list);
    }
}