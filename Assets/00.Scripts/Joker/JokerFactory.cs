using System;
using System.Collections.Generic;
using UnityEngine;

public class JokerFactory  : Factory<Joker, JokerView>
{
    private static Action<double> AddMult => ScoreCalculator.Instance.AddMult;
    protected override void Awake()
    {
        base.Awake();
        s_constructors = new Dictionary<int, Func<Joker>>
        {
            { 0, () => new EventJoker<CardScoredEventArgs>("GreedyJoker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Diamond, () => ScoreCalculator.Instance.AddMult(4))},
            { 1, () => new EventJoker<CardScoredEventArgs>("LustyJoker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Heart, () => ScoreCalculator.Instance.AddMult(4))},
            { 2, () => new EventJoker<CardScoredEventArgs>("WrathfulJoker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Spade, () => ScoreCalculator.Instance.AddMult(4))},
            { 3, () => new EventJoker<CardScoredEventArgs>("GluttonousJoker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Club, () => ScoreCalculator.Instance.AddMult(4))},
            { 4, () => new ActivateJoker("Jimbo",  2,() => AddMult(2))},
        };
    }
}