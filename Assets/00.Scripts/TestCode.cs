using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            DeckManager.Instance.InitDrawPile();
            DeckManager.Instance.Draw(8);
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            DeckManager.Instance.Draw();
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            Shop.Instance.FillGoods();
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            JokerFactory.Instance.CreateRandom().Register();
        }
    }
}