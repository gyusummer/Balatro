using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class View<T> : MonoBehaviour
{
    public abstract void Init(T source);
}

public abstract class Factory<T, TView> : Singleton<Factory<T, TView>> where TView : View<T>
{
    [SerializeField] private TView prefab;
    [SerializeField] private Transform defaultHolder;
	
    protected static Dictionary<int, Func<T>> s_constructors { get; set; }
    protected static readonly System.Random rng = new System.Random();
	
    public T CreateRandom(Transform uiParent = null)
    {
        int randomIndex = rng.Next(s_constructors.Keys.Count);
        return Create(randomIndex, uiParent);
    }

    public T Create(int index, Transform uiParent = null)
    {
        T product = s_constructors[index].Invoke();
        Print(product, uiParent);
        return product;
    }
	
    public void Print(T source, Transform uiParent = null)
    {
        if (uiParent == null)
        {
            uiParent = defaultHolder;
        }
        TView view = Instantiate(prefab, uiParent);
        view.Init(source);
    }
}