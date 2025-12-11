using System;
using UnityEngine;

[Serializable]
public abstract class Joker : ITradeable
{
    public JokerView View;
    public string Name;
    public Edition Edition = Edition.None;
    public bool IsPlayerOwned { get; set; }
    public int Price { get; set; } = 3;
    public abstract void Register();
    public abstract void Unregister();
    public Action ActivateEffect { get; set; } = null;

    public void Activate()
    {
        ActivateEffect?.Invoke();
        switch (Edition)
        {
            case Edition.Foil:
                ScoreCalculator.Instance.AddChip(50);
                break;
            case Edition.Holographic:
                ScoreCalculator.Instance.AddMult(10);
                break;
            case Edition.Polychrome:
                ScoreCalculator.Instance.ScaleMult(1.5d);
                break;
            default:
                break;
        }
    }
    
    public bool Buy()
    {
        if (Shop.Instance.Jokers.Contains(this) && Inventory.Instance.Jokers.IsFull == false)
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

public class ActivateJoker : Joker
{
    public ActivateJoker(string name, int price, Action activateEffect)
    {
        Name = name;
        Price = price;
        ActivateEffect = activateEffect;
    }

    public override void Register()
    {
        if (Edition == Edition.Negative)
        {
            Inventory.Instance.Jokers.Max++;
        }
    }

    public override void Unregister()
    {
        if (Edition == Edition.Negative)
        {
            Inventory.Instance.Jokers.Max--;
        }
    }
}

[Serializable]
public class Joker<TEventArgs> : Joker where TEventArgs : IngameEventArgs
{
    private Predicate<TEventArgs> _condition;
    private Action _effect;

    public Joker(string name, int price, Predicate<TEventArgs> condition, Action effect)
    {
        Name = name;
        Price = price;
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