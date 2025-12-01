using System;
using System.Collections.Generic;

public static class PlanetFactory
{
    private static readonly Dictionary<int, Func<PlanetCard>> PlanetConstructors = new Dictionary<int, Func<PlanetCard>>()
    {
        { 0, () => new Pluto() },
        { 1, () => new Mercury() },
        { 2, () => new Uranus() },
        { 3, () => new Venus() },
        { 4, () => new Saturn() },
        { 5, () => new Jupiter() },
        { 6, () => new Earth() },
        { 7, () => new Mars() },
        { 8, () => new Neptune() },
        { 9, () => new PlanetX() },
        { 10, () => new Ceres() },
        { 11, () => new Eris() },
    };

    private static readonly System.Random rng = new System.Random();
    
    public static PlanetCard CreateRandomCard()
    {
        List<int> availableKeys = new List<int>(PlanetConstructors.Keys);
        
        int randomIndex = rng.Next(availableKeys.Count);
        int randomCardId = availableKeys[randomIndex];

        return PlanetConstructors[randomCardId].Invoke();
    }

    public static PlanetCard CreateCard(int index)
    {
        return PlanetConstructors[index].Invoke();
    }
}