using System;
using System.Collections.Generic;
using UnityEngine;

public class JokerFactory  : Factory<Joker, JokerView>
{
    protected override void Awake()
    {
        base.Awake();
        s_constructors = new Dictionary<int, Func<Joker>>()
        {
            { 0, () => new Joker<CardScoredEventArgs>("GreedyJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Diamond, () => ScoreCalculator.Instance.AddMult(4))},
            { 1, () => new Joker<CardScoredEventArgs>("LustyJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Heart, () => ScoreCalculator.Instance.AddMult(4))},
            { 2, () => new Joker<CardScoredEventArgs>("WrathfulJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Spade, () => ScoreCalculator.Instance.AddMult(4))},
            { 3, () => new Joker<CardScoredEventArgs>("GluttonousJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Club, () => ScoreCalculator.Instance.AddMult(4))},
        };
    }
}