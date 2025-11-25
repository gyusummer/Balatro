using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayingCardView : MonoBehaviour, IPointerClickHandler
{
	public ImageContainer CardPapers;
	public ImageContainer CardPictures;
	public ImageContainer CardSeals;
	
	public PlayingCard Source;
	public Image Paper;
	public Image Picture;

	public Action<PlayingCardView> OnClick;

	private void OnValidate()
	{
		UpdatePaper();
		UpdatePicture();
	}

	public void Init(PlayingCard card)
	{
		Source = card;
		card.View = this;
		UpdatePaper();
		UpdatePicture();
	}

	public void UpdatePaper()
	{
		Paper.sprite = CardPapers.GetImageByKeyOrNull(Source.Enhancement.ToString());
	}

	public void UpdatePicture()
	{
		int pictureIndex = (int)Source.Rank - 2;
		switch (Source.Suit)
		{
			case CardSuit.Diamond:
				pictureIndex += 26;
				break;
			case CardSuit.Club:
				pictureIndex += 13;
				break;
			case CardSuit.Heart:
				break;
			case CardSuit.Spade:
				pictureIndex += 39;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		Picture.sprite = CardPictures.GetImage(pictureIndex);
	}

	public void OnSelected()
	{
		Paper.color = Color.black;
	}

	public void OnDeselected()
	{
		Paper.color = Color.white;
	}
	
	private void OnDestroy()
	{
		Source.View = null;
		Source = null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClick?.Invoke(this);
	}
}
