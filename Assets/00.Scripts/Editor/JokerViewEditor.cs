using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JokerView))]
public class JokerViewEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		JokerView script = (JokerView)target;
		
		if (GUILayout.Button("Buy"))
		{
			script.Source.Buy();
		}
		
		if (GUILayout.Button("Sell"))
		{
			script.Source.Sell();
		}
	}
}
