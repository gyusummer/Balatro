using System;
using System.Collections.Generic;
using UnityEngine;

public class JokerFactory  : Factory<Joker, JokerView>
{
    private static Action<double> AddMult => ScoreCalculator.Instance.AddMult;
    private static Action<double> AddChip => ScoreCalculator.Instance.AddChip;
    protected override void Awake()
    {
        base.Awake();
        s_constructors = new Dictionary<int, Func<Joker>>
        {
            { 0, () => new EventJoker<CardScoredEventArgs>("Greedy Joker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Diamond, () => ScoreCalculator.Instance.AddMult(4))},
            { 1, () => new EventJoker<CardScoredEventArgs>("Lusty Joker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Heart, () => ScoreCalculator.Instance.AddMult(4))},
            { 2, () => new EventJoker<CardScoredEventArgs>("Wrathful Joker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Spade, () => ScoreCalculator.Instance.AddMult(4))},
            { 3, () => new EventJoker<CardScoredEventArgs>("Gluttonous Joker", 5,(eventArgs) => eventArgs.Card.Suit == CardSuit.Club, () => ScoreCalculator.Instance.AddMult(4))},
            { 4, () => new ActivateJoker("Jimbo",  2,(handInfo) => AddMult(2))},
            { 5, () => new ActivateJoker("Jolly Joker",  3, info => PokerHand.Is(HandRank.Pair, info.PlayedCards),(handInfo) => AddMult(8))},
            { 6, () => new ActivateJoker("Zany Joker",  4, info => PokerHand.Is(HandRank.ThreeOfAKind, info.PlayedCards),(handInfo) => AddMult(12))},
            { 7, () => new ActivateJoker("Mad Joker",  4, info => PokerHand.Is(HandRank.TwoPair, info.PlayedCards),(handInfo) => AddMult(10))},
            { 8, () => new ActivateJoker("Crazy Joker",  4, info => PokerHand.Is(HandRank.Straight, info.PlayedCards),(handInfo) => AddMult(12))},
            { 9, () => new ActivateJoker("Droll Joker",  4, info => PokerHand.Is(HandRank.Flush, info.PlayedCards),(handInfo) => AddMult(10))},
            { 10, () => new ActivateJoker("Sly Joker",  3, info => PokerHand.Is(HandRank.Pair, info.PlayedCards),(handInfo) => AddChip(50))},
            { 11, () => new ActivateJoker("Wily Joker",  4, info => PokerHand.Is(HandRank.ThreeOfAKind, info.PlayedCards),(handInfo) => AddChip(100))},
            { 12, () => new ActivateJoker("Clever Joker",  4, info => PokerHand.Is(HandRank.TwoPair, info.PlayedCards),(handInfo) => AddChip(80))},
            { 13, () => new ActivateJoker("Devious Joker",  4, info => PokerHand.Is(HandRank.Straight, info.PlayedCards),(handInfo) => AddChip(100))},
            { 14, () => new ActivateJoker("Crafty Joker",  4, info => PokerHand.Is(HandRank.Flush, info.PlayedCards),(handInfo) => AddChip(80))},
            { 15, () => new ActivateJoker("Half Joker",  5, info => info.PlayedCards.Count <= 3,(handInfo) => AddMult(20))},
        };
    }
}