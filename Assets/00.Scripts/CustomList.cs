using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomList<T>
{
    public Action<T> OnAdded;
    public Action<T> OnRemoved;
    public string Name;
    [SerializeField] private List<T> _list;
    public int Count => _list.Count;
    public int Max;

    public CustomList(int max = int.MaxValue)
    {
        _list = new List<T>();
        Name = "";
        Max = max;
    }
    public CustomList(List<T> cards, string name, int max = int.MaxValue)
    {
        _list = cards;
        Name = name;
        Max = max;
    }
	
    public T GetFirst()
    {
        return _list[0];
    }
	
    public void Add(T element)
    {
        if (Count >= Max)
        {
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

    public void Clear()
    {
        Debug.Log(Count);
        for (int i = Count - 1; i >= 0; i--)
        {
            Remove(_list[i]);
        }
    }

    public List<T> CloneList()
    {
        List<T> newList = new List<T>();
        newList.AddRange(_list);
        return newList;
    }
}