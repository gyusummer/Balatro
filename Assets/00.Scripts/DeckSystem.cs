using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
	public PlayingCardView CardPrefab;
	public Transform HandParent;
	
	public CardPile Deck;
	public CardPile DrawPile;
	public CardPile Hand;

	public void Start()
	{
		// 기본 52장 덱
		var defaultDeck = new List<PlayingCard>();
		for (int suit = (int)CardSuit.Club; suit <= (int)CardSuit.Spade; suit++)
		{
			for (int rank = (int)CardRank.Ace; rank <= (int)CardRank.King; rank++)
			{
				var c = new PlayingCard((CardSuit)suit, (CardRank)rank);
				defaultDeck.Add(c);
			}
		}

		defaultDeck = RandomUtil.GetShuffled(defaultDeck);
		foreach (var playingCard in defaultDeck)
		{
			PrintCard(playingCard);
		}
	}

	public void Draw(int n)
	{
		if (n <= 0)
		{
			return;
		}

		for(int k = 0; k < n; k++)
		{
			Draw();
		}
	}
	
	public void Draw()
	{
		if (DrawPile.Count <= 0)
		{
			Debug.Log("No Card in Draw Pile");
			return;
		}
		PlayingCard c = DrawPile.GetFirstCard();
		DrawPile.RemoveCard(c);
		Hand.AddCard(c);
		PrintCard(c);
	}

	public PlayingCardView PrintCard(PlayingCard card)
	{
		PlayingCardView cardView = Instantiate(CardPrefab, HandParent);
		cardView.Init(card);
		return cardView;
	}
}
