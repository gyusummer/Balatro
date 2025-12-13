using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ITooltipData
{
	
}

public class Tooltip : MonoBehaviour
{
	public TMP_Text Prefab;
	public TMP_Text TitleText;
	public TMP_Text DescriptionText;
	public List<TMP_Text> Texts = new List<TMP_Text>();
	
	public ITooltipData Description;

	public void Start()
	{
		
	}
}
