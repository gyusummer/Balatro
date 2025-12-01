using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFactory : Factory<ConsumableCard, PlanetCardView>
{
    protected override void Awake()
    {
        s_constructors = new Dictionary<int, Func<ConsumableCard>>()
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
    }
}