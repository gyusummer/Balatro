using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ScoreBoard : Singleton<ScoreBoard>
{
	[SerializeField] private TMP_Text Chip;
	[SerializeField] private TMP_Text Mult;
	[SerializeField] private TMP_Text Score;

	public void UpdateChip(double chipValue)
	{
		Chip.text = chipValue.ToString();
	}

	public void UpdateMult(double multValue)
	{
		Mult.text = multValue.ToString();
	}
	
	public void UpdateScore(double scoreValue)
	{
		Score.text = scoreValue.ToString();
	}
}
