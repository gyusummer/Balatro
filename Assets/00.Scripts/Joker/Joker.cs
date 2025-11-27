using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public abstract class Joker
{
    public abstract void Register();
    public abstract void Unregister();
}

public class Joker<TEventArgs> : Joker where TEventArgs : IngameEventArgs
{
    private Predicate<TEventArgs> condition;
    private Action effect;

    public Joker(Predicate<TEventArgs> condition, Action effect)
    {
        this.condition = condition;
        this.effect = effect;
    }
    
    public override void Register()
    {
        IngameEventManager.AddListener<TEventArgs>(ApplyEffect);
    }

    private void ApplyEffect(TEventArgs eventArgs)
    {
        if (condition(eventArgs))
        {
            effect.Invoke();
        }
    }

    public override void Unregister()
    {
        IngameEventManager.RemoveListener<TEventArgs>(ApplyEffect);
    }
}

public class JokerManager
{
    public List<Joker> Jokers = new List<Joker>()
    {
        new Joker<CardScoredEventArgs>((eventArgs) => eventArgs.Card.Suit == CardSuit.Diamond, () => ScoreCalculator.Instance.AddMult(4)),
        new Joker<CardScoredEventArgs>((eventArgs) => eventArgs.Card.Suit == CardSuit.Club, () => ScoreCalculator.Instance.AddMult(4)),
        new Joker<CardScoredEventArgs>((eventArgs) => eventArgs.Card.Suit == CardSuit.Heart, () => ScoreCalculator.Instance.AddMult(4)),
        new Joker<CardScoredEventArgs>((eventArgs) => eventArgs.Card.Suit == CardSuit.Spade, () => ScoreCalculator.Instance.AddMult(4)),
    };
}