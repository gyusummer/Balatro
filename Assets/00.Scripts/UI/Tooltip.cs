using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
	public TMP_Text HeaderText;
	public TMP_Text ContentText;
	public LayoutElement LayoutElement;
	public int CharacterLimit;
	
	private Camera uiCamera;
	public Vector2 Padding;

	private void Awake()
	{
		uiCamera = Camera.main;
	}

	public void UpdateImmediately(ITooltipElement source)
	{
		SetText(source);
		LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
		SetPosition(source);
		transform.SetParent(source.Transform);
	}
	
	public void SetText(ITooltipElement source)
	{
		HeaderText.text = source.Header;
		ContentText.text = source.Content;
		
		int titleLength = HeaderText.text.Length;
		int descriptionLength = ContentText.text.Length;
		
		LayoutElement.enabled = titleLength > CharacterLimit || descriptionLength > CharacterLimit;
	}

	public void Clear()
	{
		HeaderText.text = "";
		ContentText.text = "";
	}
    
	public void SetPosition(ITooltipElement source)
	{
		// Anchor
		RectTransform selfRect = transform as RectTransform;
		RectTransform sourceRect = source.Rect;
		
		Vector3[] sourceCorners = new Vector3[4];
		sourceRect.GetWorldCorners(sourceCorners);

		Vector3 sourceWorldPos = source.Transform.position;
		Vector2 sourceScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, sourceWorldPos);
		
		Vector2 sourceScreenLB = RectTransformUtility.WorldToScreenPoint(uiCamera, sourceCorners[0]);
		Vector2 sourceScreenRT = RectTransformUtility.WorldToScreenPoint(uiCamera, sourceCorners[2]);
        Vector2 sourceScreenSize = sourceScreenRT - sourceScreenLB;
        
        // Offset
        Vector3[] tooltipCorners = new Vector3[4];
        selfRect.GetWorldCorners(tooltipCorners);
		
        Vector2 tooltipScreenLB = RectTransformUtility.WorldToScreenPoint(uiCamera, tooltipCorners[0]);
        Vector2 tooltipScreenRT = RectTransformUtility.WorldToScreenPoint(uiCamera, tooltipCorners[2]);
        Vector2 tooltipScreenSize = tooltipScreenRT - tooltipScreenLB;
        
        Vector2 totalOffset = sourceScreenSize / 2 + tooltipScreenSize / 2f + Padding;

        // Tooltip Position
        Vector2 finalScreenPos = sourceScreenPos;
		Vector2 sourceViewPos = uiCamera.WorldToViewportPoint(sourceRect.transform.position);
        
        if (sourceViewPos.y > 0.5f)
        {
	        finalScreenPos.y -= totalOffset.y;
        }
        else
        {
	        finalScreenPos.y += totalOffset.y;
        }
        
        Debug.Log($"SourceScreenPos {sourceScreenPos}");
        Debug.Log($"Offset {sourceScreenSize.y + totalOffset.y}");
        Debug.Log($"finalScreenPos {finalScreenPos}");
        
        // Apply
        Vector2 localPointer;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle
	            (transform.parent.GetComponent<RectTransform>(),
		            finalScreenPos, uiCamera, out localPointer))
        {
            selfRect.localPosition = localPointer;
        }
	}
}
