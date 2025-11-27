using System;
using System.Collections.Generic;
using UnityEngine;

public class JokerFactory  : Singleton<JokerFactory>
{
    // 딕셔너리: 카드 ID 또는 Enum을 키로, 해당 카드의 타입을 값으로 저장합니다.
    // 하지만 타입을 저장하는 대신, 바로 생성하는 델리게이트를 저장하는 것이 더 유연합니다.
    private static readonly Dictionary<int, Func<Joker>> JokerConstructors = new Dictionary<int, Func<Joker>>()
    {
        { 0, () => new Joker<CardScoredEventArgs>("GreedyJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Diamond, () => ScoreCalculator.Instance.AddMult(4))},
        { 1, () => new Joker<CardScoredEventArgs>("LustyJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Heart, () => ScoreCalculator.Instance.AddMult(4))},
        { 2, () => new Joker<CardScoredEventArgs>("WrathfulJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Spade, () => ScoreCalculator.Instance.AddMult(4))},
        { 3, () => new Joker<CardScoredEventArgs>("GluttonousJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Club, () => ScoreCalculator.Instance.AddMult(4))},
    };

    private static readonly System.Random rng = new System.Random();

    public Joker CreateRandomJoker()
    {
        List<int> availableKeys = new List<int>(JokerConstructors.Keys);

        int randomIndex = rng.Next(availableKeys.Count);
        int randomJokerId = availableKeys[randomIndex];

        Joker joker = JokerConstructors[randomJokerId].Invoke();
        PrintCard(joker);
        return joker;
    }

    public Joker CreateJoker(int index)
    {
        Joker joker = JokerConstructors[index].Invoke();
        PrintCard(joker);
        return joker;
    }

    [SerializeField] private JokerView viewPrefab;
    [SerializeField] private Transform jokerHolder;
    private JokerView PrintCard(Joker joker, Transform uiParent = null)
    {
        if (uiParent == null)
        {
            uiParent = jokerHolder;
        }
        JokerView jokerView = Instantiate(viewPrefab, uiParent);
        jokerView.Init(joker);
        return jokerView;
    }
}