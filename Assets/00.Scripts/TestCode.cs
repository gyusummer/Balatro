using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			DeckSystem.Instance.InitDrawPile();
		}

		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			DeckSystem.Instance.Draw();
		}
	}
}
