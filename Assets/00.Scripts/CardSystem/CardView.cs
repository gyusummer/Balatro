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
	private static readonly int ATLAS_UV = Shader.PropertyToID("_AtlasUv");
	
	public string Header => Source.ToString();
	public string Content => $"+{Source.Chip} chips";
	public RectTransform Rect { get; private set; }
	public Transform Transform => transform;
	
	public ImageContainer CardPapers;
	public ImageContainer CardPictures;
	public ImageContainer CardSeals;
	[SerializeField] private List<Shader> cardShaders;
	
	public Card Source;
	public Image Paper;
	public Image Picture;

	private CardFanLayout fanLayout;
	private CanvasGroup canvasGroup;
	
	public Action<CardView> OnClick;
	
	[Header("Click Animation Var")]
	[SerializeField] private float punchScale = 0.2f;
	[SerializeField] private int vib = 10;
	[SerializeField] private float ela = 1f;
	
	private bool isDragging = false;
	private float lastFrameX;
	
	private Vector3 targetLocalPosition;
	private Quaternion targetLocalRotation;
	private Vector3 velocity;
	private float rotVelocity;
	private bool isTargetSet = false;

	private void OnValidate()
	{
		UpdatePaper();
		UpdatePicture();
		UpdateShader();
	}
	
	void Awake()
	{
		Rect = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		fanLayout = transform.parent.GetComponent<CardFanLayout>();
	}

	private void Start()
	{
		UpdateAtlasUv();
	}

	public Image Backface;
	[Range(0.0001f, 0.15f)]public float SmoothTime;
	private void LateUpdate()
	{
		if (isDragging)
		{
			float curX = transform.position.x;
			if (Time.deltaTime > 0)
			{
				float velocityX = (curX - lastFrameX) / Time.deltaTime;
				float targetZ = -velocityX * 0.5f; 
				targetZ = Mathf.Clamp(targetZ, -45f, 45f);
				Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);
				Rect.localRotation = Quaternion.Lerp(Rect.localRotation, targetRot, Time.deltaTime * 15f);
			}
			lastFrameX = curX;
		}
		else if (isTargetSet)
		{
			transform.localPosition = Vector3.SmoothDamp(
				transform.localPosition, targetLocalPosition, ref velocity, 0.05f);

			float tiltZ = -velocity.x * 0.05f; 
			tiltZ = Mathf.Clamp(tiltZ, -40f, 40f);
			
			Vector3 finalEuler = targetLocalRotation.eulerAngles;
			finalEuler.z += tiltZ;

			float currentZ = transform.localRotation.eulerAngles.z;
			float newZ = Mathf.SmoothDampAngle(currentZ, finalEuler.z, ref rotVelocity, SmoothTime);
			
			transform.localRotation = Quaternion.Euler(finalEuler.x, finalEuler.y, newZ);

			// --- 도착 판정 추가 ---
			if (Vector3.SqrMagnitude(transform.localPosition - targetLocalPosition) < 0.001f && 
			    Mathf.Abs(Mathf.DeltaAngle(currentZ, finalEuler.z)) < 0.1f)
			{
				transform.localPosition = targetLocalPosition;
				transform.localRotation = targetLocalRotation;
				isTargetSet = false;
			}
		}

		var back = transform.forward.z;
		Backface.enabled = back < 0;
	}

	public void Init(Card card)
	{
		Source = card;
		card.View = this;
		UpdatePaper();
		UpdatePicture();
	}

	public void LocalMoveTo(Vector3 position, Quaternion rotation)
	{
		transform.DOKill();
		targetLocalPosition = position;
		targetLocalRotation = rotation;
		isTargetSet = true;
	}

	public void WorldMoveTo(Vector3 position, Quaternion rotation)
	{
		transform.DOMove(position, AnimationVariable.CardMoveTime, true);
		transform.DORotateQuaternion(rotation, AnimationVariable.CardMoveTime * 2);
	}

	public void UpdatePaper()
	{
		Paper.sprite = CardPapers.GetImageByNameOrFirst(Source.Enhancement.ToString());
		UpdateAtlasUv();
		if (Source.Enhancement == CardEnhancement.Stone)
		{
			Picture.enabled = false;
		}
		else
		{
			Picture.enabled = true;
		}
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
		UpdateAtlasUv();
	}

	public void OnSelected()
	{
		Paper.color = Color.gray;
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

	public Sequence ActivateAnimation()
	{
		Sequence seq = DOTween.Sequence();
		seq.Append(transform.DOPunchScale(Vector3.one * AnimationVariable.ActivatePunchScale, AnimationVariable.ActivatePunchTime, vib, ela));
		return seq;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClick?.Invoke(this);
		transform.DOPunchScale(Vector3.one * punchScale, AnimationVariable.ClickPunchTime, vib, ela);
	}
	
	// 1. 드래그 시작 시: 투명도를 조절하고, raycastTarget을 꺼서 다른 요소 위로 드래그될 수 있도록 합니다.
	public void OnBeginDrag(PointerEventData eventData)
	{
		isDragging = true;
		lastFrameX = transform.position.x;
		// 드롭 타겟이 드래그 요소를 '통과하여' 그 아래에 있는 요소를 감지할 수 있도록 잠시 끕니다.
		canvasGroup.blocksRaycasts = false; 
		if (fanLayout != null) fanLayout.DraggingChild = Rect;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Rect.anchoredPosition += eventData.delta / Rect.localScale.x;
		
		if (fanLayout != null)
			fanLayout.ManualSort(this);
	}

	// 3. 드래그 끝 시: 원래 상태로 되돌리고, 드롭 이벤트를 발생시킬 준비를 합니다.
	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
		canvasGroup.blocksRaycasts = true;

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

	private void UpdateAtlasUv()
	{
		var paperSprite = Paper.sprite;
		Vector4 paperUv = UnityEngine.Sprites.DataUtility.GetOuterUV(paperSprite);
		Paper.material.SetVector(ATLAS_UV, paperUv);
		
		var pictureSprite = Picture.sprite;
		Vector4 pictureUv = UnityEngine.Sprites.DataUtility.GetOuterUV(pictureSprite);
		Paper.material.SetVector(ATLAS_UV, pictureUv);
	}

	public void UpdateShader()
	{
		var shader = cardShaders[(int)Source.Edition];
		
        Material newPaperMat = new Material(Paper.material); 
        newPaperMat.shader = shader;
        Paper.material = newPaperMat;
        
        Material newPictureMat = new Material(Picture.material); 
        newPictureMat.shader = shader;
        Picture.material = newPictureMat;
	}
}
