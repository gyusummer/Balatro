using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConsumableCardView), true)]
public class ConsumableCardViewEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ConsumableCardView script = (ConsumableCardView)target;
		
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
