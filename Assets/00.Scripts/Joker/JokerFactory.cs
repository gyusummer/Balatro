using System;
using System.Collections.Generic;

public static class JokerFactory
{
    public delegate Joker JokerCreator();
    
    // 딕셔너리: 카드 ID 또는 Enum을 키로, 해당 카드의 타입을 값으로 저장합니다.
    // 하지만 타입을 저장하는 대신, 바로 생성하는 델리게이트를 저장하는 것이 더 유연합니다.
    private static readonly Dictionary<int, Func<Joker>> JokerConstructors = new Dictionary<int, Func<Joker>>()
    {
        //{ 0, new Joker<CardScoredEventArgs>("GreedyJoker",(eventArgs) => eventArgs.Card.Suit == CardSuit.Diamond, () => ScoreCalculator.Instance.AddMult(4))},
    };

    private static readonly System.Random rng = new System.Random();

    public static Joker CreateRandomJoker()
    {
        // 1. 딕셔너리의 키(ID) 목록을 가져옵니다.
        List<int> availableKeys = new List<int>(JokerConstructors.Keys);

        // 2. 랜덤한 키(카드 ID)를 선택합니다.
        int randomIndex = rng.Next(availableKeys.Count);
        int randomJokerId = availableKeys[randomIndex];

        // 3. 해당 키에 등록된 생성자(Func<T>)를 호출하여 객체를 생성합니다.
        return JokerConstructors[randomJokerId].Invoke();
    }

    public static Joker CreateJoker(int index)
    {
        return JokerConstructors[index].Invoke();
    }
}