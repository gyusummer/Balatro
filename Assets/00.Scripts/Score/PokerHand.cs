using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum HandRank
{
    HighCard = 0,
    Pair = 1,
    TwoPair = 2,
    ThreeOfAKind = 3,
    Straight = 4,
    Flush = 5,
    FullHouse = 6,
    FourOfAKind = 7,
    StraightFlush = 8,
    FiveOfAKind = 9,
    FlushHouse,
    FlushFive
}

public struct HandInfo
{
    public HandRank Rank;
    public List<PlayingCard> SelectedCards; // 선택된 카드
    public List<PlayingCard> ScoredCards; // 족보를 이룬 카드 (특수 효과 발동 대상)
    public List<PlayingCard> ExtraCards;  // 나머지 카드 (키커)
    
    public HandInfo(List<PlayingCard> hand)
    {
        Rank = HandRank.HighCard;
        SelectedCards = hand;
        ScoredCards = hand.OrderByDescending(c => c.Rank).Take(1).ToList(); // 가장 높은 카드 1장만 scoredCards에 포함
        ExtraCards = hand.OrderByDescending(c => c.Rank).Skip(1).ToList();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Rank.ToString());
        sb.AppendLine();
        sb.AppendLine("Scored:");
        foreach (var card in ScoredCards)
        {
            sb.AppendLine(card.ToString());
        }

        sb.AppendLine();
        sb.AppendLine("NotScored:");
        foreach (var card in ExtraCards)
        {
            sb.AppendLine(card.ToString());
        }
        sb.Append("## End");
        return sb.ToString();
    }
}
    
public struct PreprocessedHand
{
    public Dictionary<CardRank, List<PlayingCard>> RankGroups; // 랭크별 그룹핑 (페어, 트리플 등)
    public Dictionary<CardSuit, List<PlayingCard>> SuitGroups; // 무늬별 그룹핑 (플러시)
    public List<CardRank> SortedUniqueRanks;                   // 정렬된 고유 랭크 (스트레이트)
}

public struct ScoreComponent
{
    public double Chip;
    public double Mult;
    public double Result => Chip * Mult;

    public ScoreComponent(double chip, double mult)
    {
        Chip = chip;
        Mult = mult;
    }
}

public static class PokerHand
{
    public static PreprocessedHand Preprocess(List<PlayingCard> hand)
    {
        // C# LINQ를 사용해 단 한 번의 순회로 모든 그룹핑을 효율적으로 수행합니다.
        var rankGroups = hand.GroupBy(card => card.Rank).ToDictionary(g => g.Key, g => g.ToList());
        var suitGroups = hand.GroupBy(card => card.Suit).ToDictionary(g => g.Key, g => g.ToList());
        var sortedRanks = hand.Select(card => card.Rank).Distinct().OrderBy(r => r).ToList();
    
        return new PreprocessedHand
        {
            RankGroups = rankGroups,
            SuitGroups = suitGroups,
            SortedUniqueRanks = sortedRanks
        };
    }
    
    public static HandInfo CheckHandRank(List<PlayingCard> hand)
    {
        PreprocessedHand pHand = Preprocess(hand);
    
        // 1. 가장 높은 족보부터 검사 (우선순위 역순)
        HandInfo result = new HandInfo(hand);
        
#pragma warning disable 0642
        if (IsFlushFive(pHand, hand, ref result)) ;
        else if (IsFlushHouse(pHand, hand, ref result)) ;
        else if (IsFiveOfAKind(pHand, hand, ref result)) ;
        else if (IsStraightFlush(pHand, hand, ref result)) ;
        else if (IsFourOfAKind(pHand, hand, ref result)) ;
        else if (IsFullHouse(pHand, hand, ref result)) ;
        else if (IsFlush(pHand, hand, ref result)) ;
        else if (IsStraight(pHand, hand, ref result)) ;
        else if (IsThreeOfAKind(pHand, hand, ref result)) ;
        else if (IsTwoPair(pHand, hand, ref result));
        else if (IsPair(pHand, hand, ref result)) ;
#pragma warning restore 0642
    
        // 2. 일치하는 족보가 없으면 '하이 카드'로 처리
        return result;
    }

