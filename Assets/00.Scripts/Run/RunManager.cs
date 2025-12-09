using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RunManager : Singleton<RunManager>
{
	public static class Variables
	{
		// last played consumable
		// tarot consume count
		// skipped count
		public static int Hands = 4;
		public static int Discards = 3;
		public static int HandCapacity = 8;
	}

	public int CurrentAnte = 1;
    public int GoalAnte = 8;
    public Blind Small;
    public Blind Big;
    public Blind Boss;

    public void WinAnte()
    {
        if (CurrentAnte == GoalAnte)
        {
            WinGame();
        }
        
        CurrentAnte++;
        InitNewAnte();
		Debug.Log("<color=red>WinAnte</color>");
    }
    
	public void WinGame()
	{
		Debug.Log("<color=red>WinGame</color>");
	}
	
	public void LoseGame()
	{
		Debug.Log("<color=red>LoseGame</color>");
	}

    [SerializeField] private TMP_Text _smallScore;
    [SerializeField] private TMP_Text _bigScore;
    [SerializeField] private TMP_Text _bossScore;
    
    public void InitNewAnte()
    {
        double baseGoalScore = BlindAmountCalculator.GetBlindAmount(CurrentAnte);
        Small = new Blind(baseGoalScore, Blind.BlindRank.Small);
        Big = new Blind(baseGoalScore * 1.5, Blind.BlindRank.Big);
        Boss = new Blind(baseGoalScore * 2, Blind.BlindRank.Boss);
        
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Score at least:");
        stringBuilder.Append(Small.GoalScore);
        _smallScore.text = stringBuilder.ToString();

        stringBuilder.Clear();
        stringBuilder.AppendLine("Score at least:");
        stringBuilder.Append(Big.GoalScore);
        _bigScore.text = stringBuilder.ToString();
        
        stringBuilder.Clear();
        stringBuilder.AppendLine("Score at least:");
        stringBuilder.Append(Boss.GoalScore);
        _bossScore.text = stringBuilder.ToString();
    }

    #region Run state
    public enum RunState
    {
        None,
        Ante,
        Blind,
        Shop
    }

    public GameObject AntePanel;
    public GameObject BlindPanel;
    public GameObject ShopPanel;
    
    public void ChangeState(int state)
    {
        RunState newState = (RunState)state;
        switch (newState)
        {
            case RunState.None:
                AntePanel.SetActive(false);
                BlindPanel.SetActive(false);
                ShopPanel.SetActive(false);
                break;
            case RunState.Ante:
                AntePanel.SetActive(true);
                BlindPanel.SetActive(false);
                ShopPanel.SetActive(false);
                break;
            case RunState.Blind:
                AntePanel.SetActive(false);
                BlindPanel.SetActive(true);
                ShopPanel.SetActive(false);
                break;
            case RunState.Shop:
                Shop.Instance.FillGoods();
                AntePanel.SetActive(false);
                BlindPanel.SetActive(false);
                ShopPanel.SetActive(true);
                break;
        }
    }

    public void StartGame()
    {
        InitNewAnte();
        ChangeState(1);
    }

    public void StartSmallBlind()
    {
        ChangeState(2);
        BlindManager.Instance.StartBlind(Small);
    }
    public void StartBigBlind()
    {
        ChangeState(2);
        BlindManager.Instance.StartBlind(Big);
    }
    public void StartBossBlind()
    {
        ChangeState(2);
        BlindManager.Instance.StartBlind(Boss);
    }
    
    
    #endregion
}

public static class BlindAmountCalculator
{
    // 게임 설정 변수들을 관리하는 중첩 클래스 (실제 Unity 환경에 맞게 조정 필요)
    public static class GameModifiers 
    {
        // Lua의 G.GAME.modifiers.scaling 역할을 대신합니다.
        // 이 값은 런타임에 게임 상태에 따라 변경될 수 있습니다.
        public static int ScalingModifier = 1; // 기본값 1
    }

