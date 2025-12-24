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
	[SerializeField] private Transform discardPosition;
	
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
		Hand.OnRemoved += DestroyCardView;
	}

	private void PrintHandCard(Card card)
	{
		PrintCard(card, handHolder);
	}
	
	private void DestroyCardView(Card card)
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

	private IEnumerator Draw_Co(int n)
	{
		for(int k = 0; k < n; k++)
		{
			Draw();
			Hand.SortByRank();
			yield return new WaitForSeconds(AnimationManager.CardSequenceGap);
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
	
	public void DiscardHand(List<Card> selectedCards)
	{
		StartCoroutine(DiscardHand_Co(selectedCards));
	}

	private IEnumerator DiscardHand_Co(List<Card> selectedCards)
	{
		BlindManager.Instance.DiscardsLeft--;
		
		foreach (Card card in selectedCards)
		{
			card.View.WorldMoveTo(discardPosition.position, discardPosition.rotation);
			yield return new WaitForSeconds(AnimationManager.CardSequenceGap);
		}
		
		// wait for all discard animation
		float waitTime = (AnimationManager.CardMoveTime * 2) - AnimationManager.CardSequenceGap * selectedCards.Count;
		if (waitTime > 0)
		{
			yield return new WaitForSeconds(waitTime);
		}

		foreach (Card card in selectedCards)
		{
			Hand.Remove(card);
			IngameEventManager.CallEvent(new CardDiscardedEventArgs(card));
		}

		FillHand();
	}
	
	public void FillHand()
	{
		Draw(BlindManager.Instance.HandCapacity - Hand.Count);
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