    public static bool IsFlushFive(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        HandInfo flushResult =  new HandInfo(hand); 
        HandInfo fiveKindResult =  new HandInfo(hand); 
        
        if (IsFlush(pHand, hand, ref flushResult) && IsFiveOfAKind(pHand, hand, ref fiveKindResult))
        {
            List<PlayingCard> scoredCards = flushResult.ScoredCards.Union(fiveKindResult.ScoredCards).ToList();
            List<PlayingCard> extraCards = flushResult.ExtraCards.Intersect(fiveKindResult.ExtraCards).ToList();

            result.Rank = HandRank.FlushFive;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;

            return true;
        }

        return false;
    }

    public static bool IsFlushHouse(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        HandInfo flushResult =  new HandInfo(hand); 
        HandInfo houseResult =  new HandInfo(hand); 
        
        if (IsFlush(pHand, hand, ref flushResult) && IsFullHouse(pHand, hand, ref houseResult))
        {
            List<PlayingCard> scoredCards = flushResult.ScoredCards.Union(houseResult.ScoredCards).ToList();
            List<PlayingCard> extraCards = flushResult.ExtraCards.Intersect(houseResult.ExtraCards).ToList();

            result.Rank = HandRank.FlushHouse;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        
        return false;
    }

    public static bool IsFiveOfAKind(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        var fiveGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 5);

        if (fiveGroup.Value != null && fiveGroup.Value.Count == 5)
        {
            List<PlayingCard> scoredCards = fiveGroup.Value;
            List<PlayingCard> extraCards = new List<PlayingCard>();

            result.Rank = HandRank.FiveOfAKind;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        return false;
    }

    public static bool IsStraightFlush(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        HandInfo flushResult =  new HandInfo(hand); 
        HandInfo straightResult =  new HandInfo(hand); 
        
        if (IsFlush(pHand, hand, ref flushResult) && IsStraight(pHand, hand, ref straightResult))
        {
            List<PlayingCard> scoredCards = flushResult.ScoredCards.Union(straightResult.ScoredCards).ToList();
            List<PlayingCard> extraCards = flushResult.ExtraCards.Intersect(straightResult.ExtraCards).ToList();

            result.Rank = HandRank.StraightFlush;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;

            return true;
        }
        
        return false;
    }

    public static bool IsFourOfAKind(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        var fourGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 4);

        if (fourGroup.Value != null && fourGroup.Value.Count == 4)
        {
            List<PlayingCard> scoredCards = fourGroup.Value;
            List<PlayingCard> extraCards = hand.Except(scoredCards).OrderByDescending(c => c.Rank).ToList();


            result.Rank = HandRank.FourOfAKind;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        return false;
    }

    public static bool IsFullHouse(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        // 풀 하우스: 트리플을 이룬 그룹(3장)과 페어를 이룬 그룹(2장)이 모두 존재하는지 확인
        var tripleGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 3);
        var pairGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 2);

        if (tripleGroup.Key == default(CardRank) || pairGroup.Key == default(CardRank))
        {
            return false;
        }

        // 족보를 이룬 카드 = 트리플 + 페어 카드 전체
        List<PlayingCard> scoredCards = new List<PlayingCard>();
        scoredCards.AddRange(tripleGroup.Value);
        scoredCards.AddRange(pairGroup.Value);
        
        // 풀 하우스는 5장 모두 족보에 사용되므로 extraCards는 비어 있습니다.
        result.Rank = HandRank.FullHouse;
        result.ScoredCards = scoredCards;
        result.ExtraCards = new List<PlayingCard>();
        
