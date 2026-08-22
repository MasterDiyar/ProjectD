using Godot;
using System;
using ProjectD.scripts.units;

public enum StatEnum
{
	Hp,
	Armor,
	Damage,
	Speed,
	Mana,
	KritChance,
	KritModifier,
	Nothing
}

public partial class UnitConstructor : Node
{
	[Export] public UnitResource[] Blocks;
	[Export] public Sprite2D[]  BlockSprites;
	[Export] private Unit Unit;
	
	public override void _Ready()
	{
		Unit.StatsLoaded += Load;
	}

	void Load()
	{
		var rng = new RandomNumberGenerator();
		var count = Mathf.Min(Blocks.Length, BlockSprites.Length);
		rng.Randomize();
		for (var i = 0; i < count; i++) {
			var sum = rng.RandiRange(0, Blocks[i].Stats.Count-1);
			BlockSprites[i].Texture = Blocks[i].Textures[sum];
			Loader(Blocks[i].Stats[sum], Blocks[i].Add[sum]);
		}
	}

	void Loader(StatEnum stat, float num)
	{
		switch (stat)
		{
			case StatEnum.Hp: Unit.Hp += num; break;
			case StatEnum.Armor: Unit.Armor += num; break;
			case StatEnum.Damage: Unit.Damage += num; break;
			case StatEnum.Speed: Unit.Speed += num; break;
			case StatEnum.Mana: Unit.Mana += num; break;
			case StatEnum.KritChance: Unit.KritChance += num; break;
			case StatEnum.KritModifier: Unit.KritModifier += num; break;
			default: break;
		}
	}
}
