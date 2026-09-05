using Godot;
using System;

using System.Collections.Generic;

public partial class MoneyManager : Node
{
	public static MoneyManager Instance;
	public override void _Ready()
	{
		Instance = this;
	}

	public Action<CoinType, float> CoinChanged;
	public enum CoinType
	{
		GoldePrusse,
		Almatai,
		Forc
	}
	public Dictionary<CoinType, float> Coins = new()
	{
		{CoinType.GoldePrusse, 0},
		{CoinType.Almatai, 0},
		{CoinType.Forc, 0}
	};

	public void AddCoins(CoinType coinType, float amount)
	{
		Coins[coinType] += amount;
		CoinChanged?.Invoke(coinType, Coins[coinType]);
	}

	public bool CanSpendCoin(CoinType coinType, float amount)
	{
		return Coins[coinType] >= amount;
	}

	public void SpendCoin(CoinType coinType, float amount)
	{
		if (!CanSpendCoin(coinType, amount)) return;
		Coins[coinType] -= amount;	
		CoinChanged?.Invoke(coinType, Coins[coinType]);
	}
}
