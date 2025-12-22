using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, ITooltipSource
{
	public string Header => Source.ToString();
	public string Content => $"+{Source.Chip} chips";
	public RectTransform Rect { get; private set; }
	public Transform Transform => transform;
	
	public ImageContainer CardPapers;
	public ImageContainer CardPictures;
	public ImageContainer CardSeals;
	
	public Card Source;
	public Image Paper;
	public Image Picture;

	private CardFanLayout fanLayout;
	private CanvasGroup canvasGroup;
	
	public Action<CardView> OnClick;
	
	[Header("Click Animation Var")]
	[SerializeField] private float punch = 0.2f;
	[SerializeField] private float duration = 0.2f;
	[SerializeField] private int vib = 10;
	[SerializeField] private float ela = 1f;
	
	private bool isDragging = false;
	private float lastFrameX;

	private void OnValidate()
	{
		UpdatePaper();
		UpdatePicture();
	}
	
	void Awake()
	{
		Rect = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		fanLayout = transform.parent.GetComponent<CardFanLayout>();
	}
	
	private void LateUpdate()
	{
		if (isDragging)
		{
			float curX = transform.position.x;
			Quaternion newRot = Quaternion.Euler(0, 0, (lastFrameX - curX) * 2);
			Rect.localRotation = Quaternion.Lerp(Rect.localRotation, newRot, 0.5f);
			lastFrameX = curX;
		}
	}

	public void Init(Card card)
	{
		Source = card;
		card.View = this;
		UpdatePaper();
		UpdatePicture();
	}

	public void MoveTo(Vector3 position, Quaternion rotation)
	{
		transform.DOLocalMove(position, 0.1f, true);
		transform.DORotateQuaternion(rotation, 0.1f).SetEase(Ease.OutElastic);
	}

	public void UpdatePaper()
	{
		Paper.sprite = CardPapers.GetImageByNameOrFirst(Source.Enhancement.ToString());
	}

	public void UpdatePicture()
	{
		int pictureIndex = (int)Source.Rank - 2;
		switch (Source.SuitOrigin)
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
		transform.DOPunchScale(Vector3.one * punch, duration, vib, ela);
	}
	
	// 1. 드래그 시작 시: 투명도를 조절하고, raycastTarget을 꺼서 다른 요소 위로 드래그될 수 있도록 합니다.
	public void OnBeginDrag(PointerEventData eventData)
	{
		isDragging = true;
		// 드롭 타겟이 드래그 요소를 '통과하여' 그 아래에 있는 요소를 감지할 수 있도록 잠시 끕니다.
		canvasGroup.blocksRaycasts = false; 
		canvasGroup.alpha = 0.6f; // 살짝 반투명하게 만듦
		if (fanLayout != null) fanLayout.DraggingChild = Rect;
	}

	// 2. 드래그 중: 마우스 포인터의 위치를 따라 요소의 위치를 업데이트합니다.
	public void OnDrag(PointerEventData eventData)
	{
		// eventData.delta를 사용하여 부드럽게 위치 이동 (가장 효율적)
		Rect.anchoredPosition += eventData.delta / Rect.localScale.x;
		
		if (fanLayout != null)
			fanLayout.ManualSort(this);

		// **팁: eventData.delta 대신 eventData.position을 사용하면 Canvas 설정에 따라 움직임이 부자연스러울 수 있습니다.**
	}

	// 3. 드래그 끝 시: 원래 상태로 되돌리고, 드롭 이벤트를 발생시킬 준비를 합니다.
	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;

		if (fanLayout != null)
		{
			fanLayout.DraggingChild = null;
			fanLayout.ApplyFanEffect();
		}
		// **핵심:** 드롭 영역에 놓았는지 여부와 실제 효과는 DropTarget 스크립트에서 처리됩니다.
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f).SetEase(Ease.OutElastic);
		TooltipSystem.ShowTooltip(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
		TooltipSystem.HideTooltip();
	}

	public static implicit operator RectTransform(CardView c) => c.Rect;
}
