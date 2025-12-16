using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            BlindManager.Instance.StartBlind(new Blind(300, Blind.BlindRank.Small));
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            DeckManager.Instance.Draw(1);
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
            var joker = JokerFactory.Instance.CreateRandom(true);
            Inventory.Instance.Jokers.Add(joker);
        }
    }
}