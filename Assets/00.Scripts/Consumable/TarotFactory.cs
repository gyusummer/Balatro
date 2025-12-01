using System;
using System.Collections.Generic;

public static class TarotFactory
{
    private static readonly Dictionary<int, Func<TarotCard>> TarotConstructors = new Dictionary<int, Func<TarotCard>>()
    {
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

    public static TarotCard CreateRandomCard()
    {
        List<int> availableKeys = new List<int>(TarotConstructors.Keys);
        
        int randomIndex = rng.Next(availableKeys.Count);
        int randomCardId = availableKeys[randomIndex];

        return TarotConstructors[randomCardId].Invoke();
    }

    public static TarotCard CreateCard(int index)
    {
        return TarotConstructors[index].Invoke();
    }
}