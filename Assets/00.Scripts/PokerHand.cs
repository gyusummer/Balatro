using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public struct HandResult
{
    public HandRank Rank;
    public List<PlayingCard> RankingCards; // 족보를 이룬 카드 (특수 효과 발동 대상)
    public List<PlayingCard> KickerCards;  // 나머지 카드 (키커)
}
    
public struct PreprocessedHand
{
    public Dictionary<CardRank, List<PlayingCard>> RankGroups; // 랭크별 그룹핑 (페어, 트리플 등)
    public Dictionary<CardSuit, List<PlayingCard>> SuitGroups; // 무늬별 그룹핑 (플러시)
    public List<CardRank> SortedUniqueRanks;                   // 정렬된 고유 랭크 (스트레이트)
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
    
    public static HandResult CheckHandRank(List<PlayingCard> hand)
    {
        PreprocessedHand pHand = Preprocess(hand);
    
        // 1. 가장 높은 족보부터 검사 (우선순위 역순)
        HandResult result;

        if (IsFlushFive(pHand, hand, out result)) return result;
        if (IsFlushHouse(pHand, hand, out result)) return result;
        if (IsFiveOfAKind(pHand, hand, out result)) return result;
        if (IsStraightFlush(pHand, hand, out result)) return result;
        if (IsFourOfAKind(pHand, hand, out result)) return result;
        if (IsFullHouse(pHand, hand, out result)) return result;
        if (IsFlush(pHand, hand, out result)) return result;
        if (IsStraight(pHand, hand, out result)) return result;
        if (IsThreeOfAKind(pHand, hand, out result)) return result;
        if (IsTwoPair(pHand, hand, out result)) return result;
        if (IsPair(pHand, hand, out result)) return result;
    
        // 2. 일치하는 족보가 없으면 '하이 카드'로 처리
        return new HandResult 
        { 
            Rank = HandRank.HighCard, 
            RankingCards = hand.OrderByDescending(c => c.Rank).Take(1).ToList(), // 가장 높은 카드 1장만 RankingCards에 포함
            KickerCards = hand.OrderByDescending(c => c.Rank).Skip(1).ToList()
        };
    }

    public static bool IsFlushFive(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();

        return false;
    }

    public static bool IsFlushHouse(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        
        return false;
    }

    public static bool IsFiveOfAKind(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        
        return false;
    }

    public static bool IsStraightFlush(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();

        // A. 먼저 플러시 조건(같은 무늬 5장 이상) 확인
        var flushGroup = pHand.SuitGroups.FirstOrDefault(kv => kv.Value.Count >= 5);

        if (flushGroup.Value != null && flushGroup.Value.Count >= 5)
        {
            var flushCards = flushGroup.Value;
            var flushRanks = flushCards.Select(c => c.Rank).Distinct().OrderBy(r => r).ToList();

            // B. 플러시 그룹 내에서 스트레이트가 있는지 확인
            if (flushRanks.Count >= 5)
            {
                for (int i = 0; i <= flushRanks.Count - 5; i++)
                {
                    if (flushRanks[i + 4] - flushRanks[i] == 4)
                    {
                        // 스트레이트 플러시 확인!
                        CardRank highRank = flushRanks[i + 4];
                        List<PlayingCard> rankingCards = flushCards.Where(c => (int)c.Rank >= (int)flushRanks[i] && (int)c.Rank <= (int)highRank)
                            .ToList();

                        List<PlayingCard> kickerCards = hand.Except(rankingCards).OrderByDescending(c => c.Rank).ToList();
                    
                        result = new HandResult { Rank = HandRank.StraightFlush, RankingCards = rankingCards, KickerCards = kickerCards };
                        return true;
                    }
                }
                // A-2-3-4-5 (Wheel) 특수 처리 로직도 여기에 추가되어야 합니다.
            }
        }
        return false;
    }

    public static bool IsFourOfAKind(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        var fourGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 4);

