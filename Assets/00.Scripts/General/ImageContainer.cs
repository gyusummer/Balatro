using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Container", menuName = "Game Data/Image Container")]
public class ImageContainer : ScriptableObject
{
	[SerializeField] private List<Sprite> Images;
	[SerializeField] private List<string> ImageNames;

	public Sprite GetImage(int index)
	{
		if (index < 0 || index >= Images.Count)
		{
			Debug.LogWarning("Image Index out of range");
			return Images[0];
		}
		return Images[index];
	}
	
	public Sprite GetImageByNameOrFirst(string key)
	{
		int targetIndex = ImageNames.IndexOf(key);

		return GetImage(targetIndex);
	}
}
