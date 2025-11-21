using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 1. Manage Deck and Hand
// 2. Display Cards
public class DeckSystem : Singleton<DeckSystem>
{
	[SerializeField] private PlayingCardView cardViewPrefab;
	public Transform HandHolder;
	
	public CardPile Deck;
	public CardPile DrawPile;
	public CardPile Hand;

	protected override void Awake()
	{
		base.Awake();
		// 기본 52장 덱
		var defaultDeck = new List<PlayingCard>();
		for (int suit = (int)CardSuit.Diamond; suit <= (int)CardSuit.Spade; suit++)
		{
			for (int rank = (int)CardRank.Ace; rank <= (int)CardRank.King; rank++)
			{
				var c = new PlayingCard((CardSuit)suit, (CardRank)rank);
				defaultDeck.Add(c);
			}
		}
		
		Deck = new CardPile(defaultDeck, nameof(defaultDeck));
		Hand = new CardPile(new List<PlayingCard>(), nameof(Hand));
		
		Hand.OnCardAdded += OnCardAddedToHand;
		Hand.OnCardRemoved += OnCardRemovedFromHand;
	}

	private void OnCardAddedToHand(PlayingCard card)
	{
		PrintCard(card, HandHolder);
	}
	
	private void OnCardRemovedFromHand(PlayingCard card)
	{
		Destroy(card.View.gameObject);
		Debug.Log($"{card} removed from hand");
	}
	
	public void InitDrawPile()
	{
		var cardList = Deck.CloneCardList();
		RandomUtil.GetShuffled(cardList);
		DrawPile = new CardPile(cardList, nameof(DrawPile));
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
	}

	public PlayingCardView PrintCard(PlayingCard card, Transform uiParent = null)
	{
		PlayingCardView cardView = Instantiate(cardViewPrefab, uiParent);
		cardView.Init(card);
		return cardView;
	}

	private void OnDestroy()
	{
		Hand.OnCardAdded -= OnCardAddedToHand;
		Hand.OnCardRemoved -= OnCardRemovedFromHand;
	}
}
