using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JokerText
{
	public string Name;
	public string Description;
}

[CreateAssetMenu(fileName = "Joker Description", menuName = "Game Data/Joker Description")]
public class JokerDescription : ScriptableObject
{
	public List<JokerText> Texts;

	public JokerText GetJokerString(string key)
	{
		return Texts.Find(s => s.Name == key);
	}
}
