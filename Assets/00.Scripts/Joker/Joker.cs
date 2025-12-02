using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public abstract class Joker : ITradeable
{
    public JokerView View;
    public string Name;
    public abstract void Register();
    public abstract void Unregister();
    public int Price { get; set; }
    public bool Buy()
    {
        if (Shop.Instance.Jokers.Contains(this))
        {
            Shop.Instance.Jokers.Remove(this);
            Inventory.Instance.Jokers.Add(this);
            Debug.Log($"{this.Name} Buy");
            return true;
        }
        return false;
    }

    public bool Sell()
    {
        if (Inventory.Instance.Jokers.Contains(this))
        {
            Inventory.Instance.Jokers.Remove(this);
            Debug.Log($"{this.Name} Sell");
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class Joker<TEventArgs> : Joker where TEventArgs : IngameEventArgs
{
    private Predicate<TEventArgs> _condition;
    private Action _effect;

    public Joker(string name, Predicate<TEventArgs> condition, Action effect)
    {
        Name = name;
        _condition = condition;
        _effect = effect;
    }
    
    public override void Register()
    {
        IngameEventManager.AddListener<TEventArgs>(ApplyEffect);
    }

    private void ApplyEffect(TEventArgs eventArgs)
    {
        if (_condition(eventArgs))
        {
            _effect.Invoke();
        }
    }

    public override void Unregister()
    {
        IngameEventManager.RemoveListener<TEventArgs>(ApplyEffect);
    }
}