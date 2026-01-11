using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public abstract class Joker : ITradeable
{
    public JokerView View;
    public string Name;
    private Edition _edition = Edition.None;

    public Edition Edition
    {
        get => _edition;
        set
        {
            _edition = value;
            View?.UpdateShader();
        }
    }
    public bool IsPlayerOwned { get; set; }
    public int Price { get; set; } = 3;
    public abstract void Register();
    public abstract void Unregister();
    public Predicate<HandInfo> ActivateCondition { get; set; } = null;
    public Action<HandInfo> ActivateEffect { get; set; } = null;
	public Sequence ActivateAnimation => View.ActivateAnimation();

    public Sequence Activate(HandInfo handInfo)
    {
        Sequence seq = DOTween.Sequence();
        
        if (ActivateEffect != null && ActivateCondition(handInfo))
        {
            seq.Append(ActivateAnimation);
            seq.JoinCallback(() => ActivateEffect.Invoke(handInfo));
        }
        
        switch (Edition)
        {
            case Edition.Foil:
                seq.Append(ActivateAnimation);
                seq.JoinCallback(() => ScoreCalculator.Instance.AddChip(50));
                break;
            case Edition.Holographic:
                seq.Append(ActivateAnimation);
                seq.JoinCallback(() => ScoreCalculator.Instance.AddMult(10));
                break;
            case Edition.Polychrome:
                seq.Append(ActivateAnimation);
                seq.JoinCallback(() => ScoreCalculator.Instance.ScaleMult(1.5d));
                break;
            default:
                break;
        }

        return seq;
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
    public ActivateJoker(string name, int price, Action<HandInfo> activateEffect)
    {
        Name = name;
        Price = price;
        ActivateCondition = (handInfo) => true;
        ActivateEffect = activateEffect;
    }
    
    public ActivateJoker(string name, int price, Predicate<HandInfo> activateCondition, Action<HandInfo> activateEffect)
    {
        Name = name;
        Price = price;
        ActivateCondition = activateCondition;
        ActivateEffect = activateEffect;
    }

    public override void Register()
    {
    }

    public override void Unregister()
    {
    }
}

[Serializable]
public class EventJoker<TEventArgs> : Joker where TEventArgs : IngameEventArgs
{
    private Predicate<TEventArgs> _condition;
    private Action _effect;

    public EventJoker(string name, int price, Predicate<TEventArgs> condition, Action effect)
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
        if (_condition(eventArgs) == false)
        {
            return;
        }

        Sequence seq = eventArgs.Sequence;
        seq.Append(ActivateAnimation);
        seq.JoinCallback(() => _effect.Invoke());
        if (eventArgs is CardActivateEventArgs args)
        {
            seq.Join(args.Card.ActivateAnimation);
        }
    }
    
    public override void Unregister()
    {
        IngameEventManager.RemoveListener<TEventArgs>(ApplyEffect);
    }
}