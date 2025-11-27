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
            DeckSystem.Instance.Draw(8);
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            DeckSystem.Instance.Draw();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            HandController.Instance.PlayHand();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            HandController.Instance.DiscardHand();
        }
        
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            HandController.Instance.UseConsumable();
        }
    }
}