    /// <summary>
    /// 앤티(ante) 레벨에 따른 블라인드 금액을 계산합니다.
    /// </summary>
    /// <param name="ante">현재 앤티 레벨 (1부터 시작)</param>
    /// <returns>계산된 블라인드 금액</returns>
    public static int GetBlindAmount(int ante)
    { 
        // 1 미만일 경우 기본값 100을 반환합니다. (Lua 코드와 동일)
        if (ante < 1)
        {
            return 100;
        }

        // 스케일링 모드에 따라 사용할 금액 배열을 결정합니다.
        int[] amounts;
        int scaling = GameModifiers.ScalingModifier; 

        switch (scaling)
        {
            case 1:
            case 0: // Lua 코드에서 not G.GAME.modifiers.scaling 또는 G.GAME.modifiers.scaling == 1 일 때의 케이스
                amounts = new int[] {
                    0, 300, 800, 2000, 5000, 11000, 20000, 35000, 50000
                };
                break;
            case 2:
                amounts = new int[] {
                    0, 300, 900, 2600, 8000, 20000, 36000, 60000, 100000
                };
                break;
            case 3:
                amounts = new int[] {
                    0, 300, 1000, 3200, 9000, 25000, 60000, 110000, 200000
                };
                break;
            default:
                // 정의되지 않은 스케일링 값에 대한 기본 처리 (case 1과 동일하게 처리)
                amounts = new int[] { 
                    0, 300, 800, 2000, 5000, 11000, 20000, 35000, 50000
                };
                break;
        }
        
        // 앤티 레벨은 1부터 시작하므로, 배열 인덱스를 맞추기 위해 0번 인덱스에 더미 값(0)을 넣었습니다.
        // 이렇게 하면 amounts[ante]로 직접 접근할 수 있습니다. (amounts.Length == 9)
        if (ante < amounts.Length) 
        {
            return amounts[ante];
        }

        #region endless mode
        // 앤티가 8을 초과하는 경우의 스케일링 로직
        const double k = 0.75;
        // Lua의 a, b, c, d 변수에 해당하는 값들을 정의합니다.
        double a = amounts[8];
        const double b = 1.6;
        double c = ante - 8;
        double d = 1.0 + 0.2 * c;

        // Lua: local amount = math.floor(a*(b+(k*c)^d)^c)
        // C#: Math.Pow(base, exponent)를 사용하여 거듭제곱을 계산합니다.
        double innerPower = Math.Pow(k * c, d);
        double baseValue = b + innerPower;
        double finalPower = Math.Pow(baseValue, c);
        double rawAmount = a * finalPower;
        
        double amount = Math.Floor(rawAmount);

        // Lua: amount = amount - amount%(10^math.floor(math.log10(amount)-1))
        // 이 로직은 계산된 금액을 **특정 단위로 반올림/내림** 처리하는 부분입니다.
        // 예를 들어, 56,789가 나왔다면 50,000으로 만들거나, 
        // 567,890이 나왔다면 560,000으로 만드는 (앞의 두 자리를 남기고 나머지를 버리는) 로직입니다.

        // 1. amount의 10진수 자릿수를 구합니다. (log10)
        double log10Value = Math.Log10(amount);
        
        // 2. 반올림/내림의 기준이 될 자릿수를 계산합니다.
        // Lua: math.floor(math.log10(amount)-1)
        // (금액이 100,000이라면 log10은 5.0, -1을 하면 4.0. 즉 10^4 = 10,000 단위로 버림)
        double powerOfTen = Math.Floor(log10Value - 1.0); 
        
        // 3. 버릴 단위를 계산합니다. (divisor)
        double divisor = Math.Pow(10.0, powerOfTen); 

        // 4. 나머지(modulo) 연산을 사용하여 해당 단위 이하를 버립니다.
        // C#의 % 연산은 정수/실수 모두 지원하지만, 정밀한 계산을 위해 double로 진행합니다.
        double remainder = amount % divisor;
        
        // 5. 최종 금액을 계산합니다.
        // 예: amount(56789) - remainder(6789) = 50000
        amount = amount - remainder;
        
        // 최종적으로 정수형으로 변환하여 반환합니다.
        return (int)amount;
        #endregion
    }
}