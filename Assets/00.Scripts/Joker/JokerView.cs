
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JokerView : View<Joker>, ITradeableView, IBeginDragHandler, IEndDragHandler, IDragHandler, ITooltipSource
{
	private static readonly int ATLAS_UV = Shader.PropertyToID("_AtlasUv");
    
    public string Header => Source.Name;
    public string Content => TooltipSystem.Instance.JokerDescription.GetJokerString(Source.Name).Description;
    public RectTransform Rect => transform as RectTransform;
    public Transform Transform => transform;
    
    public ITradeable Tradeable => Source;
    public Joker Source;
    public ImageContainer JokerImageSet;
    public Image Paper;
	[SerializeField] private List<Shader> cardShaders;

    public Action<JokerView> OnClick;

    private void OnValidate()
    {
        UpdatePaper();
        UpdateShader();
    }

    public override void Init(Joker joker)
    {
        Source = joker;
        joker.View = this;
        UpdatePaper();
    }

    public void UpdatePaper()
    {
        if (Source == null) return;
        
        Debug.Log($"{gameObject.name}\n" +
                  $"{Source.Name}");
        Paper.sprite = JokerImageSet.GetImageByNameOrFirst(Source.Name);
        Paper.SetNativeSize();
        UpdateAtlasUv();
    }
    
    private void OnDestroy()
    {
        Source.View = null;
        Source = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{name}: OnPointerClick");
        OnClick?.Invoke(this);
    }
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
	
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; 
        canvasGroup.alpha = 0.6f; // 살짝 반투명하게 만듦
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rectTransform.localScale.x; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.ShowTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.HideTooltip();
    }
    
    private void UpdateAtlasUv()
    {
        var paperSprite = Paper.sprite;
        Vector4 paperUv = UnityEngine.Sprites.DataUtility.GetOuterUV(paperSprite);
        Paper.material.SetVector(ATLAS_UV, paperUv);
    }

    public void UpdateShader()
    {
        if (Source == null) return;
        
        Material newMat = new Material(Paper.material); 
        var shader = cardShaders[(int)Source.Edition];
        newMat.shader = shader;
        Paper.material = newMat;
    }
}