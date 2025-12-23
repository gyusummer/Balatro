using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 1. Manage Deck and Hand
// 2. Display Cards
public class DeckManager : Singleton<DeckManager>
{
	[SerializeField] private CardView viewPrefab;
	[SerializeField] private Transform handHolder;
	[SerializeField] private Transform deckPosition;
	
	public CardPile Deck;
	public CardPile DrawPile;
	public CardPile Hand;

	protected override void Awake()
	{
		base.Awake();
		// 기본 52장 덱
		var defaultDeck = new List<Card>();
		// for (int suit = (int)CardSuit.Diamond; suit <= (int)CardSuit.Spade; suit++)
		foreach (CardSuit suit in Card.ALL_EACH_SUITS)
		{
			for (int rank = (int)CardRank.Two; rank <= (int)CardRank.Ace; rank++)
			{
				var c = new Card((CardSuit)suit, (CardRank)rank);
				defaultDeck.Add(c);
			}
		}
		
		Deck = new CardPile(defaultDeck, "default");
		Hand = new CardPile(new List<Card>(), "hand");

		if (handHolder.TryGetComponent<CardFanLayout>(out var layout))
		{
			layout.SetSource(Hand);
		}
		
		Hand.OnAdded += PrintHandCard;
		Hand.OnRemoved += DestroyHandCard;
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

		StartCoroutine(Draw_Co(n));
		
		Hand.SortByRank();
	}

	[SerializeField] private float drawTerm;
	private IEnumerator Draw_Co(int n)
	{
		for(int k = 0; k < n; k++)
		{
			Draw();
			Hand.SortByRank();
			yield return new WaitForSeconds(drawTerm);
		}
	}
	
	private void Draw()
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
		CardView cardView = Instantiate(viewPrefab, deckPosition.position, Quaternion.Euler(0, 180, 0), uiParent);
		cardView.Init(card);
		return cardView;
	}

	private void OnDestroy()
	{
		// Hand.OnCardAdded -= OnCardAddedToHand;
		// Hand.OnCardRemoved -= OnCardRemovedFromHand;
	}
}
