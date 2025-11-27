
using System;
using System.Collections.Generic;

public static class TarotFactory
{
    // 딕셔너리: 카드 ID 또는 Enum을 키로, 해당 카드의 타입을 값으로 저장합니다.
    // 하지만 타입을 저장하는 대신, 바로 생성하는 델리게이트를 저장하는 것이 더 유연합니다.
    private static readonly Dictionary<int, Func<TarotCard>> CardConstructors = new Dictionary<int, Func<TarotCard>>()
    {
        // 모든 22개 카드의 생성자를 람다 표현식으로 등록
        { 0, () => new Fool() },
        { 1, () => new Magician() },
        { 2, () => new HighPriestess() },
        { 3, () => new Empress() },
        { 4, () => new Emperor() },
        { 5, () => new Hierophant() },
        { 6, () => new Lovers() },
        { 7, () => new Chariot() },
        { 8, () => new Justice() },
        { 9, () => new Hermit() },
        { 10, () => new WheelOfFortune() },
        { 11, () => new Strength() },
        { 12, () => new HangedMan() },
        { 13, () => new Death() },
        { 14, () => new Temperance() },
        { 15, () => new Devil() },
        { 16, () => new Tower() },
        { 17, () => new Star() },
        { 18, () => new Moon() },
        { 19, () => new Sun() },
        { 20, () => new Judgement() },
        { 21, () => new World() }
    };

    private static readonly System.Random rng = new System.Random();
	
    /// <summary>
    /// 랜덤한 타로카드 객체를 생성하여 반환합니다.
    /// </summary>
    public static TarotCard CreateRandomCard()
    {
        // 1. 딕셔너리의 키(ID) 목록을 가져옵니다.
        List<int> availableKeys = new List<int>(CardConstructors.Keys);
        
        // 2. 랜덤한 키(카드 ID)를 선택합니다.
        int randomIndex = rng.Next(availableKeys.Count);
        int randomCardId = availableKeys[randomIndex];

        // 3. 해당 키에 등록된 생성자(Func<TarotCard>)를 호출하여 객체를 생성합니다.
        return CardConstructors[randomCardId].Invoke();
    }

    public static TarotCard CreateCard(int index)
    {
        return CardConstructors[index].Invoke();
    }
}