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
		if (Source.View == this)
		{
			Source.View = null;
		}
		Source = null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClick?.Invoke(this);
	}

	private CardFanLayout fanLayout;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		fanLayout = transform.parent.GetComponent<CardFanLayout>();
	}
	
	// 1. 드래그 시작 시: 투명도를 조절하고, raycastTarget을 꺼서 다른 요소 위로 드래그될 수 있도록 합니다.
	public void OnBeginDrag(PointerEventData eventData)
	{
		// 드롭 타겟이 드래그 요소를 '통과하여' 그 아래에 있는 요소를 감지할 수 있도록 잠시 끕니다.
		canvasGroup.blocksRaycasts = false; 
		canvasGroup.alpha = 0.6f; // 살짝 반투명하게 만듦
		if (fanLayout != null) fanLayout.DraggingChild = rectTransform;
	}

	// 2. 드래그 중: 마우스 포인터의 위치를 따라 요소의 위치를 업데이트합니다.
	public void OnDrag(PointerEventData eventData)
	{
		// eventData.delta를 사용하여 부드럽게 위치 이동 (가장 효율적)
		rectTransform.anchoredPosition += eventData.delta / rectTransform.localScale.x; 
        
		// **팁: eventData.delta 대신 eventData.position을 사용하면 Canvas 설정에 따라 움직임이 부자연스러울 수 있습니다.**
	}

	// 3. 드래그 끝 시: 원래 상태로 되돌리고, 드롭 이벤트를 발생시킬 준비를 합니다.
	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;

		if (fanLayout != null)
		{
			fanLayout.DraggingChild = null;
			fanLayout.ApplyFanEffect();
		}
		// **핵심:** 드롭 영역에 놓았는지 여부와 실제 효과는 DropTarget 스크립트에서 처리됩니다.
	}
}
