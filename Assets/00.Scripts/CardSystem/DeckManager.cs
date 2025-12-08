using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// 1. Manage Deck and Hand
// 2. Display Cards
public class DeckManager : Singleton<DeckManager>
{
	[SerializeField] private CardView viewPrefab;
	[SerializeField] private Transform handHolder;
	
	public CardPile Deck;
	public CardPile DrawPile;
	public CardPile Hand;

	protected override void Awake()
	{
		base.Awake();
		// 기본 52장 덱
		var defaultDeck = new List<Card>();
		for (int suit = (int)CardSuit.Diamond; suit <= (int)CardSuit.Spade; suit++)
		{
			for (int rank = (int)CardRank.Two; rank <= (int)CardRank.Ace; rank++)
			{
				var c = new Card((CardSuit)suit, (CardRank)rank);
				defaultDeck.Add(c);
			}
		}
		
		Deck = new CardPile(defaultDeck, "default");
		Hand = new CardPile(new List<Card>(), "hand");
		
		Hand.OnAdded += PrintHandCard;
		Hand.OnRemoved += DestroyHandCard;
		Hand.OnAdded += Hand.SortByRank;
		Hand.OnRemoved += Hand.SortByRank;
	}

	private void PrintHandCard(Card card)
	{
		PrintCard(card, handHolder);
	}
	
	private void DestroyHandCard(Card card)
	{
		Debug.Log($"remove {card} from hand");
		Destroy(card.View.gameObject);
		Debug.Log($"{card} removed from hand");
	}
	
	public void InitDrawPile()
	{
		Debug.Log("InitDrawPile");
		var cardList = Deck.CloneList();
		DrawPile = new CardPile(RandomUtil.GetShuffled(cardList), "DrawPile");
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
		Card c = DrawPile.First();
		DrawPile.Remove(c);
		Hand.Add(c);
	}

	public CardView PrintCard(Card card, Transform uiParent = null)
	{
		if (uiParent == null)
		{
			uiParent = handHolder;
		}
		CardView cardView = Instantiate(viewPrefab, uiParent);
		cardView.Init(card);
		return cardView;
	}

	private void OnDestroy()
	{
		// Hand.OnCardAdded -= OnCardAddedToHand;
		// Hand.OnCardRemoved -= OnCardRemovedFromHand;
	}
}
