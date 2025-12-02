using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	public ImageContainer CardPapers;
	public ImageContainer CardPictures;
	public ImageContainer CardSeals;
	
	public Card Source;
	public Image Paper;
	public Image Picture;

	public Action<CardView> OnClick;

	private void OnValidate()
	{
		UpdatePaper();
		UpdatePicture();
	}

	public void Init(Card card)
	{
		Source = card;
		card.View = this;
		UpdatePaper();
		UpdatePicture();
	}

	public void UpdatePaper()
	{
		Paper.sprite = CardPapers.GetImageByNameOrFirst(Source.Enhancement.ToString());
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
		Debug.Log($"{Source} Deselected");
		Paper.color = Color.white;
	}
	
	private void OnDestroy()
	{
		Source.View = null;
		Source = null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_isDragging) return;
		OnClick?.Invoke(this);
	}

	private void LateUpdate()
	{
		if (_isDragging)
		{
			transform.position = Input.mousePosition;
			Debug.Log(Input.mousePosition);
		}
	}

	private bool _isDragging = false;
	public void OnBeginDrag(PointerEventData eventData)
	{
		_isDragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		_isDragging = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
	}
}