        if (fourGroup.Value != null && fourGroup.Value.Count == 4)
        {
            List<PlayingCard> rankingCards = fourGroup.Value;
            List<PlayingCard> kickerCards = hand.Except(rankingCards).OrderByDescending(c => c.Rank).ToList();

            result = new HandResult { Rank = HandRank.FourOfAKind, RankingCards = rankingCards, KickerCards = kickerCards };
            return true;
        }
        return false;
    }

    public static bool IsFullHouse(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        
        // 풀 하우스: 트리플을 이룬 그룹(3장)과 페어를 이룬 그룹(2장)이 모두 존재하는지 확인
        var tripleGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 3);
        var pairGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 2);

        if (tripleGroup.Key == default(CardRank) || pairGroup.Key == default(CardRank))
        {
            return false;
        }

        // 족보를 이룬 카드 = 트리플 + 페어 카드 전체
        List<PlayingCard> rankingCards = new List<PlayingCard>();
        rankingCards.AddRange(tripleGroup.Value);
        rankingCards.AddRange(pairGroup.Value);
        
        // 풀 하우스는 5장 모두 족보에 사용되므로 KickerCards는 비어 있습니다.
        result.Rank = HandRank.FullHouse;
        result.RankingCards = rankingCards;
        result.KickerCards = new List<PlayingCard>();
        return true;
    }

    public static bool IsFlush(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        var flushGroup = pHand.SuitGroups.FirstOrDefault(kv => kv.Value.Count >= 5);

        if (flushGroup.Value != null && flushGroup.Value.Count >= 5)
        {
            // 족보를 이룬 5장의 카드를 랭크 순으로 정렬하여 추출
            List<PlayingCard> rankingCards = flushGroup.Value
                .OrderByDescending(c => c.Rank)
                .Take(5)
                .ToList();
        
            List<PlayingCard> kickerCards = hand.Except(rankingCards).OrderByDescending(c => c.Rank).ToList();

            result = new HandResult { Rank = HandRank.Flush, RankingCards = rankingCards, KickerCards = kickerCards };
            return true;
        }
        return false;
    }

    public static bool IsStraight(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        var sortedRanks = pHand.SortedUniqueRanks;
        if (sortedRanks.Count < 5) return false;

        // A. 일반적인 스트레이트 검사
        for (int i = 0; i <= sortedRanks.Count - 5; i++)
        {
            if (sortedRanks[i + 4] - sortedRanks[i] == 4)
            {
                CardRank highRank = sortedRanks[i + 4];
            
                List<PlayingCard> rankingCards = hand.Where(c => (int)c.Rank >= (int)sortedRanks[i] && (int)c.Rank <= (int)highRank)
                    .ToList();
            
                result = new HandResult { Rank = HandRank.Straight, RankingCards = rankingCards, KickerCards = new List<PlayingCard>() };
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
            
            result = new HandResult { Rank = HandRank.Straight, RankingCards = wheelCards, KickerCards = new List<PlayingCard>() };
            return true;
        }

        return false;
    }

    public static bool IsThreeOfAKind(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
        var tripleGroup = pHand.RankGroups.FirstOrDefault(kv => kv.Value.Count == 3);

        if (tripleGroup.Value != null && tripleGroup.Value.Count == 3)
        {
            // 풀 하우스는 이미 앞서 검사했으므로, 여기서 찾은 것은 순수한 트리플입니다.
            List<PlayingCard> rankingCards = tripleGroup.Value;
        
            List<PlayingCard> kickerCards = hand.Except(rankingCards).OrderByDescending(c => c.Rank).ToList();

            result = new HandResult { Rank = HandRank.ThreeOfAKind, RankingCards = rankingCards, KickerCards = kickerCards };
            return true;
        }
        return false;
    }

    public static bool IsTwoPair(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
    
        // 페어 그룹을 랭크가 높은 순서대로 2개 찾습니다.
        var pairGroups = pHand.RankGroups.Where(kv => kv.Value.Count == 2)
            .OrderByDescending(kv => kv.Key)
            .Take(2)
            .ToList();

        if (pairGroups.Count == 2)
        {
            List<PlayingCard> rankingCards = new List<PlayingCard>();
            rankingCards.AddRange(pairGroups[0].Value);
            rankingCards.AddRange(pairGroups[1].Value);

            List<PlayingCard> kickerCards = hand.Except(rankingCards).OrderByDescending(c => c.Rank).ToList();
        
            result = new HandResult { Rank = HandRank.TwoPair, RankingCards = rankingCards, KickerCards = kickerCards };
            return true;
        }
        return false;
    }

    public static bool IsPair(PreprocessedHand pHand, List<PlayingCard> hand, out HandResult result)
    {
        result = new HandResult();
    
        // 가장 높은 랭크의 페어 그룹을 찾습니다.
        var pairGroup = pHand.RankGroups.Where(kv => kv.Value.Count == 2)
            .OrderByDescending(kv => kv.Key)
            .FirstOrDefault();

        if (pairGroup.Value != null && pairGroup.Value.Count == 2)
        {
            List<PlayingCard> rankingCards = pairGroup.Value;
        
            List<PlayingCard> kickerCards = hand.Except(rankingCards).OrderByDescending(c => c.Rank).ToList();

            result = new HandResult { Rank = HandRank.Pair, RankingCards = rankingCards, KickerCards = kickerCards };
            return true;
        }
        return false;
    }
}
