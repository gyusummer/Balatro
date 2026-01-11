using System;
using System.Collections.Generic;
using UnityEngine;

public class TarotFactory : Factory<ConsumableCard, TarotCardView>
{
    protected override void Awake()
    {
        s_constructors = new Dictionary<int, Func<ConsumableCard>>()
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
            { 21, () => new World() },
            { 22, () => new Aura() }
        };
    }
}