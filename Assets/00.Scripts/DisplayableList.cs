using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplayable
{
	public IView View { get; set; }
}

public interface IView
{
	
}

public class DisplayableList<T> where T : IDisplayable
{
	private Transform _display;
	
	public Action<T> OnAdded;
	public Action<T> OnRemoved;
	public string Name;
	[SerializeField] private List<T> _list;
	public int Count => _list.Count;

	public DisplayableList(List<T> cards, string name)
	{
		_list = cards;
		Name = name;
	}

	public void SetDisplay(Transform display)
	{
		_display = display;
	}
	
	public T GetFirst()
	{
		return _list[0];
	}
	
	public void Add(T card)
	{
		_list.Add(card);
		if (_display != null)
		{
			
		}
		OnAdded?.Invoke(card);
	}

	public void Remove(T card)
	{
		if (_list.Remove(card))
		{
			OnRemoved?.Invoke(card);
		}
	}

	public List<T> CloneList()
	{
		List<T> reVal = new List<T>();
		reVal.AddRange(_list);
		return reVal;
	}
}