        return true;
    }

    public static bool IsFlush(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        var flushGroup = pHand.SuitGroups.FirstOrDefault(kv => kv.Value.Count >= 5);

        if (flushGroup.Value != null && flushGroup.Value.Count >= 5)
        {
            // 족보를 이룬 5장의 카드를 랭크 순으로 정렬하여 추출
            List<PlayingCard> scoredCards = flushGroup.Value
                .OrderByDescending(c => c.Rank)
                .Take(5)
                .ToList();
        
            List<PlayingCard> extraCards = hand.Except(scoredCards).OrderByDescending(c => c.Rank).ToList();

            result.Rank = HandRank.Flush;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        return false;
    }

    public static bool IsStraight(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        var sortedRanks = pHand.SortedUniqueRanks;
        if (sortedRanks.Count < 5) return false;

        // A. 일반적인 스트레이트 검사
        for (int i = 0; i <= sortedRanks.Count - 5; i++)
        {
            if (sortedRanks[i + 4] - sortedRanks[i] == 4)
            {
                CardRank highRank = sortedRanks[i + 4];
            
                List<PlayingCard> scoredCards = hand.Where(c => (int)c.Rank >= (int)sortedRanks[i] && (int)c.Rank <= (int)highRank)
                    .ToList();

                result.Rank = HandRank.Straight;
                result.ScoredCards = scoredCards;
                result.ExtraCards = new List<PlayingCard>();
                
                return true;
            }
        }
    
        // B. A-2-3-4-5 (Wheel) 특수 검사
        // Ace, 2, 3, 4, 5의 랭크가 모두 포함되어 있는지 확인
        bool hasWheel = sortedRanks.Contains(CardRank.Ace) && sortedRanks.Contains(CardRank.Five) &&
                        sortedRanks.Contains(CardRank.Four) && sortedRanks.Contains(CardRank.Three) &&
                        sortedRanks.Contains(CardRank.Two);

        if (hasWheel)
        {
            List<PlayingCard> wheelCards = hand.Where(c => 
                c.Rank == CardRank.Ace || c.Rank == CardRank.Two || c.Rank == CardRank.Three || 
                c.Rank == CardRank.Four || c.Rank == CardRank.Five).ToList();

            result.Rank = HandRank.Straight;
            result.ScoredCards = wheelCards;
            result.ExtraCards = new List<PlayingCard>();
            
            return true;
        }

        return false;
    }

    public static bool IsThreeOfAKind(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        var tripleGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 3);

        if (tripleGroup.Value != null && tripleGroup.Value.Count == 3)
        {
            // 풀 하우스는 이미 앞서 검사했으므로, 여기서 찾은 것은 순수한 트리플입니다.
            List<PlayingCard> scoredCards = tripleGroup.Value;
        
            List<PlayingCard> extraCards = hand.Except(scoredCards).OrderByDescending(c => c.Rank).ToList();

            result.Rank = HandRank.ThreeOfAKind;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        return false;
    }

    public static bool IsTwoPair(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        // 페어 그룹을 랭크가 높은 순서대로 2개 찾습니다.
        var pairGroups = pHand.RankGroups.Where(kv => kv.Value.Count == 2)
            .OrderByDescending(kv => kv.Key)
            .Take(2)
            .ToList();

        if (pairGroups.Count == 2)
        {
            List<PlayingCard> scoredCards = new List<PlayingCard>();
            scoredCards.AddRange(pairGroups[0].Value);
            scoredCards.AddRange(pairGroups[1].Value);

            List<PlayingCard> extraCards = hand.Except(scoredCards).OrderByDescending(c => c.Rank).ToList();

            result.Rank = HandRank.TwoPair;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        return false;
    }

    public static bool IsPair(PreprocessedHand pHand, List<PlayingCard> hand, ref HandInfo result)
    {
        // 가장 높은 랭크의 페어 그룹을 찾습니다.
        var pairGroup = pHand.RankGroups.Where(kv => kv.Value.Count == 2)
            .OrderByDescending(kv => kv.Key)
            .FirstOrDefault();

        if (pairGroup.Value != null && pairGroup.Value.Count == 2)
        {
            List<PlayingCard> scoredCards = pairGroup.Value;
        
            List<PlayingCard> extraCards = hand.Except(scoredCards).OrderByDescending(c => c.Rank).ToList();

            result.Rank = HandRank.Pair;
            result.ScoredCards = scoredCards;
            result.ExtraCards = extraCards;
            
            return true;
        }
        return false;
    }
}
