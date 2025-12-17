using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ITooltipSource : IPointerEnterHandler, IPointerExitHandler
{
    public string Header { get; }
    public string Content { get; }
    public RectTransform Rect { get; }
    public Transform Transform { get; }
}

public class TooltipSystem : Singleton<TooltipSystem>
{
    public JokerDescription JokerDescription;
    
    [SerializeField] private Tooltip _tooltip;
    private static Tooltip Tooltip => Instance._tooltip;
    
    public static void ShowTooltip(ITooltipSource source)
    {
        Tooltip.gameObject.SetActive(true);
        Tooltip.UpdateImmediately(source);
    }

    public static void HideTooltip()
    {
        Tooltip.gameObject.SetActive(false);
        Tooltip.Clear();
    }
}